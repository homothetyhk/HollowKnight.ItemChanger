namespace ItemChanger
{
    public readonly struct FsmID
    {
        /// <summary>
        /// Create an FsmID which matches any object with this FsmName.
        /// </summary>
        public FsmID(string FsmName)
        {
            this.ObjectName = null;
            this.FsmName = FsmName;
        }


        /// <summary>
        /// Create an FsmID which matches any object with this ObjectName and FsmName.
        /// </summary>
        [Newtonsoft.Json.JsonConstructor]
        public FsmID(string ObjectName, string FsmName)
        {
            this.ObjectName = ObjectName;
            this.FsmName = FsmName;
        }

        /// <summary>
        /// Create an FsmID which matches any object with the same ObjectName and FsmName as the fsm.
        /// </summary>
        public FsmID(PlayMakerFSM fsm) : this(fsm.gameObject.name, fsm.FsmName) { }

        public readonly string ObjectName;
        public readonly string FsmName;

        public override int GetHashCode()
        {
            int h1 = ObjectName?.GetHashCode() ?? 0;
            int h2 = FsmName.GetHashCode();

            return ((h1 << 5) + h1) ^ h2;
        }

        public override string ToString()
        {
            return ObjectName == null ? FsmName : $"{ObjectName}-{FsmName}";
        }

    }
}
