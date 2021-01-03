using System;
using System.Collections.Generic;
using System.Linq;

namespace WPF.Model
{
    public static class Pieces
    {
        public static IList<Piece> ThePieces;
        static Pieces()
        {
            PuzzleGenerator puzzleGenerator = new PuzzleGenerator();
            ThePieces = (IList<Piece>)CreatePieces(puzzleGenerator.GetSumEquel(MainWindow.Size * MainWindow.Size));

        }
        private static Piece[] CreatePieces(IEnumerable<string[]> data)
        {
            var name = 'A';
            return data.Select(initStrings => new Piece(initStrings, name++)).ToArray();
        }
    }
    public class PuzzleGenerator
    {
        public IEnumerable<string[]> matrix = new[]{
                            // 7
                            new[]{
                                    "1 ",
                                    "11",
                                    "11",
                                    "11",
                                    "11"
                                },
                            new[]{
                                    "11",
                                    "1 ",
                                    "11",
                                    "11"
                                },
                            // 7
                            
                            // 6
                            new[]{
                                    "11",
                                    "1 ",
                                    "1 ",
                                    "11"
                                },

                            // 6
                            

                            
                            // 5
                            new[]
                                {
                                    "1 ",
                                    "11",
                                    " 1",
                                    " 1"
                                },
                            // 5
                            new[]
                                {
                                    " 11",
                                    " 1 ",
                                    "11 "
                                },
                            
                            // 5
                            new[]
                                {
                                    " 1",
                                    " 1",
                                    " 1",
                                    "11"
                                },
                            // 5
                            new[]
                                {
                                    "1 ",
                                    "11",
                                    "1 ",
                                    "1 "
                            },
                            // 4
                            new[]
                                {
                                    "11 ",
                                    " 11"
                                },

                            // 4
                            new[]
                                {
                                    "1  ",
                                    "111"
                                },
                             
                            // 4
                            new[]
                                {
                                    "1 ",
                                    "11",
                                    "1 "
                                },
                            // 4
                            new[]
                                {
                                    "1 ",
                                    "1 ",
                                    "11"
                                },
                             //3
                            new[]
                                {
                                    "1 ",
                                    "11"
                                },
                             
                             
                            // 3
                            new[]
                                {
                                    "1 ",
                                    "1 ",
                                    "1 "
                                },
                            
                             //3
                            new[]
                                {
                                    "1 ",
                                    "11"
                                },
                             //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                             //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                             //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                            //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                            
                             //1
                            new[]
                                {
                                    "1 "
                                },
                            //1
                            new[]
                                {
                                    "1 "
                                },
                             //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                            //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                            //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                            //2
                            new[]
                                {
                                    "1 ",
                                    "1 "
                                },
                             //1
                            new[]
                                {
                                    "1 "
                                },
                            //1
                            new[]
                                {
                                    "1 "
                                }
        };
        public List<int> GetCountCubes()
        {
            List<int> number = new List<int>();
            int count = 0;
            var list = matrix.ToList();
            foreach (var s in list)
            {
                foreach (var str in s)
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == '1')
                        {
                            count++;
                        }
                    }
                }
                number.Add(count);
                count = 0;
            }
            return number;
        }
        public List<string[]> GetSumEquel(int target_sum)
        {
            var numbers = GetCountCubes();
            List<Int32[]> output_indexes = new List<Int32[]>();
            List<Int32[]> output_numbers = new List<Int32[]>();
            Int32 combinations = (Int32)(Math.Pow(2, numbers.Count) - 1);
            int id = 0;
            for (int i = 0; i < combinations; i++)
            {
                List<Int32> subset = new List<Int32>();
                List<Int32> subindexes = new List<Int32>();
                for (int j = 0; j < numbers.Count; j++)
                {
                    if (((i & (1 << j)) >> j) == 1)
                    {
                        subset.Add(numbers[j]);
                        subindexes.Add(j);
                    }
                }
                if (subset.Sum() == target_sum)
                {
                    output_indexes.Add(subindexes.ToArray());
                    output_numbers.Add(subset.ToArray());
                    break;
                }
                if (output_indexes.Count == 0 && i == combinations - 1)
                {
                    numbers.Add(numbers[id]);
                    if (id == 0)
                    {
                        id = numbers.Count-1;
                    }
                    id++;
                    combinations = (Int32)(Math.Pow(2, numbers.Count) - 1);
                    i = 0;
                    

                }
            }
            List<string[]> output = new List<string[]>();

            var list = matrix.ToList();
            int counter = 0;
            foreach (var indexOUT in output_indexes)
            {
                for (int i = 0; i < indexOUT.Length; i++)
                {
                    int ind = (Convert.ToInt32(indexOUT[i]));

                    foreach (var s in list)
                    {
                        if (counter == ind)
                        {
                            output.Add(s);
                        }
                        counter++;

                    }
                    counter = 0;
                }
            }

            return output;
        }


    }

}
