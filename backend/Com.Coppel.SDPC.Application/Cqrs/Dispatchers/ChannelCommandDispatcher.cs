using Com.Coppel.SDPC.Application.Cqrs.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace Com.Coppel.SDPC.Application.Cqrs.Dispatchers;

public class ChannelCommandDispatcher(Channel<ICommand> commandChannel, IServiceProvider serviceProvider) : ICommandDispatcher
{
	private readonly ChannelWriter<ICommand> _commandWriter = commandChannel.Writer;

	/// <summary>
	/// Dispatches a command without a return value. Enqueues it for background processing.
	/// </summary>
	public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
	{
		await _commandWriter.WriteAsync(command);
	}

	/// <summary>
	/// Dispatches a command that expects a return value.
	/// This type of command cannot be truly asynchronous via channel for its result,
	/// so it's handled immediately by resolving its handler.
	/// In a real-world scenario with pure async commands, you might use a callback mechanism
	/// or a separate channel for results, but for simplicity and immediate feedback,
	/// commands with results are processed synchronously here.
	/// </summary>
	public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
	{
		// For commands that return a result, we need to execute them immediately
		// to provide the result back to the caller.
		// This is a common pattern when using channels for fire-and-forget vs. request-response.
		using (var scope = serviceProvider.CreateScope())
		{
			var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
			return await handler.HandleAsync(command);
		}
	}
}
