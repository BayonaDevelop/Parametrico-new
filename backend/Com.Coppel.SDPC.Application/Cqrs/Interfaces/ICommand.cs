namespace Com.Coppel.SDPC.Application.Cqrs.Interfaces;

public interface ICommand;

public interface ICommand<out TResult> : ICommand;
