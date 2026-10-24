using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Эта функция будет вызываться при нажатии на кнопку
    public void Quit()
    {
        Debug.Log("Игра закрывается..."); // Сообщение в консоль для проверки
        
        #if UNITY_EDITOR
        // Если мы в редакторе Unity, останавливаем режим Play
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Если игра собрана (билд), закрываем приложение
        Application.Quit();
        #endif
    }
}