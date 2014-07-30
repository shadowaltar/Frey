using System;
using System.Configuration;

namespace Maintenance.Common.ApplicationSettings
{
    public class DatabaseEnvironmentElement : ConfigurationElement
    {
        [ConfigurationProperty("System", IsRequired = true)]
        public string System
        {
            get { return (string)base["System"]; }
        }

        [ConfigurationProperty("Environment", IsRequired = true)]
        public string Environment
        {
            get { return (string)base["Environment"]; }
        }

        [ConfigurationProperty("EncryptedConnection")]
        public string EncryptedConnection
        {
            get { return (string)base["EncryptedConnection"]; }
        }

        [ConfigurationProperty("ServerName")]
        public string ServerName
        {
            get { return (string)base["ServerName"]; }
        }

        public override string ToString()
        {
            return string.Format("System: {0}, Environment: {1}, EncryptedConnection: (Omitted)", System, Environment);
        }
    }

    [ConfigurationCollection(typeof(DatabaseEnvironmentElement))]
    public class DatabaseEnvironmentElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Database";

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
            return new DatabaseEnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var elem = (DatabaseEnvironmentElement)(element);
            return elem.System + "|" + elem.Environment;
        }

        public DatabaseEnvironmentElement this[int idx]
        {
            get { return (DatabaseEnvironmentElement)BaseGet(idx); }
        }
    }
}