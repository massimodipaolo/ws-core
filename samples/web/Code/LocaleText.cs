using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Code
{
    public class LocaleText
    {
        public string LanguageId { get; set; }
        public string Text { get; set; }
    }

    public class LocaleTexts : List<LocaleText>
    {
    }

}
