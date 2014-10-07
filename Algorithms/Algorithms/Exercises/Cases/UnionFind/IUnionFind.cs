namespace Algorithms.Exercises.Cases.UnionFind
{
    public interface IUnionFind
    {
        void Union(int p, int q);
        /// <summary>
        /// Find the component id.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        int Find(int p);
        bool IsConnected(int p, int q);
        int Count();
    }
}