using Confluent.Kafka;
using lab_1.Cassa.Dtos.RequestDto;
using lab_1.Cassa.Dtos.ResponseDto;
using Newtonsoft.Json;
using TblComment = lab_1.Cassa.Entities.TblComment;

namespace lab_1.Cassa.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly string topic = "inTopic";
        private readonly string groupId = "test";
        private readonly string bootstrapServers = "localhost:9092";
        private readonly IConsumer<int, string> consumer;
        private readonly IBaseService<CommentRequestDto,CommentResponseDto> _commentService;

        public ConsumerService(IBaseService<CommentRequestDto,CommentResponseDto> commentService)
        {
            var conf = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            consumer = new ConsumerBuilder<int, string>(conf).Build();
            _commentService = commentService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        private TblComment StartConsumerLoop(CancellationToken cancellationToken)
        {
            TblComment? res = null;
            List<TblComment> resLit = new List<TblComment?>();
            int id = 0;
            consumer.Subscribe(topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(cancellationToken);

                    switch (cr.Message.Key)
                    {
                        case 0:
                            res = JsonConvert.DeserializeObject<TblComment>(cr.Message.Value);
                            _commentService.AddComment(res);
                            break;
                        case 1:
                            id = JsonConvert.DeserializeObject<int>(cr.Message.Value);
                            _commentService.SendOrderRequest("outTopic",
                                JsonConvert.SerializeObject(_commentService.Read(id)));
                            break;
                        case 2:
                            id = JsonConvert.DeserializeObject<int>(cr.Message.Value);
                            _commentService.SendOrderRequest("outTopic",
                                JsonConvert.SerializeObject(_commentService.Delete(id)));
                            break;
                        case 3:
                            res = JsonConvert.DeserializeObject<TblComment>(cr.Message.Value);
                            _commentService.UpdateComment(res);
                            _commentService.SendOrderRequest("outTopic", JsonConvert.SerializeObject(_commentService.Read(res.Id)));
                            break;
                        case 4:
                            _commentService.SendOrderRequest("outTopic", JsonConvert.SerializeObject(_commentService.GetAll()));
                            break;
                    }


                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
                
            }

            return res??new TblComment();
        }
        public override void Dispose()
        {
            consumer.Close(); // Commit offsets and leave the group cleanly.
            consumer.Dispose();

            base.Dispose();
        }
    }
    
}