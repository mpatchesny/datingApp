using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.PhotoManagement;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ICommandHandler<>).Assembly;
            services.AddSingleton<PhotoOrderer>();
            services.AddSingleton<StubPhotoService>();
            services.AddScoped<AddPhotoHandler>();
            services.AddScoped<ChangeLocationHandler>();
            services.AddScoped<ChangePhotoOridinalHandler>();
            services.AddScoped<ChangeUserHandler>();
            services.AddScoped<ChangeUserSettingsHandler>();
            services.AddScoped<DeleteMatchHandler>();
            services.AddScoped<DeletePhotoHandler>();
            services.AddScoped<SendMessageHandler>();
            services.AddScoped<SingUpHandler>();
            services.AddScoped<SwipeUserHandler>();
            return services;
        }
    }
}