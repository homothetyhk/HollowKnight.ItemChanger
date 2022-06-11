using ItemChanger.Extensions;

namespace ItemChanger.Deployers
{
    /// <summary>
    /// IDeployer which destroys an object specified by name or path.
    /// </summary>
    public class ObjectDestroyer : IDeployer
    {
        public string SceneName { get; init; }
        public string ObjectName { get; init; }
        public NameMatchType MatchType { get; init; }
        public enum NameMatchType
        {
            /// <summary>
            /// Destroys the first gameobject found in hierarchy with the provided ObjectName name.
            /// </summary>
            FirstName,
            /// <summary>
            /// Destroys the first gameobject found in hierarchy with the provided ObjectName path.
            /// </summary>
            Path,
            /// <summary>
            /// Destroys all gameobjects found in hierarchy with name containing the provided ObjectName as a substring.
            /// </summary>
            NameSubstring
        }

        public void OnSceneChange(Scene to)
        {
            switch (MatchType)
            {
                case NameMatchType.FirstName:
                    UObject.Destroy(to.FindGameObjectByName(ObjectName));
                    break;
                case NameMatchType.Path:
                    UObject.Destroy(to.FindGameObject(ObjectName));
                    break;
                case NameMatchType.NameSubstring:
                    foreach (GameObject g in to.GetRootGameObjects())
                    {
                        if (g.name.Contains(ObjectName)) UObject.Destroy(g);
                        else SearchUnder(g.transform);
                    }
                    break;
            }
        }

        private void SearchUnder(Transform u)
        {
            if (u.childCount == 0) return;
            foreach (Transform t in u)
            {
                if (t.name.Contains(ObjectName)) UObject.Destroy(t.gameObject);
                else SearchUnder(t);
            }
        }
    }
}
