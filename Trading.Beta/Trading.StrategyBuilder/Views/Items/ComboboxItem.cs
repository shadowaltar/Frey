namespace Trading.StrategyBuilder.Views.Items
{
    public struct ComboboxItem<T>
    {
        public string DisplayName { get; set; }
        public T Type { get; set; }

        public ComboboxItem(string displayName, T type)
            : this()
        {
            DisplayName = displayName;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0}, Type: {1}", DisplayName, Type);
        }
    }
}