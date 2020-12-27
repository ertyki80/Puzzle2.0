using System;
using System.Collections.Generic;
using System.Linq;

namespace WPF.Model
{
    public class Piece
    {
        private readonly IEnumerable<Square> _squares;
        public char Name { get; private set; }
        public int Width
        {
            get
            {
                return _squares.Max(s => s.X) + 1;
            }
        }
        public int Height
        {
            get
            {
                return _squares.Max(s => s.Y) + 1;
            }
        }
        public Piece(IEnumerable<Square> squares, char name = '?')
        {
            _squares = squares;
            Name = name;
        }
        public Piece(string[] initStrings, char name = '?')
        {
           
            int width = initStrings[0].Length;
            int height = initStrings.Length;
            var squares = new List<Square>();
            for (var y = 0; y < height; y++)
            {
                var s = initStrings[y];
                for (var x = 0; x < width; x++)
                {
                    switch (s[x])
                    {
                        case '1':
                            squares.Add(new Square(x, height - y - 1));
                            break;
                        case ' ':
                            break;
                       
                    }
                }
            }
            _squares = squares;
            Name = name;
        }
        public Square SquareAt(int x, int y)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException("y");

            return _squares.FirstOrDefault(s => s.X == x && s.Y == y);
        }
    }
}
