using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcService.Web.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;
    private Guid _correlationId;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
       
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, 
    IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
             await continuation(request,responseStream, context);
        }
        catch (Exception e)
        {
            _correlationId = Guid.NewGuid();
            _logger.LogError(e, $"Correlation Id :{_correlationId}");

            var trailers = new Metadata { { "CorrelationId", _correlationId.ToString() } };

            throw new RpcException(new Status(StatusCode.Internal,
                    $"Error sent to client with correlation Id : {_correlationId}"), trailers,
                "Error message that will appear in log server");
        }
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext
            context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception e)
        {
            _correlationId = Guid.NewGuid();
            _logger.LogError(e, $"Correlation Id :{_correlationId}");

            var trailers = new Metadata { { "CorrelationId", _correlationId.ToString() } };

            throw new RpcException(new Status(StatusCode.Internal,
                    $"Error sent to client with correlation Id : {_correlationId}"), trailers,
                "Error message that will appear in log server");
        }
    }
}