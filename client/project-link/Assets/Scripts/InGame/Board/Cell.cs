namespace ProjectLink.InGame.Board
{
    public enum CellState { Empty, Node, Path }

    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public CellState State { get; private set; }
        public int ColorId { get; private set; }

        public bool IsEmpty => State == CellState.Empty;
        public bool IsNode  => State == CellState.Node;
        public bool IsPath  => State == CellState.Path;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetNode(int colorId)
        {
            State   = CellState.Node;
            ColorId = colorId;
        }

        public void SetPath(int colorId)
        {
            State   = CellState.Path;
            ColorId = colorId;
        }

        public void Clear()
        {
            State   = CellState.Empty;
            ColorId = 0;
        }
    }
}
