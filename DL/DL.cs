using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DL.EnumerableArrayAdapter;


namespace DL
{
    public class DL
    {
        private readonly CancellationToken _cancellationToken;

        private class SearchData
        {
            public SearchData(ColumnObject root)
            {
                Root = root;
            }

            public ColumnObject Root { get; }
            public int IterationCount { get; private set; }
            public int SolutionCount { get; private set; }

            public void IncrementIterationCount()
            {
                IterationCount++;
            }

            public void IncrementSolutionCount()
            {
                SolutionCount++;
            }

            public void PushCurrentSolutionRowIndex(int rowIndex)
            {
                _currentSolution.Push(rowIndex);
            }

            public void PopCurrentSolutionRowIndex()
            {
                _currentSolution.Pop();
            }

            public Solution CurrentSolution => new Solution(_currentSolution.ToList());

            private readonly Stack<int> _currentSolution = new Stack<int>();
        }

        public DL()
            : this(CancellationToken.None)
        {
        }

        public DL(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public IEnumerable<Solution> Solve<T>(T[,] matrix)
        {
            return Solve(matrix, DefaultPredicate<T>());
        }

        public IEnumerable<Solution> Solve<T>(T[,] matrix, Func<T, bool> predicate)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            return Solve(matrix, m => new Enumerable2DArray<T>(m), r => r, predicate);
        }

        public IEnumerable<Solution> Solve<TData, TRow, TCol>(
            TData data,
            Func<TData, IEnumerable<TRow>> iterateRows,
            Func<TRow, IEnumerable<TCol>> iterateCols)
        {
            return Solve(data, iterateRows, iterateCols, DefaultPredicate<TCol>());
        }

        public IEnumerable<Solution> Solve<TData, TRow, TCol>(
            TData data,
            Func<TData, IEnumerable<TRow>> iterateRows,
            Func<TRow, IEnumerable<TCol>> iterateCols,
            Func<TCol, bool> predicate)
        {
            if (data.Equals(default(TData))) throw new ArgumentNullException(nameof(data));
            var root = BuildInternalStructure(data, iterateRows, iterateCols, predicate);
            return Search(0, new SearchData(root));
        }

        public IEnumerable<Solution> Solve<T>(T[,] matrix, int numPrimaryColumns)
        {
            return Solve(matrix, DefaultPredicate<T>(), numPrimaryColumns);
        }

        public IEnumerable<Solution> Solve<T>(T[,] matrix, Func<T, bool> predicate, int numPrimaryColumns)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            return Solve(matrix, m => new Enumerable2DArray<T>(m), r => r, predicate, numPrimaryColumns);
        }
        public IEnumerable<Solution> Solve<TData, TRow, TCol>(
            TData data,
            Func<TData, IEnumerable<TRow>> iterateRows,
            Func<TRow, IEnumerable<TCol>> iterateCols,
            int numPrimaryColumns)
        {
            if (data.Equals(default(TData))) throw new ArgumentNullException(nameof(data));
            var root = BuildInternalStructure(data, iterateRows, iterateCols, DefaultPredicate<TCol>(), numPrimaryColumns);
            return Search(0, new SearchData(root));
        }

        public IEnumerable<Solution> Solve<TData, TRow, TCol>(
            TData data,
            Func<TData, IEnumerable<TRow>> iterateRows,
            Func<TRow, IEnumerable<TCol>> iterateCols,
            Func<TCol, bool> predicate,
            int numPrimaryColumns)
        {
            if (data.Equals(default(TData))) throw new ArgumentNullException(nameof(data));
            var root = BuildInternalStructure(data, iterateRows, iterateCols, predicate, numPrimaryColumns);
            return Search(0, new SearchData(root));
        }

        public event EventHandler Started;

        public event EventHandler Finished;

        public event EventHandler Cancelled;

        public event EventHandler<SearchStepEventArgs> SearchStep;

        public event EventHandler<SolutionFoundEventArgs> SolutionFound;

        private static Func<T, bool> DefaultPredicate<T>()
        {
            return t => !EqualityComparer<T>.Default.Equals(t, default(T));
        }

        private bool IsCancelled()
        {
            return _cancellationToken.IsCancellationRequested;
        }

