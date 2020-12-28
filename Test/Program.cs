using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class PuzzleGenerator
    {
        public IEnumerable<string[]> matrix = new[]{
                            // 7
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

        string[] GenerateCubes( int valCubes)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < valCubes; i++)
            {
                    output.Add("1 ");
            }


            return output.ToArray();
        }
        public List<int> GetCountCubes()
        {
            List<int> number = new List<int>();
            int count = 0;
            var list = (matrix.ToList());
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
                    combinations = (Int32)(Math.Pow(2, numbers.Count) - 1);
                    id++;
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


    class Program
    {

       
        static void Main(string[] args)
        {
            PuzzleGenerator puzzleGenerator = new PuzzleGenerator();

            var output = puzzleGenerator.GetSumEquel(121);
            foreach (var s in output)
            {
                foreach (var str in s)
                {

                    Console.WriteLine("|"+str+"|");

                }
                Console.WriteLine("---------------------------------------------------");
            }
            // Pause the program
            Console.ReadKey();

        }
        
    }
}



