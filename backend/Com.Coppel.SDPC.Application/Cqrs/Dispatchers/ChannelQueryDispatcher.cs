using Com.Coppel.SDPC.Application.Cqrs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Coppel.SDPC.Application.Cqrs.Dispatchers;

public class ChannelQueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
	/// <summary>
	/// Dispatches a query and returns its result by resolving and executing the appropriate handler.
	/// </summary>
	public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
	{
		using var scope = serviceProvider.CreateScope();
		var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
		return await handler.HandleAsync(query);
	}
}
