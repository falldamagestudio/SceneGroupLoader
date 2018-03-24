﻿#define SCENEGROUPLOADER_LOGGING

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SceneGroupLoader
{
    public class SceneGroupLoader {

        public class SceneGroupHandle { }

        public delegate void OnDone(SceneGroupHandle sceneGroupHandle);

        private AsyncSceneGroupLoader asyncSceneGroupLoader = new AsyncSceneGroupLoader();

        private SceneGroupHandle currentOperation;
        private OnDone currentCompletionCallback;

        private Queue<SceneGroupHandle> loadedButNotActivatedSceneGroups = new Queue<SceneGroupHandle>();

        public void LoadSceneGroup(SceneGroup sceneGroup, OnDone onDone)
        {
            Debug.LogFormat("LoadSceneGroup: load sceneGroup {0}", sceneGroup.name);

            Assert.IsNull(currentOperation, "Scene group load is not allowed when another async operation is in progress");
            SceneGroupHandle sceneGroupHandle = asyncSceneGroupLoader.LoadSceneGroupAsync(sceneGroup);
            loadedButNotActivatedSceneGroups.Enqueue(sceneGroupHandle);
            currentOperation = sceneGroupHandle;
            currentCompletionCallback = (x) => LoadSceneGroupDone(onDone);
        }

        private void LoadSceneGroupDone(OnDone onDone)
        {
            Debug.LogFormat("LoadSceneGroup: load sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)currentOperation).SceneGroup.name);

            currentOperation = null;
            onDone(currentOperation);
        }

        public void ActivateSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Debug.LogFormat("ActivateSceneGroup: activate sceneGroup {0}", ((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup).SceneGroup.name);

            Assert.IsNull(currentOperation, "Scene group activation is not allowed when another async operation is in progress");
            Assert.AreEqual(loadedButNotActivatedSceneGroups.Peek(), asyncSceneGroup, "Activation must activate the next loaded scene group in the queue");
            loadedButNotActivatedSceneGroups.Dequeue();
            asyncSceneGroupLoader.ActivateSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            currentOperation = asyncSceneGroup;
            currentCompletionCallback = (x) => ActivateSceneGroupDone(onDone);
        }

        private void ActivateSceneGroupDone(OnDone onDone)
        {
            Debug.LogFormat("ActivateSceneGroup: activate sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)currentOperation).SceneGroup.name);

            currentOperation = null;
            onDone(currentOperation);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0}", ((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup).SceneGroup.name);

            Assert.IsNull(currentOperation, "Scene group load is not allowed when another async operation is in progress");
            Assert.AreEqual(0, loadedButNotActivatedSceneGroups.Count, "Unload is not allowed when another scene group has been loaded but not yet activated");
            asyncSceneGroupLoader.UnloadSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            currentOperation = asyncSceneGroup;
            currentCompletionCallback = (x) => UnloadSceneGroupDone(onDone);
        }

        private void UnloadSceneGroupDone(OnDone onDone)
        {
            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)currentOperation).SceneGroup.name);

            currentOperation = null;
            onDone(currentOperation);
        }

        public void UpdateStatus()
        {
            asyncSceneGroupLoader.UpdateStatus((AsyncSceneGroupLoader.AsyncSceneGroup)currentOperation);

            if ((currentCompletionCallback != null) && !asyncSceneGroupLoader.IsAsyncSceneGroupOperationInProgress((AsyncSceneGroupLoader.AsyncSceneGroup)currentOperation))
            {
                OnDone callback = currentCompletionCallback;
                currentCompletionCallback = null;
                callback(currentOperation);
            }
        }
    }
}