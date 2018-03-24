using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneGroupLoader
{
    public class SceneGroupLoader {

        public class SceneGroupHandle { }

        public delegate void OnDone(SceneGroupHandle sceneGroupHandle);

        private AsyncSceneGroupLoader asyncSceneGroupLoader = new AsyncSceneGroupLoader();

        public void LoadSceneGroup(SceneGroup sceneGroup, OnDone onDone)
        {
            SceneGroupHandle sceneGroupHandle = asyncSceneGroupLoader.LoadSceneGroupAsync(sceneGroup);
        }

        public void ActivateSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
        }
    }
}