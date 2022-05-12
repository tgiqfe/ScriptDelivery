using System;
using System.Text;
using ScriptDelivery.Server.ServerLib;
using ScriptDelivery.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var options = new System.Text.Json.JsonSerializerOptions()
{
    IgnoreReadOnlyProperties = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
};

app.MapGet("/", () => "");

app.MapPost("/", () => "");

app.MapPost("/map", () =>
{
    return Item.MappingFileCollection.Content;
});

app.MapPost("/download/list", async (HttpContext context) =>
{
    if (context.Request.HasJsonContentType())
    {
        List<DownloadFile> dlFileList = await context.Request.ReadFromJsonAsync<List<DownloadFile>>();
        Item.DownloadFileCollection.GetResponse(dlFileList);
        await context.Response.WriteAsJsonAsync(dlFileList, options);
    }
});

app.MapGet("/download/files", async (HttpContext context) =>
{
    string fileName = context.Request.Query["fileName"];
    string filePath = Path.Combine(Item.Setting.FilesPath, fileName);
    context.Response.Headers.Add("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(fileName));
    await context.Response.SendFileAsync(filePath);
});


var worker = new SessionWorker();
worker.OnStart();

app.Run();

worker.OnStop();


//  �Q�l
//  https://docs.microsoft.com/ja-jp/aspnet/core/web-api/route-to-code?view=aspnetcore-6.0
