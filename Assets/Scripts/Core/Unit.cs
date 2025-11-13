using UnityEngine;

/// <summary>
/// Базовый юнит (растения, враги). Хранит здоровье.
/// </summary>
public sealed class Unit : MonoBehaviour
{
    // Текущее здоровье юнита
    public float Health;

    /// <summary>
    /// Получение урона
    /// </summary>
    public void TakeDamage(float amount)
    {
        Health -= amount;

        if (Health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Уничтожение объекта + освобождение плитки, если это растение
    /// </summary>
    private void Die()
    {
        if (transform.parent != null && transform.parent.CompareTag("Tile"))
        {
            Collider col = transform.parent.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
        }

        Destroy(gameObject);
    }
}