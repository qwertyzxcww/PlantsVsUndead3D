using UnityEngine;

/// <summary>
/// Управляет движением врага и атакой растений.
/// </summary>
public sealed class EnemyController : MonoBehaviour
{
    [Header("Параметры врага")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _damage;
    [SerializeField] private float _damageCooldown;
    [SerializeField] private float _worth;

    private float _damageTimer;
    private Unit _unit;

    public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
    public float Damage { get => _damage; set => _damage = value; }
    public float DamageCooldown { get => _damageCooldown; set => _damageCooldown = value; }
    public float Worth { get => _worth; set => _worth = value; }

    private void Start()
    {
        _unit = GetComponent<Unit>();
        gameObject.tag = "Enemy";
    }

    private void Update()
    {
        _damageTimer -= Time.deltaTime;

        // Проверяем коллизию перед собой
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 0.6f))
        {
            // Дошёл до конца поля
            if (hit.transform.CompareTag("TileGoal"))
            {
                GameManager.Instance.LoseLife();
                Destroy(gameObject);
                return;
            }

            // Атакуем растение
            if (hit.transform.CompareTag("Tower"))
            {
                if (_damageTimer <= 0f)
                {
                    Unit towerUnit = hit.transform.GetComponent<Unit>();
                    if (towerUnit != null)
                    {
                        towerUnit.TakeDamage(_damage);
                    }

                    _damageTimer = _damageCooldown;
                }

                return;
            }
        }

        // Движение вперёд
        transform.Translate(Vector3.back * (_movementSpeed * Time.deltaTime), Space.World);
    }

    private void OnDestroy()
    {
        // Выдаём деньги игроку, если враг умер
        if (_unit != null && _unit.Health <= 0f)
        {
            GameManager.Instance.AddMoney(_worth);
        }
    }
}
