using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which certain placements or locations may use to add a scene change after obtaining items.
    /// </summary>
    public class ChangeSceneTag : Tag
    {
        public Transition changeTo;
    }
}
