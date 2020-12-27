using System;
using System.Collections.Generic;

namespace DL
{
    public class SearchStepEventArgs : EventArgs
    {
        internal SearchStepEventArgs(int iteration, IEnumerable<int> rowIndexes)
        {
            Iteration = iteration;
            RowIndexes = rowIndexes;
        }

        public int Iteration { get; }

        public IEnumerable<int> RowIndexes { get; }
    }
}
