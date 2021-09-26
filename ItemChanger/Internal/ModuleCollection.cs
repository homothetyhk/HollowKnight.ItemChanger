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

        public Module Add(Module m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            Modules.Add(m);
            if (Settings.loaded) m.Initialize();
            return m;
        }

        public T Add<T>() where T : Module, new()
        {
            T t = new();
            return (T)Add(t);
        }

        public Module Add(Type T)
        {
            try
            {
                Module m = (Module)Activator.CreateInstance(T);
                return Add(m);
            }
            catch (Exception e)
            {
                ItemChangerMod.instance.LogError($"Unable to instantiate module of type {T.Name} through reflection:\n{e}");
                throw;
            }
        }

        /// <summary>
        /// Returns the first module of type T, or default.
        /// </summary>
        public T Get<T>()
        {
            return Modules.OfType<T>().FirstOrDefault();
        }

        public T GetOrAdd<T>() where T : Module, new()
        {
            T t = Modules.OfType<T>().FirstOrDefault();
            if (t == null) t = Add<T>();
            return t;
        }

        public Module GetOrAdd(Type T)
        {
            Module m = Modules.FirstOrDefault(m => T.IsInstanceOfType(m));
            if (m == null) m = Add(T);
            return m;
        }

        public void Remove(Module m)
        {
            if (Modules.Remove(m) && Settings.loaded) m.Unload();
        }

        public void Remove<T>()
        {
            if (Modules.OfType<T>().FirstOrDefault() is Module m) Remove(m);
        }

        public void Remove(Type T)
        {
            Modules.RemoveAll(m => m.GetType() == T);
        }

        public void Remove(string name)
        {
            if (Modules.Where(m => m.Name == name).FirstOrDefault() is Module m) Remove(m);
        }

        public static ModuleCollection Create()
        {
            ModuleCollection mc = new();

            foreach (Type T in typeof(Module).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Module)) && !t.IsAbstract && Attribute.IsDefined(t, typeof(DefaultModuleAttribute))))
            {
                ConstructorInfo ci = T.GetConstructor(Type.EmptyTypes);
                Module m = ci?.Invoke(Array.Empty<object>()) as Module;
                if (m != null) mc.Modules.Add(m);
            }

            return mc;
        }
    }
}
