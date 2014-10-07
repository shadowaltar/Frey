using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Trading.Portal
{
    /// <summary>
    /// Class represents a tile button in Portal. It's based on a metro-style Tile UIElement.
    /// Dimension of a tile: width 310, height 150, tile margin 4, border 1; total width 316, total height 156.
    /// </summary>
    public class PortalTile : Tile
    {
        public PortalTile(ScreenInformation info)
        {
            Setup(info);
        }

        public ScreenInformation ScreenInformation { get; set; }

        /// <summary>
        /// Setup a tile instance' DependencyObjects.
        /// </summary>
        /// <param name="info"></param>
        private void Setup(ScreenInformation info)
        {
            SetValue(WidthProperty, 310d);
            SetValue(HeightProperty, 150d);
            SetValue(BackgroundProperty, new SolidColorBrush(info.ThemeColor));
            SetValue(BorderThicknessProperty, new Thickness(1));
            SetValue(BorderBrushProperty, new SolidColorBrush(Colors.White));
            SetValue(Message.AttachProperty, string.Format("[Event Click] = [Action Launch('{0}')]", info.AssemblyName));

            SetValue(ContentProperty, SetupContent(info));
        }

        private FrameworkElement SetupContent(ScreenInformation info)
        {
            var canvas = new Canvas();
            var image = new Image();
            var text = new TextBlock();
            canvas.Children.Add(image);
            canvas.Children.Add(text);

            image.Margin = new Thickness(35, 35, 0, 0);
            image.SetValue(WidthProperty, 80d);
            image.SetValue(HeightProperty, 80d);

            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Format("pack://application:,,,/{0};component/icon.png", info.AssemblyName));
            logo.EndInit();
            image.Source = logo;

            text.SetValue(MarginProperty, new Thickness(125, 35, 0, 0));
            text.SetValue(WidthProperty, 150d);
            text.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            text.Style = Application.Current.FindResource("SessionTitleStyle") as Style;
            text.SetValue(TextBlock.TextProperty, info.DisplayName);


            canvas.SetValue(WidthProperty, 310d);
            canvas.SetValue(HeightProperty, 150d);

            return canvas;
        }
    }
}