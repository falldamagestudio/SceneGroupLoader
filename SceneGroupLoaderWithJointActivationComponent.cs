using UnityEngine;

namespace SceneGroupLoader
{
    public class SceneGroupLoaderWithJointActivationComponent : MonoBehaviour
    {
        private SceneGroupLoaderWithJointActivation SceneGroupLoader = new SceneGroupLoaderWithJointActivation();

        public void LoadAndActivateSceneGroup(SceneGroup sceneGroup, SceneGroupLoaderWithJointActivation.OnDone onDone)
        {
            SceneGroupLoader.LoadAndActivateSceneGroup(sceneGroup, onDone);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, SceneGroupLoaderWithJointActivation.OnDone onDone)
        {
            SceneGroupLoader.UnloadSceneGroup(asyncSceneGroup, onDone);
        }

        void Update()
        {
            SceneGroupLoader.UpdateStatus();
        }
    }
}
