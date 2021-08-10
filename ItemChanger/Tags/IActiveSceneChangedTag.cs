using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Tags
{
    public interface IActiveSceneChangedTag
    {
        /// <summary>
        /// Called immediately before Location.OnActiveSceneChanged.
        /// </summary>
        void OnActiveSceneChanged(Scene from, Scene to);
    }
}
