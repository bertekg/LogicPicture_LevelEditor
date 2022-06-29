using System.Collections.Generic;

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
        WidthX = 5;
        HeightY = 5;
        ColorDataNeutral = new ColorData(0xC0, 0xC0, 0xC0); //Silver <-> #FFC0C0C0
        ColorDataBackground = new ColorData(0xFF, 0xFF, 0xFF); //White <-> #FFFFFFFF
        ColorDataMarker = new ColorData(0xFF, 0xA5, 0x00); //Orange <-> #FFFFA500
        ColorsDataTiles = new List<ColorData>();
        TilesData = new List<TileData>();
        HintsDataVertical = new List<List<HintData>>();
        HintsDataHorizontal = new List<List<HintData>>();
    }
    public Level(byte widthX, byte heightY, ColorData colorDataNeutral,
        ColorData colorDataBackground, ColorData colorDataMarker, 
        List<ColorData> colorsDataTiles, List<TileData> tilesData, 
        List<List<HintData>> hintsDataVertical, List<List<HintData>> hintsDataHorizontal)
    {
        WidthX = widthX;
        HeightY = heightY;
        ColorDataNeutral = colorDataNeutral;
        ColorDataBackground = colorDataBackground;
        ColorDataMarker = colorDataMarker;
        ColorsDataTiles = colorsDataTiles;
        TilesData = tilesData;
        HintsDataVertical = hintsDataVertical;
        HintsDataHorizontal = hintsDataHorizontal;
    }
}
