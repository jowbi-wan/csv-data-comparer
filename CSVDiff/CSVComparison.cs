using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CSVDiff
{
    public class CSVComparison
    {
        private int _maxExamples;
        public List<CSVHeader> LeftHeaders = new List<CSVHeader>();
        public List<CSVHeader> RightHeaders = new List<CSVHeader>();

        public readonly string LeftFilename;
        public readonly string RightFilename;
        public Dictionary<string, List<CSVColumnDifference>> Differences = new Dictionary<string, List<CSVColumnDifference>>();
        public bool HasError { get; internal set; } = false;
        public string ErrorMessage { get; internal set; }

        public int LeftFileLines { get; internal set; } = 0;
        public int RightFileLines { get; internal set; } = 0;

        public delegate void ComparisonStatusandler(CSVComparison comparisonTask);
        public event ComparisonStatusandler OnComplete;
        public event ComparisonStatusandler OnStatus;

        public CSVComparison(string leftFile, string rightFile, int maxExamples)
        {
            LeftFilename = leftFile;
            RightFilename = rightFile;
        }

        public async void Compare()
        {
            await Task.Run(() => DoComparison());
        }

        private void DoComparison()
        {
            using (StreamReader left = new StreamReader(File.Open(LeftFilename, FileMode.Open, FileAccess.Read)))
            {
                using (StreamReader right = new StreamReader(File.Open(RightFilename, FileMode.Open, FileAccess.Read)))
                {
                    List<string> messages = new List<string>();

                    if (left.EndOfStream) messages.Add("Left file is empty.");
                    if (right.EndOfStream) messages.Add("Right file is empty.");

                    if (messages.Count > 0)
                    {
                        this.HasError = true;
                        this.ErrorMessage = string.Join(Environment.NewLine, messages.ToArray());
                        OnComplete?.Invoke(this);
                        return;
                    }

                    CSVLine leftLine = new CSVLine(left.ReadLine());
                    CSVLine rightLine = new CSVLine(right.ReadLine());

                    if (leftLine.Columns.Count != rightLine.Columns.Count)
                    {
                        this.HasError = true;
                        this.ErrorMessage = "Selected files do not have the same number of columns. Comparison not possible.";
                        OnComplete?.Invoke(this);
                        return;
                    }

                    int columnIndex = 0;
                    leftLine.Columns.ForEach(x =>
                    {
                        LeftHeaders.Add(new CSVHeader(columnIndex, x.Value));
                        columnIndex++;
                    });

                    columnIndex = 0;
                    rightLine.Columns.ForEach(x =>
                    {
                        RightHeaders.Add(new CSVHeader(columnIndex, x.Value));
                        columnIndex++;
                    });

                    int lineCounter = 1;
                    while (!(left.EndOfStream || right.EndOfStream))
                    {
                        leftLine = new CSVLine(left.ReadLine());
                        rightLine = new CSVLine(right.ReadLine());

                        if (leftLine.Columns.Count != rightLine.Columns.Count)
                        {
                            HasError = true;
                            messages.Add($"Line {lineCounter}: Left file has {leftLine.Columns.Count} columns, but right file has {rightLine.Columns.Count} columns. Skipping comparison.");
                            ErrorMessage = string.Join(Environment.NewLine, messages.ToArray());
                            OnStatus?.Invoke(this);
                            lineCounter++;
                            continue;
                        }

                        for (columnIndex = 0; columnIndex < leftLine.Columns.Count; columnIndex++)
                        {
                            if (Differences.Keys.Contains(LeftHeaders[columnIndex].Description) && Differences[LeftHeaders[columnIndex].Description].Count >= _maxExamples) continue;

                            if (leftLine.Columns[columnIndex].Value != rightLine.Columns[columnIndex].Value)
                            {
                                if (!Differences.Keys.Contains(LeftHeaders[columnIndex].Description)) Differences.Add(LeftHeaders[columnIndex].Description, new List<CSVColumnDifference>());
                                Differences[LeftHeaders[columnIndex].Description].Add(new CSVColumnDifference(columnIndex, LeftHeaders[columnIndex].Description, leftLine.Columns[columnIndex].Value, rightLine.Columns[columnIndex].Value));
                            }

                        }

                        lineCounter++;
                    }

                    if (left.EndOfStream && !right.EndOfStream)
                    {
                        HasError = true;
                        messages.Add("Left file has fewer records.");
                        ErrorMessage = string.Join(Environment.NewLine, messages.ToArray());
                        OnComplete?.Invoke(this);
                        return;
                    }

                    if (right.EndOfStream && !left.EndOfStream)
                    {
                        HasError = true;
                        messages.Add("Right file has fewer records.");
                        ErrorMessage = string.Join(Environment.NewLine, messages.ToArray());
                        OnComplete?.Invoke(this);
                        return;
                    }

                    ErrorMessage = string.Join(Environment.NewLine, messages.ToArray());

                }
            }

            OnComplete?.Invoke(this);
        }

    }
}
