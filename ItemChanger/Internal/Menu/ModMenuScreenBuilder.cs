using Modding;
using Modding.Menu;
using Modding.Menu.Config;
using UnityEngine.UI;

namespace ItemChanger.Internal.Menu
{
    /// <summary>
    /// Provides a simple way to create menu screens, that supports adding subpages and IMenuMod.MenuEntries
    /// with the default size and position data.
    /// </summary>
    public class ModMenuScreenBuilder
    {
        private readonly MenuScreen returnScreen;
        private readonly Dictionary<string, MenuScreen> MenuScreens = new();
        public readonly MenuBuilder menuBuilder;
        public readonly MenuButton backButton;

        // Defer creating the menu screen until we know whether we will need a scroll pane
        public List<Action<ContentArea>> buildActions = new();
        private void ApplyBuildActions(ContentArea c)
        {
            foreach (Action<ContentArea> action in buildActions)
            {
                action(c);
            }
        }

        public ModMenuScreenBuilder(string title, MenuScreen returnScreen)
        {
            this.returnScreen = returnScreen;
            this.menuBuilder = ModMenuHelper.CreateMenuBuilder(title, returnScreen, out this.backButton);
        }

        public MenuScreen CreateMenuScreen()
        {
            if (buildActions.Count > 5)
            {
                menuBuilder.AddContent(new NullContentLayout(), c => c.AddScrollPaneContent(
                    new ScrollbarConfig
                    {
                        CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                        Navigation = new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnUp = backButton,
                            selectOnDown = backButton
                        },
                        Position = new AnchoredPosition
                        {
                            ChildAnchor = new Vector2(0f, 1f),
                            ParentAnchor = new Vector2(1f, 1f),
                            Offset = new Vector2(-310f, 0f)
                        }
                    },
                    new RelLength(buildActions.Count * 105f),
                    RegularGridLayout.CreateVerticalLayout(105f),
                    ApplyBuildActions
                ));
            }
            else
            {
                menuBuilder.AddContent(
                    RegularGridLayout.CreateVerticalLayout(105f),
                    ApplyBuildActions
                );
            }

            return this.menuBuilder.Build();
        }

        /// <summary>
        /// Adds a button which proceeds to a subpage consisting of a list of MenuOptionHorizontals.
        /// </summary>
        public void AddSubpage(string title, string description, IReadOnlyList<IMenuMod.MenuEntry> entries)
        {
            MenuScreen screen = ModMenuHelper.CreateMenuScreen(title, this.menuBuilder.Screen, entries);
            AddSubpage(title, description, screen);
        }

        /// <summary>
        /// Adds a button which proceeds to a subpage.
        /// </summary>
        public void AddSubpage(string title, string description, MenuScreen screen)
        {
            MenuScreens.Add(title, screen);
            MenuButtonConfig config = new()
            {
                Label = title,
                Description = new()
                {
                    Text = description
                },
                Proceed = true,
                SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(MenuScreens[title]),
                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(this.menuBuilder.Screen),
            };

            this.buildActions.Add(c => c.AddMenuButton(title, config));
        }

        /// <summary>
        /// Adds a horizontal option.
        /// </summary>
        /// <param name="entry">The struct containing the data for the menu entry.</param>
        public void AddHorizontalOption(IMenuMod.MenuEntry entry)
        {
            HorizontalOptionConfig config = new()
            {
                ApplySetting = (_, i) => entry.Saver(i),
                RefreshSetting = (s, _) => s.optionList.SetOptionTo(entry.Loader()),
                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(this.returnScreen),
                Description = string.IsNullOrEmpty(entry.Description) ? null : new DescriptionInfo
                {
                    Text = entry.Description
                },
                Label = entry.Name,
                Options = entry.Values,
                Style = HorizontalOptionStyle.VanillaStyle
            };

            this.buildActions.Add(c =>
            {
                c.AddHorizontalOption(entry.Name, config, out MenuOptionHorizontal option);
                option.menuSetting.RefreshValueFromGameSettings();
            });
        }

        /// <summary>
        /// Adds a clickable button which executes a custom action on click.
        /// </summary>
        public void AddButton(string title, string description, Action onClick)
        {
            MenuButtonConfig config = new()
            {
                Label = title,
                Description = new()
                {
                    Text = description
                },
                Proceed = false,
                SubmitAction = _ => onClick(),
                CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(this.returnScreen),
            };

            this.buildActions.Add(c => c.AddMenuButton(title, config));
        }
    }
}
