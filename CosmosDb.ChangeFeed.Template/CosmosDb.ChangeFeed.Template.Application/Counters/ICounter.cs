﻿namespace CosmosDb.ChangeFeed.Template.Application.Counters;

public interface ICounter
{
    ValueTask<int> GetCountAsync();
}
