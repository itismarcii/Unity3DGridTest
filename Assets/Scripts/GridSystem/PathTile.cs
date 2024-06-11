namespace GridSystem
{
    public class PathTile : GridTile
    {
        public enum Type
        {
            Default,
            Spawn,
            Finish
        }

        public Type TileType { get; private set; }

        public PathTile(in GridTile tile, in Type type) : base(in tile)
        {
            TileType = type;
        }
    }
}
