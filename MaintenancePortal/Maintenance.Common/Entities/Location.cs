namespace Maintenance.Common.Entities
{
    public class Location : Entity
    {
        public bool IsCountry { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}", base.Code, base.Name);
        }
    }
}