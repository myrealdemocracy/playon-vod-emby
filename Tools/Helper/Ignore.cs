using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Ignore
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool Item(List<Scaffold.Xml.Item> items, Scaffold.Xml.Item item, string name = null)
        {
            var itemsCount = 0;
            var itemName = "";
            var itemHref = "";

            try
            {
                itemsCount = items.Count;
                itemName = item.Name.ToLower();
                itemHref = item.Href;
            }
            catch (Exception exception)
            {}

            if (!String.IsNullOrEmpty(name)) itemName = name.ToLower();

            if (String.IsNullOrWhiteSpace(itemName))
            {
                Logger.Debug("Ignoring null or empty name");

                return true;
            }

            foreach (var ignore in Constant.Ignore.Name)
            {
                if (itemName == ignore)
                {
                    Logger.Debug("Ignoring - itemName == " + ignore);

                    return true;
                }
            }

            foreach (var ignore in Constant.Ignore.NameContains)
            {
                if (itemName.Contains(ignore))
                {
                    Logger.Debug("Ignoring - itemName.Contains: " + ignore);

                    return true;
                }
            }

            foreach (var ignore in Constant.Ignore.NameStartsWith)
            {
                if (itemName.StartsWith(ignore))
                {
                    Logger.Debug("Ignoring - itemName.StartsWith: " + ignore);

                    return true;
                }
            }

            foreach (var ignore in Constant.Ignore.NameEndsWith)
            {
                if (itemName.EndsWith(ignore))
                {
                    Logger.Debug("Ignoring - itemName.EndsWith: " + ignore);

                    return true;
                }
            }

            foreach (var ignore in Constant.Ignore.HrefContains)
            {
                if (itemHref.Contains(ignore))
                {
                    Logger.Debug("Ignoring - itemHref.Contains: " + ignore);

                    return true;
                }
            }

            return false;
        }
    }
}
