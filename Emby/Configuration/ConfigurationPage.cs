using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;

namespace PlayOn.Emby.Configuration
{
    public class ConfigurationPage : IPluginConfigurationPage
    {
        public Stream GetHtmlStream()
        {
            return GetType().Assembly.GetManifestResourceStream("PlayOn.Configuration.configPage.html");
        }

        public string Name
        {
            get { return "Video On Demand"; }
        }

        public ConfigurationPageType ConfigurationPageType
        {
            get { return ConfigurationPageType.PluginConfiguration; }
        }

        public IPlugin Plugin
        {
            get { return Emby.Plugin.Instance; }
        }
    }
}
