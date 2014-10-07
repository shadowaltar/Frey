namespace Trading.Common.Entities
{
    public class Market : Entity
    {
        public Country Country { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}