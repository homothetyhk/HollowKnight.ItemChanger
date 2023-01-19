using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using TMPro;

namespace ItemChanger.Components
{
    /// <summary>
    /// A component which displays custom dream text when triggered by proximity.
    /// </summary>
    public class HintBox : MonoBehaviour
    {
        /// <summary>
        /// Create a HintBox at the specified position with the specified delegates.
        /// </summary>
        public static HintBox Create(Vector2 pos, Func<string?>? getDisplayText, Func<bool>? displayTest = null, Action<string>? onDisplay = null)
        {
            var hint = Create(pos, new Vector2(5f, 5f));
            hint.GetDisplayText = getDisplayText;
            hint.DisplayTest = displayTest;
            hint.OnDisplay = onDisplay;
            return hint;
        }

        /// <summary>
        /// Create a HintBox at the position of the transform using the placement's GetUIName, AllObtained, and AddVisitFlag(VisitState.Previewed) methods.
        /// </summary>
        public static HintBox Create(Transform transform, AbstractPlacement placement)
        {
            var hint = Create(transform.position, new Vector2(5f, 5f));
            hint.GetDisplayText = placement.GetUIName;
            hint.DisplayTest = () => !placement.AllObtained();
            hint.OnDisplay = placement.OnPreview;
            return hint;
        }

        /// <summary>
        /// Creates a HintBox of specified position and size. The delegate fields of the HintBox are not set.
        /// </summary>
        public static HintBox Create(Vector2 pos, Vector2 size)
        {
            GameObject obj = new GameObject("Hint Box");
            obj.transform.position = pos;
            BoxCollider2D box = obj.AddComponent<BoxCollider2D>();
            box.size = size;
            box.isTrigger = true;

            HintBox hint = obj.AddComponent<HintBox>();

            return hint;
        }

        public Func<bool>? DisplayTest;
        public Func<string?>? GetDisplayText;
        public Action<string>? OnDisplay;
        PlayMakerFSM display;

        public void Setup(GameObject prefab)
        {
            display = Instantiate(prefab).LocateMyFSM("Display");
            display.GetState("Init").RemoveActionsOfType<SetGameObject>();
            display.GetState("Check Convo").RemoveActionsOfType<StringCompare>();
            display.GetState("Set Convo").ClearActions();
        }

        public void Update()
        {
            if (display == null) // Succeeds on the second update
            {
                var g = FsmVariables.GlobalVariables.FindFsmGameObject("Enemy Dream Msg");
                if (g.Value != null) Setup(g.Value);
            }
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && display != null && (DisplayTest?.Invoke() ?? true))
            {
                string? s = GetDisplayText?.Invoke();
                if (!string.IsNullOrEmpty(s))
                {
                    SetText(s!);
                    ShowConvo();
                    OnDisplay?.Invoke(s!);
                }
            }
        }

        private void SetText(string text)
        {
            display.FsmVariables.FindFsmGameObject("Text").Value.GetComponent<TextMeshPro>().text = text;
        }

        private void ShowConvo()
        {
            display.SendEvent("DISPLAY ENEMY DREAM");
        }
    }
}
