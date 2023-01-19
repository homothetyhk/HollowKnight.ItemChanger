namespace ItemChanger.Extensions
{
    /// <summary>
    /// Extensions for Unity objects, and particularly for interacting with Scenes.
    /// </summary>
    public static class UnityExtensions
    {
        public static GameObject? FindChild(this GameObject g, string name)
        {
            Transform t = g.transform.Find(name);
            return t != null ? t.gameObject : null;
        }

        public static GameObject FindChild(this GameObject g, IEnumerable<string> steps)
        {
            var t = g.transform;
            foreach (string s in steps) t = t.Find(s);
            return t.gameObject;
        }

        public static string GetPathInHierarchy(this Transform t)
        {
            if (t.parent == null) return t.name;
            else return $"{t.parent.GetPathInHierarchy()}/{t.name}";
        }

        static readonly List<GameObject> rootObjects = new List<GameObject>(500);
        /// <summary>
        /// Finds a GameObject in the given scene by its full path.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="path">The full path to the GameObject, with forward slash ('/') separators.</param>
        /// <returns></returns>
        public static GameObject? FindGameObject(this Scene s, string path)
        {
            s.GetRootGameObjects(rootObjects); // clears list

            int index = path.IndexOf('/');
            GameObject? result = null;
            if (index >= 0)
            {
                string rootName = path.Substring(0, index);
                GameObject root = rootObjects.FirstOrDefault(g => g.name == rootName);
                if (root != null) result = root.transform.Find(path.Substring(index + 1)).gameObject;
            }
            else
            {
                result = rootObjects.FirstOrDefault(g => g.name == path);
            }

            rootObjects.Clear();
            return result;
        }

        /// <summary>
        /// Breadth first search through the entire hierarchy. Returns the first GameObject with the given name, or null.
        /// </summary>
        public static GameObject? FindGameObjectByName(this Scene s, string name)
        {
            s.GetRootGameObjects(rootObjects);
            GameObject? result = null;

            foreach (GameObject g in rootObjects)
            {
                if (g.name == name)
                {
                    result = g;
                    break;
                }
            }

            if (result == null)
            {
                foreach (GameObject g in rootObjects)
                {
                    result = g.FindChildInHierarchy(name);
                    if (result != null) break;
                }
            }

            rootObjects.Clear();
            return result;
        }

        /// <summary>
        /// Returns a list of objects in the scene hierarchy, ordered by depth-first-search.
        /// <br/>The list consists of pairs where the first entry is the object path and the second entry is the object.
        /// </summary>
        public static List<(string path, GameObject go)> Traverse(this Scene s)
        {
            s.GetRootGameObjects(rootObjects);
            List<(string, GameObject)> results = new();
            foreach (GameObject g in rootObjects)
            {
                TraverseInternal(string.Empty, g.transform, results);
            }
            return results;
        }

        private static void TraverseInternal(string path, Transform t, List<(string, GameObject)> results)
        {
            path = $"{path}/{t.name}";
            results.Add((path, t.gameObject));
            foreach (Transform u in t)
            {
                TraverseInternal(path, u, results);
            }
        }


        /// <summary>
        /// Breadth first search. Returns GameObject with given name, or null if not found. Parent object not included in search.
        /// </summary>
        public static GameObject? FindChildInHierarchy(this GameObject g, string name)
        {
            Queue<Transform> q = new();
            q.Enqueue(g.transform);

            while (q.Any())
            {
                Transform t = q.Dequeue();
                foreach (Transform u in t)
                {
                    if (u.name == name) return u.gameObject;
                    else q.Enqueue(u);
                }
            }

            return null;
        }
    }
}
