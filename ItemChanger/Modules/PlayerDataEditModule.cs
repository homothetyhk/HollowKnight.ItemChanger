using System.Reflection;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which stores requested PlayerData changes and applies them when the save is loaded. Useful for ensuring those changes are applied if the same IC data is stored and loaded with a new save.
    /// </summary>
    [DefaultModule]
    public class PlayerDataEditModule : Module
    {
        public override void Initialize()
        {
            Events.OnEnterGame += OnEnterGame;
        }

        public override void Unload()
        {
            Events.OnEnterGame -= OnEnterGame;
        }

        private void OnEnterGame() => ProcessEdits();

        public void ProcessEdits()
        {
            while (PDEdits.Count > 0)
            {
                PDEdit edit = PDEdits.Dequeue();
                try
                {
                    edit.Apply();
                }
                catch (Exception e)
                {
                    LogError($"Error processing PlayerDataEdits on {edit}:\n{e}");
                }

                PDEditHistory.Add(edit);
            }
        }

        public void AddPDEdit(string fieldName, object value)
        {
            PDEdits.Enqueue(new(fieldName, value));
        }

        public record PDEdit(string FieldName, object Value)
        {
            public void Apply()
            {
                switch (Value)
                {
                    case bool b:
                        PlayerData.instance.SetBool(FieldName, b);
                        break;
                    case int i:
                        PlayerData.instance.SetInt(FieldName, i);
                        break;
                    case long l:
                        PlayerData.instance.SetInt(FieldName, (int)l); // boxed ints round-trip deserialize to boxed longs
                        break;
                    case string s:
                        PlayerData.instance.SetString(FieldName, s);
                        break;
                    case float f:
                        PlayerData.instance.SetFloat(FieldName, f);
                        break;
                    case double d:
                        PlayerData.instance.SetFloat(FieldName, (float)d);
                        break;
                }
            }
        }

        public Queue<PDEdit> PDEdits = new();
        public List<PDEdit> PDEditHistory = new();
    }

}
