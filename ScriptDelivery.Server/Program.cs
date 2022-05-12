using System;
using System.Text;
using ScriptDelivery.Server.ServerLib;
using ScriptDelivery.Net;

/*
var setting = Setting.Deserialize("setting.json");
setting.Serialize("setting.json");
Console.ReadLine();
Environment.Exit(0);
*/

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "");

app.MapPost("/", () => "");

app.MapPost("/map", () =>
{
    return Item.MappingFileCollection.Content;
});

var options = new System.Text.Json.JsonSerializerOptions()
{
    IgnoreReadOnlyProperties = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
};

app.MapPost("/download/list", async (HttpContext context) =>
{
    if (context.Request.HasJsonContentType())
    {
        List<DownloadFile> dlFileList = await context.Request.ReadFromJsonAsync<List<DownloadFile>>();
        Item.DownloadFileCollection.GetResponse(dlFileList);
        await context.Response.WriteAsJsonAsync(dlFileList, options);
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
