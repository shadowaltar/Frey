using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Trading.TradeWatch.Views.Controls
{
    /// <summary>
    /// Interaction logic for DisplayedSecurityButton.xaml
    /// </summary>
    public partial class DisplayedSecurityButton : UserControl
    {
        public DisplayedSecurityButton()
        {
            InitializeComponent();
            CodeLabel.DataContext = DataContext;
        }

        public static readonly DependencyProperty SecurityCodeProperty = DependencyProperty.Register("SecurityCode",
            typeof(string), typeof(DisplayedSecurityButton), new PropertyMetadata(""));

        public static readonly RoutedEvent SecurityCodeChangedEvent =
            EventManager.RegisterRoutedEvent("SecurityCodeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(DisplayedSecurityButton));

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("ClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(DisplayedSecurityButton));

        public event RoutedEventHandler SecurityCodeChanged
        {
            add { AddHandler(SecurityCodeChangedEvent, value); }
            remove { RemoveHandler(SecurityCodeChangedEvent, value); }
        }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public string SecurityCode
        {
            get { return (string)GetValue(SecurityCodeProperty); }
            set { SetValue(SecurityCodeProperty, value); }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var args = new DisplaySecurityButtonCloseArgs(SecurityCode, ClickEvent, sender);
            RaiseEvent(args);
        }
    }

    public class DisplaySecurityButtonCloseArgs : RoutedEventArgs
    {
        public string SecurityCode { get; set; }
        public DisplaySecurityButtonCloseArgs(string code, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            SecurityCode = code;
        }
    }
}
