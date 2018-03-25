using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SceneGroupLoader
{
    public class AsyncSceneGroupLoader
    {
        public enum LoadStatus
        {
            NotLoaded,
            Loading,
            LoadingAndActivating,
            Loaded,
            Activating,
            Activated,
            Unloading,
            Unloaded
        }

        public class AsyncSceneGroup : SceneGroupLoaderWithSeparateActivation.SceneGroupHandle
        {
            public class AsyncScene
            {
                public LoadStatus Status;
                public Scene Scene;

                public AsyncOperation AsyncOperation;

                public AsyncScene()
                {
                    Status = LoadStatus.NotLoaded;
                    AsyncOperation = null;
                }
            }

            public List<AsyncScene> AsyncScenes;
            public SceneGroup SceneGroup;

            public LoadStatus Status;

            public AsyncSceneGroup(SceneGroup sceneGroup)
            {
                Status = LoadStatus.NotLoaded;
                SceneGroup = sceneGroup;
                AsyncScenes = new List<AsyncScene>();
                for (int i = 0; i < sceneGroup.Scenes.Count; i++)
                    AsyncScenes.Add(new AsyncScene());
            }
        }

        public enum LoadAndActivationMode
        {
            Separate,
            Joint,
        }

        private LoadAndActivationMode loadAndActivationMode;

        public AsyncSceneGroupLoader(LoadAndActivationMode loadAndActivationMode)
        {
            this.loadAndActivationMode = loadAndActivationMode;
        }

        public AsyncSceneGroup LoadSceneGroupAsync(SceneGroup sceneGroup)
        {
            Assert.IsNotNull(sceneGroup);
            Assert.AreNotEqual(0, sceneGroup.Scenes.Count);

            AsyncSceneGroup asyncSceneGroup = new AsyncSceneGroup(sceneGroup);

            for (int i = 0; i < asyncSceneGroup.SceneGroup.Scenes.Count; i++)
                LoadSceneAsync(asyncSceneGroup, i);

            asyncSceneGroup.Status = ((loadAndActivationMode == LoadAndActivationMode.Separate) ? LoadStatus.Loading : LoadStatus.LoadingAndActivating);

            return asyncSceneGroup;
        }

        private void LoadSceneAsync(AsyncSceneGroup asyncSceneGroup, int i)
        {
            Assert.IsNotNull(asyncSceneGroup);
            AsyncSceneGroup.AsyncScene asyncScene = asyncSceneGroup.AsyncScenes[i];
            Assert.AreEqual(LoadStatus.NotLoaded, asyncScene.Status);
            AsyncOperation op = SceneManager.LoadSceneAsync(asyncSceneGroup.SceneGroup.Scenes[i].SceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            if (loadAndActivationMode == LoadAndActivationMode.Separate)
                op.allowSceneActivation = false;

            asyncScene.AsyncOperation = op;

            asyncScene.Status = ((loadAndActivationMode == LoadAndActivationMode.Separate) ? LoadStatus.Loading : LoadStatus.LoadingAndActivating);
            asyncScene.Scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        }

        public void ActivateSceneGroupAsync(AsyncSceneGroup asyncSceneGroup)
        {
            Assert.IsNotNull(asyncSceneGroup);
            Assert.AreEqual(LoadStatus.Loaded, asyncSceneGroup.Status);
            Assert.AreEqual(LoadAndActivationMode.Separate, loadAndActivationMode);

            for (int i = 0; i < asyncSceneGroup.SceneGroup.Scenes.Count; i++)
                ActivateSceneAsync(asyncSceneGroup, i);

            asyncSceneGroup.Status = LoadStatus.Activating;
        }

        private void ActivateSceneAsync(AsyncSceneGroup asyncSceneGroup, int i)
        {
            Assert.IsNotNull(asyncSceneGroup);
            Assert.AreEqual(LoadAndActivationMode.Separate, loadAndActivationMode);

            AsyncSceneGroup.AsyncScene asyncScene = asyncSceneGroup.AsyncScenes[i];
            Assert.AreEqual(LoadStatus.Loaded, asyncScene.Status);

            asyncScene.AsyncOperation.allowSceneActivation = true;
            asyncScene.Status = LoadStatus.Activating;
        }

        public void UnloadSceneGroupAsync(AsyncSceneGroup asyncSceneGroup)
        {
            Assert.IsNotNull(asyncSceneGroup);
            Assert.AreEqual(LoadStatus.Activated, asyncSceneGroup.Status);

            for (int i = 0; i < asyncSceneGroup.SceneGroup.Scenes.Count; i++)
                UnloadSceneAsync(asyncSceneGroup, i);

            asyncSceneGroup.Status = LoadStatus.Unloading;
        }

        private void UnloadSceneAsync(AsyncSceneGroup asyncSceneGroup, int i)
        {
            Assert.IsNotNull(asyncSceneGroup);

            AsyncSceneGroup.AsyncScene asyncScene = asyncSceneGroup.AsyncScenes[i];
            Assert.AreEqual(LoadStatus.Activated, asyncScene.Status);

            AsyncOperation op = SceneManager.UnloadSceneAsync(asyncScene.Scene);
            asyncScene.AsyncOperation = op;
            asyncScene.Status = LoadStatus.Unloading;
        }

        public void UpdateStatus(AsyncSceneGroup asyncSceneGroup)
        {
            Assert.IsNotNull(asyncSceneGroup);

            for (int i = 0; i < asyncSceneGroup.SceneGroup.Scenes.Count; i++)
                UpdateStatus(asyncSceneGroup, i);

            bool asyncOperationInProgress = false;

            for (int i = 0; i < asyncSceneGroup.SceneGroup.Scenes.Count; i++)
                if (IsAsyncOperationInProgress(asyncSceneGroup, i))
                    asyncOperationInProgress = true;

            if (!asyncOperationInProgress)
                asyncSceneGroup.Status = asyncSceneGroup.AsyncScenes[0].Status;
        }

        private void UpdateStatus(AsyncSceneGroup asyncSceneGroup, int i)
        {
            Assert.IsNotNull(asyncSceneGroup);
            AsyncSceneGroup.AsyncScene asyncScene = asyncSceneGroup.AsyncScenes[i];

            // LoadAndActivationMode.Separate cases
            if (asyncScene.Status == LoadStatus.Loading && asyncScene.AsyncOperation.progress >= 0.9f)
                asyncScene.Status = LoadStatus.Loaded;
            if (asyncScene.Status == LoadStatus.Activating && asyncScene.AsyncOperation.isDone)
                asyncScene.Status = LoadStatus.Activated;

            // LoadAndActivationMode.Joint cases
            if (asyncScene.Status == LoadStatus.LoadingAndActivating && asyncScene.AsyncOperation.isDone)
                asyncScene.Status = LoadStatus.Activated;

            if (asyncScene.Status == LoadStatus.Unloading && asyncScene.AsyncOperation.isDone)
                asyncScene.Status = LoadStatus.Unloaded;
        }

        public bool IsAsyncSceneGroupOperationInProgress(AsyncSceneGroup asyncSceneGroup)
        {
            Assert.IsNotNull(asyncSceneGroup);
            return IsAsyncOperationInProgress(asyncSceneGroup.Status);
        }

        private bool IsAsyncOperationInProgress(AsyncSceneGroup asyncSceneGroup, int i)
        {
            Assert.IsNotNull(asyncSceneGroup);
            AsyncSceneGroup.AsyncScene asyncScene = asyncSceneGroup.AsyncScenes[i];
            return IsAsyncOperationInProgress(asyncScene.Status);
        }

        private static bool IsAsyncOperationInProgress(LoadStatus status)
        {
            bool asyncOperationInProgress =
                  (status == LoadStatus.Loading
                || status == LoadStatus.Activating
                || status == LoadStatus.LoadingAndActivating
                || status == LoadStatus.Unloading);

            return asyncOperationInProgress;
        }
    }
}
