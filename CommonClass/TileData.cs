[System.Serializable]
public class TileData
{
    public bool IsSelected;
    public int ColorID { get; set; }
    public TileData() { }
    public TileData(bool isSelected, byte colorID)
    {
        IsSelected = isSelected;
        ColorID = colorID;
    }
}
