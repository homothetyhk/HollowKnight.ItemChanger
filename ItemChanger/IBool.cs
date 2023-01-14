using ItemChanger.Extensions;
using Newtonsoft.Json;

namespace ItemChanger
{
    /// <summary>
    /// Interface which can supply a bool value. Used frequently for serializable bool tests.
    /// </summary>
    public interface IBool
    {
        bool Value { get; }
        IBool Clone();
    }

    /// <summary>
    /// IBool which supports write operations.
    /// </summary>
    public interface IWritableBool : IBool
    {
        new bool Value { get; set; }
    }

    /// <summary>
    /// IBool which represents a constant value.
    /// </summary>
    public class BoxedBool : IWritableBool
    {
        public bool Value { get; set; }

        public BoxedBool(bool Value) => this.Value = Value;

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    /// <summary>
    /// IBool which represents a PlayerData bool.
    /// </summary>
    public class PDBool : IWritableBool
    {
        public string boolName;

        public PDBool(string boolName) => this.boolName = boolName;

        [JsonIgnore]
        public bool Value
        {
            get => PlayerData.instance.GetBool(boolName);
            set => PlayerData.instance.SetBool(boolName, value);
        }

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    /// <summary>
    /// IBool which represents the value of a PersistentBoolData in SceneData.
    /// </summary>
    public class SDBool : IWritableBool
    {
        public string id;
        public string sceneName;
        public bool semiPersistent;

        [JsonConstructor]
        public SDBool(string id, string sceneName)
        {
            this.id = id;
            this.sceneName = sceneName;
            this.semiPersistent = false;
        }

        public SDBool(string id, string sceneName, bool semiPersistent)
        {
            this.id = id;
            this.sceneName = sceneName;
            this.semiPersistent = semiPersistent;
        }

        [JsonIgnore]
        public bool Value
        {
            get
            {
                return SceneData.instance.FindMyState(new PersistentBoolData
                {
                    id = id,
                    sceneName = sceneName,
                })?.activated ?? false;
            }
            set
            {
                Util.SceneDataUtil.Save(sceneName, id, value, semiPersistent);
            }
        }

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    /// <summary>
    /// IBool which represents comparison on a PlayerData int.
    /// <br/>Supports IWritableBool in one direction only (direction depends on comparison operator).
    /// </summary>
    public class PDIntBool : IWritableBool
    {
        public string intName;
        public int amount;
        public ComparisonOperator op;
        

        public PDIntBool(string intName, int amount, ComparisonOperator op = ComparisonOperator.Ge)
        {
            this.intName = intName;
            this.amount = amount;
            this.op = op;
        }

        [JsonIgnore]
        public bool Value
        {
            get
            {
                return PlayerData.instance.GetInt(intName).Compare(op, amount);
            }
            set
            {
                if (value)
                {
                    switch (op)
                    {
                        case ComparisonOperator.Ge:
                        case ComparisonOperator.Eq:
                        case ComparisonOperator.Le:
                            PlayerData.instance.SetInt(intName, amount);
                            break;
                        default: throw new NotSupportedException($"Cannot set PDIntBool true with operator {op}.");
                    }
                }
                else
                {
                    switch (op)
                    {
                        case ComparisonOperator.Gt:
                        case ComparisonOperator.Neq:
                        case ComparisonOperator.Lt:
                            PlayerData.instance.SetInt(intName, amount);
                            break;
                        default: throw new NotSupportedException($"Cannot set PDIntBool false with operator {op}.");
                    }
                }
            }
        }

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    /// <summary>
    /// IBool which searches for a placement by name and checks whether all items on the placement are obtained.
    /// <br/>If the placement does not exist, defaults to the value of missingPlacementTest, or true if missingPlacementTest is null.
    /// </summary>
    public class PlacementAllObtainedBool : IBool
    {
        public PlacementAllObtainedBool(string placementName, IBool missingPlacementTest)
        {
            this.placementName = placementName;
            this.missingPlacementTest = missingPlacementTest;
        }

        public string placementName;
        public IBool missingPlacementTest;

        [JsonIgnore]
        public bool Value
        {
            get
            {
                if (placementName != null && Internal.Ref.Settings is Settings s && s.Placements != null && s.Placements.TryGetValue(placementName, out var p) && p != null)
                {
                    return p.AllObtained();
                }
                return missingPlacementTest?.Value ?? true;
            }
        }

        public IBool Clone()
        {
            PlacementAllObtainedBool obj = (PlacementAllObtainedBool)MemberwiseClone();
            obj.missingPlacementTest = obj.missingPlacementTest?.Clone();
            return obj;
        }
    }

    /// <summary>
    /// IBool which searches for a placement by name and checks whether its VisitState includes specified flags.
    /// <br/>If the placement does not exist, defaults to the value of missingPlacementTest, or true if missingPlacementTest is null.
    /// </summary>
    public class PlacementVisitStateBool : IBool
    {
        public PlacementVisitStateBool(string placementName, VisitState requiredFlags, IBool missingPlacementTest)
        {
            this.placementName = placementName;
            this.requiredFlags = requiredFlags;
            this.missingPlacementTest = missingPlacementTest;
        }

        public string placementName;
        public VisitState requiredFlags;
        /// <summary>
        /// If true, requires any flag in requiredFlags to be contained in the VisitState. If false, requires all flags in requiredFlags to be contained in VisitState. Defaults to false.
        /// </summary>
        public bool requireAny;
        public IBool missingPlacementTest;

        [JsonIgnore]
        public bool Value
        {
            get
            {
                if (placementName != null && Internal.Ref.Settings is Settings s && s.Placements != null && s.Placements.TryGetValue(placementName, out var p) && p != null)
                {
                    return requireAny ? p.CheckVisitedAny(requiredFlags) : p.CheckVisitedAll(requiredFlags);
                }
                return missingPlacementTest?.Value ?? true;
            }
        }

        public IBool Clone()
        {
            PlacementVisitStateBool obj = (PlacementVisitStateBool)MemberwiseClone();
            obj.missingPlacementTest = obj.missingPlacementTest?.Clone();
            return obj;
        }
    }

    public class Disjunction : IBool
    {
        public List<IBool> bools = new();
        public Disjunction() { }
        public Disjunction(IEnumerable<IBool> bools)
        {
            this.bools.AddRange(bools);
        }
        public Disjunction(params IBool[] bools)
        {
            this.bools.AddRange(bools);
        }

        [JsonIgnore]
        public bool Value => bools.Any(b => b.Value);

        public IBool Clone()
        {
            return new Disjunction(bools.Select(b => b.Clone()));
        }

        public void OrWith(IBool b) => bools.Add(b);
    }

    public class Conjunction : IBool
    {
        public List<IBool> bools = new();
        public Conjunction() { }
        public Conjunction(IEnumerable<IBool> bools)
        {
            this.bools.AddRange(bools);
        }
        public Conjunction(params IBool[] bools)
        {
            this.bools.AddRange(bools);
        }

        [JsonIgnore]
        public bool Value => bools.All(b => b.Value);

        public IBool Clone()
        {
            return new Conjunction(bools.Select(b => b.Clone()));
        }

        public void AndWith(IBool b) => bools.Add(b);
    }

    public class Negation : IBool
    {
        public IBool Bool;

        [JsonConstructor]
        public Negation(IBool Bool)
        {
            this.Bool = Bool;
        }

        [JsonIgnore]
        public bool Value => !Bool.Value;

        public IBool Clone()
        {
            return new Negation(Bool.Clone());
        }
    }
}
