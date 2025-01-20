#if UNITY_2019_4_OR_NEWER
using System;

namespace YooAsset.Editor
{
    public class BooleanValueCell : ITableCell, IComparable
    {
        public object CellValue { set; get; }
        public string HeaderTitle { private set; get; }
        public bool BooleanValue
        {
            get
            {
                return (bool)CellValue;
            }
        }

        public BooleanValueCell(string headerTitle, object cellValue)
        {
            HeaderTitle = headerTitle;
            CellValue = cellValue;
        }
        public object GetDisplayObject()
        {
            return CellValue.ToString();
        }
        public int CompareTo(object other)
        {
            if (other is BooleanValueCell cell)
            {
                return this.BooleanValue.CompareTo(cell.BooleanValue);
            }
            else
            {
                return 1;
            }
        }
    }
}
#endif