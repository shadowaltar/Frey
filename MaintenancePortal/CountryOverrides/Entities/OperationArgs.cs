namespace Maintenance.CountryOverrides.Entities
{
    public class PostProcessArgs
    {
        public PostProcessArgs(OperationType type)
        {
            Type = type;
        }

        public PostProcessArgs(OperationType type, object item)
            : this(type)
        {
            Item = item;
        }

        public OperationType Type { get; set; }
        public object Item { get; set; }
    }
}