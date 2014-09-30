namespace Algorithms.Algos
{
    public class Searches
    {
        /// <summary>
        /// O(LogN) search algorithm.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sortedArray"></param>
        /// <returns></returns>
        public static int BinarySearch(int key, int[] sortedArray)
        {
            return BinarySearch(key, sortedArray, 0, sortedArray.Length - 1);
        }

        private static int BinarySearch(int key, int[] sortedArray, int low, int high)
        {
            if (low > high) return -1;
            int mid = low + (high - low) / 2;
            if (key < sortedArray[mid]) return BinarySearch(key, sortedArray, low, mid - 1);
            if (key > sortedArray[mid]) return BinarySearch(key, sortedArray, mid + 1, high);
            return mid;
        }
    }
}