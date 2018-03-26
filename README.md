# SceneGroupLoader

Standardized logic for asynchronous loading/unloading of groups of Unity scenes

## Purpose

This provides APIs on top of Unity's `LoadSceneAsync()` / `UnloadSceneAsync()` that make it easy to work with groups of scenes, instead of individual scenes.

There are two versions of the API; one with joint activation, and one with separate activation. Each has different tradeoffs.

## Joint Activation API

This API has two operations: `LoadAndActivateSceneGroup()` and `UnloadSceneGroup()`. It allows concurrent loading and unloading of scenes 
in arbitrary order.

It is suitable for games where you want to stream in and out scenes, and where it is not important to be able to delay the activation of the scenes.

## Separate Activation API

This API has three operations: `LoadSceneGroup()`, `ActivateSceneGroup()` and `UnloadSceneGroup()`.

The API allows you to separate the loading step from the activation steps. However, since delayed activation is prone to race conditions (see [DEADLOCKS.md](DEADLOCKS.md) for details), the API is much more restrictive.

It is intended to remove long, static loading screens from games that have mostly linear flows. It will not work well for games that have a more free-streaming approach.

### Restrictions

Several restrictions are enforced via asserts. They are there to minimize the risk that you write code that sometimes deadlocks on some machines due to timing changes.

The load/activate/unload operations are not queued. You are not allowed to initiate a new operation when an existing operation is in progress. If you want queueing, you must implement it yourself while ensuring you never fall foul of the other restrictions in this section.

You must initiate activation of scene groups in the same order that you have initiated loading of them.

You must not initiate unloading of a scene group when there are any scene groups which are loaded, but not yet activated.

### Timing considerations

Activation of all scenes in a scene group will not be perfectly concurrent. For example, Unity 2017.3.1f1 will activate one scene per frame.

Unloading in Unity 2017.3.1f1 will deactivate all scenes in a scene group within a single frame.

## How to use

Create a master game scene for your game. This scene will only be unloaded at application exit. Place a game object with either a `JointActivation.SceneGroupLoaderComponent` or a `SeparateActivation.SceneGroupLoaderComponent` component in that scene.

Initiate load/activate/unload of various portions of your game through calls to that component.

There is an example project available at https://github.com/Kalmalyzer/SceneGroupLoader-Example. It contains examples for both APIs.
