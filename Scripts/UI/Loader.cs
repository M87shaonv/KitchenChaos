using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scenes
    {
        MainMenu,
        Game,
        Loading,
    }

    public static int targetSceneIndex;

    public static void Load(Scenes sceneIndex)
    {
        targetSceneIndex = (int)sceneIndex;

        SceneManager.LoadScene(Scenes.Loading.ToString());
    }

    public static void LoadCallBack()
    {
        SceneManager.LoadScene(targetSceneIndex);
    }
}