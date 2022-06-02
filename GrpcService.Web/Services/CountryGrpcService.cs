using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Samples.gRPC;

namespace GrpcService.Web.Services;
using static   Samples.gRPC.CountryService;
public class CountryGrpcService : CountryServiceBase
{
    private readonly CountryManagmentService _countryManagmentService;

    public CountryGrpcService(CountryManagmentService countryManagmentService)
    {
        _countryManagmentService = countryManagmentService;
    }

    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream, IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {


        List<CountryCreationRequest> countryCreationRequests = new();

         await foreach (var item in requestStream.ReadAllAsync())
        {
            
            countryCreationRequests.Add(item);
        }


        var response= await _countryManagmentService.CreateAsync(countryCreationRequests);

        foreach (var item in response)
        {
            await responseStream.WriteAsync(item);
        }
        
        
        
        
        
    }


    public override async Task<Empty> Delete(IAsyncStreamReader<CountryIdRequest> requestStream, ServerCallContext context)
    {
        var countryIdRequestList = new List<CountryIdRequest>();
       await  foreach (var countryIdRequest in requestStream.ReadAllAsync())
        {
            countryIdRequestList.Add(countryIdRequest);
        }

       await _countryManagmentService.DeleteAsync(countryIdRequestList);
       return new Empty();
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

        foreach (var countryReply in countries)
        {
            await responseStream.WriteAsync(countryReply);
        }

        await Task.CompletedTask;
    }
}