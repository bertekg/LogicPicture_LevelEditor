public class SingleLevel
{
    public string Name { get; set; }
    public Level Level { get; set; }
    public SingleLevel()
    {
        Name = string.Empty;
        Level = new Level();
    }
}
