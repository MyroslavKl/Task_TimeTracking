using TimeTracking.API.Data;
using TimeTracking.API.Repositories.Interfaces;
using TimeTracking.API.Repositories;
using TimeTracking.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();

var app = builder.Build();

await app.EnsureDatabaseCreated();

app.UseMiddleware<ExceptionHandlingMiddleware>(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();