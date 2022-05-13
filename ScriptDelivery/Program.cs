using System;
using System.Text;
using ScriptDelivery;
using ScriptDelivery.Files;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var options = new System.Text.Json.JsonSerializerOptions()
{
    //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    IgnoreReadOnlyProperties = true,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    //WriteIndented = true,
    //Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
};

#region Routing

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

#endregion

var worker = new SessionWorker();
worker.OnStart();

app.Run();

worker.OnStop();

