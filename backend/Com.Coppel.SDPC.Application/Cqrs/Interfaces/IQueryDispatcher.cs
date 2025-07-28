namespace Com.Coppel.SDPC.Application.Cqrs.Interfaces;

public interface IQueryDispatcher
{
	Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
}
