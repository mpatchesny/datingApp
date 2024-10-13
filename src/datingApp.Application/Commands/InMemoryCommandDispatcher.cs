using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands;

public class InMemoryCommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    public InMemoryCommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");
        }
        await handler.HandleAsync(command);
    }
}