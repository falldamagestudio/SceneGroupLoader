﻿using UnityEngine;

namespace SceneGroupLoader
{
    public class SceneGroupLoaderWithSeparateActivationComponent : MonoBehaviour
    {
        private SceneGroupLoaderWithSeparateActivation SceneGroupLoader = new SceneGroupLoaderWithSeparateActivation();

        public void LoadSceneGroup(SceneGroup sceneGroup, SceneGroupLoaderWithSeparateActivation.OnDone onDone)
        {
            SceneGroupLoader.LoadSceneGroup(sceneGroup, onDone);
        }

        public void ActivateSceneGroup(SceneGroupHandle asyncSceneGroup, SceneGroupLoaderWithSeparateActivation.OnDone onDone)
        {
            SceneGroupLoader.ActivateSceneGroup(asyncSceneGroup, onDone);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, SceneGroupLoaderWithSeparateActivation.OnDone onDone)
        {
            SceneGroupLoader.UnloadSceneGroup(asyncSceneGroup, onDone);
        }

        void Update()
        {
            SceneGroupLoader.UpdateStatus();
        }
    }
}
