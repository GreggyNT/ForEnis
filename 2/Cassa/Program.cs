using Cassandra.Mapping;
using FluentValidation;
using lab_1.Cassa.Services;
using lab_1.Cassa.Dtos.RequestDto;
using lab_1.Cassa.Dtos.ResponseDto;
using lab_1.Cassa.Entities;
using lab_1.Cassa.Services;
using lab_1.Cassa.Services.Validators;
using Microsoft.EntityFrameworkCore;
using TblComment = lab_1.Cassa.Entities.TblComment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddTransient<IBaseService<CommentRequestDto, CommentResponseDto>, CommentService>()
    .AddTransient<IValidator<CommentRequestDto>, CommentValidator>().AddTransient<IHostedService,ConsumerService>();
MappingConfiguration.Global.Define(new Map<TblComment>().TableName("tbl_comments").ClusteringKey(u=>u.Country)
    .PartitionKey(u => u.Id,u=>u.Country).Column(u=>u.Content,cm=>cm.WithName("content")).Column(u=>u.StoryId,cm=>cm.WithName("storyId")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
