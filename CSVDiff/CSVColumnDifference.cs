using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVDiff
{
    public class CSVColumnDifference
    {
        public readonly int ColumnIndex;
        public readonly string ColumnHeader;
        public readonly string LeftValue;
        public readonly string RightValue;

        public CSVColumnDifference(int columnIndex, string header, string leftValue, string rightValue)
        {
            ColumnIndex = columnIndex;
            ColumnHeader = header;
            LeftValue = leftValue;
            RightValue = rightValue;
        }
    }
}
