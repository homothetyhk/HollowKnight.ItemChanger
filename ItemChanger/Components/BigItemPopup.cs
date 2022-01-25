using System.Collections;
using Modding;
using UnityEngine.UI;
using ItemChanger.Internal;
using GlobalEnums;

namespace ItemChanger.Components
{
    /// <summary>
    /// Component which creates a full-screen popup when an item is obtained.
    /// </summary>
    public class BigItemPopup : MonoBehaviour
    {
        private static readonly Sprite BlackPixel = CanvasUtil.NullSprite(new byte[] {0x00, 0x00, 0x00, 0xAA});
        private static readonly Sprite[] Frames;
        private string _pressText;
        private string _descOneText;
        private string _descTwoText;
        private ButtonSkin _buttonSkin;
        private Action _callback;

        private Sprite _imagePrompt;
        private string _nameText;

        private bool _showInstantly;
        private string _takeText;

        static BigItemPopup()
        {
            Frames = new[]
            {
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.0"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.1"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.2"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.3"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.4"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.5"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.6"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.7"),
                SpriteManager.Instance.GetSprite("Anim.BigItemFleur.8")
            };
        }

        /// <summary>
        /// Creates a BigItemPopup with the given parameters. All parameters can be null except the sprite and name.
        /// </summary>
        public static GameObject Show(Sprite bigSprite, string take, string name, string press, ButtonSkin buttonSkin, string descOne, string descTwo, Action callback)
        {
            // Create base canvas
            GameObject canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));

            // Add popup component, set values
            BigItemPopup popup = canvas.AddComponent<BigItemPopup>();
            popup._imagePrompt = bigSprite;
            popup._takeText = take;
            popup._nameText = name;
            popup._pressText = press;
            popup._buttonSkin = buttonSkin; 
            popup._descOneText = descOne;
            popup._descTwoText = descTwo;
            popup._callback = callback;

            return canvas;
        }

        public void Start()
        {
            Ref.GM.SaveGame(Ref.GM.profileID, x => { });
            StartCoroutine(ShowPopup());
        }

        private IEnumerator ShowPopup()
        {
            // Check for skipping popup
            Coroutine skipCoroutine = StartCoroutine(LookForShowInstantly());

            // Begin dimming the scene
            GameObject dimmer = CanvasUtil.CreateImagePanel(gameObject, BlackPixel,
                new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one));
            dimmer.GetComponent<Image>().preserveAspect = false;
            CanvasGroup dimmerCG = dimmer.AddComponent<CanvasGroup>();

            dimmerCG.blocksRaycasts = false;
            dimmerCG.interactable = false;
            dimmerCG.alpha = 0;

            StartCoroutine(FadeInCanvasGroup(dimmerCG));

            yield return WaitForSeconds(0.1f);

            // Aim for 400 high prompt image
            float scaler = _imagePrompt.texture.height / 400f;
            Vector2 size = new Vector2(_imagePrompt.texture.width / scaler, _imagePrompt.texture.height / scaler);

            SoundManager.Instance.PlayClipAtPoint("BigItemJingle", HeroController.instance.transform.position);


            // Begin fading in the top bits of the popup
            GameObject topImage = CanvasUtil.CreateImagePanel(gameObject, _imagePrompt,
                new CanvasUtil.RectData(size, Vector2.zero, new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.8f)));

            CanvasGroup topImageCG = topImage.AddComponent<CanvasGroup>();
            topImageCG.blocksRaycasts = false;
            topImageCG.interactable = false;
            topImageCG.alpha = 0;
            StartCoroutine(FadeInCanvasGroup(topImageCG));

            if (!string.IsNullOrEmpty(_takeText))
            {
                GameObject topTextOne = CanvasUtil.CreateTextPanel(gameObject, _takeText, 34, TextAnchor.MiddleCenter,
                    new CanvasUtil.RectData(new Vector2(1920, 100), Vector2.zero, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f)), 
                    CanvasUtil.GetFont("Perpetua"));
                CanvasGroup topTextOneCG = topTextOne.AddComponent<CanvasGroup>();
                topTextOneCG.blocksRaycasts = false;
                topTextOneCG.interactable = false;
                topTextOneCG.alpha = 0;
                StartCoroutine(FadeInCanvasGroup(topTextOneCG));
            }
            
            GameObject topTextTwo = CanvasUtil.CreateTextPanel(gameObject, _nameText, 76, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(1920, 300), Vector2.zero, new Vector2(0.5f, 0.49f),
                    new Vector2(0.5f, 0.49f)), CanvasUtil.GetFont("TrajanPro-Bold"));
            CanvasGroup topTextTwoCG = topTextTwo.AddComponent<CanvasGroup>();
            topTextTwoCG.blocksRaycasts = false;
            topTextTwoCG.interactable = false;
            topTextTwoCG.alpha = 0;
            yield return StartCoroutine(FadeInCanvasGroup(topTextTwoCG));


            // Animate the middle fleur
            GameObject fleur = CanvasUtil.CreateImagePanel(gameObject, Frames[0],
                new CanvasUtil.RectData(new Vector2(Frames[0].texture.width / 1.6f, Frames[0].texture.height / 1.6f), Vector2.zero, new Vector2(0.5f, 0.4125f), new Vector2(0.5f, 0.4125f)));
            yield return StartCoroutine(AnimateFleur(fleur, 12));
            yield return WaitForSeconds(0.25f);


            // Fade in the remaining text
            if (!string.IsNullOrEmpty(_pressText))
            {
                float x = _buttonSkin == null ? 0.5f : 0.48f;

                GameObject press = CanvasUtil.CreateTextPanel(gameObject, _pressText, 34, TextAnchor.MiddleCenter,
                    new CanvasUtil.RectData(new Vector2(100, 100), Vector2.zero, new(x, 0.335f), new(x, 0.335f)), CanvasUtil.GetFont("Perpetua"));
                CanvasGroup pressCG = press.AddComponent<CanvasGroup>();
                pressCG.blocksRaycasts = false;
                pressCG.interactable = false;
                pressCG.alpha = 0;
                StartCoroutine(FadeInCanvasGroup(pressCG));
            }


            if (_buttonSkin != null)
            {
                Font f = Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(f => f.name == "Arial");

                // unfortunately, since we use Text and not TextMeshPro, we have this very fragile manual scale adjustment
                int fontSize = _buttonSkin.skinType switch
                {
                    ButtonSkinType.SQUARE => _buttonSkin.symbol.Length <= 2 ? 48 : 36, // the accursed F10 bind
                    ButtonSkinType.WIDE => 24, // the accursed Backspace bind
                    _ => 48
                };

                GameObject iconText = _buttonSkin.skinType switch
                {
                    ButtonSkinType.SQUARE => CanvasUtil.CreateTextPanel(
                        gameObject, _buttonSkin.symbol, fontSize, TextAnchor.MiddleCenter, new CanvasUtil.RectData(new Vector2(70, 70), Vector2.zero, new(0.52f, 0.336f), new(0.52f, 0.336f)), f),
                    ButtonSkinType.WIDE => CanvasUtil.CreateTextPanel(
                        gameObject, _buttonSkin.symbol, fontSize, TextAnchor.MiddleCenter, new CanvasUtil.RectData(new Vector2(168.5f, 60), Vector2.zero, new(0.54f, 0.3345f), new(0.54f, 0.3345f)), f),
                    _ => CanvasUtil.CreateTextPanel(
                        gameObject, _buttonSkin.symbol, fontSize, TextAnchor.MiddleCenter, new CanvasUtil.RectData(new Vector2(168.5f, 60), Vector2.zero, new(0.52f, 0.335f), new(0.52f, 0.335f)), f),
                };
                iconText.GetComponent<Text>().alignByGeometry = true;


                GameObject iconSprite = _buttonSkin.skinType switch
                {
                    ButtonSkinType.WIDE => CanvasUtil.CreateImagePanel(
                        gameObject, _buttonSkin.sprite, new CanvasUtil.RectData(new Vector2(168.5f, 60), Vector2.zero, new(0.54f, 0.335f), new(0.54f, 0.335f))),
                    _ => CanvasUtil.CreateImagePanel(
                        gameObject, _buttonSkin.sprite, new CanvasUtil.RectData(new Vector2(70, 80), Vector2.zero, new(0.52f, 0.335f), new(0.52f, 0.335f))),
                };

                
                CanvasGroup iconTextCG = iconText.AddComponent<CanvasGroup>();
                iconTextCG.blocksRaycasts = false;
                iconTextCG.interactable = false;
                iconTextCG.alpha = 0;
                
                CanvasGroup iconSpriteCG = iconSprite.AddComponent<CanvasGroup>();
                iconSpriteCG.blocksRaycasts = false;
                iconSpriteCG.interactable = false;
                iconSpriteCG.alpha = 0;
                
                StartCoroutine(FadeInCanvasGroup(iconSpriteCG));
                StartCoroutine(FadeInCanvasGroup(iconTextCG));
            }

            if (!string.IsNullOrEmpty(_descOneText))
            {
                GameObject descTextOne = CanvasUtil.CreateTextPanel(gameObject, _descOneText, 34, TextAnchor.MiddleCenter,
                    new CanvasUtil.RectData(new Vector2(1920, 100), Vector2.zero, new Vector2(0.5f, 0.26f), new Vector2(0.5f, 0.26f)), 
                    CanvasUtil.GetFont("Perpetua"));
                CanvasGroup descTextOneCG = descTextOne.AddComponent<CanvasGroup>();
                descTextOneCG.blocksRaycasts = false;
                descTextOneCG.interactable = false;
                descTextOneCG.alpha = 0;
                StartCoroutine(FadeInCanvasGroup(descTextOneCG));
            }

            if (!string.IsNullOrEmpty(_descTwoText))
            {
                GameObject descTextTwo = CanvasUtil.CreateTextPanel(gameObject, _descTwoText, 34, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(1920, 100), Vector2.zero, new Vector2(0.5f, 0.205f), new Vector2(0.5f, 0.205f)), 
                CanvasUtil.GetFont("Perpetua"));
                CanvasGroup descTextTwoCG = descTextTwo.AddComponent<CanvasGroup>();
                descTextTwoCG.blocksRaycasts = false;
                descTextTwoCG.interactable = false;
                descTextTwoCG.alpha = 0;
                StartCoroutine(FadeInCanvasGroup(descTextTwoCG));
            }

            yield return WaitForSeconds(1.5f);


            // Can I offer you an egg in this trying time?
            GameObject egg = CanvasUtil.CreateImagePanel(gameObject, SpriteManager.Instance.GetSprite("UI.egg"),
                new CanvasUtil.RectData(
                    new Vector2(SpriteManager.Instance.GetSprite("UI.egg").texture.width / 1.65f,
                        SpriteManager.Instance.GetSprite("UI.egg").texture.height / 1.65f), Vector2.zero,
                    new Vector2(0.5f, 0.1075f), new Vector2(0.5f, 0.1075f)));
            CanvasGroup eggCG = egg.AddComponent<CanvasGroup>();

            eggCG.blocksRaycasts = false;
            eggCG.interactable = false;
            eggCG.alpha = 0;

            // Should wait for one fade in, don't want to poll input immediately
            yield return FadeInCanvasGroup(eggCG);

            // Stop doing things instantly before polling input
            if (!_showInstantly)
            {
                StopCoroutine(skipCoroutine);
            }

            _showInstantly = false;

            // Save the coroutine to stop it later
            Coroutine coroutine = StartCoroutine(BlinkCanvasGroup(eggCG));

            // Wait for the user to cancel the menu
            while (true)
            {
                HeroActions actions = Ref.GM.inputHandler.inputActions;
                if (actions.jump.WasPressed || actions.attack.WasPressed || actions.menuCancel.WasPressed)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            // Fade out the full popup
            yield return FadeOutCanvasGroup(gameObject.GetComponent<CanvasGroup>());

            // Small delay before hero control
            //yield return WaitForSeconds(0.75f);
            yield return new WaitForEndOfFrame();

            // Notify location that popup has finished
            _callback?.Invoke();

            // Stop the egg routine and destroy everything
            StopCoroutine(coroutine);
            Destroy(gameObject);
        }

        private IEnumerator AnimateFleur(GameObject fleur, float fps)
        {
            Image img = fleur.GetComponent<Image>();
            int spriteNum = 0;

            while (spriteNum < Frames.Length)
            {
                img.sprite = Frames[spriteNum];
                spriteNum++;
                yield return WaitForSeconds(1 / fps);
            }
        }

        // ReSharper disable once IteratorNeverReturns
        private IEnumerator BlinkCanvasGroup(CanvasGroup cg)
        {
            while (true)
            {
                yield return FadeOutCanvasGroup(cg);
                yield return FadeInCanvasGroup(cg);
            }
        }

        private IEnumerator WaitForSeconds(float seconds)
        {
            float timePassed = 0f;
            while (timePassed < seconds && !_showInstantly)
            {
                timePassed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LookForShowInstantly()
        {
            while (true)
            {
                HeroActions actions = Ref.GM.inputHandler.inputActions;
                if (actions.jump.WasPressed || actions.attack.WasPressed || actions.menuCancel.WasPressed)
                {
                    _showInstantly = true;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        // Below functions ripped from CanvasUtil in order to change the speed
        private IEnumerator FadeInCanvasGroup(CanvasGroup cg)
        {
            float loopFailsafe = 0f;
            cg.alpha = 0f;
            cg.gameObject.SetActive(true);
            while (cg.alpha < 1f && !_showInstantly)
            {
                cg.alpha += Time.deltaTime * 2f;
                loopFailsafe += Time.deltaTime;
                if (cg.alpha >= 0.95f)
                {
                    cg.alpha = 1f;
                    break;
                }

                if (loopFailsafe >= 2f)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            cg.alpha = 1f;
            cg.interactable = true;
            cg.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
        }

        // Identical to CanvasUtil version except it doesn't randomly set the canvas object inactive at the end
        private IEnumerator FadeOutCanvasGroup(CanvasGroup cg)
        {
            float loopFailsafe = 0f;
            cg.interactable = false;
            while (cg.alpha > 0.05f && !_showInstantly)
            {
                cg.alpha -= Time.deltaTime * 4f;
                loopFailsafe += Time.deltaTime;
                if (cg.alpha <= 0.05f)
                {
                    break;
                }

                if (loopFailsafe >= 2f)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            cg.alpha = 0f;
            yield return new WaitForEndOfFrame();
        }
    }
}
