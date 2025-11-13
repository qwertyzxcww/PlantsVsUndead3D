using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Простая загрузка сцен.
/// Сбрасывает Time.timeScale, чтобы не зависнуть после победы.
/// </summary>
public sealed class Loader : MonoBehaviour
{
    public void LoadScene(int index)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
    }
}