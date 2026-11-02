using Murmur.SharedKernel.Modules;

namespace Murmur.Api.Modules;

internal static class ModuleExtensions
{
    public static IHostApplicationBuilder AddModules(this IHostApplicationBuilder builder, IReadOnlyList<IModule> modules)
    {
        foreach (IModule module in modules)
        {
            module.AddServices(builder);
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder app, IReadOnlyList<IModule> modules)
    {
        RouteGroupBuilder api = app.MapGroup("/api");

        foreach (IModule module in modules)
        {
            module.MapEndpoints(api);
        }

        return app;
    }
}
