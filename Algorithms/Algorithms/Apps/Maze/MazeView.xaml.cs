using Algorithms.Apps.Maze.Algorithms;

namespace Algorithms.Apps.Maze
{
    /// <summary>
    /// Interaction logic for MazeView.xaml
    /// </summary>
    public partial class MazeView
    {
        private readonly Graphics graphics;
        private Game game;

        public MazeView()
        {
            InitializeComponent();
            graphics = new Graphics { LineThickness = 1 };
        }

        public void Generate<T>(int width, int height) where T : IMazeGenerator
        {
            var mg = new MazeGraphics(graphics);
            game = new Game();
            game.GenerateRectangular<T>(width, height);
            mg.DrawingMazeEx(ref Image, game.Maze);
        }

        public void Solve<T>() where T : IMazeSolver
        {
            if (game == null)
                return;
            game.Maze.SetDefaultExit();
            var solution = game.Solve<T>();
        }
    }
}
