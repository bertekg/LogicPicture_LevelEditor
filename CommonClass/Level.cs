using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public byte WidthX { get; set; }
    public byte HeightY { get; set; }
    public ColorData ColorDataNeutral { get; set; }
    public ColorData ColorDataBackground { get; set; }
    public ColorData ColorDataMarker { get; set; }
    public ColorData[] ColorsDataTiles { get; set; }
    public TileData[][] TilesData { get; set; }
    public HintData[][] HintsDataHorizontal { get; set; }
    public HintData[][] HintsDataVertical { get; set; }
    public Level()
    {

    }
    public Level(byte widthX, byte heightY, ColorData colorDataNeutral,
        ColorData colorDataBackground, ColorData colorDataMarker,
        ColorData[] colorsDataTiles, TileData[][] tilesData,
        HintData[][] hintsDataHorizontal, HintData[][] hintsDataVertical)
    {
        WidthX = widthX;
        HeightY = heightY;
        ColorDataNeutral = colorDataNeutral;
        ColorDataBackground = colorDataBackground;
        ColorDataMarker = colorDataMarker;
        ColorsDataTiles = colorsDataTiles;
        TilesData = tilesData;
        HintsDataHorizontal = hintsDataHorizontal;
        HintsDataVertical = hintsDataVertical;
    }
}
