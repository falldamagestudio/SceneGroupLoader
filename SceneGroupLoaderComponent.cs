using UnityEngine;

namespace SceneGroupLoader
{
    public class SceneGroupLoaderComponent : MonoBehaviour
    {
        public SceneGroupLoader SceneGroupLoader = new SceneGroupLoader();

        public void LoadSceneGroup(SceneGroup sceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.LoadSceneGroup(sceneGroup, onDone);
        }

        public void ActivateSceneGroup(SceneGroupLoader.SceneGroupHandle asyncSceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.ActivateSceneGroup(asyncSceneGroup, onDone);
        }

        public void UnloadSceneGroup(SceneGroupLoader.SceneGroupHandle asyncSceneGroup, SceneGroupLoader.OnDone onDone)
        {
            SceneGroupLoader.UnloadSceneGroup(asyncSceneGroup, onDone);
        }

        void Update()
        {
            SceneGroupLoader.UpdateStatus();
        }
    }
}
