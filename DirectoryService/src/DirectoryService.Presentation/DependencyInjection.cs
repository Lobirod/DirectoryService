using DirectoryService.Application;

namespace DirectoryService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        return services.AddWebDependencies()
            .AddApplication();
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen();

        return services;
    }
}