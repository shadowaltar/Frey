namespace Automata.Entities
{
    public abstract class Security
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}