using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVDiff
{
    public class CSVColumn
    {
        private const string SPACE_CHAR = "¤";

        public readonly string Value;
        public CSVColumn(string value)
        {
            Value = value.Replace(" ", SPACE_CHAR);
        }
    }
}
