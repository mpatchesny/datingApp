using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.Notifications;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Application
{
    public static class Extensions
    {
        private const string EmailGeneratorOptionsName = "AccessCodeEmail";
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationAssembly = typeof(ICommandHandler<>).Assembly;
            services.Configure<EmailGeneratorOptions>(configuration.GetRequiredSection(EmailGeneratorOptionsName));

            services.Scan(s => s.FromCallingAssembly()
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddSingleton<IPhotoOrderer, PhotoOrderer>();
            services.AddSingleton<AccessCodeVerificator, AccessCodeVerificator>();
            services.AddSingleton<INotificationMessageGenerator<Email>, EmailGenerator>();
            return services;
        }
    }
}