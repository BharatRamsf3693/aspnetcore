// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Rendering;

namespace Microsoft.AspNetCore.Components.QuickGrid;

/// <summary>
/// Represents a <see cref="QuickGrid{TGridItem}"/> column whose cells render a supplied template.
/// </summary>
/// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
public class TemplateColumn<TGridItem> : ColumnBase<TGridItem>
{
    private static readonly RenderFragment<TGridItem> EmptyChildContent = _ => builder => { };

    /// <summary>
    /// Specifies the content to be rendered for each row in the table.
    /// </summary>
    [Parameter] public RenderFragment<TGridItem> ChildContent { get; set; } = EmptyChildContent;

    /// <inheritdoc/>
    [Parameter] public override GridSort<TGridItem>? SortBy { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Validate that SortBy type matches the grid's item type, if SortBy is specified
        ValidateSortByTypeAtRender();
    }

    /// <inheritdoc />
    protected internal override void CellContent(RenderTreeBuilder builder, TGridItem item)
        => builder.AddContent(0, ChildContent(item));

    /// <inheritdoc />
    protected override bool IsSortableByDefault()
        => SortBy is not null;

    /// <summary>
    /// Validates that SortBy type matches the grid's item type during render phase.
    /// This runs after cascading parameters are injected.
    /// </summary>
    private void ValidateSortByTypeAtRender()
    {
        var sortBy = SortBy;
        if (sortBy is null)
        {
            return; // No sorting specified, nothing to validate
        }

		// Get the grid's actual item type from the non-generic interface
        var gridItemType = InternalGridContext.GridItemType;

        // Extracts the underlying model type (T) from the generic GridSort<T> object
        var sortByItemType = sortBy.GetType().GetGenericArguments()[0];

        // Compare the grid's item type with the SortBy's item type
        if (sortByItemType != gridItemType)
        {
            var columnDescription = string.IsNullOrEmpty(Title)
                ? "Column"
                : $"Column '{Title}'";
            throw new InvalidOperationException(
                $"The {columnDescription} was configured with a GridSort<{sortByItemType.Name}> but the containing QuickGrid uses item type {gridItemType.Name}. " +
                $"The GridSort type must match the QuickGrid item type. Verify the SortBy expression.");
        }
    }
}
