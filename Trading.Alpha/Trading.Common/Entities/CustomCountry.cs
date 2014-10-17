namespace Trading.Common.Entities
{
    public class CustomCountry : Country
    {
        public string FmrCode { get; set; }

        public CustomCountry Copy()
        {
            return new CustomCountry { Code = Code, Name = Name, FmrCode = FmrCode };
        }
    }
}