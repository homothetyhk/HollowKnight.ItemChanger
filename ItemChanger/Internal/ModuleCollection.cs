using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ItemChanger.Modules;
using Module = ItemChanger.Modules.Module;

namespace ItemChanger.Internal
{
    public class ModuleCollection
    {
        public List<Module> Modules = new();

        public void Initialize()
        {
            foreach (Module m in Modules)
            {
                try
                {
                    m.Initialize();
                }
                catch (Exception e)
                {
                    ItemChangerMod.instance.LogError($"Error initializing module {m.Name}:\n{e}");
                }
            }
        }

        public void Unload()
        {
            foreach (Module m in Modules)
            {
                try
                {
                    m.Unload();
                }
                catch (Exception e)
                {
                    ItemChangerMod.instance.LogError($"Error unloading module {m.Name}:\n{e}");
                }
            }
        }

        public void Add(Module m)
        {
            Modules.Add(m);
            if (Settings.loaded) m.Initialize();
        }

        public void Add<T>() where T : Module, new()
        {
            Add(new T());
        }

        /// <summary>
        /// Returns the first module of type T, or default.
        /// </summary>
        public T Get<T>()
        {
            return Modules.OfType<T>().FirstOrDefault();
        }

        public void Remove(Module m)
        {
            if (Modules.Remove(m) && Settings.loaded) m.Unload();
        }

        public void Remove<T>()
        {
            if (Modules.OfType<T>().FirstOrDefault() is Module m) Remove(m);
        }

        public void Remove(string name)
        {
            if (Modules.Where(m => m.Name == name).FirstOrDefault() is Module m) Remove(m);
        }

        public static ModuleCollection Create()
        {
            ModuleCollection mc = new();

            foreach (Type T in typeof(Module).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Module)) && !t.IsAbstract))
            {
                ConstructorInfo ci = T.GetConstructor(Type.EmptyTypes);
                Module m = ci?.Invoke(Array.Empty<object>()) as Module;
                if (m?.Default ?? false) mc.Modules.Add(m);
            }

            return mc;
        }
    }
}
