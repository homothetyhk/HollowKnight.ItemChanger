using System;
using System.Reflection;
using HutongGames.PlayMaker;
using MonoMod.Utils;

namespace ItemChanger.FsmStateActions
{
    internal class RandomizerCallStaticMethod : FsmStateAction
    {
        private readonly FastReflectionDelegate _method;
        private readonly object[] _parameters;

        public RandomizerCallStaticMethod(Type t, string methodName, params object[] parameters)
        {
            MethodInfo info = t.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (info == null)
            {
                throw new ArgumentException($"Class {t} has no static method {methodName}");
            }

            _method = info.CreateFastDelegate();

            _parameters = parameters;
        }

        public override void OnEnter()
        {
            try
            {
                _method(null, _parameters);
            }
            catch (Exception e)
            {
                LogError("Error invoking static method from FSM:\n" + e);
            }

            Finish();
        }
    }
}