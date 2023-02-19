using System.Collections.Generic;

[System.Serializable]
public class SingleLevel
{
    public ProjectStory ProjectStoryEN { get; set; }
    public ProjectStory ProjectStoryPL { get; set; }
    public Level LevelData { get; set; }
    public SingleLevel() { }
    public SingleLevel(ProjectStory projectStoryEN, ProjectStory projectStoryPL, Level levelData)
    {
        ProjectStoryEN = projectStoryEN;
        ProjectStoryPL = projectStoryPL;
        LevelData = levelData;
    }
    public void CalcHintsData()
    {
        LevelData.HintsDataVertical = new HintData[LevelData.HeightY][];
        for (byte y = 0; y < LevelData.HeightY; y++)
        {
            int previousColorId = -1;
            byte currentIdCombo = 0;
            List<HintData> verticalHints = new List<HintData>();
            for (byte x = 0; x < LevelData.WidthX; x++)
            {
                TileData tileDataFound = LevelData.TilesData[x][y];
                if (tileDataFound.IsSelected)
                {
                    if (tileDataFound.ColorID == previousColorId)
                    {
                        currentIdCombo++;
                    }
                    else if (previousColorId >= 0)
                    {
                        HintData hdTemp = new HintData();
                        hdTemp.ColorID = (byte)previousColorId;
                        hdTemp.Value = currentIdCombo;
                        verticalHints.Add(hdTemp);
                        currentIdCombo = 1;
                    }
                    else
                    {
                        currentIdCombo = 1;
                    }
                    previousColorId = tileDataFound.ColorID;
                }
                else
                {
                    if (currentIdCombo > 0)
                    {
                        HintData hdTemp = new HintData();
                        hdTemp.ColorID = (byte)previousColorId;
                        hdTemp.Value = currentIdCombo;
                        verticalHints.Add(hdTemp);
                        currentIdCombo = 0;
                        previousColorId = -1;
                    }
                }
            }
            if (currentIdCombo > 0)
            {
                HintData hdTemp = new HintData();
                hdTemp.ColorID = (byte)(previousColorId);
                hdTemp.Value = currentIdCombo;
                verticalHints.Add(hdTemp);
            }
            if (verticalHints.Count == 0)
            {
                verticalHints.Add(new HintData(0, 0));
            }
            LevelData.HintsDataVertical[y] = ConvertListToArray(verticalHints);
        }

        LevelData.HintsDataHorizontal = new HintData[LevelData.WidthX][];
        for (byte x = 0; x < LevelData.WidthX; x++)
        {
            int prevCellId = -1;
            byte currentIdCombo = 0;
            List<HintData> horizontalHints = new List<HintData>();
            for (byte y = LevelData.HeightY; y > 0; y--)
            {
                TileData tileDataFound = LevelData.TilesData[x][y - 1];
                if (tileDataFound.IsSelected)
                {
                    if (tileDataFound.ColorID == prevCellId)
                    {
                        currentIdCombo++;
                    }
                    else if (prevCellId >= 0)
                    {
                        HintData hint = new HintData();
                        hint.ColorID = (byte)(prevCellId);
                        hint.Value = currentIdCombo;
                        horizontalHints.Add(hint);
                        currentIdCombo = 1;
                    }
                    else
                    {
                        currentIdCombo = 1;
                    }
                    prevCellId = tileDataFound.ColorID;
                }
                else
                {
                    if (currentIdCombo > 0)
                    {
                        HintData hdTemp = new HintData();
                        hdTemp.ColorID = (byte)(prevCellId);
                        hdTemp.Value = currentIdCombo;
                        horizontalHints.Add(hdTemp);
                        currentIdCombo = 0;
                        prevCellId = -1;
                    }
                }
            }
            if (currentIdCombo > 0)
            {
                HintData hdTemp = new HintData();
                hdTemp.ColorID = (byte)(prevCellId);
                hdTemp.Value = currentIdCombo;
                horizontalHints.Add(hdTemp);
            }
            if (horizontalHints.Count == 0)
            {
                horizontalHints.Add(new HintData(0, 0));
            }
            LevelData.HintsDataHorizontal[x] = ConvertListToArray(horizontalHints);
        }
    }

    private HintData[] ConvertListToArray(List<HintData> listHints)
    {
        HintData[] arrayHints = new HintData[listHints.Count];
        for (int i = 0; i < listHints.Count; i++)
        {
            arrayHints[i] = listHints[i];
        }
        return arrayHints;
    }
}
