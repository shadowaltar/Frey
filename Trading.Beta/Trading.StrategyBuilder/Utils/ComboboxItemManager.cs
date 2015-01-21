using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Trading.StrategyBuilder.Views.Items;

namespace Trading.StrategyBuilder.Utils
{
    public static class ComboboxItemManager
    {
        private static readonly Dictionary<Type, object> Items = new Dictionary<Type, object>();

        public static BindableCollection<ComboboxItem<T>> NewBindables<T>()
        {
            object results;
            if (!Items.TryGetValue(typeof(T), out results))
            {
                var typedResults = Enum.GetValues(typeof(T)).Cast<T>()
                    .Select(c => new ComboboxItem<T>(c.ToString(), c)).ToList();
                Items[typeof(T)] = typedResults;
                results = typedResults;
            }
            return new BindableCollection<ComboboxItem<T>>((List<ComboboxItem<T>>)results);
        }

        public static List<ComboboxItem<T>> Load<T>()
        {
            object results;
            if (!Items.TryGetValue(typeof(T), out results))
            {
                var typedResults = Enum.GetValues(typeof(T)).Cast<T>()
                    .Select(c => new ComboboxItem<T>(c.ToString(), c)).ToList();
                Items[typeof(T)] = typedResults;
                results = typedResults;
            }
            return (List<ComboboxItem<T>>)results;
        }
    }
}