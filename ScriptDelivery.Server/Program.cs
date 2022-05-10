using System;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "");

app.MapPost("/", () => "");

app.MapPost("/map", (HttpContext context) =>
{

});

app.MapPost("/download/list", (HttpContext context) =>
{
    
});

app.MapPost("/download/get", (HttpContext context) =>
{

});


app.Run();
