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
}
