using FrameRecall.DataAccess;
using FrameRecall.DataAccess.Models;
using FrameRecall.Services;

using MongoDB.Driver;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")
    ?? "mongodb://localhost:27017";
string mongoDatabaseName = builder.Configuration["MongoDb:DatabaseName"]
    ?? "FrameRecall";

MongoClient mongoClient = new(mongoConnectionString);
IMongoDatabase database = mongoClient.GetDatabase(mongoDatabaseName);

builder.Services.AddSingleton(database);
builder.Services.AddScoped<IRepository<Film>, FilmRepository>();
builder.Services.AddScoped<IFilmService, FilmService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
