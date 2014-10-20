using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Algorithms.Apps.Maze
{
    public class Graphics
    {
        public Graphics()
        {
            LineStroke = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Black);
            LineThickness = 1;
        }

        public SolidColorBrush LineStroke { get; set; }
        public SolidColorBrush FillColor { get; set; }

        public double LineThickness { get; set; }

        public void DrawRectangle(WriteableBitmap bmp, int x, int y, int width, int height)
        {
            bmp.DrawRectangle(x, y, x + width, y + height, LineStroke.Color);
        }

        public void FillRectangle(WriteableBitmap bmp, int x1, int y1, int x2, int y2)
        {
            bmp.FillRectangle(x1, y1, x2, y2, FillColor.Color);
        }

        public void DrawLine(WriteableBitmap bmp, int x1, int y1, int x2, int y2)
        {
            bmp.DrawLine(x1, y1, x2, y2, LineStroke.Color);
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