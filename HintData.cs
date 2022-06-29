public class HintData
{
    public byte Value { get; set; }
    public byte ColorID { get; set; }
    public HintData()
    {
        Value = 0;
        ColorID = 0;
    }
    public HintData(byte value, byte colorID)
    {
        Value = value;
        ColorID = colorID;
    }
}