[System.Serializable]
public class HintData
{
    public byte Value { get; set; }
    public byte ColorID { get; set; }
    public HintData() { }
    public HintData(byte value, byte colorID)
    {
        Value = value;
        ColorID = colorID;
    }
}
