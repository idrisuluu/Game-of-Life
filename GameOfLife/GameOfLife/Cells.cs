namespace Game_of_Life
{

    /*
      I'm going to create a enum called State for Cell States,
      I did not want to use boolean variables for Cell states, so creating an enum
      can make the code clear.
      I assigned their value; Dead as 0 and Alive as 1.
     */

    public enum State
    {
        Dead = 0,
        Alive = 1
    }

    /*
     * Default Cell constructor is a dead cell.
     */

    public class Cell
    {
        public State CellState;

        public Cell(State cellState)
        {
            CellState = cellState;
        }

        public Cell()
        {
            CellState = State.Dead;
        }

    }
}