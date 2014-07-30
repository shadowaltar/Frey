using Maintenance.Common.Entities;

namespace Maintenance.Common.Utils
{
    public class Dummies
    {
        public static Country NewAnyCountry { get { return new AnyCountry(); } }

        public static Location NewDummyLocation { get { return new DummyLocation(); } }

        public class AnyCountry : Country
        {
            public override string Code { get { return "Any"; } }
            public override string Name { get { return "Any"; } }
            public override string DisplayName { get { return "Any"; } }
        }

        public class DummyLocation : Location
        {
            public DummyLocation()
            {
                Id = -1;
            }

            public override string DisplayName
            {
                get { return Texts.NotSet; }
            }
        }
    }
}