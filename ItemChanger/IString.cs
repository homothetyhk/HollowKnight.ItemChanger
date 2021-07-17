using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public interface IString
    {
        string Value { get; }
        IString Clone();
    }

    public class LanguageString : IString
    {
        public string key;
        public string sheet;

        public string Value => Language.Language.Get(key, sheet)?.Replace("<br>", "\n");
        public IString Clone() => (IString)MemberwiseClone();
    }

    public class BoxedString : IString
    {
        public string Value { get; set; }
        public IString Clone() => (IString)MemberwiseClone();
    }

}
