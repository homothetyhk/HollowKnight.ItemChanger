using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger
{
    public interface ITransition
    {
        string SceneName { get; }
        string GateName { get; }
    }

    public readonly struct Transition : ITransition, IEquatable<Transition>
    {
        public string SceneName { get; }
        public string GateName { get; }

        [Newtonsoft.Json.JsonConstructor]
        public Transition(string SceneName, string GateName)
        {
            this.SceneName = SceneName;
            this.GateName = GateName;
        }


        public override string ToString()
        {
            return $"{SceneName}[{GateName}]";
        }

        public override int GetHashCode()
        {
            return SceneName.GetHashCode() + 31 * GateName.GetHashCode();
        }

        public bool Equals(Transition t)
        {
            return SceneName == t.SceneName && GateName == t.GateName;
        }

        public override bool Equals(object obj)
        {
            return obj is Transition t && Equals(t);
        }

        public static bool operator==(Transition t1, Transition t2)
        {
            return t1.SceneName == t2.SceneName && t1.GateName == t2.GateName;
        }

        public static bool operator !=(Transition t1, Transition t2) => !(t1 == t2);
    }
}
