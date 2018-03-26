# Deadlocks when using delayed scene activation

Unity provides asynchronous APIs for loading and unloading scenes. It also has a feature for delaying activation until the user code requests it. 

Improper use of the delayed-activation mechanism can lead to application deadlocks.

All below observations have been performed in the Editor in Unity 2017.3.1f1.

## Starting observations

1. When an async load is started, the scene will show up as "<scenename> - in progress" in the Hierarchy view in the editor. This list of in-progress scenes is effectively a view of Unity's internal AsyncOperation queue.
2. Unity will complete async operations in the order in which they have been added to the queue (so top-to-bottom in the "in progress" list).
3. If an async load is started with allowSceneActivation = false, then that async operation will be paused partway through, until you manually set the allowSceneActivation flag back to true.
4. If all scenes are loaded with allowSceneActivation = false, then the activation will also occur in the async operation queue order. I am not sure if this sync point also exists when loading scenes with allowSceneActivation = true.

For the rest, let's assume that scenes are always loaded with allowSceneActivation = false.

## Some observations on functionality
4. Scenes will be activated in the order that they were loaded. If you set allowSceneActivation flags in an order different to the load order, Unity will still work through activations in the async operation queue order, and stall at the first that it encounters that does not have the flag set. This means that you can only set the activation flag for a scene (or group of scenes) and then safely wait for it to become true if you know that no other loads are queued up beforehand. If there are any previously queued loads, you will deadlock.
5. Unloads will only be performed when there are no previous async operations in the queue. (Otherwise, they would complete before a previous operation.) This means that you can only request an unload of a scene and safely wait for it to become true if you know that no other loads are queued up beforehand. If there are any previously queued loads, you will deadlock.

## Some observations on timing

6. Bulk activation of many scenes results in Unity activating 1 scene per frame - at least when the scenes are small.
7. Bulk unload of many scenes results in Unity unloading all of them at once.

## Composability and deadlocks

The limitations in 4 & 5 mean that load operations (when using allowSceneActivation = false) are not arbitrarily composable; it is inherently unsafe to create a self-contained component that loads in a level with allowSceneActivation = false, then waits for the 90% mark, then enables scene activation, then waits for loading to complete: another component, in another scene with a longer lifetime, might be halfway through loading another scene, and waiting on this self-contained component to report "initialization done" back to it -> deadlock.

## Strategies

Strategy 1: Use allowSceneActivation = false all the way. Design a non-composable API, which supports loading / activation / unloading of scenes in bulk. Add assertions to detect potential-deadlock scenarios: do not allow activation of a scene group, if a previously-loaded scene group is already awaiting activation; require activation of groups to occur in load order; do not allow unloading, if there already are any load/activation operations in the queue. This API provides an even stricter rule set: it does not allow invoking load/activate/unload if another operation is in progress. It is not intended for dynamic streaming, but rather to enable ahead-of-time loading of scenes when the game flow is fixed.

Strategy 2: Use allowSceneActivation = true all the way. The API on top will then be composable, and it will be safe to do things like "ask to load+activate scene, and wait for operation to complete, then broadcast to the rest of the application".
