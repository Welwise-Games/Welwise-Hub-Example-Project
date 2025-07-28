using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class SceneTools
    {
        public static T[] FindComponentsByType<T>(this Scene scene) where T : Component
        {
            return scene.GetRootGameObjects()
                .Select(gameObj => gameObj.GetComponent<T>()).Where(component => component != null).ToArray();
        }
        
        public static T FindComponentByType<T>(this Scene scene) where T : Component
        {
            return scene.GetRootGameObjects()
                .FirstOrDefault(gameObj => gameObj.GetComponent<T>())
                ?.gameObject.GetComponent<T>();
        }
    }
}