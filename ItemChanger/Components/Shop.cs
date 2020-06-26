using System.Collections;
using System.Collections.Generic;
using Modding;
using ItemChanger.Actions;
using SeanprCore;
using UnityEngine;
using UnityEngine.UI;

namespace ItemChanger.Components
{
    internal class Shop : MonoBehaviour
    {
        public enum ShopType
        {
            Geo,
            Essence
        }

        // float[] can't be const and this is mostly the same thing (Not really but whatever)
        private static readonly float[] DefaultPositions = {0.8f, 0.7f, 0.5825f, 0.465f, 0.365f, 0.265f};

        private static readonly Sprite blackPixel = CanvasUtil.NullSprite(new byte[] {0x00, 0x00, 0x00, 0xAA});

        private static readonly Sprite[] bottomFrames;
        private static readonly Sprite[] topFrames;

        private Sprite geoSprite;

        private GameObject[,] itemImages;

        private ShopItemDef[] items;
        private int selected;
        private ShopType type;

        private int[] validItems;

        static Shop()
        {
            bottomFrames = new[]
            {
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.2"),
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.3"),
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.4"),
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.5"),
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.6"),
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.7"),
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.8")
            };

            topFrames = new[]
            {
                ItemChanger.GetSprite("Anim.Shop.TopFleur.0"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.1"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.2"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.3"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.4"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.5"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.6"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.7"),
                ItemChanger.GetSprite("Anim.Shop.TopFleur.8")
            };
        }

        public static void Show()
        {
            // Create base canvas
            GameObject canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));

