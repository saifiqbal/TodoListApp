using TodoList.Application.Services;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Persistence;
using TodoList.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Service//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Todo List API", 
        Version = "v1",
        Description = "A RESTful API for managing todo items"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS //
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var dataPath = builder.Configuration.GetValue<string>("DataFilePath") 
               ?? Path.Combine(AppContext.BaseDirectory, "Data", "todos.json");

builder.Services.AddScoped<ITodoRepository>(sp => new JsonFileRepository(dataPath));
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo List API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();