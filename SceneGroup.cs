﻿using System.Collections.Generic;
using UnityEngine;

namespace SceneGroupLoader
{
    public class SceneGroup : ScriptableObject
    {
        public List<string> Scenes;

        public int ActiveSceneIndex;

        public int ActiveScenePriority;
    }
}