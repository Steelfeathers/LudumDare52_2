using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirebirdGames.Utilities
{
    public class GameRoot<T> : SingletonComponent<T> where T : MonoBehaviour
    {
        [SerializeField] private GameObject LoadingScreen;
      
        private void Start()
        {
            StartCoroutine(DoBootstrapLoad());
        }

        private IEnumerator DoBootstrapLoad()
        {
            ShowLoadingScreen(true);

            //Load all the managers in this scene, aka singleton component objects
            yield return StartCoroutine(DoLoadManagers());

            //Instantiate and initialize all of the code modules unique to this game
            yield return StartCoroutine(DoInitializeModules());

            ShowLoadingScreen(false);

            //Game is ready, now do the thing
            OnBoostrapLoadComplete();
        }
        
        protected virtual IEnumerator DoInitializeModules()
        {
            yield return null;
        }
        
        protected virtual void OnBoostrapLoadComplete() {}

        /// <summary>
        /// Call after all scenes have been loaded to initialize any managers in that scene
        /// </summary>
        private IEnumerator DoLoadManagers()
        {
            //Make sure all singletons gameobjects have been instantiated 
            var objectsToInit = FindObjectsOfType<MonoBehaviour>().OfType<IInitializable>().ToList();
            while (objectsToInit.Any(m => !m.IsLoaded))
            {
                yield return null;
            }
            
            //Call Init for all managers, if they aren't already initialized
            foreach (IInitializable m in objectsToInit) 
            {
                if (!m.IsReady) m.Initialize();
            }
            
            //Wait for all managers to finish initializing
            while (objectsToInit.Any(m => !m.IsReady))
            {
                yield return null;
            }
        }
        
        //---------------------------------------------------------------------------------
        //--Handle scene loading--
        //---------------------------------------------------------------------------------
        protected int curSceneIndex = -1;
        protected List<AsyncOperation> sceneLoadingOps = new List<AsyncOperation>();
        
        /// <summary>
        /// Switch the current active scene, unloading the previous one
        /// </summary>
        public void GoToScene(int sceneIndex)
        {
            StartCoroutine(DoSwitchScene(sceneIndex));
        }

        protected IEnumerator DoSwitchScene(int sceneIndex)
        {
            ShowLoadingScreen(true);

            //Unload prev scene, unless it's the persistant starting scene
            if (curSceneIndex > 0) sceneLoadingOps.Add(SceneManager.UnloadSceneAsync(curSceneIndex));
            //Load new scene, if it isn't the persistant starting scene
            if (sceneIndex > 0) sceneLoadingOps.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
            curSceneIndex = sceneIndex;
            
            //Wait for transitions to finish, then clear the asyncOps list
            while (sceneLoadingOps.Any(op => !op.isDone))
            {
                yield return null;
            }
            sceneLoadingOps.Clear();

            //Load any custom managers in this new scene
            yield return DoLoadManagers();
            
            ShowLoadingScreen(false);
            
            OnSceneSwitchDone();
        }

        protected virtual void OnSceneSwitchDone()
        {
        }

        private void ShowLoadingScreen(bool show)
        {
            if (LoadingScreen != null) LoadingScreen.SetActive(show);
        }

    }
}

