using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Helper.Attributes;
using System.Reflection;

namespace OrderFlow.Infrastructure
{
    public static class RegistrationInfrastructure
    {
        public static IServiceCollection AddRegistrationInfrastructure(this IServiceCollection services)
        {
            Type scopedAttribute = typeof(ScopedAttribute);
            var scopedAttributed = new[] { typeof(RegistrationInfrastructure).Assembly }
                .SelectMany(s => s.GetTypes()
                    .Where(a => (a.IsDefined(scopedAttribute) && !a.IsInterface && !a.IsAbstract))
                    .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
                    .ToList());

            foreach (var type in scopedAttributed)
            {
                if (type.assignedType.IsDefined(scopedAttribute, false))
                {
                    foreach (var item in type.serviceTypes)
                    {

                        services.AddScoped(item, type.assignedType);
                    }
                }
            }

            return services;
        }

    }
}
