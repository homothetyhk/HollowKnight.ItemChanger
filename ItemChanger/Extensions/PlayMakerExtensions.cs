using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;

namespace ItemChanger.Extensions
{
    /// <summary>
    /// Extensions for interacting with and modifying FSMs.
    /// </summary>
    public static class PlayMakerExtensions
    {
        public static void AddState(this PlayMakerFSM fsm, FsmState state)
        {
            FsmState[] states = new FsmState[fsm.FsmStates.Length + 1];

            fsm.FsmStates.CopyTo(states, 0);
            states[fsm.FsmStates.Length] = state;

            fsm.Fsm.States = states;
        }

        public static FsmState GetState(this PlayMakerFSM fsm, string name)
        {
            return fsm.FsmStates.FirstOrDefault(s => s.Name == name);
        }

        public static void AddFirstAction(this FsmState state, FsmStateAction action)
        {
            FsmStateAction[] actions = new FsmStateAction[state.Actions.Length + 1];
            actions[0] = action;
            state.Actions.CopyTo(actions, 1);
            state.Actions = actions;
        }

        public static void AddLastAction(this FsmState state, FsmStateAction action)
        {
            FsmStateAction[] actions = new FsmStateAction[state.Actions.Length + 1];
            actions[state.Actions.Length] = action;
            state.Actions.CopyTo(actions, 0);
            state.Actions = actions;
        }

        public static void InsertAction(this FsmState state, FsmStateAction action, int index)
        {
            FsmStateAction[] actions = new FsmStateAction[state.Actions.Length + 1];
            for (int i = 0; i < state.Actions.Length; i++)
            {
                if (i < index) actions[i] = state.Actions[i];
                else actions[i + 1] = state.Actions[i];
            }
            actions[index] = action;
            state.Actions = actions;
        }

        public static void RemoveAction(this FsmState state, int index)
        {
            FsmStateAction[] actions = new FsmStateAction[state.Actions.Length - 1];
            for (int i = 0; i < state.Actions.Length - 1; i++)
            {
                if (i < index) actions[i] = state.Actions[i];
                else actions[i] = state.Actions[i + 1];
            }
            state.Actions = actions;
        }

        public static void ClearActions(this FsmState state) => state.Actions = Array.Empty<FsmStateAction>();

        public static void RemoveActionsOfType<T>(this FsmState state) where T : FsmStateAction
        {
            state.Actions = state.Actions.Where(a => !(a is T)).ToArray();
        }

        public static void RemoveFirstActionOfType<T>(this FsmState state) where T : FsmStateAction
        {
            int i = Array.FindIndex(state.Actions, a => a is T);
            if (i >= 0) state.RemoveAction(i);
        }

        public static T[] GetActionsOfType<T>(this FsmState state) where T : FsmStateAction
        {
            return state.Actions.OfType<T>().ToArray();
        }

        public static T GetFirstActionOfType<T>(this FsmState state) where T : FsmStateAction
        {
            return state.Actions.OfType<T>().FirstOrDefault();
        }

        public static FsmBool AddFsmBool(this PlayMakerFSM fsm, string name, bool value)
        {
            FsmBool fb = new FsmBool
            {
                Name = name,
                Value = value
            };

            FsmBool[] bools = new FsmBool[fsm.FsmVariables.BoolVariables.Length + 1];
            fsm.FsmVariables.BoolVariables.CopyTo(bools, 0);
            bools[bools.Length - 1] = fb;
            fsm.FsmVariables.BoolVariables = bools;

            return fb;
        }

        public static FsmInt AddFsmInt(this PlayMakerFSM fsm, string name, int value)
        {
            FsmInt fi = new FsmInt
            {
                Name = name,
                Value = value
            };

            FsmInt[] ints = new FsmInt[fsm.FsmVariables.IntVariables.Length + 1];
            fsm.FsmVariables.IntVariables.CopyTo(ints, 0);
            ints[ints.Length - 1] = fi;
            fsm.FsmVariables.IntVariables = ints;

            return fi;
        }

        public static FsmTransition AddTransition(this FsmState state, FsmEvent fsmEvent, FsmState toState)
        {
            FsmTransition[] transitions = new FsmTransition[state.Transitions.Length + 1];
            state.Transitions.CopyTo(transitions, 0);

            FsmTransition t = new FsmTransition
            {
                FsmEvent = fsmEvent,
                ToFsmState = toState,
                ToState = toState.Name,
            };
            transitions[state.Transitions.Length] = t;
            state.Transitions = transitions;

            return t;
        }

        public static FsmTransition AddTransition(this FsmState state, string eventName, FsmState toState)
        {
            return state.AddTransition(FsmEvent.GetFsmEvent(eventName), toState);
        }

        public static FsmTransition AddTransition(this FsmState state, string eventName, string toState)
        {
            return state.AddTransition(eventName == "FINISHED" ? FsmEvent.Finished : FsmEvent.GetFsmEvent(eventName), state.Fsm.GetState(toState));
        }

        public static void RemoveTransitionsTo(this FsmState state, string toState)
        {
            state.Transitions = state.Transitions.Where(t => (t.ToFsmState?.Name ?? t.ToState) != toState).ToArray();
        }

        public static void RemoveTransitionsOn(this FsmState state, string eventName)
        {
            state.Transitions = state.Transitions.Where(t => t.EventName != eventName).ToArray();
        }

        public static void SetToState(this FsmTransition transition, FsmState toState)
        {
            transition.ToFsmState = toState;
            transition.ToState = toState.Name;
        }

        public static void ClearTransitions(this FsmState state)
        {
            state.Transitions = Array.Empty<FsmTransition>();
        }
    }
}
