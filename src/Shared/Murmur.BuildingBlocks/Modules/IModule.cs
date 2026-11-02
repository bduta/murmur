using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace Murmur.BuildingBlocks.Modules;

/// <summary>
/// Entry point of a module (ADR-0004, ADR-0020). The implementation is the only public type in a module's
/// implementation assembly; the API host registers each module explicitly.
/// </summary>
public interface IModule
{
    /// <summary>Registers the module's services: its DbContext, handlers, validators and event handlers.</summary>
    void AddServices(IHostApplicationBuilder builder);

    /// <summary>Maps the module's endpoints. <paramref name="api"/> is already prefixed with <c>/api</c>.</summary>
    void MapEndpoints(IEndpointRouteBuilder api);
}
