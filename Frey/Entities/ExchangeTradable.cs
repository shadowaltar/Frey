namespace Automata.Entities
{
    public class ExchangeTradable : Security
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Exchange Exchange { get; set; }
    }
}
