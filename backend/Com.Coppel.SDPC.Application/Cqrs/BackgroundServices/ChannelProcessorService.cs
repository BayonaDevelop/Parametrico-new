using Com.Coppel.SDPC.Application.Cqrs.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Com.Coppel.SDPC.Application.Cqrs.BackgroundServices;

public class ChannelProcessorService(
		Channel<ICommand> commandChannel,
		IServiceProvider serviceProvider,
		ILogger<ChannelProcessorService> logger) : BackgroundService
{
	private readonly ChannelReader<ICommand> _commandReader = commandChannel.Reader;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("ChannelProcessorService started.");

		await foreach (var command in _commandReader.ReadAllAsync(stoppingToken))
		{
			try
			{
				logger.LogInformation("Processing command: {CommandType}", command.GetType().Name);

				// Create a new scope for each command to ensure isolated dependencies
				using var scope = serviceProvider.CreateScope();
				// Dynamically resolve the correct handler for the command
				var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
				var handler = scope.ServiceProvider.GetService(handlerType);

				if (handler != null)
				{
					// Use reflection to call the HandleAsync method
					var handleMethod = handlerType.GetMethod("HandleAsync");
					if (handleMethod != null)
					{
						await (Task)handleMethod.Invoke(handler, [command])!;
						logger.LogInformation("Command {CommandType} processed successfully.", command.GetType().Name);
					}
					else
					{
						logger.LogError("HandleAsync method not found for command handler {HandlerType}.", handler.GetType().Name);
					}
				}
				else
				{
					logger.LogWarning("No handler found for command type: {CommandType}", command.GetType().Name);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing command {CommandType}: {ErrorMessage}", command.GetType().Name, ex.Message);
				// Depending on requirements, you might want to re-queue, dead-letter, or alert.
			}
		}

		logger.LogInformation("ChannelProcessorService stopped.");
	}
}
