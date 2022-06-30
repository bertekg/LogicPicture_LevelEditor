public class TileData
{
    public byte PosX { get; set; }
    public byte PosY { get; set; }
    public byte ColorID { get; set; }
    public TileData(byte posX, byte posY, byte colorID)
    {
        PosX = posX;
        PosY = posY;
        ColorID = colorID;
    }
}
