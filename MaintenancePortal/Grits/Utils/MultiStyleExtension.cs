using System;
using System.Windows;
using System.Windows.Markup;

namespace GritsMaintenance.Utils
{
    /// <summary>
    /// Make XAML elements being able to accept multiple styles by their keys.
    /// Source code is given by:
    /// http://www.zagstudio.com/blog/384#.Uxan5PmSx0o
    /// </summary>
    [MarkupExtensionReturnType(typeof(Style))]
    public class MultiStyleExtension : MarkupExtension
    {
        private static readonly char[] Delimiter = { ' ' };

        private string[] resourceKeys;

        public MultiStyleExtension(string inputResourceKeys)
        {
            if (inputResourceKeys == null)
            {
                throw new ArgumentNullException("inputResourceKeys");
            }

            resourceKeys = inputResourceKeys.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);

            if (resourceKeys.Length == 0)
            {
                throw new ArgumentException("No input resource keys specified.");
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var resultStyle = new Style();
            foreach (string currentResourceKey in resourceKeys)
            {
                var currentStyle = new StaticResourceExtension(currentResourceKey).ProvideValue(serviceProvider) as Style;

                if (currentStyle == null)
                {
                    throw new InvalidOperationException("Could not find style with resource key " + currentResourceKey + ".");
                }

                Merge(resultStyle, currentStyle);
            }
            return resultStyle;
        }

        private static void Merge(Style style1, Style style2)
        {
            if (style1 == null)
            {
                throw new ArgumentNullException("style1");
            }
            if (style2 == null)
            {
                throw new ArgumentNullException("style2");
            }

            if (style1.TargetType.IsAssignableFrom(style2.TargetType))
            {
                style1.TargetType = style2.TargetType;
            }

            if (style2.BasedOn != null)
            {
                Merge(style1, style2.BasedOn);
            }

            foreach (SetterBase currentSetter in style2.Setters)
            {
                style1.Setters.Add(currentSetter);
            }

            foreach (TriggerBase currentTrigger in style2.Triggers)
            {
                style1.Triggers.Add(currentTrigger);
            }

            // This code is only needed when using DynamicResources.
            foreach (object key in style2.Resources.Keys)
            {
                style1.Resources[key] = style2.Resources[key];
            }
        }
    }
}