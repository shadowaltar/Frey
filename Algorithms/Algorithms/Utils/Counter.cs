namespace Algorithms.Utils
{
    public class Counter
    {
        public string Name { get; protected set; }
        private int counter;

        public Counter(string name)
        {
            Name = name;
        }

        public void Increment()
        {
            counter++;
        }

        public int Tally()
        {
            return counter;
        }

        public override string ToString()
        {
            return string.Format("{1}: {0}", counter, Name);
        }
    }
}