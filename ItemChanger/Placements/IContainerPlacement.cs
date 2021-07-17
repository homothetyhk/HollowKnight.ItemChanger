using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Placements
{
    public interface IContainerPlacement
    {
        void GetContainer(AbstractLocation location, out GameObject obj, out string containerType);
    }
}
