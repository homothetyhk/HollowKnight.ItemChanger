using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using SereCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using System.Collections;
using RandomizerMod.SceneChanges;

namespace RandomizerMod.SceneChanges
{
    /*
    public static class DreamPlantEdits
    {
        public static void ReplaceDreamPlantOrbs(Scene newScene)
        {
            if (!RandomizerMod.Instance.Settings.RandomizeWhisperingRoots) return;
            switch (newScene.name)
            {
                case SceneNames.Crossroads_07:
                case SceneNames.Crossroads_ShamanTemple:
                case SceneNames.Fungus1_13:
                case SceneNames.Fungus2_33:
                case SceneNames.Fungus2_17:
                case SceneNames.Deepnest_39:
                case SceneNames.Fungus3_11:
                case SceneNames.Deepnest_East_07:
                case SceneNames.Abyss_01:
                case SceneNames.Ruins1_17:
                case SceneNames.RestingGrounds_05:
                case SceneNames.RestingGrounds_08:
                case SceneNames.Mines_23:
                case SceneNames.Cliffs_01:
                case SceneNames.Hive_02:
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Dream Orb"))
                    {
                        GameObject orb = go.transform.parent.gameObject;
                        orb.AddComponent<RandomizerDreamPlantOrb>();
                        orb.GetComponent<RandomizerDreamPlantOrb>().Awake();
                    }
                    break;
            }
        }
    }
    */
    /*
     * Sloppy fix using a little reflection
     */

    public class RandomizerDreamPlantOrb : MonoBehaviour
    {
        public void Awake()
        {
        }

        private void Start()
        {
            orb = this.gameObject.GetComponent<DreamPlantOrb>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && canPickup && !removedEssence)
            {
                PlayerData.instance.DecrementInt("dreamOrbs");
                removedEssence = true;
                EventRegister.SendEvent("DREAM ORB COLLECT");
            }
        }

        private DreamPlantOrb orb;
        private bool removedEssence;

        private bool canPickup => (bool)typeof(DreamPlantOrb).GetField("canPickup", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(orb);
    }

}
