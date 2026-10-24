using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Укажите имя сцены, на которую нужно перейти (без расширения .unity)
    public string sceneToLoad = "game";

    // Вызовите этот метод, например, при нажатии на кнопку
    public void LoadTargetScene()
    {
        // LoadSceneMode.Single закрывает текущую сцену и загружает новую
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}