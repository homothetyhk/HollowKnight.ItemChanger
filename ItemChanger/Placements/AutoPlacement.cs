using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using ItemChanger.Util;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public class AutoPlacement : AbstractPlacement
    {
        public AutoLocation location;
        public override AbstractLocation Location => location;
    }
}
