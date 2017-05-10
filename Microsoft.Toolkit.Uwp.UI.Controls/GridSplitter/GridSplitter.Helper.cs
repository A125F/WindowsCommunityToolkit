﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows of a Grid control.
    /// </summary>
    public partial class GridSplitter
    {
        private static bool IsStarColumn(ColumnDefinition definition)
        {
            return ((GridLength)definition.GetValue(ColumnDefinition.WidthProperty)).IsStar;
        }

        private static bool IsStarRow(RowDefinition definition)
        {
            return ((GridLength)definition.GetValue(RowDefinition.HeightProperty)).IsStar;
        }

        private bool SetColumnWidth(ColumnDefinition columnDefinition, double horizontalChange, GridUnitType unitType)
        {
            var newWidth = columnDefinition.ActualWidth + horizontalChange;

            var minWidth = columnDefinition.MinWidth;
            if (minWidth != double.NaN && newWidth < minWidth)
            {
                newWidth = minWidth;
            }

            var maxWidth = columnDefinition.MaxWidth;
            if (maxWidth != double.NaN && newWidth > maxWidth)
            {
                newWidth = maxWidth;
            }

            if (newWidth > ActualWidth)
            {
                if (columnDefinition.Width.Value != newWidth)
                {
                    columnDefinition.Width = new GridLength(newWidth, unitType);
                    return true;
                }
            }

            return false;
        }

        private bool IsValidColumnWidth(ColumnDefinition columnDefinition, double horizontalChange)
        {
            var newWidth = columnDefinition.ActualWidth + horizontalChange;

            var minWidth = columnDefinition.MinWidth;
            if (minWidth != double.NaN && newWidth < minWidth)
            {
                return false;
            }

            var maxWidth = columnDefinition.MaxWidth;
            if (maxWidth != double.NaN && newWidth > maxWidth)
            {
                return false;
            }

            if (newWidth <= ActualWidth)
            {
                return false;
            }

            return true;
        }

        private bool SetRowHeight(RowDefinition rowDefinition, double verticalChange, GridUnitType unitType)
        {
            var newHeight = rowDefinition.ActualHeight + verticalChange;

            var minHeight = rowDefinition.MinHeight;
            if (minHeight != double.NaN && newHeight < minHeight)
            {
                newHeight = minHeight;
            }

            var maxWidth = rowDefinition.MaxHeight;
            if (maxWidth != double.NaN && newHeight > maxWidth)
            {
                newHeight = maxWidth;
            }

            if (newHeight > ActualHeight)
            {
                if (rowDefinition.Height.Value != newHeight)
                {
                    rowDefinition.Height = new GridLength(newHeight, unitType);
                    return true;
                }
            }

            return false;
        }

        private bool IsValidRowHeight(RowDefinition rowDefinition, double verticalChange)
        {
            var newHeight = rowDefinition.ActualHeight + verticalChange;

            var minHeight = rowDefinition.MinHeight;
            if (minHeight != double.NaN && newHeight < minHeight)
            {
                return false;
            }

            var maxHeight = rowDefinition.MaxHeight;
            if (maxHeight != double.NaN && newHeight > maxHeight)
            {
                return false;
            }

            if (newHeight <= ActualHeight)
            {
                return false;
            }

            return false;
        }

        // Return the targeted Column based on the resize behavior
        private int GetTargetedColumn()
        {
            var currentIndex = Grid.GetColumn(TargetControl);
            return GetTargetIndex(currentIndex);
        }

        // Return the sibling Row based on the resize behavior
        private int GetTargetedRow()
        {
            var currentIndex = Grid.GetRow(TargetControl);
            return GetTargetIndex(currentIndex);
        }

        // Return the sibling Column based on the resize behavior
        private int GetSiblingColumn()
        {
            var currentIndex = Grid.GetColumn(TargetControl);
            return GetSiblingIndex(currentIndex);
        }

        // Return the sibling Row based on the resize behavior
        private int GetSiblingRow()
        {
            var currentIndex = Grid.GetRow(TargetControl);
            return GetSiblingIndex(currentIndex);
        }

        // Gets index based on resize behavior for first targeted row/column
        private int GetTargetIndex(int currentIndex)
        {
            switch (_resizeBehavior)
            {
                case GridResizeBehavior.CurrentAndNext:
                    return currentIndex;
                case GridResizeBehavior.PreviousAndNext:
                    return currentIndex - 1;
                case GridResizeBehavior.PreviousAndCurrent:
                    return currentIndex - 1;
                default:
                    return -1;
            }
        }

        // Gets index based on resize behavior for second targeted row/column
        private int GetSiblingIndex(int currentIndex)
        {
            switch (_resizeBehavior)
            {
                case GridResizeBehavior.CurrentAndNext:
                    return currentIndex + 1;
                case GridResizeBehavior.PreviousAndNext:
                    return currentIndex + 1;
                case GridResizeBehavior.PreviousAndCurrent:
                    return currentIndex;
                default:
                    return -1;
            }
        }

        // Checks the control alignment and Width/Height to detect the control resize direction columns/rows
        private GridResizeDirection GetResizeDirection()
        {
            GridResizeDirection direction = ResizeDirection;

            if (direction == GridResizeDirection.Auto)
            {
                // When HorizontalAlignment is Left, Right or Center, resize Columns
                if (HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    direction = GridResizeDirection.Columns;
                }

                // When VerticalAlignment is Top, Bottom or Center, resize Rows
                else if (VerticalAlignment != VerticalAlignment.Stretch)
                {
                    direction = GridResizeDirection.Rows;
                }

                // Check Width vs Height
                else if (ActualWidth <= ActualHeight)
                {
                    direction = GridResizeDirection.Columns;
                }
                else
                {
                    direction = GridResizeDirection.Rows;
                }
            }

            return direction;
        }

        // Get the resize behavior (Which columns/rows should be resized) based on alignment and Direction
        private GridResizeBehavior GetResizeBehavior()
        {
            GridResizeBehavior resizeBehavior = ResizeBehavior;

            if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
            {
                if (_resizeDirection == GridResizeDirection.Columns)
                {
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case HorizontalAlignment.Right:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }

                // resize direction is vertical
                else
                {
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
                            break;
                        case VerticalAlignment.Bottom:
                            resizeBehavior = GridResizeBehavior.CurrentAndNext;
                            break;
                        default:
                            resizeBehavior = GridResizeBehavior.PreviousAndNext;
                            break;
                    }
                }
            }

            return resizeBehavior;
        }
    }
}
