using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
#pragma warning disable IDE1006 // Naming Styles

namespace ItemChanger
{
    public class CustomSkills
    {
        public static event Action<string, bool> AfterSetBool;

        // The default setting for a bool should be the one which gives vanilla behavior
        // Custom items which do not give vanilla behavior with either setting do not belong here
        private Dictionary<string, bool> _bools = new Dictionary<string, bool>
        {
            { nameof(canFocus), true },
            { nameof(canSwim), true },
            { nameof(canUpslash), true },
            { nameof(canSideslashRight), true },
            { nameof(canSideslashLeft), true },
            { nameof(canDashRight), false },
            { nameof(canDashLeft), false },
            { nameof(hasWalljumpLeft), false },
            { nameof(hasWalljumpRight), false },
        };

        /// <returns>The value of the bool in CustomSkills, or False if it was not found.</returns>
        public bool GetBool(string name)
        {
            return _bools.TryGetValue(name, out bool value) && value;
        }
        public void SetBool(string name, bool value)
        {
            _bools[name] = value;
            AfterSetBool?.Invoke(name, value);
        }

        private bool Get([CallerMemberName] string name = null)
        {
            return GetBool(name);
        }
        private void Set(bool value, [CallerMemberName] string name = null)
        {
            SetBool(name, value);
        }

        public bool canSwim
        {
            get => Get();
            set => Set(value);
        }


        public bool canFocus
        {
            get => Get();
            set => Set(value);
        }

        public bool canUpslash
        {
            get => Get();
            set => Set(value);
        }

        public bool canSideslashRight
        {
            get => Get();
            set => Set(value);
        }

        public bool canSideslashLeft
        {
            get => Get();
            set => Set(value);
        }

        public bool canDashRight
        {
            get => Get();
            set => Set(value);
        }

        public bool canDashLeft
        {
            get => Get();
            set => Set(value);
        }

        public bool hasWalljumpLeft
        {
            get => Get();
            set => Set(value);
        }

        public bool hasWalljumpRight
        {
            get => Get();
            set => Set(value);
        }
    }
}
