namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which adds extra hazard respawn markers to rooms that ordinarily don't contain hazards, but do contain water which can become hazardous without Swim.
    /// </summary>
    [DefaultModule]
    public class WaterHazardRespawns : Module
    {
        public override void Initialize()
        {
            Events.OnSceneChange += AddWaterSpawns;
        }

        public override void Unload()
        {
            Events.OnSceneChange -= AddWaterSpawns;
        }

        public static void AddWaterSpawns(Scene to)
        {
            switch (to.name)
            {
                case SceneNames.Crossroads_50:
                    CreateWaterSpawn(19f, 45.5f, 1f, 5f, true);
                    CreateWaterSpawn(21f, 26.5f, 6f, 8f, true);
                    CreateWaterSpawn(231f, 26.5f, 6f, 8f, false);
                    break;

                case SceneNames.GG_Atrium:
                    CreateWaterSpawn(148.5f, 63f, 10f, 5f, false);
                    CreateWaterSpawn(140f, 18f, 1f, 13f, true);
                    break;

                case SceneNames.GG_Lurker:
                    CreateWaterSpawn(84f, 112.5f, 1f, 15f, true);
                    CreateWaterSpawn(102f, 110f, 1f, 10f, false);
                    CreateWaterSpawn(79f, 80f, 1f, 10f, true);
                    CreateWaterSpawn(109f, 59.5f, 1f, 17f, true);
                    CreateWaterSpawn(177.5f, 57.5f, 1f, 15f, true);
                    CreateWaterSpawn(181f, 82.5f, 1f, 10f, true);
                    CreateWaterSpawn(189f, 82.5f, 1f, 10f, false);
                    break;

                case SceneNames.GG_Waterways:
                    CreateWaterSpawn(72.5f, 16f, 2f, 9f, true);
                    CreateWaterSpawn(61.5f, 63.5f, 4f, 7f, true);
                    CreateWaterSpawn(81f, 63f, 1f, 6f, false);
                    CreateWaterSpawn(91f, 63f, 1f, 6f, true);
                    CreateWaterSpawn(95f, 49.75f, 5.5f, 6f, false);
                    CreateWaterSpawn(128f, 62f, 1f, 22f, false);
                    break;

                case SceneNames.RestingGrounds_08:
                    CreateWaterSpawn(37.5f, 11f, 1f, 15f, true);
                    CreateWaterSpawn(62f, 11f, 1f, 15f, false);
                    CreateWaterSpawn(44f, 38f, 1f, 14f, true);
                    CreateWaterSpawn(56f, 38f, 1f, 14f, false);
                    CreateWaterSpawn(44f, 63f, 1f, 14f, true);
                    CreateWaterSpawn(56f, 62.5f, 1f, 17f, false);
                    CreateWaterSpawn(107.7f, 9.5f, 1f, 11f, true);
                    CreateWaterSpawn(107.7f, 20.95f, 1f, 10.9f, true);
                    CreateWaterSpawn(107.7f, 33.45f, 1f, 14.6f, true);
                    CreateWaterSpawn(116.5f, 13f, 2f, 6f, true);
                    CreateWaterSpawn(127.5f, 27f, 1f, 5f, false);
                    CreateWaterSpawn(116.5f, 61f, 1f, 10f, true);
                    CreateWaterSpawn(124f, 61f, 1f, 10f, false);
                    break;

                case SceneNames.Room_GG_Shortcut:
                    CreateWaterSpawn(36.5f, 43f, 4f, 7f, false);
                    CreateWaterSpawn(32f, 74f, 4f, 9f, true);
                    CreateWaterSpawn(9.5f, 89f, 9f, 5f, true);
                    break;

                case SceneNames.GG_Pipeway:
                    CreateWaterSpawn(8f, 15f, 1f, 7f, false);
                    CreateWaterSpawn(6f, 24f, 1f, 7f, false);
                    break;

                case SceneNames.Ruins1_03:
                    CreateWaterSpawn(125f, 48f, 1f, 16f, false);
                    CreateWaterSpawn(127f, 14.5f, 1f, 15f, false);
                    CreateWaterSpawn(81f, 29.75f, 1f, 8.5f, true);
                    CreateWaterSpawn(54.5f, 29.75f, 1f, 8.5f, false);
                    CreateWaterSpawn(40f, 29.25f, 1f, 9.5f, true);
                    CreateWaterSpawn(38.45f, 15.9f, 3.9f, 12.2f, true);
                    CreateWaterSpawn(48.6f, 13.9f, 4f, 8.2f, true);
                    CreateWaterSpawn(57.1f, 11.75f, 1.2f, 12.5f, false);
                    CreateWaterSpawn(80f, 11.75f, 1.2f, 12.5f, true);
                    CreateWaterSpawn(96.05f, 14.25f, 10.7f, 15.5f, false);
                    CreateWaterSpawn(110.95f, 13.75f, 2.9f, 12.5f, false);
                    CreateWaterSpawn(24.5f, 11.75f, 1f, 9.5f, true);
                    break;

                case SceneNames.Ruins1_04:
                    CreateWaterSpawn(93f, 14.5f, 1f, 8f, false);
                    CreateWaterSpawn(59.52f, 14.5f, 1f, 8f, false);
                    CreateWaterSpawn(49f, 15.5f, 1f, 6f, false);
                    CreateWaterSpawn(39.5f, 13.75f, 2f, 9.5f, false);
                    CreateWaterSpawn(32f, 7f, 1f, 5f, true);
                    CreateWaterSpawn(32f, 14.05f, 1f, 8.9f, true);
                    CreateWaterSpawn(49.5f, 39.75f, 1f, 10.5f, true);
                    break;

                case SceneNames.Ruins1_27:
                    CreateWaterSpawn(6f, 9f, 1f, 12f, true);
                    CreateWaterSpawn(51.5f, 8f, 1f, 10f, false);
                    break;

                case SceneNames.Ruins2_04:
                    CreateWaterSpawn(91.5f, 11.9f, 1f, 12.2f, true);
                    CreateWaterSpawn(101.8f, 9.15f, 1f, 10.3f, false);
                    CreateWaterSpawn(96.65f, 10.2f, 9.3f, 1.6f, true);
                    break;

                case SceneNames.Ruins2_06:
                    CreateWaterSpawn(25.75f, 45.25f, 12.5f, 5.5f, true);
                    CreateWaterSpawn(25.75f, 38.5f, 12.5f, 5f, true);
                    CreateWaterSpawn(25.75f, 29.25f, 12.5f, 9.5f, true);
                    CreateWaterSpawn(25.75f, 17.25f, 12.5f, 9.5f, true);
                    break;

                case SceneNames.Ruins2_07:
                    CreateWaterSpawn(30.4f, 8.25f, 1f, 7.5f, true);
                    CreateWaterSpawn(65f, 14f, 1f, 6f, false);
                    CreateWaterSpawn(81.5f, 15f, 1f, 8f, true);
                    break;

                case SceneNames.Waterways_01:
                    CreateWaterSpawn(127f, 32f, 1f, 6f, true);
                    CreateWaterSpawn(127f, 12.5f, 1f, 7f, true);
                    CreateWaterSpawn(65.5f, 12.5f, 1f, 7f, true);
                    CreateWaterSpawn(96f, 12.5f, 1f, 7f, false);
                    break;

                case SceneNames.Waterways_02:
                    CreateWaterSpawn(69f, 12f, 1f, 4f, true);
                    CreateWaterSpawn(90f, 7.5f, 1f, 5f, false);
                    CreateWaterSpawn(85.5f, 28.75f, 1f, 4.5f, true);
                    CreateWaterSpawn(75.5f, 28.75f, 1f, 4.5f, false);
                    break;

                case SceneNames.Waterways_04:
                    CreateWaterSpawn(70.5f, 32.55f, 1f, 6.1f, true);
                    CreateWaterSpawn(127f, 20.55f, 1f, 4.9f, false);
                    break;

                case SceneNames.Waterways_04b:
                    CreateWaterSpawn(108f, 24.95f, 1f, 4.5f, true);
                    CreateWaterSpawn(32.5f, 21.15f, 1f, 5.3f, true);
                    CreateWaterSpawn(96f, 21.05f, 1f, 5.1f, false);
                    CreateWaterSpawn(26.5f, 10.5f, 1f, 6f, true);
                    break;

                case SceneNames.Waterways_12:
                    CreateWaterSpawn(11.4f, 6f, 5.8f, 4f, true);
                    CreateWaterSpawn(27f, 6f, 7f, 4f, false);
                    CreateWaterSpawn(25.45f, 10.65f, 1.9f, 3.3f, false);
                    CreateWaterSpawn(29.575f, 20.35f, 2.55f, 5.3f, false);
                    CreateWaterSpawn(25.36f, 24.325f, 1.68f, 5.35f, false);
                    CreateWaterSpawn(11.95f, 24.325f, 1.7f, 5.35f, true);
                    CreateWaterSpawn(9.9f, 15.25f, 2.8f, 4f, true);
                    CreateWaterSpawn(12.4f, 10.65f, 1.6f, 3.3f, true);
                    break;
            }
        }

        private static void CreateWaterSpawn(float x, float y, float xSize, float ySize, bool respawnFacingRight = true)
        {
            GameObject go = new("ItemChanger Hazard Respawn");
            go.transform.SetPosition2D(new Vector2(x, y));

            BoxCollider2D box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(xSize, ySize);

            HazardRespawnMarker hrm = go.AddComponent<HazardRespawnMarker>();
            HazardRespawnTrigger hrt = go.AddComponent<HazardRespawnTrigger>();
            hrt.respawnMarker = hrm;
            hrm.respawnFacingRight = respawnFacingRight;

            go.SetActive(true);
        }
    }
}
