
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Samples.gRPC.v1;
AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Error);
});


var channel = GrpcChannel.ForAddress("http://localhost:5000",new GrpcChannelOptions()
{
    LoggerFactory = loggerFactory
});



var countryClient= new CountryService.CountryServiceClient(channel);

using var serverStreamingCall= countryClient.GetAll(new Empty());
await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"{response.Name} {response.Description}");
}

