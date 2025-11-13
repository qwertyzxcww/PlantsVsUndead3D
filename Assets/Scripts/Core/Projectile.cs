using UnityEngine;

/// <summary>
/// Снаряд, летящий вперёд. Наносит урон врагам.
/// </summary>
public sealed class Projectile : MonoBehaviour
{
    [Header("Параметры снаряда")]
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _maxDistance = 6f;

    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        // Двигаем снаряд в локальном forward-направлении
        transform.Translate(Vector3.back * (_speed * Time.deltaTime), Space.Self);

        // Уничтожаем если пролетел слишком далеко
        float distance = Vector3.Distance(_startPos, transform.position);
        if (distance > _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Урон наносим только врагам
        if (other.CompareTag("Enemy") == false)
        {
            return;
        }

        Unit unit = other.GetComponent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}