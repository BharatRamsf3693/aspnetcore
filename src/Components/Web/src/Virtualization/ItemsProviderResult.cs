// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;

namespace Microsoft.AspNetCore.Components.Web.Virtualization;

/// <summary>
/// Represents the result of a <see cref="ItemsProviderDelegate{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The type of the context for each item in the list.</typeparam>
public readonly struct ItemsProviderResult<TItem>
{
    /// <summary>
    /// The items to provide.
    /// </summary>
    public IEnumerable<TItem> Items { get; }

    /// <summary>
    /// The total item count in the source generating the items provided.
    /// </summary>
    public int TotalItemCount { get; }

    /// <summary>
    /// Instantiates a new <see cref="ItemsProviderResult{TItem}"/> instance.
    /// </summary>
    /// <param name="items">The items to provide.</param>
    /// <param name="totalItemCount">The total item count in the source generating the items provided.</param>
    public ItemsProviderResult(IEnumerable<TItem> items, int totalItemCount)
    {
        Items = items;
        TotalItemCount = totalItemCount;
    }

    /// <summary>
    /// Instantiates a new <see cref="ItemsProviderResult{TItem}"/> instance.
    /// </summary>
#pragma warning disable RS0016 // Add public types and members to the declared API
    public ItemsProviderResult() : this(Enumerable.Empty<TItem>(), 0)
#pragma warning restore RS0016 // Add public types and members to the declared API
    {
    }
}
