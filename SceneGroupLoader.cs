#define SCENEGROUPLOADER_LOGGING

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SceneGroupLoader
{
    public class SceneGroupLoader {

        public class SceneGroupHandle { }

        public delegate void OnDone(SceneGroupHandle sceneGroupHandle);

        private AsyncSceneGroupLoader asyncSceneGroupLoader = new AsyncSceneGroupLoader();

        private SceneGroupHandle currentInProgressSceneGroup;
        private Action currentCompletionCallback;

        private Queue<SceneGroupHandle> loadedButNotActivatedSceneGroups = new Queue<SceneGroupHandle>();

        public void LoadSceneGroup(SceneGroup sceneGroup, OnDone onDone)
        {
            Assert.IsNotNull(sceneGroup);

            Debug.LogFormat("LoadSceneGroup: load sceneGroup {0}", sceneGroup.name);

            Assert.IsNull(currentInProgressSceneGroup, "Scene group load is not allowed when another async operation is in progress");
            SceneGroupHandle sceneGroupHandle = asyncSceneGroupLoader.LoadSceneGroupAsync(sceneGroup);
            loadedButNotActivatedSceneGroups.Enqueue(sceneGroupHandle);
            SetInProgressSceneGroup(sceneGroupHandle, () => { LoadSceneGroupDone(sceneGroupHandle, onDone); });
        }

        private void LoadSceneGroupDone(SceneGroupHandle handle, OnDone onDone)
        {
            Debug.LogFormat("LoadSceneGroup: load sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)handle).SceneGroup.name);

            if (onDone != null)
                onDone(handle);
        }

        public void ActivateSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Assert.IsNotNull(asyncSceneGroup);

            Debug.LogFormat("ActivateSceneGroup: activate sceneGroup {0}", ((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup).SceneGroup.name);

            Assert.IsNull(currentInProgressSceneGroup, "Scene group activation is not allowed when another async operation is in progress");
            Assert.AreEqual(loadedButNotActivatedSceneGroups.Peek(), asyncSceneGroup, "Activation must activate the next loaded scene group in the queue");
            loadedButNotActivatedSceneGroups.Dequeue();
            asyncSceneGroupLoader.ActivateSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            SetInProgressSceneGroup(asyncSceneGroup, () => { ActivateSceneGroupDone(asyncSceneGroup, onDone); });
        }

        private void ActivateSceneGroupDone(SceneGroupHandle handle, OnDone onDone)
        {
            Debug.LogFormat("ActivateSceneGroup: activate sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)handle).SceneGroup.name);

            if (onDone != null)
                onDone(handle);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Assert.IsNotNull(asyncSceneGroup);

            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0}", ((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup).SceneGroup.name);

            Assert.IsNull(currentInProgressSceneGroup, "Scene group load is not allowed when another async operation is in progress");
            Assert.AreEqual(0, loadedButNotActivatedSceneGroups.Count, "Unload is not allowed when another scene group has been loaded but not yet activated");
            asyncSceneGroupLoader.UnloadSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            SetInProgressSceneGroup(asyncSceneGroup, () => { UnloadSceneGroupDone(asyncSceneGroup, onDone); });
        }

        private void UnloadSceneGroupDone(SceneGroupHandle handle, OnDone onDone)
        {
            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)handle).SceneGroup.name);

            if (onDone != null)
                onDone(handle);
        }

        private void SetInProgressSceneGroup(SceneGroupHandle sceneGroupHandle, Action callback)
        {
            Assert.IsNotNull(sceneGroupHandle);
            Assert.IsNotNull(callback);
            currentInProgressSceneGroup = sceneGroupHandle;
            currentCompletionCallback = callback;
        }

        public void UpdateStatus()
        {
            if (currentInProgressSceneGroup != null)
            {
                Assert.IsNotNull(currentCompletionCallback);

                asyncSceneGroupLoader.UpdateStatus((AsyncSceneGroupLoader.AsyncSceneGroup)currentInProgressSceneGroup);

                if (!asyncSceneGroupLoader.IsAsyncSceneGroupOperationInProgress((AsyncSceneGroupLoader.AsyncSceneGroup)currentInProgressSceneGroup))
                {
                    Action callback = currentCompletionCallback;
                    currentCompletionCallback = null;
                    currentInProgressSceneGroup = null;
                    callback();
                }
            }
        }
    }
}