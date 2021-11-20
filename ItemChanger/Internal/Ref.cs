namespace ItemChanger.Internal
{
    public static class Ref
    {
        public static Settings Settings => ItemChangerMod.SET;
        public static PlayerData PD => PlayerData.instance;
        public static GameManager GM => GameManager.instance;
        public static HeroController HC => HeroController.instance;

        public static void QuickSave(params AbstractPlacement[] placements) => Settings.SavePlacements(placements);
    }
}
