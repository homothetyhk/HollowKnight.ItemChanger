using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public abstract class Tag
    {
        /// <summary>
        /// Intrinsic tags are cloned when the parent is cloned. Extrinsic tags are cloned when the parent is replaced.
        /// </summary>
        public virtual bool Intrinsic => false;
    }
}
