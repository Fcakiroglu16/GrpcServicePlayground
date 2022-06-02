using GrpcService.Web.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    
    options.ListenLocalhost(5000, o => o.Protocols =HttpProtocols.Http2);
});

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(options =>
{
 //    options.MaxReceiveMessageSize = 1024 * 1024 * 6; // 6 mb
 // options.MaxSendMessageSize= 1024 * 1024 * 6;

});
builder.Services.AddSingleton<CountryManagmentService>();
var app = builder.Build();
app.MapGrpcService<CountryGrpcService>();
// Configure the HTTP request pipeline.
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();