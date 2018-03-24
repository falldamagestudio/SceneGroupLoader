using UnityEngine.Assertions;

namespace SceneGroupLoader
{
    public class SceneGroupLoader {

        public class SceneGroupHandle { }

        public delegate void OnDone(SceneGroupHandle sceneGroupHandle);

        private AsyncSceneGroupLoader asyncSceneGroupLoader = new AsyncSceneGroupLoader();

        private SceneGroupHandle currentOperation;
        private OnDone currentCompletionCallback;

        public void LoadSceneGroup(SceneGroup sceneGroup, OnDone onDone)
        {
            Assert.IsNull(currentOperation, "Scene group load is not allowed when another async operation is in progress");
            SceneGroupHandle sceneGroupHandle = asyncSceneGroupLoader.LoadSceneGroupAsync(sceneGroup);
            currentOperation = sceneGroupHandle;
            currentCompletionCallback = (x) => LoadSceneGroupDone(onDone);
        }

        private void LoadSceneGroupDone(OnDone onDone)
        {
            onDone(currentOperation);
        }

        public void ActivateSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Assert.IsNull(currentOperation, "Scene group load is not allowed when another async operation is in progress");
            Assert.AreEqual(currentOperation, asyncSceneGroup, "Activation must activate the previously-loaded scene group");
            asyncSceneGroupLoader.ActivateSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            currentCompletionCallback = (x) => ActivateSceneGroupDone(onDone);
        }

        private void ActivateSceneGroupDone(OnDone onDone)
        {
            currentOperation = null;
            onDone(currentOperation);
        }

        public void UnloadSceneGroup(SceneGroupHandle asyncSceneGroup, OnDone onDone)
        {
            Assert.IsNull(currentOperation, "Scene group load is not allowed when another async operation is in progress");
            asyncSceneGroupLoader.UnloadSceneGroupAsync((AsyncSceneGroupLoader.AsyncSceneGroup)asyncSceneGroup);
            currentOperation = asyncSceneGroup;
            currentCompletionCallback = (x) => UnloadSceneGroupDone(onDone);
        }

        private void UnloadSceneGroupDone(OnDone onDone)
        {
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