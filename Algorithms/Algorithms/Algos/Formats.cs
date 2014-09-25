namespace Algorithms.Algos
{
    public class Formats
    {
        public static string ToBinary(int a)
        {
            var s = "";
            for (int i = a; i > 0; i /= 2)
            {
                s = (i % 2) + s;
            }
            return s;
        }

        public static string ToHexidecimal(int a)
        {
            var s = "";
            for (int i = a; i > 0; i /= 16)
            {
                var x = i % 16;
                if (x < 10)
                    s = x + s;
                else if (x == 10)
                    s = "A" + s;
                else if (x == 11)
                    s = "B" + s;
                else if (x == 12)
                    s = "C" + s;
                else if (x == 13)
                    s = "D" + s;
                else if (x == 14)
                    s = "E" + s;
                else if (x == 15)
                    s = "F" + s;
            }
            return s;
        }
    }
}