using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// Invoke the supplied method every frame.
    /// </summary>
    public class LambdaEveryFrame : FsmStateAction
    {
        private readonly Action _method;

        public LambdaEveryFrame(Action method)
        {
            _method = method;
        }

        public override void OnUpdate()
        {
            try
            {
                _method();
            }
            catch (Exception e)
            {
                LogError($"Error in FsmStateAction {nameof(LambdaEveryFrame)} in {this.Fsm.FsmComponent.gameObject.name} - {this.Fsm.FsmComponent.FsmName}:\n{e}");
            }
        }
    }
}
