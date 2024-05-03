using System.Diagnostics;
using System.Net;
using Cassandra;
using Cassandra.Mapping;
using Confluent.Kafka;
using lab_1.Cassa.Dtos.RequestDto;
using lab_1.Cassa.Dtos.RequestDto.RequestConverters;
using lab_1.Cassa.Dtos.ResponseDto;
using lab_1.Cassa.Dtos.ResponseDto.ResponseConverters;
using ISession = Cassandra.ISession;
using TblComment = lab_1.Cassa.Entities.TblComment;

namespace lab_1.Cassa.Services;

public class CommentService : IBaseService<CommentRequestDto, CommentResponseDto>
{
    private ISession _context;
    private Mapper mapper;
    private BaseRequest<TblComment, CommentRequestDto> _request;
    private BaseResponse<CommentResponseDto, TblComment> _response;


    public CommentService()
    {
        _context = Cluster.Builder().AddContactPoint("localhost").WithPort(55001).Build().Connect("distcomp");
        _context.Execute(
            "CREATE  TABLE if not exists tbl_comments (country text,storyId  bigint,id bigint, content text, primary key ((country), id));");
        _request = new();
        _response = new();
        mapper = new (_context);
    }

    public void AddComment(TblComment comment) => mapper.Insert(comment);

    public CommentResponseDto Create(CommentRequestDto dto)
    {
        dto.Id = _context.Execute("select count(*) from tbl_comments").FirstOrDefault().GetValue<long>(0);
        var entity = _request.FromDto(dto);
        mapper.Insert(entity);
        return _response.ToDto(entity);
    }

    public CommentResponseDto? Read(long id)
    {
       return _response.ToDto(mapper.Single<TblComment>("where id = ? and country = \'Belarus\' ", id));
    }

    public CommentResponseDto? Update(CommentRequestDto dto)
    {
        var entity = _request.FromDto(dto);
        mapper.Update(entity);
        return _response.ToDto(entity);
    }

    public bool Delete(long id)
    {
        mapper.Delete(mapper.Single<TblComment>("where id = ? and country = \'Belarus\' ", id));
        return true;
    }

    public IEnumerable<CommentResponseDto> GetAll()
    {
            foreach (var entity in mapper.Fetch<TblComment>())
            {
                yield return _response.ToDto(entity);
            }
    }
    public async Task <bool> SendOrderRequest
        (string topic, string message) {
        ProducerConfig config = new ProducerConfig {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName()
        };

        try {
            using(var producer = new ProducerBuilder
                      <Null, string> (config).Build()) {
                var result = await producer.ProduceAsync
                (topic, new Message <Null, string> {
                    Value = message
                });

                Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
                return await Task.FromResult(true);
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error occured: {ex.Message}");
        }

        return await Task.FromResult(false);
    }

    public void UpdateComment(TblComment comment)
    {
        mapper.Update(comment);
    }
}