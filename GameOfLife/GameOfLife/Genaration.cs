using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Game_of_Life
{
    public class Generation
    {
        public int SizeRow;
        public int SizeCol;
        private Random _random;

        private Cell[] _cells;
        public Cell[,] Board;

        public char cellChar = '■';
        public char deadCellChar = ' ';

        /* Constructors */
        /* READ FIRST CONSTRUCTOR FIRST, THEN SECOND */



        // Creating the generation by its row size, column size and its living cell count.
        public Generation(int sizeRow, int sizeCol, int livingCellCount)
        {
            _random = new Random();
            SizeRow = sizeRow;
            SizeCol = sizeCol;
            Board = new Cell[sizeRow, sizeCol];


            // Double click for description
            FillCells();


            // If there was a problem for initializing Cells, in FillCells method, I'll use this if statement to prevent any NullException.
            if (_cells == null)
            {
                return;
            }

            // Loop for generating random state-cells.
            // If it creates any living cell, then we decrement livingCellCount; so when all of the livingCells are done;
            // It keeps creating dead cells instead.
            foreach (Cell cell in _cells)
            {
                if (livingCellCount > 0)
                {
                    int state = _random.Next(2);
                    if (state == 0)
                    {
                        cell.CellState = State.Dead;
                    }
                    else
                    {
                        cell.CellState = State.Alive;
                        livingCellCount--;
                    }

                }
                else
                {
                    cell.CellState = State.Dead;
                }
            }

            // Double click for description
            FillBoard();
        }

        // Creating the generation by its row size, column size and its living cell density.
        // If density is less than 0, it will adjust it to 0; if density is more than 1.0, it will adjust it to 1.0
        public Generation(int sizeRow, int sizeCol, double density)
        {
            if (density < 0.0)
            {
                density = 0;
            }

            if (density > 1.0)
            {
                density = 1.0;
            }

            _random = new Random();
            SizeRow = sizeRow;
            SizeCol = sizeCol;
            Board = new Cell[sizeRow, sizeCol];


            // Double click for description
            FillCells();


            // Density is = LivingCellCount / BoardArea, So I'll generate livingCellCount in here instead.
            // Remaining code is the same as the previous constructor.

            int area = SizeRow * SizeCol;
            int livingCellCount = (int)(density * area);


            if (_cells == null)
            {
                return;
            }

            foreach (Cell cell in _cells)
            {
                if (livingCellCount > 0)
                {
                    int state = _random.Next(2);
                    if (state == 0)
                    {
                        cell.CellState = State.Dead;
                    }
                    else
                    {
                        cell.CellState = State.Alive;
                    }
                    livingCellCount--;

                }
                else
                {
                    cell.CellState = State.Dead;
                }
            }

            // Double click for description
            FillBoard();
        }

        // Creating the generation by a string map/board of characters
        public Generation(List<string> stringBoard)
        {
            _random = new Random();
            // Double click the methods for description.
            SizeRow = getRowSizeFromString(stringBoard);
            SizeCol = getColSizeFromString(stringBoard);
            Board = new Cell[SizeRow, SizeCol];

            // Double click for description
            FillCells();
            FillBoard();

            if (_cells == null)
            {
                return;
            }


            // These loop below, simply places Alive Cells when it occurs a character specified in IsValidAliveChar()
            // and places Dead Cells when it occurs a character specified in IsValidDeadChar()
            int rowIndex = 0;
            int colIndex = 0;
            foreach (string str in stringBoard)
            {
                int index = 0;
                while (index < SizeCol)
                {
                    if (IsValidAliveChar(str[index]))
                    {
                        Board[rowIndex, colIndex].CellState = State.Alive;
                    }
                    else if (IsValidDeadChar(str[index]))
                    {
                        Board[rowIndex, colIndex].CellState = State.Dead;
                    }

                    index++;
                    colIndex++;
                }

                colIndex = 0;
                rowIndex++;

            }
        }


        // Empty constructor
        public Generation()
        {
            _random = new Random();
        }






        /* METHODS - FUNCTIONS */



        // Utility Functions for creating the next game state
        public void NextGeneration()
        {
            Cell[,] nextBoard = new Cell[SizeRow, SizeCol];

            // Double click for description
            FillGenerationDefault(nextBoard, SizeRow, SizeCol);

            for (int rowIndex = 1; rowIndex < SizeRow - 1; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex < SizeCol - 1; columnIndex++)
                {
                    // This whole block represents for looking all alive neighbours of a cell.
                    // That's the reason we started rowIndex and columnIndex as 1 and loop count as their onw sizes - 1.
                    // Because we want to check specific cells;
                    //   -Starting from second row and second column, until rowSize - 1'th row and colSize - 1'th column.
                    //  (Because these cells mentioned have 4 neighbours)
                    // When we found any living cell, we count them.


                    // Starting loop indexes from -1 is only about getting the south-north and west-east neighbours of a cell
                    int aliveNeighbors = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (Board[rowIndex + i, columnIndex + j].CellState == State.Alive)
                            {
                                aliveNeighbors += 1;
                            }
                        }
                    }

                    // The cell needs to be subtracted from its neighbors, because it was counted before
                    Cell currentCell = Board[rowIndex, columnIndex];
                    if (currentCell.CellState == State.Alive)
                    {
                        aliveNeighbors -= 1;

                    }


                    // GAME OF LIFE RULES
                    // 1 - Any live cell with two or three live neighbours survives.
                    // 2 - Any dead cell with three live neighbours becomes a live cell.
                    // 3 - All other live cells die in the next generation. Similarly, all other dead cells stay dead.

                    // This switch statements purpose the determine the next board, by its conditions.
                    switch (currentCell.CellState)
                    {
                        // A cell is still alive if it has 2 or 3 neighbours.
                        case State.Alive:
                            if (aliveNeighbors == 2 || aliveNeighbors == 3)
                            {
                                nextBoard[rowIndex, columnIndex].CellState = State.Alive;
                            }
                            // All other live cells will die in the next generation.
                            else
                            {
                                nextBoard[rowIndex, columnIndex].CellState = State.Dead;
                            }
                            break;
                        case State.Dead:
                            // A cell becomes reborn if it has 3 neighbours.
                            if (aliveNeighbors == 3)
                            {
                                nextBoard[rowIndex, columnIndex].CellState = State.Alive;
                            }
                            // Otherwise, it will still remain dead.
                            break;

                    }
                }
            }

            // Initializing current board as nextBoard.
            Board = nextBoard;
        }

        // Utility method for filling Board with already initialized Cells.
        private void FillBoard()
        {
            int arrayIndex = 0;
            for (int i = 0; i < SizeRow; i++)
            {
                for (int j = 0; j < SizeCol; j++)
                {
                    Board[i, j] = _cells[arrayIndex];
                    arrayIndex++;
                }
            }
        }

        // Utility method for filling Cell list with default initialized dead cells.
        private void FillCells()
        {
            _cells = new Cell[SizeRow * SizeCol];
            for (int i = 0; i < SizeRow * SizeCol; i++)
            {
                _cells[i] = new Cell();
            }
        }

        // Utility method for filling 2D Cell array with default initialized dead cells.
        private void FillGenerationDefault(Cell[,] generation, int sizeRow, int sizeCol)
        {
            for (int i = 0; i < sizeRow; i++)
            {
                for (int j = 0; j < sizeCol; j++)
                {
                    generation[i, j] = new Cell();
                }
            }
        }


        // Overloaded == (equals) operator enabling the comparison of two boards,
        // e.g. to check if there was any change in the next step (whether a stable structure was created)
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            Cell[,] otherBoard = ((Generation)obj).Board;
            bool isEqual = false;
            for (int i = 0; i < SizeRow && !isEqual; i++)
            {
                for (int j = 0; j < SizeCol; j++)
                {
                    if (this.Board[i, j].CellState == otherBoard[i, j].CellState)
                    {
                        isEqual = true;
                        break;
                    }
                }

            }

            return isEqual;
        }

        private int getColSizeFromString(List<string> stringBoard)
        {
            return stringBoard[0].Length;
        }

        private int getRowSizeFromString(List<String> stringBoard)
        {
            return stringBoard.Count;
        }

        // A method generating the board after passing a given number of simulation steps
        // Also, it regenerates board in every 100 miliseconds.
        public void GenerateBoard(int simCount)
        {
            while (simCount > 0)
            {
                Console.Write(this);
                Thread.Sleep(50);
                this.NextGeneration();
                simCount--;
            }
        }


        // Checking valid characters (Square for cell, space for dead-cell)
        private bool IsValidAliveChar(char character)
        {
            return character == '■' || character == '*';
        }

        private bool IsValidDeadChar(char character)
        {
            return character == ' ' || character == '-';
        }

        // An overloaded ToString method for printing board.
        // I used stringBuilder to construct my output string.
        // If detected cell is alive, it will be printed as ■
        // If detected cell is dead, it will be printed as blank space.
        public override string ToString()
        {

            StringBuilder stringBuilder = new StringBuilder();
            for (int row = 0; row < this.SizeRow; row++)
            {
                for (int column = 0; column < this.SizeCol; column++)
                {
                    Cell cell = this.Board[row, column];
                    if (cell.CellState == State.Alive)
                    {
                        stringBuilder.Append(cellChar);
                    }
                    else
                    {
                        stringBuilder.Append(deadCellChar);
                    }
                }
                // Go to next line after first row is done.
                stringBuilder.Append("\n");
            }

            // Just for making the game more good looking, I'll be making the cursor invisible in console POV.
            // And setting the cursor position on the center.
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            return stringBuilder.ToString();


        }
    }
}