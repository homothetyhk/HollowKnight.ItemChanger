using ItemChanger.Extensions;

namespace ItemChanger.StartDefs
{
    public class RelativeStartDef : StartDef
    {
        /// <summary>
        /// The full path to the GameObject, with '/' separators.
        /// </summary>
        public string objPath;

        public override void CreateRespawnMarker(Scene startScene)
        {
            GameObject go = startScene.FindGameObject(objPath);
            CreateRespawnMarker(new Vector3(go.transform.position.x + Start.X, go.transform.position.y + Start.Y, 7.4f));
        }
    }
}
