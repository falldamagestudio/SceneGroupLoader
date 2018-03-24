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

        private class InProgressSceneGroup
        {
            public SceneGroupHandle CurrentInProgressSceneGroup;
            public Action CurrentCompletionCallback;

            public InProgressSceneGroup(SceneGroupHandle currentInProgressSceneGroup, Action currentCompletionCallback)
            {
                Assert.IsNotNull(currentInProgressSceneGroup);
                Assert.IsNotNull(currentCompletionCallback);
                CurrentInProgressSceneGroup = currentInProgressSceneGroup;
                CurrentCompletionCallback = currentCompletionCallback;
            }
        }

        private InProgressSceneGroup inProgressSceneGroup;

        private Queue<SceneGroupHandle> loadedButNotActivatedSceneGroups = new Queue<SceneGroupHandle>();

        public void LoadSceneGroup(SceneGroup sceneGroup, OnDone onDone)
        {
            Assert.IsNotNull(sceneGroup);

            Debug.LogFormat("LoadSceneGroup: load sceneGroup {0}", sceneGroup.name);

            Assert.IsNull(inProgressSceneGroup, "Scene group load is not allowed when another async operation is in progress");
            SceneGroupHandle sceneGroupHandle = asyncSceneGroupLoader.LoadSceneGroupAsync(sceneGroup);
            loadedButNotActivatedSceneGroups.Enqueue(sceneGroupHandle);
            inProgressSceneGroup = new InProgressSceneGroup(sceneGroupHandle, () => { LoadSceneGroupDone(sceneGroupHandle, onDone); });
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

            Assert.IsNull(inProgressSceneGroup, "Scene group activation is not allowed when another async operation is in progress");
            Assert.AreEqual(loadedButNotActivatedSceneGroups.Peek(), asyncSceneGroup, "Activation must activate the next loaded scene group in the queue");
            loadedButNotActivatedSceneGroups.Dequeue();
            asyncSceneGroupLoader.ActivateSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            inProgressSceneGroup = new InProgressSceneGroup(asyncSceneGroup, () => { ActivateSceneGroupDone(asyncSceneGroup, onDone); });
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

            Assert.IsNull(inProgressSceneGroup, "Scene group load is not allowed when another async operation is in progress");
            Assert.AreEqual(0, loadedButNotActivatedSceneGroups.Count, "Unload is not allowed when another scene group has been loaded but not yet activated");
            asyncSceneGroupLoader.UnloadSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            inProgressSceneGroup = new InProgressSceneGroup(asyncSceneGroup, () => { UnloadSceneGroupDone(asyncSceneGroup, onDone); });
        }

        private void UnloadSceneGroupDone(SceneGroupHandle handle, OnDone onDone)
        {
            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)handle).SceneGroup.name);

            if (onDone != null)
                onDone(handle);
        }

        public void UpdateStatus()
        {
            if (inProgressSceneGroup != null)
            {
                asyncSceneGroupLoader.UpdateStatus((AsyncSceneGroupLoader.AsyncSceneGroup)inProgressSceneGroup.CurrentInProgressSceneGroup);

                if (!asyncSceneGroupLoader.IsAsyncSceneGroupOperationInProgress((AsyncSceneGroupLoader.AsyncSceneGroup)inProgressSceneGroup.CurrentInProgressSceneGroup))
                {
                    Action callback = inProgressSceneGroup.CurrentCompletionCallback;
                    inProgressSceneGroup = null;
                    callback();
                }
            }
        }
    }
}