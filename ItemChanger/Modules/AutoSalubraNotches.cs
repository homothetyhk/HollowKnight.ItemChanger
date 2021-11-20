namespace ItemChanger.Modules
{
    // Not a default module
    /// <summary>
    /// Module which gives the notches originally sold by Salubra when the corresponding number of charms has been obtained.
    /// </summary>
    public class AutoSalubraNotches : Module
    {
        public override void Initialize()
        {
            On.PlayerData.CountCharms += OnCountCharms; // hook for ItemChanger charms
            On.PlayerData.IncrementInt += OnIncrementCharms; // hook for vanilla charms
        }

        public override void Unload()
        {
            On.PlayerData.CountCharms -= OnCountCharms;
            On.PlayerData.IncrementInt -= OnIncrementCharms;
        }

        private void OnIncrementCharms(On.PlayerData.orig_IncrementInt orig, PlayerData self, string intName)
        {
            orig(self, intName);
            if (intName == nameof(PlayerData.charmsOwned)) TestNotchUnlocked();
        }

        private void OnCountCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
        {
            orig(self);
            TestNotchUnlocked();
        }

        private void TestNotchUnlocked()
        {
            int charms = PlayerData.instance.GetInt(nameof(PlayerData.charmsOwned));
            bool notch1 = PlayerData.instance.GetBool(nameof(PlayerData.salubraNotch1));
            bool notch2 = PlayerData.instance.GetBool(nameof(PlayerData.salubraNotch2));
            bool notch3 = PlayerData.instance.GetBool(nameof(PlayerData.salubraNotch3));
            bool notch4 = PlayerData.instance.GetBool(nameof(PlayerData.salubraNotch4));

            if (!notch1 && charms >= 5)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.salubraNotch1), true);
                PlayerData.instance.IncrementInt(nameof(PlayerData.charmSlots));
                GameManager.instance.RefreshOvercharm();
            }

            if (!notch2 && charms >= 10)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.salubraNotch2), true);
                PlayerData.instance.IncrementInt(nameof(PlayerData.charmSlots));
                GameManager.instance.RefreshOvercharm();
            }

            if (!notch3 && charms >= 18)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.salubraNotch3), true);
                PlayerData.instance.IncrementInt(nameof(PlayerData.charmSlots));
                GameManager.instance.RefreshOvercharm();
            }

            if (!notch4 && charms >= 25)
            {
                PlayerData.instance.SetBool(nameof(PlayerData.salubraNotch4), true);
                PlayerData.instance.IncrementInt(nameof(PlayerData.charmSlots));
                GameManager.instance.RefreshOvercharm();
            }
        }

    }
}
