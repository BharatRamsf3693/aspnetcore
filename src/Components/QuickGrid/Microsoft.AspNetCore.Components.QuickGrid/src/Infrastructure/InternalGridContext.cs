// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Components.QuickGrid.Infrastructure;

// Non-generic interface to support cascading across different column generic types
internal interface IInternalGridContext
{
    object Grid { get; }
    Type GridItemType { get; }
    EventCallbackSubscribable<object?> ColumnsFirstCollected { get; }
}

// The grid cascades this so that descendant columns can talk back to it. It's an internal type
// so that it doesn't show up by mistake in unrelated components.
internal sealed class InternalGridContext<TGridItem> : IInternalGridContext
{
    public QuickGrid<TGridItem> Grid { get; }
    public EventCallbackSubscribable<object?> ColumnsFirstCollected { get; } = new();
    object IInternalGridContext.Grid => Grid;
    Type IInternalGridContext.GridItemType => typeof(TGridItem);
    public InternalGridContext(QuickGrid<TGridItem> grid)
    {
        Grid = grid;
    }
}
