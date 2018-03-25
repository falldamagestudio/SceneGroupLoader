using UnityEngine;

namespace SceneGroupLoader.SeparateActivation
{
    public class SceneGroupLoaderComponent : MonoBehaviour
    {
        private SceneGroupLoader SceneGroupLoader = new SceneGroupLoader();

        public void LoadSceneGroup(SceneGroup sceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.LoadSceneGroup(sceneGroup, onDone);
        }

        public void ActivateSceneGroup(SceneGroupHandle asyncSceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.ActivateSceneGroup(asyncSceneGroup, onDone);
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
