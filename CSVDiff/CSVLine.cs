using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVDiff
{
    public class CSVLine
    {
        public List<CSVColumn> Columns = new List<CSVColumn>();

        public CSVLine(string line)
        {
            List<string> raw = line.Split(new char[] { ',' }).ToList();
            string temp = "";
            bool openQuote = false;

            foreach(string value in raw)
            {
                if (openQuote)
                {
                    temp += ("," + value);
                    openQuote = value.EndsWith("\"");
                }
                else
                {
                    temp = value;
                    openQuote = value.StartsWith("\"") && !value.EndsWith("\"");
                }

                if (!openQuote)
                {
                    Columns.Add(new CSVColumn(temp));
                }
            }
        }

    }
}
