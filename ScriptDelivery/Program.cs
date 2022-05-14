using System;
using System.Text;
using ScriptDelivery;
using ScriptDelivery.Files;
using System.Web;
using ScriptDelivery.Logs;

#region Debug

bool debug = false;
if (debug)
{
    ScriptDelivery.Misc.samplefile.Sample01.Create01();
    Console.ReadLine();
    Environment.Exit(0);
}

#endregion

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

//  Map�t�@�C���̗v���ւ̏���
app.MapPost("/map", async (HttpContext context) =>
{
    string address = $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}";

    Item.Logger.Write(ScriptDelivery.Logs.LogLevel.Info, address, "Post_map", "Send, MapFile");

    context.Response.Headers.Add("App-Version", Item.CurrentVersion);
    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.WriteAsync(Item.MappingFileCollection.Content);
});

//  HttpDownload�t�@�C���̃��X�g�̗v���ւ̏���
app.MapPost("/download/list", async (HttpContext context) =>
{
    string address = $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}";

    if (context.Request.HasJsonContentType())
    {
        Item.Logger.Write(ScriptDelivery.Logs.LogLevel.Info, address, "Post_download_list", "Send, DownloadList");

        List<DownloadFile> dlFileList = await context.Request.ReadFromJsonAsync<List<DownloadFile>>();
        Item.DownloadFileCollection.RequestToResponse(dlFileList);
        await context.Response.WriteAsJsonAsync(dlFileList, options);
    }
    else
    {
        Item.Logger.Write(ScriptDelivery.Logs.LogLevel.Warn, address, "Post_download_list", "Invalid post body. Require Json.");
    }
});

//  HttpDownload�t�@�C���𑗐M
app.MapGet("/download/files", async (HttpContext context) =>
{
    string address = $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}";

    string fileName = context.Request.Query["fileName"];
    string filePath = Path.Combine(Item.Setting.FilesPath, fileName);

    Item.Logger.Write(ScriptDelivery.Logs.LogLevel.Info, address, "Get_download_files", $"Send, File => {fileName}");

    context.Response.Headers.Add("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(fileName));
    await context.Response.SendFileAsync(filePath);
});

#endregion

using (var worker = new SessionWorker())
{
    app.Run();
}


