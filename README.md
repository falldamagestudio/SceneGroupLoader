# SceneGroupLoader
Standardized logic for asynchronous loading/unloading of groups of Unity scenes

## Purpose

This is an interface on top of Unity's LoadSceneAsync() / UnloadSceneAsync() methods that is more convenient, but also more restrictive, than Unity's own API. It is intended to remove long, static loading screens from games that have mostly linear flows. It will not work well for games that have a more free-streaming approach.

## Features
All operations work on groups of scenes. You request a group of scenes to be loaded, and then you wait for all of them to complete loading, before you intiate activation on all of them. This makes it easy for content creators to split up large scenes into multiple, smaller scene files. It also makes the API small and easy to use.

## Restrictions
Several restrictions are enforced via asserts. They are there to minimize the risk that you write code that sometimes deadlocks due to timing changes.

The load/activate/unload operations are not queued. You are not allowed to initiate a new operation when an existing operation is in progress. If you want queueing, you must implement it yourself while ensuring you never fall foul of the other restrictions in this section.

You must initiate activation of scene groups in the same order that you have initiated loading of them.

You must not initiate unloading of a scene group when there are any scene groups which are loaded, but not yet activated.

## How to use

Create a master game scene for your game. This scene will only be unloaded at application exit. Place a game object with a SceneGroupLoaderComponent in that scene.

Initiate load/activate/unload of various portions of your game through calls to that component.

There is an example project available at https://github.com/Kalmalyzer/SceneGroupLoader-Example
