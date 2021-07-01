using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Locations
{
    public class PDDualLocation : DualLocation
    {
        public string pdBool;
        public override bool Decide()
        {
            return PlayerData.instance.GetBool(pdBool);
        }
    }
}
