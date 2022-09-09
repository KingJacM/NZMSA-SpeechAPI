using Microsoft.OpenApi.Models;
using System.Reflection;
using WebApplication3.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.EntityFrameworkCore.Design;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<FileDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("GoogleConnection")));
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Google Cloud Storage API",
        Description = "An ASP.NET Core Web API for managing Google Cloud Storage items",
      
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    
}


// Configure the HTTP request pipeline.

    
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
