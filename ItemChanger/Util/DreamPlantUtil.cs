using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using System.Collections;

namespace ItemChanger.Util
{
    public static class DreamPlantUtil
    {
        static FieldInfo completedField = typeof(DreamPlant).GetField("completed", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo activatedField = typeof(DreamPlant).GetField("activated", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo animField = typeof(DreamPlant).GetField("anim", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo audioSourceField = typeof(DreamPlant).GetField("audioSource", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo dreamDialogueField = typeof(DreamPlant).GetField("dreamDialogue", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo spriteFlashField = typeof(DreamPlant).GetField("spriteFlash", BindingFlags.NonPublic | BindingFlags.Instance);

        static FieldInfo checkOrbRoutineField = typeof(DreamPlant).GetField("checkOrbRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
        static MethodInfo checkOrbsMethod = typeof(DreamPlant).GetMethod("CheckOrbs", BindingFlags.NonPublic | BindingFlags.Instance);


        public static void SetCompleted(this DreamPlant plant, bool value)
        {
            completedField.SetValue(plant, value);
        }

        public static void SetActivated(this DreamPlant plant, bool value)
        {
            activatedField.SetValue(plant, value);
        }

        public static void SetAnim(this DreamPlant plant, tk2dSpriteAnimator value)
        {
            animField.SetValue(plant, value);
        }

        public static void SetAudioSource(this DreamPlant plant, AudioSource value)
        {
            audioSourceField.SetValue(plant, value);
        }

        public static GameObject GetDreamDialogue(this DreamPlant plant)
        {
            return (GameObject)dreamDialogueField.GetValue(plant);
        }

        public static void SetDreamDialogue(this DreamPlant plant, GameObject value)
        {
            dreamDialogueField.SetValue(plant, value);
        }

        public static void SetSpriteFlash(this DreamPlant plant, SpriteFlash value)
        {
            spriteFlashField.SetValue(plant, value);
        }

        public static Coroutine GetCheckOrbRoutine(this DreamPlant plant)
        {
            return checkOrbRoutineField.GetValue(plant) as Coroutine;
        }

        public static void SetCheckOrbRoutine(this DreamPlant plant, Coroutine value)
        {
            checkOrbRoutineField.SetValue(plant, value);
        }

        public static IEnumerator CheckOrbs(this DreamPlant plant)
        {
            return (IEnumerator)checkOrbsMethod.Invoke(plant, new object[0]);
        }

    }
}
