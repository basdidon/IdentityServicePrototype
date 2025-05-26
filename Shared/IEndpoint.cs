using Microsoft.AspNetCore.Routing;

namespace Shared
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
