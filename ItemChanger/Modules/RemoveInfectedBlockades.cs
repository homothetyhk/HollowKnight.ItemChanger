using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which completely removes all four blockades from Infected Crossroads.
    /// </summary>
    public class RemoveInfectedBlockades : Module
    {
        public static readonly Transition[] BlockedTransitions = new Transition[]
        {
            new(SceneNames.Crossroads_03, "bot1"),
            new(SceneNames.Crossroads_06, "right1"),
            new(SceneNames.Crossroads_10, "left1"),
            new(SceneNames.Crossroads_19, "top1"),
        };

        public override void Initialize()
        {
            Events.AddSceneChangeEdit(SceneNames.Crossroads_03, DestroyBlockade_Crossroads_03);
            Events.AddSceneChangeEdit(SceneNames.Crossroads_06, DestroyBlockade_Crossroads_06);
            Events.AddSceneChangeEdit(SceneNames.Crossroads_10, DestroyBlockade_Crossroads_10);
            Events.AddSceneChangeEdit(SceneNames.Crossroads_19, DestroyBlockade_Crossroads_19);
        }

        public override void Unload()
        {
            Events.RemoveSceneChangeEdit(SceneNames.Crossroads_03, DestroyBlockade_Crossroads_03);
            Events.RemoveSceneChangeEdit(SceneNames.Crossroads_06, DestroyBlockade_Crossroads_06);
            Events.RemoveSceneChangeEdit(SceneNames.Crossroads_10, DestroyBlockade_Crossroads_10);
            Events.RemoveSceneChangeEdit(SceneNames.Crossroads_19, DestroyBlockade_Crossroads_19);
        }

        private static readonly string[] crossroads_03_bot1_blockade = new[]
        {
            "Infected_event/infected_large_blob_020000 (2)",
            "Infected_event/infected_large_blob_020000",
            "Infected_event/infected_large_blob_sil (6)",
            "Infected_event/infected_large_blob_sil (2)",
            "Infected_event/infected_large_blob_sil (1)",
            "Infected_event/infected_large_blob_sil",
            "Infected_event/infected_blockade", // this is the only object with a collider, the rest are blobs that make the transition appear blocked.
            "Infected_event/infected_large_blob_sil (7)",
            "Infected_event/infected_large_blob_sil (8)",
            "Infected_event/infected_large_blob_sil (9)",
            "Infected_event/infected_large_blob_sil (11)",
            "Infected_event/infected_vine_03 sil (1)/infected_small_blob_010000 (4)",
            "Infected_event/infected_vine_03 sil (1)/infected_small_blob_010000 (5)",
            "Infected_event/infected_large_blob_020000 (4)",
        };

        internal static void DestroyBlockade_Crossroads_03(Scene scene)
        {
            foreach (string path in crossroads_03_bot1_blockade)
            {
                UObject.Destroy(scene.FindGameObject(path));
            }
        }

        internal static void DestroyBlockade_Crossroads_06(Scene scene)
        {
            UObject.Destroy(scene.FindGameObject("Infected Parent/infected_blockade"));
        }

        internal static void DestroyBlockade_Crossroads_10(Scene scene)
        {
            UObject.Destroy(scene.FindGameObject("infected_event/infected_blockade"));
        }

        internal static void DestroyBlockade_Crossroads_19(Scene scene)
        {
            UObject.Destroy(scene.FindGameObject("Infected Parent/infected_blockade"));
        }
    }
}
