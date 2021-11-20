namespace ItemChanger.StartDefs
{
    /// <summary>
    /// Accounts for the fact that transitions are not consistently placed as root gameobjects
    /// </summary>
    public class TransitionBasedStartDef : RelativeStartDef
    {
        [Newtonsoft.Json.JsonConstructor]
        private TransitionBasedStartDef() { }

        /// <summary>
        /// Attempts to find a start location near the corresponding transition. Unlikely to succeed for bottom transitions.
        /// </summary>
        public static TransitionBasedStartDef FromGate(string sceneName, string entryGateName, int mapZone = 2)
        {
            var def = new TransitionBasedStartDef
            {
                SceneName = sceneName,
                objPath = entryGateName,
                MapZone = mapZone,
                X = 0f,
                Y = 0f,
            };

            switch (entryGateName[0])
            {
                case 'l':
                    def.X += 1.5f;
                    break;
                case 'b':
                    def.X += 3f;
                    def.Y += 5f;
                    break;
                case 't':
                    def.Y -= 1.5f;
                    break;
                case 'r':
                    def.X -= 1.5f;
                    break;
                case 'd':
                    break;
            }

            return def;
        }

        public override void CreateRespawnMarker(Scene startScene)
        {
            GameObject[] objects = startScene.GetRootGameObjects();
            GameObject go = null;
            foreach (GameObject g in objects)
            {
                if (g.name == objPath && g.GetComponent<TransitionPoint>() != null)
                {
                    go = g;
                    break;
                }
                else if (g.name == "_Transition Gates" && g.GetComponentsInChildren<TransitionPoint>().FirstOrDefault(t => t.gameObject.name == objPath) is TransitionPoint tp)
                {
                    go = tp.gameObject;
                    break;
                }
            }
            if (go == null)
            {
                go = objects.SelectMany(g => g.GetComponentsInChildren<TransitionPoint>()).FirstOrDefault(t => t.gameObject.name == objPath).gameObject;
            }
            if (go == null) throw new ArgumentException($"Could not find transition point {objPath}.");

            CreateRespawnMarker(new Vector3(go.transform.position.x + Start.X, go.transform.position.y + Start.Y, 7.4f));
        }
    }
}
