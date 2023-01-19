namespace ItemChanger.FsmStateActions
{
    public class WaitForDelegate : FsmStateAction
    {
        Func<bool> test;
        FsmEvent? sendEvent;

        public WaitForDelegate(Func<bool> test, string? eventName)
        {
            this.test = test;
            this.sendEvent = eventName == null ? null
                : FsmEvent.GetFsmEvent(eventName);
        }
        public WaitForDelegate(Func<bool> test, FsmEvent sendEvent)
        {
            this.test = test;
            this.sendEvent = sendEvent;
        }

        public override void OnEnter()
        {
            if (test())
            {
                if (sendEvent != null)
                {
                    Fsm.Event(sendEvent);
                }
                Finish();
            }
        }

        public override void OnUpdate()
        {
            OnEnter();
        }
    }
}
