using ItemChanger.Components;
using ItemChanger.Extensions;

namespace ItemChanger.Locations
{
    /// <summary>
    /// A location for modifying an object with an fsm in-place with the specified Container.
    /// </summary>
    public class ExistingFsmContainerLocation : ExistingContainerLocation, ILocalHintLocation
    {
        public string objectName;
        public string fsmName;
        /// <summary>
        /// The path to find the object on active scene change, if it is to be replaced. If this is null, replacement happens when the fsm is enabled instead.
        /// </summary>
        public string replacePath;
        public float elevation;

        /// <summary>
        /// Creates a HintBox at the transform of the existing container or its replacement.
        /// </summary>
        public bool HintActive { get; set; } = false;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, fsmName), OnEnable);
            if (!nonreplaceable && replacePath != null) Events.AddSceneChangeEdit(sceneName, ReplaceOnSceneChange);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), OnEnable);
            Events.RemoveSceneChangeEdit(sceneName, ReplaceOnSceneChange);
        }

        public void OnEnable(PlayMakerFSM fsm)
        {
            if (fsm.gameObject.GetComponent<ContainerInfo>() is ContainerInfo info && info != null) return;
            if (!WillBeReplaced())
            {
                info = fsm.gameObject.AddComponent<ContainerInfo>();
                FillInfo(info);
                info.containerType = containerType;
                if (HintActive) HintBox.Create(fsm.transform, Placement);
            }
            else if (replacePath == null)
            {
                Container c = Container.GetContainer(Placement.MainContainerType);
                GameObject obj = c.GetNewContainer(Placement, Placement.Items, flingType, (Placement as Placements.ISingleCostPlacement)?.Cost);
                c.ApplyTargetContext(obj, fsm.gameObject, elevation);
                UnityEngine.Object.Destroy(fsm.gameObject);
                if (HintActive) HintBox.Create(obj.transform, Placement);
            }
        }

        public void ReplaceOnSceneChange(Scene to)
        {
            if (WillBeReplaced())
            {
                Container c = Container.GetContainer(Placement.MainContainerType);
                GameObject obj = c.GetNewContainer(Placement, Placement.Items, flingType, (Placement as Placements.ISingleCostPlacement)?.Cost);
                GameObject target = to.FindGameObject(replacePath);
                c.ApplyTargetContext(obj, target, elevation);
                UnityEngine.Object.Destroy(target);
                if (HintActive) HintBox.Create(obj.transform, Placement);
            }
        }

        private void FillInfo(ContainerInfo info)
        {
            info.containerType = containerType;
            info.giveInfo = new ContainerGiveInfo
            {
                placement = Placement,
                items = Placement.Items,
                flingType = flingType,
            };

            if (Placement is Placements.ISingleCostPlacement iscp && iscp.Cost != null)
            {
                info.costInfo = new CostInfo
                {
                    placement = Placement,
                    previewItems = Placement.Items,
                    cost = iscp.Cost,
                };
            }

            Tags.ChangeSceneTag cst = GetTags<Tags.ChangeSceneTag>().Concat(Placement.GetTags<Tags.ChangeSceneTag>()).FirstOrDefault();
            if (cst != null)
            {
                info.changeSceneInfo = new()
                {
                    transition = cst.changeTo
                };
            }
        }
    }
}
