// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;

namespace Microsoft.AspNetCore.Components.Web.Virtualization;

/// <summary>
/// Represents a request to an <see cref="ItemsProviderDelegate{TItem}"/>.
/// </summary>
public readonly struct ItemsProviderRequest
{
    /// <summary>
    /// The start index of the data segment requested.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// The requested number of items to be provided. The actual number of provided items does not need to match
    /// this value.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    // Callback delegate for partial updates
    private readonly Action<IEnumerable<object>, int>? _partialUpdateCallback;

    /// <summary>
    /// Internal constructor that includes the partial update callback.
    /// </summary>
    internal ItemsProviderRequest(int startIndex, int count, CancellationToken cancellationToken, Action<IEnumerable<object>, int>? partialUpdateCallback)
    {
        StartIndex = startIndex;
        Count = count;
        CancellationToken = cancellationToken;
        _partialUpdateCallback = partialUpdateCallback;
    }

    /// <summary>
    /// Provides items for a partial range of the requested data.
    /// Can be called multiple times to progressively render items as they become available.
    /// Each call represents a partial segment that will trigger an immediate re-render for the affected range.
    /// </summary>
    /// <typeparam name="TItem">The type of items being provided.</typeparam>
    /// <param name="items">The partially available items.</param>
    /// <param name="totalItemCount">The total item count in the source generating the items provided.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this ItemsProviderRequest does not support partial updates.
    /// </exception>
#pragma warning disable RS0016 // Add public types and members to the declared API
    public void ProvideItems<TItem>(IEnumerable<TItem> items, int totalItemCount)
#pragma warning restore RS0016 // Add public types and members to the declared API
    {
        if (_partialUpdateCallback is null)
        {
            throw new InvalidOperationException("This ItemsProviderRequest does not support partial updates.");
        }

        ArgumentNullException.ThrowIfNull(items);

        // Use object list as intermediate since we can't pass generic types through the callback
        var itemList = items.Cast<object>().ToList();
        _partialUpdateCallback(itemList, totalItemCount);
    }
}