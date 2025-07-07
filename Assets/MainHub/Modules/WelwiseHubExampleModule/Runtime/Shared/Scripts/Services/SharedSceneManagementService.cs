using System;
using System.Linq;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.SceneManagement;
using WelwiseHubExampleModule.Runtime.Shared.Scripts.Holders;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts.Services
{
    public class SharedSceneManagementService
    {
        public event Action<Scene> SceneLoaded;
        private readonly FishNet.Managing.Scened.SceneManager _sceneManager;
        private readonly ClientManager _clientManager;

        public SharedSceneManagementService(FishNet.Managing.Scened.SceneManager sceneManager, ClientManager clientManager)
        {
            _sceneManager = sceneManager;
            _clientManager = clientManager;
            //InstanceFinder.TimeManager.OnPostPhysicsSimulation += SimulatePhysicsForAllScenes;

            sceneManager.OnLoadEnd += OnLoadScene;
        }

        private void OnLoadScene(SceneLoadEndEventArgs args)
        {
            var initialScene = SceneManager.GetSceneByName(ScenesNames.Initial);

            var loadedScene = args.LoadedScenes.FirstOrDefault();

            if (loadedScene.IsValid())
            {
                SceneManager.SetActiveScene(args.LoadedScenes.FirstOrDefault());
                SceneLoaded?.Invoke(loadedScene);
            }

            if (!initialScene.isLoaded) return;

            if (_clientManager.Started && loadedScene.IsValid())
                initialScene.GetRootGameObjects().ForEach(gameObject => SceneManager.MoveGameObjectToScene(gameObject, loadedScene));
            
            SceneManager.UnloadSceneAsync(ScenesNames.Initial);
        }

        private static void SimulatePhysicsForAllScenes(float step) =>
            Enumerable.Range(0, SceneManager.sceneCount)
                .Select(SceneManager.GetSceneAt).Where(scene => scene.IsValid() && scene.name != ScenesNames.Initial)
                .ForEach(
                    scene => scene.GetPhysicsScene().Simulate(step));
    }
}