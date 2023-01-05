using Newtonsoft.Json;

namespace ItemChanger
{
    /// <summary>
    /// Stores a delegate by name for use with a value-providing interface (IBool, IString, ISprite).
    /// The function must be defined before the value property is accessed. To ensure compatibility with round-trip deserialization, it is advised to define the function from a Module or Tag's Load.
    /// </summary>
    public class NamedFunction<T>
    {
        private static readonly Dictionary<string, Func<T>> lookupTable = new();

        public string Name { get; }
        [JsonIgnore]
        public T Value
        {
            get
            {
                if (!lookupTable.TryGetValue(Name, out Func<T> f))
                {
                    throw new InvalidOperationException($"NamedFunction {Name} for type {typeof(T).Name} is not defined.");
                }
                return f();
            }
        }

        public NamedFunction(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        /// Defines the named function, and returns a object which refers to it. Throws an exception if a function with the same name has already been defined.
        /// </summary>
        public static NamedFunction<T> Define(string name, Func<T> getter)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            if (getter is null) throw new ArgumentNullException(nameof(getter));
            if (lookupTable.ContainsKey(name)) throw new InvalidOperationException($"NamedFunction {name} for type {typeof(T).Name} is already defined.");

            lookupTable.Add(name, getter);
            return new NamedFunction<T>(name);
        }

        /// <summary>
        /// Defines the named function, and returns a object which refers to it. Overwrites any function with the same name which has already been defined.
        /// </summary>
        public static NamedFunction<T> Redefine(string name, Func<T> getter)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            if (getter is null) throw new ArgumentNullException(nameof(getter));

            lookupTable[name] = getter;
            return new NamedFunction<T>(name);
        }

        internal static void Clear() => lookupTable.Clear();
    }

    public class NamedBoolFunction : NamedFunction<bool>, IBool
    {
        [JsonConstructor] public NamedBoolFunction(string name) : base(name) { }
        public IBool Clone() => this;
    }

    public class NamedStringFunction : NamedFunction<string>, IString
    {
        [JsonConstructor] public NamedStringFunction(string name) : base(name) { }
        public IString Clone() => this;
    }

    public class NamedSpriteFunction : NamedFunction<Sprite>, ISprite
    {
        [JsonConstructor] public NamedSpriteFunction(string name) : base(name) { }
        public ISprite Clone() => this;
    }
}
