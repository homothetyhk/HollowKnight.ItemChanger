using System.Collections;

namespace ItemChanger.Internal
{
    public class MessageController : MonoBehaviour
    {
        private static MessageController instance;
        private static Coroutine activeDisplay;
        private readonly Queue<(Sprite sprite, string text)> messages = new Queue<(Sprite icon, string text)>();

        public void Update()
        {
            if (messages.Any() && activeDisplay == null)
            {
                var (sprite, text) = messages.Dequeue();
                activeDisplay = StartCoroutine(SendMessage(sprite, text));
            }
        }

        public static void Enqueue(Sprite sprite, string text)
        {
            instance.messages.Enqueue((sprite, text));
        }

        public static void Error()
        {
            Enqueue(Modding.CanvasUtil.NullSprite(), "Error: see ModLog for details.");
        }

        private IEnumerator SendMessage(Sprite sprite, string text)
        {
            GameObject popup = ObjectCache.RelicGetMsg;
            popup.transform.Find("Text").GetComponent<TMPro.TextMeshPro>().text = text;
            popup.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = sprite;
            popup.transform.Find("Icon").GetComponent<SpriteRenderer>().sortingOrder = 1; // show on top of blankers, etc

            popup.SetActive(true);
            yield return new WaitForSeconds(3f);
            activeDisplay = null;
        }


        internal static void Setup()
        {
            GameObject obj = new GameObject("MessageController parent");
            instance = obj.AddComponent<MessageController>();
            DontDestroyOnLoad(obj);
        }

        internal static void Clear()
        {
            instance.messages.Clear();
        }
    }
}
