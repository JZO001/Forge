using Forge.Configuration;
using Forge.Configuration.Shared;
using Forge.Persistence.StorageProviders.ConfigSection;
using Microsoft.Extensions.Configuration;

namespace Testing.Configuration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            List<PropertyItem> settings = config.GetSection("SettingsA").Get<List<PropertyItem>>();
            IsTrue(settings.Count == 1);
            PropertyItem settingsA = settings[0];
            IsTrue(settingsA.Id == "id");
            IsTrue(settingsA.Value == "value");
            IsTrue(settingsA.Items.Count == 2);
            IsTrue(settingsA.Items["Key"].Id == "Key");
            IsTrue(settingsA.Items["Key"].Value == "value2");
            IsTrue(settingsA.Items["Key2"].Id == "Key2");
            IsTrue(settingsA.Items["Key2"].Value == "value3");

            PropertyItem? settingsB = config.GetSection("SettingsB").Get<PropertyItem>();
            IsTrue(settingsB.Id == "id");
            IsTrue(settingsB.Value == "value");
            IsTrue(settingsB.Items.Count == 2);
            IsTrue(settingsB.Items["Key"].Id == "Key");
            IsTrue(settingsB.Items["Key"].Value == "value2");
            IsTrue(settingsB.Items["Key2"].Id == "Key2");
            IsTrue(settingsB.Items["Key2"].Value == "value3");

            CategoryPropertyItems items = StorageConfiguration.Settings.CategoryPropertyItems;
            CategoryPropertyItem item = ConfigurationAccessHelper.GetCategoryPropertyByPath(StorageConfiguration.Settings.CategoryPropertyItems, "NHibernateProvider/Default/NHibernateSettings");

            Console.WriteLine("Test completed.");
        }

        static void IsTrue(bool value)
        {
            if (!value) throw new Exception("Test failed.");
        }

    }
}