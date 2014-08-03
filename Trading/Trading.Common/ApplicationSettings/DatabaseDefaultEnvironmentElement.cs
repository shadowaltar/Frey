using System;
using System.Configuration;

namespace Trading.Common.ApplicationSettings
{
    public class DatabaseDefaultEnvironmentElement : ConfigurationElement
    {
        [ConfigurationProperty("System", IsRequired = true, IsKey = true)]
        public string System
        {
            get { return (string)base["System"]; }
            set { base["System"] = value; }
        }

        [ConfigurationProperty("Environment", IsRequired = true)]
        public string Environment
        {
            get { return (string)base["Environment"]; }
        }

        public override string ToString()
        {
            return string.Format("System: {0}, Environment: {1}", System, Environment);
        }
    }

    [ConfigurationCollection(typeof(DatabaseDefaultEnvironmentElement))]
    public class DatabaseDefaultEnvironmentElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Default";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseDefaultEnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DatabaseDefaultEnvironmentElement)(element)).System;
        }

        public DatabaseDefaultEnvironmentElement this[int idx]
        {
            get { return (DatabaseDefaultEnvironmentElement)BaseGet(idx); }
        }
    }
}