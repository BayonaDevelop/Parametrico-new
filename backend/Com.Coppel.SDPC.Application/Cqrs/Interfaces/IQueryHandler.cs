﻿namespace Com.Coppel.SDPC.Application.Cqrs.Interfaces;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
	Task<TResult> HandleAsync(TQuery query);
}
