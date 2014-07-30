using System;
using System.Configuration;

namespace Maintenance.Portal.ApplicationSettings
{
    public class ScreenElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["Name"]; }
        }

        [ConfigurationProperty("TileColor", IsRequired = true)]
        public string TileColor
        {
            get { return (string)base["TileColor"]; }
        }

        [ConfigurationProperty("AssemblyName", IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)base["AssemblyName"]; }
        }

        [ConfigurationProperty("ExecutableName", IsRequired = true)]
        public string ExecutableName
        {
            get { return (string)base["ExecutableName"]; }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, TileColor: {1}, AssemblyName: {2}, ExecutableName: {3}", Name, TileColor, AssemblyName, ExecutableName);
        }
    }

    [ConfigurationCollection(typeof(ScreenElement))]
    public class ScreenElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Screen";

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        protected override string ElementName { get { return PropertyName; } }

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
            return new ScreenElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var elem = (ScreenElement)(element);
            return elem.Name;
        }

        public ScreenElement this[int idx]
        {
            get { return (ScreenElement)BaseGet(idx); }
        }
    }
}