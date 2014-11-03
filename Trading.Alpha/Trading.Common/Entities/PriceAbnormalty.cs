namespace Trading.Common.Entities
{
    public class PriceAbnormalty
    {
        public Security Security { get; set; }
        public Price Price { get; set; }
        public string Message { get; set; }
    }
}