namespace Com.Coppel.SDPC.Application.Cqrs.Interfaces;

public interface ICommandDispatcher
{
	Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
	Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
}
