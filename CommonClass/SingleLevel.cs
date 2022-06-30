public class SingleLevel
{
    public string NameEnglish { get; set; }
    public string NamePolish { get; set; }
    public Level LevelData { get; set; }
    //public SingleLevel()
    //{
    //    NameEnglish = string.Empty;
    //    NamePolish = string.Empty;
    //    LevelData = new Level();
    //}

    public SingleLevel(string nameEnglish, string namePolish, Level levelData)
    {
        NameEnglish = nameEnglish;
        NamePolish = namePolish;
        LevelData = levelData;
    }
}
