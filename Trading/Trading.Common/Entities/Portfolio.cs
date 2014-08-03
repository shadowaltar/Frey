namespace Trading.Common.Entities
{
    public class Portfolio : Entity
    {
        private static readonly Portfolio AnyPortfolio = new AnyPortfolio();

        public PortfolioManager PortfolioManager { get; set; }

        public virtual Index Index { get; set; }

        public override string DisplayName
        {
            get { return string.IsNullOrEmpty(Name) ? Code : Code + ", " + Name; }
        }

        public static Portfolio Any
        {
            get { return AnyPortfolio; }
        }

        public override string ToString()
        {
            return Code;
        }
    }

    internal class AnyPortfolio : Portfolio
    {
        public new string Code
        {
            get { return "Any"; }
        }

        public new string Name
        {
            get { return "Any"; }
        }

        public new string DisplayName
        {
            get { return "Any"; }
        }
    }
}