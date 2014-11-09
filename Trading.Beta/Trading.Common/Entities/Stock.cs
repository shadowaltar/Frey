namespace Trading.Common.Entities
{
    public class Stock : Security
    {
        public string Sector { get; set; }
        public string Industry { get; set; }
    }
}