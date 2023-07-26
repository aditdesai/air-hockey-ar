using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    string sceneNameToBeLoaded;
    public void LoadScene(string sceneName)
    {
        sceneNameToBeLoaded = sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        // Load the loading scene
        yield return SceneManager.LoadSceneAsync("LoadingScene");

        // Start loading of actual scene
        StartCoroutine(LoadActualScene());
    }

    IEnumerator LoadActualScene()
    {
        var asyncSceneLoading = SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        // scene not visible yet
        asyncSceneLoading.allowSceneActivation = false;

        while (!asyncSceneLoading.isDone)
        {
            if (asyncSceneLoading.progress >= 0.9f)
            {
                // Finally, show the scene
                asyncSceneLoading.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
