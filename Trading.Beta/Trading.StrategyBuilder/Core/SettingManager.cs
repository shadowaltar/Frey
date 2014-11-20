using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Trading.Common;
using Trading.Common.Utils;
using Trading.StrategyBuilder.ViewModels;

namespace Trading.StrategyBuilder.Core
{
    public class SettingManager
    {
        private Dictionary<string, string> settings;
        private readonly string settingsFileName = Path.Combine(Constants.OtherDataDirectory, "Settings");

        public void LoadSettings(MainViewModel mainViewModel)
        {
            if (!File.Exists(settingsFileName))
                return;

            settings = FileHelper.ReadAsDictionary(settingsFileName);

            var props = typeof(MainViewModel).GetProperties();

            var handledKeys = new HashSet<string>();
            foreach (var setting in settings)
            {
                var propertyInfo = props.FirstOrDefault(p => p.Name == setting.Key);
                if (propertyInfo == null)
                    continue;

                handledKeys.Add(setting.Key);

                var t = propertyInfo.PropertyType;
                if (t == typeof(int))
                {
                    propertyInfo.SetValue(mainViewModel, setting.Value.Int());
                }
                else if (t == typeof(double))
                {
                    propertyInfo.SetValue(mainViewModel, setting.Value.Double());
                }
                else if (t != typeof(string) && t.GetInterfaces().Any(i => i.Name.Contains("IEnumerable")))
                {
                    var elemType = t.GetGenericArguments();
                    var collectionType = typeof(List<>).MakeGenericType(elemType);
                    try
                    {
                        var ser = new XmlSerializer(collectionType);
                        if (string.IsNullOrWhiteSpace(setting.Value))
                            continue;

                        using (var reader = new StringReader(setting.Value))
                        {
                            var propertyCollection = propertyInfo.GetValue(mainViewModel);
                            var addMethod = propertyCollection.GetType().GetMethod("Add");

                            var values = (IEnumerable)ser.Deserialize(reader);
                            foreach (var val in values)
                            {
                                addMethod.Invoke(propertyCollection, new[] { val });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("WANRING: cannot deserialize type " + t);
                    }
                }
                else
                    propertyInfo.SetValue(mainViewModel, setting.Value);
            }
        }

        public SettingManager ToXml<T>(string key, List<T> values)
        {
            if (values.Count == 0)
            {
                settings[key] = "";
            }
            else
            {
                var ser = new XmlSerializer(typeof(List<T>));
                using (var writer = new StringWriter())
                {
                    ser.Serialize(writer, values);
                    settings[key] = writer.ToString().Replace("\r", "").Replace("\n", "");
                }
            }
            return this;
        }

        public SettingManager ToSave(string key, string value)
        {
            settings[key] = value;
            return this;
        }

        public void Save()
        {
            FileHelper.Write(settingsFileName, settings);
        }
    }
}