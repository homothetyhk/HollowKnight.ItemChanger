using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class AutoUnlockIselda : Module
    {
        public override void Initialize()
        {
            PlayerData.instance.SetBool(nameof(PlayerData.openedMapperShop), true);
        }

        public override void Unload() { }
    }
}
