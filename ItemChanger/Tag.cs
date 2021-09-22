using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public abstract class Tag
    {
        public virtual void Load(object parent) { }
        public virtual void Unload(object parent) { }
        public virtual Tag Clone() => (Tag)MemberwiseClone();
    }
}
