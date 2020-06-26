using HutongGames.PlayMaker;
using SeanprCore;

namespace ItemChanger.FsmStateActions
{
    internal class RandomizerSetBool : FsmStateAction
    {
        private readonly string _name;
        private readonly bool _playerdata;
        private readonly bool _val;

        public RandomizerSetBool(string boolName, bool val, bool playerdata = false)
        {
            _name = boolName;
            _val = val;
            _playerdata = playerdata;
        }

        public override void OnEnter()
        {
            if (_playerdata)
            {
                Ref.PD.SetBool(_name, _val);
            }
            else
            {
                ItemChanger.instance.Settings.SetBool(_val, _name);
            }

            Finish();
        }
    }
}