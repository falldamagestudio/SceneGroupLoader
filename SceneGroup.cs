using System.Collections.Generic;
using UnityEngine;

namespace SceneGroupLoader
{
    [CreateAssetMenu(fileName = "SceneGroup", menuName = "SceneGroup", order = 1000)]
    public class SceneGroup : ScriptableObject
    {
        public List<string> Scenes;
    }
}
