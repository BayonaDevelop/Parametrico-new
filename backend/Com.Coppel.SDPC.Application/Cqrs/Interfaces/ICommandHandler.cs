﻿namespace Com.Coppel.SDPC.Application.Cqrs.Interfaces;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
	Task HandleAsync(TCommand command);
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
	Task<TResult> HandleAsync(TCommand command);
}
