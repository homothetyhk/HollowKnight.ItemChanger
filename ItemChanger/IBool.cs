using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public interface IBool
    {
        bool Value { get; }
        IBool Clone();
    }

    public class BoxedBool : IBool
    {
        public bool Value { get; set; }
        public IBool Clone() => (IBool)MemberwiseClone();
    }

    public class PDBool : IBool
    {
        public string boolName;
        public bool Value => PlayerData.instance.GetBool(boolName);
        public IBool Clone() => (IBool)MemberwiseClone();
    }

    public class SDBool : IBool
    {
        public string id;
        public string sceneName;

        public bool Value => SceneData.instance.FindMyState(new PersistentBoolData
        {
            id = id,
            sceneName = sceneName,
        })?.activated ?? false;
        public IBool Clone() => (IBool)MemberwiseClone();
    }
}
