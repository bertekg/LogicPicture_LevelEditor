[System.Serializable]
public class ProjectStory
{
    public string Title { get; set; }
    public string Description { get; set; }
    public ProjectStory()
    {
    }
    public ProjectStory(string title, string description)
    {
        Title = title;
        Description = description;
    }
}