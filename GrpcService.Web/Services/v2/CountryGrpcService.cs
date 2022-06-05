using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Web.ManagmentService.v2;
using Samples.gRPC.v2;

namespace GrpcService.Web.Services.v2;

public class CountryGrpcService : Samples.gRPC.v2.CountryService.CountryServiceBase
{
    private readonly CountryManagmentService _countryManagmentService;
    private readonly ILogger<CountryGrpcService> _logger;

    public CountryGrpcService(CountryManagmentService countryManagmentService, ILogger<CountryGrpcService> logger)
    {
        _countryManagmentService = countryManagmentService;
        _logger = logger;
    }

    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream,
        IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {
        List<CountryCreationRequest> countryCreationRequests = new();
        await foreach (var item in requestStream.ReadAllAsync()) countryCreationRequests.Add(item);
        var response = await _countryManagmentService.CreateAsync(countryCreationRequests);
        foreach (var item in response) await responseStream.WriteAsync(item);
    }

  

    public override async Task<Empty> Update(CountryUpdateRequest request, ServerCallContext context)
    {
        await _countryManagmentService.UpdateAsync(request);
        return new Empty();
    }

    public override async Task<CountryReply?> Get(CountryIdRequest request, ServerCallContext context)
    {
        return await _countryManagmentService.GetAsync(request);
    }

    public override async Task GetAll(Empty request, IServerStreamWriter<CountryReply> responseStream, ServerCallContext
        context)
    {
        
        var countries = await _countryManagmentService.GetAllAsync();
        foreach (var countryReply in countries) await responseStream.WriteAsync(countryReply);
        await Task.CompletedTask;
    }
}