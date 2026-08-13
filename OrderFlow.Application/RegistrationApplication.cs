using OrderFlow.Application.Helper.Attributes;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OrderFlow.Application
{
    public static class RegistrationApplication
    {
        public static IServiceCollection AddRegistrationApplication(this IServiceCollection services)
        {
            Type scopedAttribute = typeof(ScopedAttribute);
            var scopedAttributed = new[] { typeof(RegistrationApplication).Assembly }
            .SelectMany(s => s.GetTypes()
                .Where(a => (a.IsDefined(scopedAttribute) && !a.IsInterface && !a.IsAbstract))
                  .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
                     .ToList());

            foreach (var type in scopedAttributed)
            {
                if (type.assignedType.IsDefined(scopedAttribute, false))
                {
                    foreach (var itemno in type.serviceTypes)
                    {

                        services.AddScoped(itemno, type.assignedType);
                    }
                }
            }

            return services;
        }
    }
}
