namespace Algorithms.Apps.Maze.Algorithms
{
    public interface IMazeGenerator
    {
        Maze Generate();
        int Width { get; set; }
        int Height { get; set; }
    }
}