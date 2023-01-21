using HutongGames.PlayMaker.Actions;

namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// FsmStateAction which invokes a supplied test to choose an FsmEvent.
    /// </summary>
    public class DelegateBoolTest : FsmStateAction
    {
        private readonly Func<bool> test;
        private readonly FsmEvent? isTrue;
        private readonly FsmEvent? isFalse;

        public DelegateBoolTest(Func<bool> newTest, BoolTest? oldTest)
            : this(newTest, oldTest?.isTrue, oldTest?.isFalse) { }

        public DelegateBoolTest(Func<bool> newTest, PlayerDataBoolTest? oldTest)
            : this(newTest, oldTest?.isTrue, oldTest?.isFalse) { }

        public DelegateBoolTest(Func<bool> test, string? trueEvent, string? falseEvent)
        {
            this.test = test;
            isTrue = trueEvent == null ? null
                : FsmEvent.GetFsmEvent(trueEvent);
            isFalse = falseEvent == null ? null
                : FsmEvent.GetFsmEvent(falseEvent);
        }

        public DelegateBoolTest(Func<bool> test, FsmEvent? isTrue, FsmEvent? isFalse)
        {
            this.test = test;
            this.isTrue = isTrue;
            this.isFalse = isFalse;
        }

        public override void OnEnter()
        {
            bool result = test();

            if (result && isTrue != null)
            {
                Fsm.Event(isTrue);
            }
            else if (!result && isFalse != null)
            {
                Fsm.Event(isFalse);
            }

            Finish();
        }

    }
}
