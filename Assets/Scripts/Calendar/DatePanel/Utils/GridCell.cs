using System;
using Utils;

namespace Calendar.DatePanel.Utils
{
    /// <summary>
    /// Ячейка таблицы
    /// </summary>
    public struct GridCell
    {
        public int Row;
        public int Column;

        public GridCell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Возвращает ячейку из ее табличного индекса 
        /// </summary>
        public static GridCell FromTotalIndex(int totalIndex, in GridCell GridSize)
            => new GridCell(totalIndex / Params.GridSize.Column, totalIndex % Params.GridSize.Column);
    }
    
    public static class GridCellExtensions
    {
        public static int GetTotalIndex(this GridCell cell, GridCell size)
        {
            return cell.Row * size.Column + cell.Column;
        }
    }
}