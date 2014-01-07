namespace Automata.Entities
{
    public class Forex : Security
    {
        public Currency Base { get; set; }
        public Currency Quote { get; set; }
    }
}