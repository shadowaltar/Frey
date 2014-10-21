using Algorithms.Apps.Maze.Algorithms;

namespace Algorithms.Apps.Maze
{
    /// <summary>
    /// Interaction logic for MazeView.xaml
    /// </summary>
    public partial class MazeView
    {
        private readonly MazeGraphics graphics;
        private Game game;

        public MazeView()
        {
            InitializeComponent();
            var g = new Graphics();
            graphics = new MazeGraphics(g);
        }

        public void Generate<T>(int width, int height) where T : IMazeGenerator
        {

            game = new Game();
            game.GenerateRectangular<T>(width, height);
            graphics.DrawMaze(ref Image, game.Maze);
        }

        public void Solve<T>() where T : IMazeSolver
        {
            if (game == null)
                return;
            game.Maze.SetDefaultExit();
            game.Solve<T>();
            graphics.DrawSolution(ref Image, game.Maze);
        }
    }
}
