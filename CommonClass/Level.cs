using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public byte WidthX { get; set; }
    public byte HeightY { get; set; }
    public ColorData ColorDataNeutral { get; set; }
    public ColorData ColorDataBackground { get; set; }
    public ColorData ColorDataMarker { get; set; }
    public List<ColorData> ColorsDataTiles { get; set; }
    public List<TileData> TilesData { get; set; }
    public List<List<HintData>> HintsDataVertical { get; set; }
    public List<List<HintData>> HintsDataHorizontal { get; set; }
    public Level()
    {

    }
    public Level(byte widthX, byte heightY, ColorData colorDataNeutral,
        ColorData colorDataBackground, ColorData colorDataMarker, 
        List<ColorData> colorsDataTiles, List<TileData> tilesData, 
        List<List<HintData>> hintsDataHorizontal, List<List<HintData>> hintsDataVertical)
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
