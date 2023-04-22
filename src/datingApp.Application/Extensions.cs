using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Commands;
using datingApp.Application.Commands.Handlers;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Security;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ICommandHandler<>).Assembly;
            services.AddSingleton<IPhotoOrderer, PhotoOrderer>();
            services.AddScoped<ICommandHandler<SignUp>, SignUpHandler>();
            services.AddScoped<ICommandHandler<ChangeUser>, ChangeUserHandler>();
            services.AddScoped<ICommandHandler<ChangeLocation>, ChangeLocationHandler>();
            services.AddScoped<ICommandHandler<AddPhoto>, AddPhotoHandler>();
            services.AddScoped<ICommandHandler<ChangePhotoOridinal>, ChangePhotoOridinalHandler>();
            services.AddScoped<ICommandHandler<DeletePhoto>, DeletePhotoHandler>();
            services.AddScoped<ICommandHandler<DeleteMatch>, DeleteMatchHandler>();
            services.AddScoped<ICommandHandler<SwipeUser>, SwipeUserHandler>();
            services.AddScoped<ICommandHandler<SendMessage>, SendMessageHandler>();
            services.AddScoped<ICommandHandler<DeleteUser>, DeleteUserHandler>();
            services.AddScoped<ICommandHandler<SetMessagesAsDisplayed>, SetMessagesAsDisplayedHandler>();
            services.AddScoped<ICommandHandler<SetMatchAsDisplayed>, SetMatchAsDisplayedHandler>();
            services.AddSingleton<AccessCodeVerificator, AccessCodeVerificator>();
            services.AddScoped<ICommandHandler<RequestEmailAccessCode>, RequestEmailAccessCodeHandler>();
            services.AddScoped<ICommandHandler<SignInByEmail>, SignInByEmailHandler>();
            services.AddScoped<ICommandHandler<AddMatch>, AddMatchHandler>();
            return services;
        }
    }
}