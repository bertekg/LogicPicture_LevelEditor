[System.Serializable]
public class SingleLevel
{
    public string NameEnglish { get; set; }
    public string NamePolish { get; set; }
    public Level LevelData { get; set; }
    public SingleLevel() { }
    public SingleLevel(string nameEnglish, string namePolish, Level levelData)
    {
        NameEnglish = nameEnglish;
        NamePolish = namePolish;
        LevelData = levelData;
    }
}
