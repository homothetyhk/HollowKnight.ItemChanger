using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Internal;
using UnityEngine;

namespace ItemChanger.Items
{
    public class CustomSkillItem : AbstractItem
    {
        public string boolName;
        public string moduleName;

        protected override void OnLoad()
        {
            Type T = Type.GetType(moduleName);
            if (T == null || !T.IsSubclassOf(typeof(Modules.Module))) throw new InvalidOperationException($"Module type {moduleName} was not found.");
            ItemChangerMod.Modules.GetOrAdd(T);
        }

        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(boolName, true);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(boolName);
        }
    }
}
