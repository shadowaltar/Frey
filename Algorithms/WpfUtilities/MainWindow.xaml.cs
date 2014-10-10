using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace WpfUtilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Load();
        }

        private void Load()
        {
            var controlType = typeof(Control);
            var derivedTypes = new List<Type>();
            Assembly assembly = Assembly.GetAssembly(typeof(Control));
            foreach (Type type in assembly.GetTypes())
            {
                // Only add a type of the list if it's a Control, a concrete class,
                // and public.
                if (type.IsSubclassOf(controlType) && !type.IsAbstract && type.IsPublic)
                {
                    derivedTypes.Add(type);
                }
            }
            derivedTypes.Sort((t1, t2) => string.Compare(t1.FullName, t2.FullName, StringComparison.Ordinal));
            ClassNames.ItemsSource = derivedTypes;
        }

        private void ClassNames_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Remove the control from the grid.
                Grid.Children.Clear();
                // Get the selected type.
                var type = (Type)ClassNames.SelectedItem;

                // Instantiate the type.
                ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
                var control = (Control)info.Invoke(null);

                // Add it to the grid (but keep it hidden).
                control.HorizontalAlignment = HorizontalAlignment.Stretch;
                control.VerticalAlignment = VerticalAlignment.Stretch;
                Grid.Children.Add(control);

                // Get the template.
                ControlTemplate template = control.Template;

                // Get the XAML for the template.
                var settings = new XmlWriterSettings { Indent = true };
                var sb = new StringBuilder();
                XmlWriter writer = XmlWriter.Create(sb, settings);
                XamlWriter.Save(template, writer);

                // Display the template.
                Structure.Text = sb.ToString();

            }
            catch (Exception err)
            {
                Structure.Text = "<< Error generating template: " + err.Message + ">>";
            }
        }
    }
}
