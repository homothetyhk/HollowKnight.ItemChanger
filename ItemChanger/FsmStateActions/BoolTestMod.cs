using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace ItemChanger.FsmStateActions
{
    public class BoolTestMod : FsmStateAction
    {
        Func<bool> test;
        FsmEvent isTrue;
        FsmEvent isFalse;

        public BoolTestMod(Func<bool> newTest, BoolTest oldTest)
            : this(newTest, oldTest.isTrue, oldTest.isFalse) { }

        public BoolTestMod(Func<bool> newTest, PlayerDataBoolTest oldTest)
            : this(newTest, oldTest.isTrue, oldTest.isFalse) { }

        public BoolTestMod(Func<bool> test, string trueEvent, string falseEvent)
        {
            this.test = test;
            isTrue = trueEvent == null ? null 
                : FsmEvent.EventListContains(trueEvent) ? FsmEvent.GetFsmEvent(trueEvent) : new FsmEvent(trueEvent);
            isFalse = falseEvent == null ? null
                : FsmEvent.EventListContains(falseEvent) ? FsmEvent.GetFsmEvent(falseEvent) : new FsmEvent(falseEvent);
        }

        public BoolTestMod(Func<bool> test, FsmEvent isTrue, FsmEvent isFalse)
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
