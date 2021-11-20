namespace ItemChanger.FsmStateActions
{
    /// <summary>
    /// FsmStateAction which invokes a delegate.
    /// </summary>
    public class Lambda : FsmStateAction
    {
        private readonly Action _method;

        public Lambda(Action method)
        {
            _method = method;
        }

        public override void OnEnter()
        {
            try
            {
                _method();
            }
            catch (Exception e)
            {
                LogError($"Error in FsmStateAction Lambda in {this.Fsm.FsmComponent.gameObject.name} - {this.Fsm.FsmComponent.FsmName}:\n{e}");
            }

            Finish();
        }
    }
}