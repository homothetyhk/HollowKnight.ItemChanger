using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;

namespace ItemChanger.FsmStateActions
{
    public class AsyncLambda : FsmStateAction
    {
        private readonly Action<Action> _method;
        private readonly string _eventName = null;

        public AsyncLambda(Action<Action> method)
        {
            _method = method;
        }

        public AsyncLambda(Action<Action> method, string eventName) : this(method)
        {
            _eventName = eventName;
        }

        public override void OnEnter()
        {
            try
            {
                _method(Finish);
            }
            catch (Exception e)
            {
                LogError($"Error in FsmStateAction AsyncLambda in {this.Fsm.FsmComponent.gameObject.name} - {this.Fsm.FsmComponent.FsmName}:\n{e}");
            }
        }

        new private void Finish()
        {
            if (!string.IsNullOrEmpty(_eventName)) // this check is repeated in Fsm.Event
            {
                Fsm.Event(_eventName);
            }
            base.Finish();
        }

        
    }
}
