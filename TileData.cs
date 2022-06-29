public class TileData
{
    public byte PosX { get; set; }
    public byte PosY { get; set; }
    public byte ColorID { get; set; }
    public TileData()
    {
        PosX = 0;
        PosY = 0;
        ColorID = 0;
    }
    public TileData(byte posX, byte posY, byte colorID)
    {
        PosX = posX;
        PosY = posY;
        ColorID = colorID;
    }
}
