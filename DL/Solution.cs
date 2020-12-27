using System.Collections.Generic;
using System.Linq;

namespace DL
{
    public class Solution
    {
        internal Solution(IEnumerable<int> rowIndexes)
        {
            RowIndexes = rowIndexes.OrderBy(rowIndex => rowIndex);
        }

        public IEnumerable<int> RowIndexes { get; }
    }
}