        private static ColumnObject BuildInternalStructure<TData, TRow, TCol>(
            TData data,
            Func<TData, IEnumerable<TRow>> iterateRows,
            Func<TRow, IEnumerable<TCol>> iterateCols,
            Func<TCol, bool> predicate,
            int? numPrimaryColumns = null)
        {
            var root = new ColumnObject();

            int? numColumns = null;
            var rowIndex = 0;
            var colIndexToListHeader = new Dictionary<int, ColumnObject>();

            foreach (var row in iterateRows(data))
            {
                DataObject firstDataObjectInThisRow = null;
                var localRowIndex = rowIndex;
                var colIndex = 0;

                foreach (var col in iterateCols(row))
                {
                    if (localRowIndex == 0)
                    {
                        var listHeader = new ColumnObject();
                        var isPrimaryColumn = !numPrimaryColumns.HasValue || colIndex < numPrimaryColumns.Value;
                        if (isPrimaryColumn) root.AppendColumnHeader(listHeader);
                        colIndexToListHeader[colIndex] = listHeader;
                    }

                    if (predicate(col))
                    {
                        var listHeader = colIndexToListHeader[colIndex];
                        var dataObject = new DataObject(listHeader, localRowIndex);

                        if (firstDataObjectInThisRow != null)
                            firstDataObjectInThisRow.AppendToRow(dataObject);
                        else
                            firstDataObjectInThisRow = dataObject;
                    }

                    colIndex++;
                }

                if (numColumns.HasValue)
                {
                    if (colIndex != numColumns)
                    {
                        throw new ArgumentException("All rows must contain the same number of columns!", nameof(data));
                    }
                }
                else
                {
                    numColumns = colIndex;
                }

                rowIndex++;
            }

            return root;
        }

        private static bool MatrixIsEmpty(ColumnObject root)
        {
            return root.NextColumnObject == root;
        }

        private IEnumerable<Solution> Search(int k, SearchData searchData)
        {
            try
            {
                if (k == 0) RaiseStarted();

                if (IsCancelled())
                {
                    RaiseCancelled();
                    yield break;
                }

                RaiseSearchStep(searchData.IterationCount, searchData.CurrentSolution.RowIndexes);
                searchData.IncrementIterationCount();

                if (MatrixIsEmpty(searchData.Root))
                {
                    if (searchData.CurrentSolution.RowIndexes.Any())
                    {
                        searchData.IncrementSolutionCount();
                        var solutionIndex = searchData.SolutionCount - 1;
                        var solution = searchData.CurrentSolution;
                        RaiseSolutionFound(solution, solutionIndex);
                        yield return solution;
                    }

                    yield break;
                }

                var c = ChooseColumnWithLeastRows(searchData.Root);
                CoverColumn(c);

                for (var r = c.Down; r != c; r = r.Down)
                {
                    if (IsCancelled())
                    {
                        RaiseCancelled();
                        yield break;
                    }

                    searchData.PushCurrentSolutionRowIndex(r.RowIndex);

                    for (var j = r.Right; j != r; j = j.Right)
                        CoverColumn(j.ListHeader);

                    var recursivelyFoundSolutions = Search(k + 1, searchData);
                    foreach (var solution in recursivelyFoundSolutions) yield return solution;

                    for (var j = r.Left; j != r; j = j.Left)
                        UncoverColumn(j.ListHeader);

                    searchData.PopCurrentSolutionRowIndex();
                }

                UncoverColumn(c);

            }
            finally
            {
                if (k == 0) RaiseFinished();
            }
        }

        private static ColumnObject ChooseColumnWithLeastRows(ColumnObject root)
        {
            ColumnObject chosenColumn = null;

            for (var columnHeader = root.NextColumnObject; columnHeader != root; columnHeader = columnHeader.NextColumnObject)
            {
                if (chosenColumn == null || columnHeader.NumberOfRows < chosenColumn.NumberOfRows)
                    chosenColumn = columnHeader;
            }

            return chosenColumn;
        }

        private static void CoverColumn(ColumnObject c)
        {
            c.UnlinkColumnHeader();

            for (var i = c.Down; i != c; i = i.Down)
            {
                for (var j = i.Right; j != i; j = j.Right)
                {
                    j.ListHeader.UnlinkDataObject(j);
                }
            }
        }

        private static void UncoverColumn(ColumnObject c)
        {
            for (var i = c.Up; i != c; i = i.Up)
            {
                for (var j = i.Left; j != i; j = j.Left)
                {
                    j.ListHeader.RelinkDataObject(j);
                }
            }

            c.RelinkColumnHeader();
        }

        private void RaiseStarted()
        {
            var handler = Started;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseFinished()
        {
            var handler = Finished;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseCancelled()
        {
            var handler = Cancelled;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseSearchStep(int iteration, IEnumerable<int> rowIndexes)
        {
            var handler = SearchStep;
            handler?.Invoke(this, new SearchStepEventArgs(iteration, rowIndexes));
        }

        private void RaiseSolutionFound(Solution solution, int solutionIndex)
        {
            var handler = SolutionFound;
            handler?.Invoke(this, new SolutionFoundEventArgs(solution, solutionIndex));
        }
    }
}
