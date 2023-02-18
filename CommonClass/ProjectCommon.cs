[System.Serializable]
public class ProjectCommon
{
    public ProjectStory ProjectStoryEN { get; set; }
    public ProjectStory ProjectStoryPL { get; set; }
    public ProjectCommon() { }
    public ProjectCommon(ProjectStory projectStoryEN, ProjectStory projectStoryPL)
    {
        ProjectStoryEN = projectStoryEN;
        ProjectStoryPL = projectStoryPL;
    }
}
