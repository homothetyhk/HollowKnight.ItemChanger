using System.Collections;
using ItemChanger.Extensions;
using TMPro;

namespace ItemChanger.Internal
{
    public static class DialogueCenter
    {
        static GameObject DialogueManager => FsmVariables.GlobalVariables.FindFsmGameObject("DialogueManager").Value;
        static PlayMakerFSM BoxOpenFsm => DialogueManager.LocateFSM("Box Open");
        static GameObject DialogueText => FsmVariables.GlobalVariables.FindFsmGameObject("DialogueText").Value;
        static GameObject Arrow => DialogueText.transform.parent.Find("Arrow").gameObject;
        static GameObject Stop => DialogueText.transform.parent.Find("Stop").gameObject;

        static DialogueBox DialogueBox => DialogueText.GetComponent<DialogueBox>();
        static DialogueBox DialogueBoxYN => DialogueManager.FindChild("Text YN").GetComponent<DialogueBox>();

        public static void SendLoreMessage(string text, Action callback, TextType type)
        {
            switch (type)
            {
                case TextType.LeftLore:
                    DialogueBox.StartCoroutine(LeftLoreCoroutine(text, callback));
                    break;
                case TextType.Lore:
                    DialogueBox.StartCoroutine(LoreCoroutine(text, callback));
                    break;
                case TextType.MajorLore:
                    DialogueBox.StartCoroutine(MajorLoreCoroutine(text, callback));
                    break;
            }
        }

        public static IEnumerator MajorLoreCoroutine(string text, Action callback)
        {
            PlayMakerFSM.BroadcastEvent("LORE PROMPT UP");
            PlayLoreSound();
            yield return new WaitForSeconds(0.5f); // orig: 0.85f

            DialogueText.transform.localPosition = new Vector3(0, 2.44f, -1.0f);
            Stop.transform.localPosition = new Vector3(0, -0.23f, -2.0f);
            Arrow.transform.localPosition = new Vector3(0, -0.3f, -2.0f);
            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top;

            convoEnded = false;
            StartConversation(text);
            yield return new WaitUntil(ConvoEnded);

            PlayMakerFSM.BroadcastEvent("LORE PROMPT DOWN");
            yield return new WaitForSeconds(0.15f); // orig: 0.5f

            callback?.Invoke();

            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.TopLeft;
            DialogueText.transform.localPosition = new Vector3(0, 4.49f, -1.0f);
            Stop.transform.localPosition = new Vector3(0, 1.695f, -2.0f);
            Arrow.transform.localPosition = new Vector3(0, 1.695f, -2.0f);
        }

        public static IEnumerator LoreCoroutine(string text, Action callback)
        {
            BoxOpenFsm.Fsm.Event("BOX UP");
            yield return new WaitForSeconds(0.15f); // orig: 0.3f

            DialogueText.LocateFSM("Dialogue Page Control").FsmVariables.GetFsmGameObject("Requester").Value = null;
            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top;

            convoEnded = false;
            StartConversation(text);
            yield return new WaitUntil(ConvoEnded);

            BoxOpenFsm.Fsm.Event("BOX DOWN");
            yield return new WaitForSeconds(0.15f); // orig: 0.5f

            callback?.Invoke();

            DialogueText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.TopLeft;
        }

        public static IEnumerator LeftLoreCoroutine(string text, Action callback)
        {
            BoxOpenFsm.Fsm.Event("BOX UP");
            yield return new WaitForSeconds(0.15f); // orig: 0.3f

            DialogueText.LocateFSM("Dialogue Page Control").FsmVariables.GetFsmGameObject("Requester").Value = null;

            convoEnded = false;
            StartConversation(text);
            yield return new WaitUntil(ConvoEnded);

            BoxOpenFsm.Fsm.Event("BOX DOWN");
            yield return new WaitForSeconds(0.15f); // orig: 0.5f

            callback?.Invoke();
        }

        public static void PlayLoreSound()
        {
            Vector3 pos = HeroController.instance != null ? HeroController.instance.transform.position : Camera.main.transform.position + 2 * Vector3.up;
            SoundManager.Instance.PlayClipAtPoint("LoreSound", pos);
        }

        public static void StartConversation(string text)
        {
            DialogueBox box = DialogueBox;
            box.currentPage = 1;
            TextMeshPro textMesh = box.GetComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.ForceMeshUpdate();
            box.ShowPage(1);
        }

        public static void StartConversationYN(string text)
        {
            DialogueBox box = DialogueBoxYN;
            box.currentPage = 1;
            TextMeshPro textMesh = box.GetComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.ForceMeshUpdate();
            box.ShowPage(1);
        }

        public static void Hook()
        {
            On.DialogueBox.HideText += HideTextListener;
        }

        public static void Unhook()
        {
            On.DialogueBox.HideText -= HideTextListener;
        }

        private static void HideTextListener(On.DialogueBox.orig_HideText orig, DialogueBox self)
        {
            convoEnded = true;
            orig(self);
        }
        static bool ConvoEnded() => convoEnded;
        static bool convoEnded = false;
    }
}
