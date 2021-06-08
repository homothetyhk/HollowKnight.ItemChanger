using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ItemChanger
{
    public class WorldEvents
    {
        private Dictionary<string, bool> _bools = new Dictionary<string, bool>
        {
            { nameof(slyRescued), false },
            { nameof(slyBasementCompleted), false },
            { nameof(dreamNailCutsceneCompleted), false }
        };

        /// <returns>The value of the bool, or False if it was not found.</returns>
        public bool GetBool(string name)
        {
            return _bools.TryGetValue(name, out bool value) && value;
        }
        public void SetBool(string name, bool value)
        {
            _bools[name] = value;
        }
        private bool Get([CallerMemberName] string name = null)
        {
            return GetBool(name);
        }
        private void Set(bool value, [CallerMemberName] string name = null)
        {
            SetBool(name, value);
        }

        public bool slyRescued
        {
            get => Get();
            set => Set(value);
        }

        public bool slyBasementCompleted
        {
            get => Get();
            set => Set(value);
        }

        public bool dreamNailCutsceneCompleted
        {
            get => Get();
            set => Set(value);
        }
    }
}
