namespace UserService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}