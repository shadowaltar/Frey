namespace Trading.Common.Technicals
{
    public class MovingAverage
    {
        private readonly int length;
        private int circularIndex = -1;
        private bool filled;
        private double current = double.NaN;
        private readonly double oneOverLength;
        private readonly double[] circularBuffer;
        private double total;

        public MovingAverage(int length)
        {
            this.length = length;
            oneOverLength = 1d / length;
            circularBuffer = new double[length];
        }

        public int Length { get { return length; } }
        public double Current { get { return current; } }

        public MovingAverage Update(double value)
        {
            double lostValue = circularBuffer[circularIndex];
            circularBuffer[circularIndex] = value;

            // Maintain totals for Push function
            total += value;
            total -= lostValue;

            // If not yet filled, just return. Current value should be double.NaN
            if (!filled)
            {
                current = double.NaN;
                return this;
            }

            // Compute the average
            double average = 0.0d;
            for (int i = 0; i < circularBuffer.Length; i++)
            {
                average += circularBuffer[i];
            }

            current = average * oneOverLength;

            return this;
        }

        public MovingAverage Push(double value)
        {
            // Apply the circular buffer
            if (++circularIndex == length)
            {
                circularIndex = 0;
            }

            double lostValue = circularBuffer[circularIndex];
            circularBuffer[circularIndex] = value;

            // Compute the average
            total += value;
            total -= lostValue;

            // If not yet filled, just return. Current value should be double.NaN
            if (!filled && circularIndex != length - 1)
            {
                current = double.NaN;
                return this;
            }

            // Set a flag to indicate this is the first time the buffer has been filled
            filled = true;

            current = total * oneOverLength;

            return this;
        }
    }
}