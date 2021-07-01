using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Placements
{
    public interface IContainerPlacement
    {
        void GetPrimaryContainer(out GameObject obj, out Container container);
    }
}
