using UnityEngine;

namespace SceneGroupLoader.JointActivation
{
    public class SceneGroupLoaderComponent : MonoBehaviour
    {
        private SceneGroupLoader SceneGroupLoader = new SceneGroupLoader();

        public void LoadAndActivateSceneGroup(SceneGroup sceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.LoadAndActivateSceneGroup(sceneGroup, onDone);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.UnloadSceneGroup(asyncSceneGroup, onDone);
        }

        void Update()
        {
            SceneGroupLoader.UpdateStatus();
        }
    }
}
