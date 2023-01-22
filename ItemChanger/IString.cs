using ItemChanger.Extensions;
using Newtonsoft.Json;

namespace ItemChanger
{
    public interface IString
    {
        string Value { get; }
        IString Clone();
    }

    public class LanguageString : IString
    {
        public string sheet;
        public string key;

        public LanguageString(string sheet, string key)
        {
            this.sheet = sheet;
            this.key = key;
        }

        [JsonIgnore]
        public string Value => Language.Language.Get(key, sheet).Replace("<br>", "\n");
        public IString Clone() => (IString)MemberwiseClone();
    }

    /// <summary>
    /// An IString which substitutes arguments into a format string provided by Language.
    /// </summary>
    public class FormattedLanguageString : IString
    {
        public string sheet = "Fmt";
        public string key;
        public object[] args;

        public FormattedLanguageString() { }

        /// <summary>
        /// Creates a FormattedLanguageString for the specified key and args, targeting the "Fmt" sheet.
        /// </summary>
        public FormattedLanguageString(string key, params object[] args)
        {
            this.key = key;
            this.args = args;
        }

        [JsonIgnore] public string Value => string.Format(Language.Language.Get(key, sheet), args);
        public IString Clone()
        {
            return new FormattedLanguageString()
            {
                key = key,
                sheet = sheet,
                args = (object[])args.Clone()
            };
        }
    }

    public class BoxedString : IString
    {
        public string Value { get; set; }

        public BoxedString(string Value) => this.Value = Value;
        
        public IString Clone() => (IString)MemberwiseClone();
    }

    public class PaywallString : IString
    {
        public string key;
        public string sheet;

        public PaywallString(string sheet, string key)
        {
            this.sheet = sheet;
            this.key = key;
        }

        [JsonIgnore]
        public string Value
        {
            get
            {
                if (!int.TryParse(Language.Language.Get("PAYWALL_LIMIT", "IC"), out int limit))
                {
                    limit = 125;
                }

                return Language.Language.Get(key, sheet).Replace("<br>", "\n").CapLength(limit) + Language.Language.Get("PAYWALL_TEXT", "IC").Replace("<br>", "\n");
            }
        }

        public IString Clone() => (IString)MemberwiseClone();
    }

}
