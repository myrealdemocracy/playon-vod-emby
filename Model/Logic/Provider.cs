using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Model.Logic
{
    public class Provider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Save(Tools.Scaffold.Xml.Item item)
        {
            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var code = item.Href.Split(Convert.ToChar("="))[1];

                    var adoProvider = db.Providers.FirstOrDefault(q => q.Code == code);

                    if(adoProvider == null) adoProvider = new Ado.Provider();

                    if (adoProvider.Id == 0)
                    {
                        db.Providers.Add(new Ado.Provider
                        {
                            Code = code,
                            Name = item.Name,
                            Searchable = item.Searchable == "true" ? 1 : 0
                        });
                    }
                    else
                    {
                        adoProvider.Searchable = item.Searchable == "true" ? 1 : 0;

                        db.Entry(adoProvider).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}
