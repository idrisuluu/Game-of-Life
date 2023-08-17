using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Game_of_Life
{
    public class Game
    {
        private static int _rowCount;
        private static int _colCount;
        private const int MaxRowCount = 60;
        private const int MaxColCount = 230;

        private static int _livingCellCount;
        private static List<string> _lineList;


        // Path'i kendininkine göre değiştir.
        private static string path = "C:\\Users\\idris\\ProjectsC\\Game of Life\\save.txt";



        public static void Main(string[] args)
        {
            Generation generation;

            // If save file exists, game will be continued from the file.
            if (DoesFileExist(path))
            {
                Console.WriteLine("Game loading...");
                Thread.Sleep(3000); // Sleep for 3 sec.
                generation = LoadGame();
            }
            // If there's no save file. User have to input their values:
            // 1- row count,  2- col count,  3- living cell count
            // The constraints for inputs are given below.
            // If user inputs, they'll keep trying until they input correct values.
            else
            {
                Console.WriteLine("Enter a non-negative row_count (has to be less than max_row, which is 60) = ");
                _rowCount = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter a non-negative col_count (has to be less than max_col, which is 230) = ");
                _colCount = Convert.ToInt32(Console.ReadLine());
                while (_rowCount > MaxRowCount || _colCount > MaxColCount || _rowCount <= 0 || _colCount <= 0)
                {
                    Console.WriteLine("Re enter row-col values");
                    Console.WriteLine("Enter a non-negative row_count (has to be less then max_row, which is 60) = ");
                    _rowCount = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter a non-negative col_count (has to be less then max_col, which is 230) = ");
                    _colCount = Convert.ToInt32(Console.ReadLine());
                }
                Console.Clear();
                Console.WriteLine("Enter a positive livingCellCount (has to be less or equal than board size, which is " + _rowCount * _colCount);
                _livingCellCount = Convert.ToInt32(Console.ReadLine());
                while (_livingCellCount > _rowCount * _colCount || _livingCellCount < 0)
                {
                    Console.WriteLine("Re enter livingCellCount value.");
                    Console.WriteLine("Enter a positive livingCellCount (has to be less or equal than board size, which is " + _rowCount * _colCount);
                    _livingCellCount = Convert.ToInt32(Console.ReadLine());
                }
                Console.Clear();
                generation = new Generation(_rowCount, _colCount, _livingCellCount);
                Console.WriteLine("Preparing board...");
                Thread.Sleep(3000); // Sleep for 3 sec.
            }



            /*
            To test constructor that creates board with string characters, you can use the code lines below;
            But use it without getting inputs.
            Kod ayarlaması tamamen sana kalmış. Input almayıp bunu da yapabilirsin.
            */
            /*
            List<string> stringBoard = new List<string>();
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("-----------------------------■■------■■-----------------------------");
            stringBoard.Add("----------------------------■-■------■-■----------------------------");
            stringBoard.Add("----------------------------■----------■----------------------------");
            stringBoard.Add("------------------------■■--■----------■--■■------------------------");
            stringBoard.Add("------------------------■■--■-■--■■--■-■--■■------------------------");
            stringBoard.Add("----------------------------■-■-■--■-■-■----------------------------");
            stringBoard.Add("----------------------------■-■-■--■-■-■----------------------------");
            stringBoard.Add("------------------------■■--■-■--■■--■-■--■■------------------------");
            stringBoard.Add("------------------------■■--■----------■--■■------------------------");
            stringBoard.Add("----------------------------■----------■----------------------------");
            stringBoard.Add("----------------------------■-■------■-■----------------------------");
            stringBoard.Add("-----------------------------■■------■■-----------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");
            stringBoard.Add("--------------------------------------------------------------------");

         */



            // If you press Ctrl + C while game is playing, you'll save and quit the game.
            // Save data will be print on a text file.

            Console.CancelKeyPress += (sender, cancelEventArgs) =>
            {
                Console.WriteLine("\n... Automatic Save activated ... \n" +
                                  "... Quiting ... ");
                SaveGame(generation, path);
            };

            Console.Clear();
            generation.GenerateBoard(1000000);
        }


        // Saving the game state to a file
        static void SaveGame(Generation generation, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                // Storing row and col info, then writing each cell by 0's and 1's.
                _rowCount = generation.SizeRow;
                _colCount = generation.SizeCol;
                writer.Write(_rowCount + ",");
                writer.Write(_colCount + "\n");
                for (int i = 0; i < _rowCount; i++)
                {
                    for (int j = 0; j < _colCount; j++)
                    {
                        if (generation.Board[i, j].CellState == State.Alive)
                        {
                            writer.Write("1 ");
                        }
                        else
                        {
                            writer.Write("0 ");

                        }
                    }
                    writer.Write("\n");
                }
            }
        }

        // Reading it(loading) from a save file
        static Generation LoadGame()
        {
            IEnumerable<string> saveFile = File.ReadLines(path);
            _lineList = new List<string>();
            bool firstIteration = true;
            foreach (string line in saveFile)
            {

                // Get row - col values from file from the first line.
                if (firstIteration)
                {
                    string input = line;
                    string[] splitString = input.Split(',');
                    _rowCount = Convert.ToInt32(splitString[0]);
                    Console.Write(_rowCount);
                    _colCount = Convert.ToInt32(splitString[1]);
                    Console.WriteLine(_colCount);
                    firstIteration = false;
                    continue;
                }
                _lineList.Add(line);
            }

            SetLivingCellCount(_lineList);
            Generation generation = new Generation(_rowCount, _colCount, _livingCellCount);


            int rowInd = 0;
            int colInd = 0;
            for (int i = 0; i < _lineList.Count; i++)
            {
                for (int j = 0; j < _lineList[0].Length; j++)
                {

                    if (_lineList[i][j] == '1')
                    {
                        generation.Board[rowInd, colInd].CellState = State.Alive;
                        colInd++;
                    }
                    else if (_lineList[i][j] == '0')
                    {
                        generation.Board[rowInd, colInd].CellState = State.Dead;
                        colInd++;
                    }

                }

                rowInd++;
                colInd = 0;
            }

            rowInd = 0;
            colInd = 0;
            return generation;


        }

        // Utility function for setting living cell count in LoadGame.
        // !!!! USE IT IN LOAD GAME METHOD !!!!
        static void SetLivingCellCount(List<string> list)
        {
            int rowInd = 0;
            int colInd = 0;
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[0].Length; j++)
                {

                    if (_lineList[i][j] == '1')
                    {
                        _livingCellCount++;
                    }
                    colInd++;

                }

                rowInd++;
                colInd = 0;
            }

            rowInd = 0;
            colInd = 0;
        }


        // Utility function to check if specified file exists on the path, we're going to use it for checking if save file does exist.
        static bool DoesFileExist(string path)
        {
            return File.Exists(path);
        }
    }


}