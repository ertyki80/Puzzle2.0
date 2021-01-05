using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DL;
using WPF.Model;

namespace WPF
{
    internal class Solver
    {

        private class RowPieces
        {
            public RowPieces(bool[] matrixRow, PiecePlacement piecePlacement)
            {
                MatrixRow = matrixRow;
                PiecePlacement = piecePlacement;
            }

            public bool[] MatrixRow { get; private set; }
            public PiecePlacement PiecePlacement { get; private set; }
        }

        private readonly Piece[] _pieces;
        private readonly Board _board;
        private readonly IList<RowPieces> _data = new List<RowPieces>();
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly DL.DL _dlx;
        private Thread _thread;

        public ConcurrentQueue<SearchStep> SearchSteps { get; private set; }

        public Solver(IEnumerable<Piece> pieces, int boardSize)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _dlx = new DL.DL(_cancellationTokenSource.Token);
            _pieces = pieces.ToArray();
            _board = new Board(boardSize);
            SearchSteps = new ConcurrentQueue<SearchStep>();
        }

        public void Solve()
        {
            _thread = new Thread(SolveOnBackgroundThread);
            _thread.Start();
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _thread.Join();
        }

        private void SolveOnBackgroundThread()
        {
            Thread.CurrentThread.Name = "DL";
            BuildBoard();
            _dlx.SearchStep += (_, e) => SearchSteps.Enqueue(new SearchStep(e.RowIndexes.Select(rowIndex => _data[rowIndex].PiecePlacement)));
            try
            {
                FirstSolution = _dlx.Solve(_data, d => d, r => r.MatrixRow).First();
            }
            catch { }

        }

        public Solution FirstSolution { get; private set; }

        private void BuildBoard()
        {
            for (var pieceIndex = 0; pieceIndex < _pieces.Length; pieceIndex++)
            {
                var piece = _pieces[pieceIndex];
                AddPiece(pieceIndex, piece, Orientation.North);
                var isFirstPiece = (pieceIndex == 0);
                if (!isFirstPiece)
                {
                    AddPiece(pieceIndex, piece, Orientation.South);
                    AddPiece(pieceIndex, piece, Orientation.East);
                    AddPiece(pieceIndex, piece, Orientation.West);
                }
            }
        }

        private void AddPiece(int pieceIndex, Piece piece, Orientation orientation)
        {
            var rotatedPiece = new RotatedPiece(piece, orientation);

            for (var x = 0; x < _board.BoardSize; x++)
            {
                for (var y = 0; y < _board.BoardSize; y++)
                {
                    _board.Reset();
                    if (!_board.PlacePieceAt(rotatedPiece, x, y)) continue;
                    var dataItem = BuildDataItem(pieceIndex, rotatedPiece, new Coords(x, y));
                    _data.Add(dataItem);
                }
            }
        }

        private RowPieces BuildDataItem(int pieceIndex, RotatedPiece rotatedPiece, Coords coords)
        {
            var numColumns = _pieces.Length + _board.BoardSize * _board.BoardSize;
            var matrixRow = new bool[numColumns];

            matrixRow[pieceIndex] = true;

            var w = rotatedPiece.Width;
            var h = rotatedPiece.Height;

            for (var pieceX = 0; pieceX < w; pieceX++)
            {
                for (var pieceY = 0; pieceY < h; pieceY++)
                {
                    if (rotatedPiece.SquareAt(pieceX, pieceY) == null) continue;
                    var boardX = coords.X + pieceX;
                    var boardY = coords.Y + pieceY;
                    var boardLocationColumnIndex = _pieces.Length + (_board.BoardSize * boardX) + boardY;
                    matrixRow[boardLocationColumnIndex] = true;
                }
            }

            return new RowPieces(matrixRow, new PiecePlacement(rotatedPiece, coords));
        }
    }
}
