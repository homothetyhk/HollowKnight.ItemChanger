using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ItemChanger
{
    [Obsolete("Move toward location-based model in place of this.")]
    public class WorldEvents
    {
        private Dictionary<string, bool> _bools = new Dictionary<string, bool>
        {
            { nameof(slyRescued), false },
            { nameof(slyBasementCompleted), false },
            { nameof(dreamNailCutsceneCompleted), false },
            { nameof(nonlinearColosseums), true }
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

        /// <summary>
        /// Controls whether Sly appears in Room_ruinhouse or Room_shop. PlayerData.instance.slyRescued still controls whether the shop door is open in Dirtmouth.
        /// </summary>
        public bool slyRescued
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Controls whether Sly's basement is available when all three nail arts have been obtained.
        /// </summary>
        public bool slyBasementCompleted
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Controls whether the dreamer cutscene and binding shield appear in RestingGrounds_04.
        /// </summary>
        public bool dreamNailCutsceneCompleted
        {
            get => Get();
            set => Set(value);
        }

        /// <summary>
        /// Controls whether colosseum trials can be accessed out of order.
        /// </summary>
        public bool nonlinearColosseums
        {
            get => Get();
            set => Set(value);
        }
    }
}
