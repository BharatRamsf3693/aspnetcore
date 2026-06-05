// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    private readonly Func<IReadOnlyList<object>, int, ValueTask>? _partialUpdateCallback;
    
    /// <summary>
    /// Constructs a new <see cref="ItemsProviderRequest"/> instance.
    /// </summary>
    /// <param name="startIndex">The start index of the data segment requested.</param>
    /// <param name="count">The requested number of items to be provided.</param>
    /// <param name="cancellationToken">
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </param>
    public ItemsProviderRequest(int startIndex, int count, CancellationToken cancellationToken): this(startIndex, count, cancellationToken, null)
    {
    }

    /// <summary>
    /// Internal constructor that includes the partial update callback.
    /// </summary>
    internal ItemsProviderRequest(int startIndex, int count, CancellationToken cancellationToken, Func<IReadOnlyList<object>, int, ValueTask>? partialUpdateCallback)
    {
        StartIndex = startIndex;
        Count = count;
        CancellationToken = cancellationToken;
        _partialUpdateCallback = partialUpdateCallback;
    }

    /// <summary>
    /// Provides items for a partial range of the requested data before the full result is returned.
    /// Can be called multiple times to progressively render items as they become available.
    /// Each call represents a partial segment that will trigger an immediate re-render for the affected range.
    /// </summary>
    /// <typeparam name="TItem">The type of items being provided.</typeparam>
    /// <param name="items">The items to provide for this partial segment.</param>
    /// <param name="startIndex">The start index where these items should be placed in the virtualized list.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the request was created without partial update support (e.g., called outside a provider).
    /// </exception>
#pragma warning disable RS0016 // Add public types and members to the declared API
    public async ValueTask ProvideItems<TItem>(IReadOnlyList<TItem> items, int startIndex)
#pragma warning restore RS0016 // Add public types and members to the declared API
    {
        if (_partialUpdateCallback is null)
        {
            throw new InvalidOperationException(
                "This ItemsProviderRequest does not support partial updates. " +
                "Ensure ProvideItems is only called from within an ItemsProvider delegate.");
        }

        ArgumentNullException.ThrowIfNull(items);

        // Use object list as intermediate since we can't pass generic types through the callback
        var itemList = items.Cast<object>().ToList();
        await _partialUpdateCallback(itemList, startIndex);
    }
}
