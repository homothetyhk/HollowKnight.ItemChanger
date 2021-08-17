namespace ItemChanger
{
    // Encapsulate the ModHook arguments to make it easier to deal with breaking API changes.
    public class LanguageGetArgs
    {
        public readonly string convo;
        public readonly string sheet;
        public readonly string orig;
        public string current;

        public LanguageGetArgs(string convo, string sheet, string orig)
        {
            this.convo = convo;
            this.sheet = sheet;
            this.current = this.orig = orig;
        }
    }
}
