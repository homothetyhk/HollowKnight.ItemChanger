using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Modules
{
    public abstract class Module
    {
        public string Name => GetType().Name;
        public abstract void Initialize();
        public abstract void Unload();
        public virtual bool Default => true;
    }
}
