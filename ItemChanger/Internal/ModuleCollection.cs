using System.Reflection;
using ItemChanger.Modules;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Module = ItemChanger.Modules.Module;

namespace ItemChanger.Internal
{
    public class ModuleCollection
    {
        [JsonConverter(typeof(ModuleListDeserializer))]
        public List<Module> Modules = new();

        public void Initialize()
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].LoadOnce();
            }
        }

        public void Unload()
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].UnloadOnce();
            }
        }

        public Module Add(Module m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            Modules.Add(m);
            if (Settings.loaded) m.LoadOnce();
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
                LogError($"Unable to instantiate module of type {T.Name} through reflection:\n{e}");
                throw;
            }
        }

        /// <summary>
        /// Returns the first module of type T, or default.
        /// </summary>
        public T? Get<T>()
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
            if (Modules.Remove(m) && Settings.loaded) m.UnloadOnce();
        }

        public void Remove<T>()
        {
            if (Modules.OfType<T>().FirstOrDefault() is Module m) Remove(m);
        }

        public void Remove(Type T)
        {
            if (Settings.loaded)
            {
                foreach (Module m in Modules.Where(m => m.GetType() == T)) m.UnloadOnce();
            }
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
                mc.Modules.Add((Module)Activator.CreateInstance(T));
            }

            return mc;
        }

        public class ModuleListSerializer : JsonConverter<List<Module>>
        {
            public override bool CanRead => false;
            public override bool CanWrite => true;
            public bool RemoveNewProfileModules;
            public override List<Module> ReadJson(JsonReader reader, Type objectType, List<Module>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
            public override void WriteJson(JsonWriter writer, List<Module>? value, JsonSerializer serializer)
            {
                if (value is null)
                {
                    writer.WriteNull();
                    return;
                }

                if (RemoveNewProfileModules)
                {
                    value = new(value.Where(t => !t.ModuleHandlingProperties.HasFlag(ModuleHandlingFlags.RemoveOnNewProfile)));
                }

                serializer.Serialize(writer, value.ToArray());
            }
        }

        public class ModuleListDeserializer : JsonConverter<List<Module>>
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override List<Module>? ReadJson(JsonReader reader, Type objectType, List<Module>? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JToken jt = JToken.Load(reader);
                if (jt.Type == JTokenType.Null) return null;
                else if (jt.Type == JTokenType.Array)
                {
                    JArray ja = (JArray)jt;
                    List<Module> list = new(ja.Count);
                    foreach (JToken jModule in ja)
                    {
                        Module t;
                        try
                        {
                            t = jModule.ToObject<Module>(serializer)!;
                        }
                        catch (Exception e)
                        {
                            ModuleHandlingFlags flags = ((JObject)jModule).GetValue(nameof(Module.ModuleHandlingProperties))?.ToObject<ModuleHandlingFlags>(serializer) ?? ModuleHandlingFlags.None;
                            if (flags.HasFlag(ModuleHandlingFlags.AllowDeserializationFailure))
                            {
                                t = new InvalidModule
                                {
                                    JSON = jModule,
                                    DeserializationError = e,
                                };
                            }
                            else throw;
                        }
                        list.Add(t);
                    }
                    return list;
                }
                throw new JsonSerializationException("Unable to handle tag list pattern: " + jt.ToString());
            }

            public override void WriteJson(JsonWriter writer, List<Module>? value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
