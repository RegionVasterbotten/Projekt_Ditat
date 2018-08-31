using System.Collections.Generic;
using System.Linq;
using Ditat.Data.EntityModels;
using System.Dynamic;
using NLog;

namespace Ditat.Logic.Managers
{
    public static class LangaugeManager
   {        
        public static dynamic GetLanguage(int languageId)
        {
            
            using (var db = new DitatEntities())
            {

                var dbLanguage = db.Languages.Single(i => i.Id == languageId);

                var texts = db.LanguageTexts.Where(l => l.LanguageId == languageId).ToList();

                dynamic item = new ExpandoObject();

                foreach (var text in texts)
                {                    
                    AddProperty(item, text.KeyName, text.Value);                    
                }

                dynamic language = new ExpandoObject();
                AddProperty(language, dbLanguage.Shortname, item);
                return language;
            }
            
        }

        public static dynamic GetLanguages()
        {
            //Logger log = LogManager.GetCurrentClassLogger();

            using (var db = new DitatEntities())
            {
                dynamic language = new ExpandoObject();

                var dbLanguages = db.Languages.Where(i => i.Active == true);                

                foreach (var dbLanguage in dbLanguages)
                {
                    var languageId = dbLanguage.Id;
                    var texts = db.LanguageTexts.Where(l => l.LanguageId == languageId).ToList();

                    //log.Info("GetLanguages LanguageTexts: " + texts.Count().ToString());

                    dynamic item = new ExpandoObject();

                    foreach (var text in texts)
                    {
                        AddProperty(item, text.KeyName, text.Value);
                    }

                    
                    AddProperty(language, dbLanguage.Shortname, item);
                }
                
                return language;
            }
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
