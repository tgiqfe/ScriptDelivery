using System;
using System.Text;
using ScriptDelivery.Server.ServerLib;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "");

app.MapPost("/", () => "");

app.MapPost("/map", (HttpContext context) =>
{

});

app.MapPost("/download/list", async (HttpContext context) =>
{
    if (context.Request.HasJsonContentType())
    {
        DownloadFileRequest fileReq = await context.Request.ReadFromJsonAsync<DownloadFileRequest>();
        DownloadFileResponse fileRes = Item.DownloadFileCollection.GetResponse(fileReq);
        await context.Response.WriteAsJsonAsync(fileRes);
    }
});

app.MapPost("/download/file", (HttpContext context) =>
{

});


var worker = new SessionWorker();
worker.OnStart();

app.Run();

worker.OnStop();


//  éQçl
//  https://docs.microsoft.com/ja-jp/aspnet/core/web-api/route-to-code?view=aspnetcore-6.0
