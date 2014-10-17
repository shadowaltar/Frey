using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Algorithms.Apps.Maze
{
    public class Graphics
    {
        private readonly Canvas canvas;
        private readonly Window root;
        private double canvasWidth;
        private double canvasHeight;
        private double widthOffset;
        private double heightOffset;

        public Graphics(Canvas canvas)
        {
            this.canvas = canvas;
            LineStroke = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Black);
            LineThickness = 1;
            root = FindAncestor<Window>(canvas);
            if (root == null)
                throw new InvalidOperationException("No Window!");
        }

        public SolidColorBrush LineStroke { get; set; }
        public Brush FillColor { get; set; }

        public double LineThickness { get; set; }

        public double CanvasWidth
        {
            get { return canvasWidth; }
            set
            {
                widthOffset = root.ActualWidth - canvas.ActualWidth;
                root.Width = value + widthOffset;
                canvasWidth = value;
            }
        }

        public double CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                heightOffset = root.ActualHeight - canvas.ActualHeight;
                root.Height = value + heightOffset;
                canvasHeight = value;
            }
        }

        public void SetCanvasSize(double width, double height)
        {
            CanvasWidth = width;
            CanvasHeight = height;
            root.Width = CanvasWidth + 20;
            root.Height = CanvasHeight + 42;
        }

        public void Clear()
        {
            canvas.Children.Clear();
        }

        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                Stroke = LineStroke,
                StrokeThickness = LineThickness,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            canvas.Children.Add(line);
        }

        public void DrawRectangle(double x, double y, double width, double height)
        {
            DrawRectangle(x, y, width, height, null);
        }

        public void DrawRectangle(WriteableBitmap bmp, int x, int y, int width, int height)
        {
            bmp.DrawRectangle(x,y,x+width,y+height, LineStroke.Color);
        }

        public void DrawLine(WriteableBitmap bmp, int x1, int y1, int x2, int y2)
        {
            bmp.DrawLine(x1, y1, x2, y2, LineStroke.Color);
        }

        protected void DrawRectangle(double x, double y, double width, double height, Brush fill)
        {
            var rectangle = new Rectangle
            {
                Stroke = LineStroke,
                StrokeThickness = LineThickness,
                Fill = fill,
                Width = width,
                Height = height,
                Margin = new Thickness(x, y, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            canvas.Children.Add(rectangle);
        }

        public void DrawRectangle(WriteableBitmap bitmap, int x, int y, int width, int height, Color fill)
        {
            int color = fill.R << 16;
            color |= fill.G << 8;
            color |= fill.B;
            int bpp = bitmap.Format.BitsPerPixel / 8;

            unsafe
            {
                for (int j = 0; j < height; j++)
                {
                    // Get a pointer to the back buffer
                    int buf = (int)bitmap.BackBuffer;

                    // Find the address of the pixel to draw
                    buf += (y + j) * bitmap.BackBufferStride;
                    buf += x * bpp;

                    for (int i = 0; i < width; i++)
                    {
                        // Assign the color data to the pixel
                        *((int*)buf) = color;

                        // Increment the address of the pixel to draw
                        buf += bpp;
                    }
                }
            }
            bitmap.AddDirtyRect(new Int32Rect(x, y, width, height));
        }

        public void DrawBitmap()
        {

            WriteableBitmap writeableBmp = BitmapFactory.New(512, 512);

        }

        public static void DrawLine(WriteableBitmap bitmap, int x, int y)
        {
            int column = x;
            int row = y;

            // Reserve the back buffer for updates.
            bitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer. 
                int pBackBuffer = (int)bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += row * bitmap.BackBufferStride;
                pBackBuffer += column * 4;

                // Compute the pixel's color. 
                int colorData = 255 << 16; // R
                colorData |= 128 << 8;   // G
                colorData |= 255 << 0;   // B 

                // Assign the color data to the pixel.
                *((int*)pBackBuffer) = colorData;
            }

            // Specify the area of the bitmap that changed.
            bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));

            // Release the back buffer and make it available for display.
            bitmap.Unlock();
        }

        public static void DrawPixel(WriteableBitmap bitmap, int x, int y)
        {
            int column = x;
            int row = y;

            // Reserve the back buffer for updates.
            bitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer. 
                int pBackBuffer = (int)bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += row * bitmap.BackBufferStride;
                pBackBuffer += column * 4;

                // Compute the pixel's color. 
                int colorData = 255 << 16; // R
                colorData |= 128 << 8;   // G
                colorData |= 255 << 0;   // B 

                // Assign the color data to the pixel.
                *((int*)pBackBuffer) = colorData;
            }

            // Specify the area of the bitmap that changed.
            bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));

            // Release the back buffer and make it available for display.
            bitmap.Unlock();
        }

        public T FindAncestor<T>(FrameworkElement element) where T : FrameworkElement
        {
            var parent = element.Parent as FrameworkElement;
            if (parent is T)
            {
                return (T)parent;
            }
            if (parent != null)
            {
                return FindAncestor<T>(parent);
            }
            return null;
        }
    }
}