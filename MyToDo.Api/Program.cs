using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyToDo.Api;
using MyToDo.Api.Context;
using MyToDo.Api.Context.Repository;
using MyToDo.Api.Extensions;
using MyToDo.Api.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 连接数据库
var connectionString = builder.Configuration.GetConnectionString("ToDoConnection");
builder.Services.AddDbContext<MyToDoContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddUnitOfWork<MyToDoContext>();
//builder.Services.AddCustomRepository<User, UserRepository>();
//builder.Services.AddCustomRepository<ToDo, ToDoRepository>();
//builder.Services.AddCustomRepository<Memo, MemoRepository>();
builder.Services.AddTransient<IToDoService, ToDoService>();
builder.Services.AddTransient<IMemoService, MemoService>();
builder.Services.AddTransient<ILoginService, LoginService>();

var autoMapperConfig = new MapperConfiguration(config =>
{
    config.AddProfile(new AutoMapperProfile());
});
builder.Services.AddSingleton(autoMapperConfig.CreateMapper());

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