            // Add shop component, set values
            Shop shop = canvas.AddComponent<Shop>();
            shop.items = new[]
            {
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_1",
                    NameConvo = "CHARM_NAME_1",
                    DescConvo = "RANDOMIZER_CHARM_DESC_1",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_1",
                    Cost = 420,
                    SpriteName = "Charms.1.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_2",
                    NameConvo = "CHARM_NAME_2",
                    DescConvo = "RANDOMIZER_CHARM_DESC_2",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_2",
                    Cost = 420,
                    SpriteName = "Charms.2.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_3",
                    NameConvo = "CHARM_NAME_3",
                    DescConvo = "RANDOMIZER_CHARM_DESC_3",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_3",
                    Cost = 420,
                    SpriteName = "Charms.3.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_4",
                    NameConvo = "CHARM_NAME_4",
                    DescConvo = "RANDOMIZER_CHARM_DESC_4",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_4",
                    Cost = 420,
                    SpriteName = "Charms.4.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_5",
                    NameConvo = "CHARM_NAME_5",
                    DescConvo = "RANDOMIZER_CHARM_DESC_5",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_5",
                    Cost = 420,
                    SpriteName = "Charms.5.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_6",
                    NameConvo = "CHARM_NAME_6",
                    DescConvo = "RANDOMIZER_CHARM_DESC_6",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_6",
                    Cost = 420,
                    SpriteName = "Charms.6.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_7",
                    NameConvo = "CHARM_NAME_7",
                    DescConvo = "RANDOMIZER_CHARM_DESC_7",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_7",
                    Cost = 420,
                    SpriteName = "Charms.7.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_8",
                    NameConvo = "CHARM_NAME_8",
                    DescConvo = "RANDOMIZER_CHARM_DESC_8",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_8",
                    Cost = 420,
                    SpriteName = "Charms.8.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_9",
                    NameConvo = "CHARM_NAME_9",
                    DescConvo = "RANDOMIZER_CHARM_DESC_9",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_9",
                    Cost = 420,
                    SpriteName = "Charms.9.png"
                },
                new ShopItemDef
                {
                    PlayerDataBoolName = "gotCharm_10",
                    NameConvo = "CHARM_NAME_10",
                    DescConvo = "RANDOMIZER_CHARM_DESC_10",
                    RequiredPlayerDataBool = string.Empty,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = false,
                    NotchCostBool = "charmCost_10",
                    Cost = 420,
                    SpriteName = "Charms.10.png"
                }
            };
            shop.type = ShopType.Geo;
        }

        public void Start()
        {
            geoSprite = type == ShopType.Geo
                ? ItemChanger.GetSprite("UI.Shop.Geo")
                : ItemChanger.GetSprite("UI.Shop.Essence");

            StartCoroutine(ShowShop());
        }

        public bool HasItems()
        {
            UpdateValidItems();

            return validItems.Length > 0;
        }

        private void UpdateValidItems()
        {
            List<int> validItemsList = new List<int>();
            for (int i = 0; i < items.Length; i++)
            {
                if (IsValid(items[i]))
                {
                    validItemsList.Add(i);
                }
            }

            validItems = validItemsList.ToArray();
        }

        private bool IsValid(ShopItemDef item)
        {
            PlayerData pd = Ref.PD;

            // These ones can't be empty
            if (string.IsNullOrEmpty(item.PlayerDataBoolName) || string.IsNullOrEmpty(item.NameConvo) ||
                string.IsNullOrEmpty(item.DescConvo) || string.IsNullOrEmpty(item.SpriteName))
            {
                return false;
            }

            if (ItemChanger.GetSprite(item.SpriteName) == null)
            {
                return false;
            }

            // These ones are fine to be empty, replacing null with empty since they're mostly the same thing in this context
            // No harm changing structs around since they're value types
            if (item.RequiredPlayerDataBool == null)
            {
                item.RequiredPlayerDataBool = string.Empty;
            }

            if (item.RemovalPlayerDataBool == null)
            {
                item.RemovalPlayerDataBool = string.Empty;
            }

            if (item.NotchCostBool == null)
            {
                item.NotchCostBool = string.Empty;
            }

            if (pd.GetBool(item.PlayerDataBoolName) || pd.GetBool(item.RemovalPlayerDataBool) ||
                item.RequiredPlayerDataBool != string.Empty && !pd.GetBool(item.RequiredPlayerDataBool))
            {
                return false;
            }

            if (item.Cost < 0)
            {
                return false;
            }

            return true;
        }

        private void BuildItemImages()
        {
            if (itemImages != null && itemImages.Length > 0)
            {
                foreach (GameObject obj in itemImages)
                {
                    Destroy(obj);
                }
            }

            itemImages = new GameObject[validItems.Length, 4];

            for (int i = 0; i < itemImages.GetLength(0); i++)
            {
                itemImages[i, 0] = CanvasUtil.CreateImagePanel(gameObject,
                    ItemChanger.GetSprite(items[validItems[i]].SpriteName),
                    new CanvasUtil.RectData(new Vector2(90, 90), Vector2.zero, new Vector2(0.525f, 0f),
                        new Vector2(0.525f, 0f)));
                itemImages[i, 1] = CanvasUtil.CreateImagePanel(gameObject, geoSprite,
                    new CanvasUtil.RectData(new Vector2(50, 50), Vector2.zero, new Vector2(0.57f, 0f),
                        new Vector2(0.57f, 0f)));

                int cost = (int) (items[validItems[i]].Cost * (items[validItems[i]].DungDiscount ? 0.75f : 1));
                itemImages[i, 2] = CanvasUtil.CreateTextPanel(gameObject, cost.ToString(), 34, TextAnchor.MiddleCenter,
                    new CanvasUtil.RectData(new Vector2(1920, 1080), Vector2.zero, new Vector2(0.61f, 0f),
                        new Vector2(0.61f, 0f)), Fonts.Get("Perpetua"));

                if (type == ShopType.Geo && cost > Ref.PD.geo ||
                    type == ShopType.Essence && cost > Ref.PD.dreamOrbs)
                {
                    itemImages[i, 3] = CanvasUtil.CreateImagePanel(gameObject, blackPixel,
                        new CanvasUtil.RectData(new Vector2(300, 100), Vector2.zero, new Vector2(0.57f, 0f),
                            new Vector2(0.57f, 0f)));
                    itemImages[i, 3].GetComponent<Image>().preserveAspect = false;
                }
            }

            foreach (GameObject obj in itemImages)
            {
                obj.SetActive(false);
            }
        }

        private void UpdatePositions()
        {
            if (itemImages == null)
            {
                return;
            }

            int pos = 2 - selected;

            for (int i = 0; i < itemImages.GetLength(0); i++)
            {
                for (int j = 0; j < itemImages.GetLength(1); j++)
                {
                    Log(i + " " + j);
                    if (itemImages[i, j] != null)
                    {
                        if (pos >= 0 && pos < DefaultPositions.Length)
                        {
                            itemImages[i, j].SetActive(true);
                            RectTransform rect = itemImages[i, j].GetComponent<RectTransform>();
                            if (rect != null)
                            {
                                rect.anchorMin = new Vector2(rect.anchorMin.x, DefaultPositions[pos]);
                                rect.anchorMax = new Vector2(rect.anchorMax.x, DefaultPositions[pos]);
                            }
                        }
                        else
                        {
                            itemImages[i, j].SetActive(false);
                        }
                    }
                }

                pos++;
            }
        }

        private void ResetItems()
        {
            selected = 0;
            UpdateValidItems();
            BuildItemImages();
            UpdatePositions();
        }

        private IEnumerator ShowShop()
        {
            GameObject background = CanvasUtil.CreateImagePanel(gameObject,
                ItemChanger.GetSprite("UI.Shop.Background"),
                new CanvasUtil.RectData(new Vector2(810, 813), Vector2.zero, new Vector2(0.675f, 0.525f),
                    new Vector2(0.675f, 0.525f)));

            GameObject bottomFleur = CanvasUtil.CreateImagePanel(gameObject,
                ItemChanger.GetSprite("Anim.Shop.BottomFleur.0"),
                new CanvasUtil.RectData(new Vector2(811, 241), Vector2.zero, new Vector2(0.675f, 0.3f),
                    new Vector2(0.675f, 0.3f)));
            StartCoroutine(AnimateImage(bottomFleur, bottomFrames, 12));
            StartCoroutine(TweenY(bottomFleur, 0.3f, 0.2f, 60, 15));

            GameObject topFleur = CanvasUtil.CreateImagePanel(gameObject,
                ItemChanger.GetSprite("Anim.Shop.TopFleur.0"),
                new CanvasUtil.RectData(new Vector2(808, 198), Vector2.zero, new Vector2(0.675f, 0.6f),
                    new Vector2(0.675f, 0.6f)));
            StartCoroutine(AnimateImage(topFleur, topFrames, 12));
            StartCoroutine(TweenY(topFleur, 0.6f, 0.85f, 60, 15));

            yield return StartCoroutine(CanvasUtil.FadeInCanvasGroup(background.AddComponent<CanvasGroup>()));

            CanvasUtil.CreateImagePanel(gameObject, ItemChanger.GetSprite("UI.Shop.Selector"),
                new CanvasUtil.RectData(new Vector2(340, 113), Vector2.zero, new Vector2(0.57f, 0.5825f),
                    new Vector2(0.57f, 0.5825f)));
            CanvasUtil.CreateImagePanel(gameObject, ItemChanger.GetSprite("UI.Shop.Shitpost"),
                new CanvasUtil.RectData(new Vector2(112, 112), Vector2.zero, new Vector2(0.6775f, 0.92f),
                    new Vector2(0.6775f, 0.92f)));

            ResetItems();

            StartCoroutine(ListenForInput());
        }

        private IEnumerator ListenForInput()
        {
            HeroActions buttons = Ref.Input.inputActions;

            while (true)
            {
                if (selected > 0 && buttons.up.WasPressed)
                {
                    selected--;
                    UpdatePositions();
                }
                else if (selected < validItems.Length - 1 && buttons.down.WasPressed)
                {
                    selected++;
                    UpdatePositions();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator AnimateImage(GameObject fleur, Sprite[] frames, float fps)
        {
            Image img = fleur.GetComponent<Image>();
            int spriteNum = 0;

            while (spriteNum < frames.Length)
            {
                img.sprite = frames[spriteNum];
                spriteNum++;
                yield return new WaitForSeconds(1 / fps);
            }
        }

        private IEnumerator TweenY(GameObject obj, float start, float end, float fps, int updateCount)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(rect.anchorMin.x, start);
            rect.anchorMax = new Vector2(rect.anchorMax.x, start);

            float updateAmount = (end - start) / updateCount;

            while (updateCount > 0)
            {
                yield return new WaitForSeconds(1 / fps);
                rect.anchorMin = new Vector2(rect.anchorMin.x, rect.anchorMin.y + updateAmount);
                rect.anchorMax = new Vector2(rect.anchorMax.x, rect.anchorMax.y + updateAmount);
                updateCount--;
            }

            rect.anchorMin = new Vector2(rect.anchorMin.x, end);
            rect.anchorMax = new Vector2(rect.anchorMax.x, end);
        }
    }
}
