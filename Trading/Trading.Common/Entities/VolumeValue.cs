namespace Trading.Common.Entities
{
    public class VolumeValue
    {
        public VolumeValue(decimal value, long volume)
        {
            Value = value;
            Volume = volume;
        }

        public decimal Value { get; set; } 
        public long Volume { get; set; } 
    }
}