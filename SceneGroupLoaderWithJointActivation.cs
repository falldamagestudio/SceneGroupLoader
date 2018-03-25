#define SCENEGROUPLOADER_LOGGING

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SceneGroupLoader
{
    public class SceneGroupLoaderWithJointActivation {

        public delegate void OnDone(SceneGroupHandle sceneGroupHandle);

        private AsyncSceneGroupLoader asyncSceneGroupLoader = new AsyncSceneGroupLoader(AsyncSceneGroupLoader.LoadAndActivationMode.Joint);

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

        private List<InProgressSceneGroup> inProgressSceneGroups = new List<InProgressSceneGroup>();

        public void LoadAndActivateSceneGroup(SceneGroup sceneGroup, OnDone onDone)
        {
            Assert.IsNotNull(sceneGroup);

#if SCENEGROUPLOADER_LOGGING
            Debug.LogFormat("LoadAndActivateSceneGroup: load and activate sceneGroup {0}", sceneGroup.name);
#endif

            SceneGroupHandle sceneGroupHandle = asyncSceneGroupLoader.LoadSceneGroupAsync(sceneGroup);
            inProgressSceneGroups.Add(new InProgressSceneGroup(sceneGroupHandle, () => { LoadAndActivateSceneGroupDone(sceneGroupHandle, onDone); }));
        }

        private void LoadAndActivateSceneGroupDone(SceneGroupHandle handle, OnDone onDone)
        {
#if SCENEGROUPLOADER_LOGGING
            Debug.LogFormat("LoadAndActivateSceneGroup: load and activate sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)handle).SceneGroup.name);
#endif

            if (onDone != null)
                onDone(handle);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Assert.IsNotNull(asyncSceneGroup);

#if SCENEGROUPLOADER_LOGGING
            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0}", ((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup).SceneGroup.name);
#endif

            asyncSceneGroupLoader.UnloadSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            inProgressSceneGroups.Add(new InProgressSceneGroup(asyncSceneGroup, () => { UnloadSceneGroupDone(asyncSceneGroup, onDone); }));
        }

        private void UnloadSceneGroupDone(SceneGroupHandle handle, OnDone onDone)
        {
#if SCENEGROUPLOADER_LOGGING
            Debug.LogFormat("UnloadSceneGroup: unload sceneGroup {0} done", ((AsyncSceneGroupLoader.AsyncSceneGroup)handle).SceneGroup.name);
#endif

            if (onDone != null)
                onDone(handle);
        }

        public void UpdateStatus()
        {
            List<InProgressSceneGroup> inProgressSceneGroupsCompleted = new List<InProgressSceneGroup>();
            foreach (InProgressSceneGroup inProgressSceneGroup in inProgressSceneGroups)
            {
                asyncSceneGroupLoader.UpdateStatus((AsyncSceneGroupLoader.AsyncSceneGroup)inProgressSceneGroup.CurrentInProgressSceneGroup);
                if (!asyncSceneGroupLoader.IsAsyncSceneGroupOperationInProgress((AsyncSceneGroupLoader.AsyncSceneGroup)inProgressSceneGroup.CurrentInProgressSceneGroup))
                {
                    inProgressSceneGroupsCompleted.Add(inProgressSceneGroup);
                }
            }

            foreach (InProgressSceneGroup inProgressSceneGroup in inProgressSceneGroupsCompleted)
            {
                inProgressSceneGroups.Remove(inProgressSceneGroup);
                Action callback = inProgressSceneGroup.CurrentCompletionCallback;
                callback();
            }
        }
    }
}