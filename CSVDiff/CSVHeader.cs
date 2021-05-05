using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVDiff
{
    public class CSVHeader
    {
        public readonly int Number;
        public readonly string Description;

        public CSVHeader(int index, string value)
        {
            Number = index;
            Description = value;
        }
    }
}
