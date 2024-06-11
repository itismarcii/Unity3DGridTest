using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneTransitionManager : MonoBehaviour
    {
        [field: SerializeField] public SceneHolderScriptableObject SceneHolder { get; private set; }

        public Action OnSceneTransitionFinish;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        public void TransitionScene(in string scene)
        {
            SceneManager.LoadScene(scene);
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            OnSceneTransitionFinish?.Invoke();
        }
    }
}
