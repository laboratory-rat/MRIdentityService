using System.Collections.Generic;
using System.Linq;
using Infrastructure.Entity.AppLanguage;
using Infrastructure.Model.Common;

namespace Tools
{
    public static class TranslationExtensions
    {
        public static Translation SelectTranslation(this List<Translation> source, string languageCode)
        {
            if(source == null || !source.Any())
            {
                return null;
            }

            var t = source.FirstOrDefault(x => x.LanguageCode == languageCode);
            if(t == null)
            {
                t = source.FirstOrDefault(x => x.LanguageCode == "en");
            }

            if(t == null)
            {
                t = source.First();
            }

            return t;
        }

        public static TranslationModel SelectTranslation(this List<TranslationModel> source, string languageCode)
        {
            if (source == null || !source.Any())
            {
                return null;
            }

            var t = source.FirstOrDefault(x => x.LanguageCode == languageCode);
            if (t == null)
            {
                t = source.FirstOrDefault(x => x.LanguageCode == "en");
            }

            if (t == null)
            {
                t = source.First();
            }

            return t;
        }
    }
}
