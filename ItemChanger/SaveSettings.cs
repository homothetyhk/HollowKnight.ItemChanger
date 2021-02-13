using Modding;
using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public class SaveSettings : BaseSettings
    {
        SerializableBoolDictionary obtainedItems = new SerializableBoolDictionary();

        public SaveSettings() 
        {
            AfterDeserialize += () => obtainedItems = obtainedItems ?? new SerializableBoolDictionary();
        }

        public bool gotSlyCharm
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool canFocus
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool CheckObtained(string id)
        {
            if (obtainedItems.TryGetValue(id, out bool val)) return val;
            return false;
        }

        public void SetObtained(string id)
        {
            try
            {
                obtainedItems[id] = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
