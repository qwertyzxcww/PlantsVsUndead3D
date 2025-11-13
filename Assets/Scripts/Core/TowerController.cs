using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _shootCooldown;
    [SerializeField] private float _shootRange;

    [SerializeField] private bool _isIncomeGenerator;
    [SerializeField] private float _incomeCooldown;
    [SerializeField] private float _incomeAmount;

    private float _shootTimer;
    private float _incomeTimer;

    public GameObject ProjectilePrefab
    {
        get { return _projectilePrefab; }
        set { _projectilePrefab = value; }
    }

    public float ShootCooldown
    {
        get { return _shootCooldown; }
        set { _shootCooldown = value; }
    }

    public float ShootRange
    {
        get { return _shootRange; }
        set { _shootRange = value; }
    }

    public bool IsIncomeGenerator
    {
        get { return _isIncomeGenerator; }
        set { _isIncomeGenerator = value; }
    }

    public float IncomeCooldown
    {
        get { return _incomeCooldown; }
        set { _incomeCooldown = value; }
    }

    public float IncomeAmount
    {
        get { return _incomeAmount; }
        set { _incomeAmount = value; }
    }

    private void Start()
    {
        _incomeTimer = _incomeCooldown;
        gameObject.tag = "Tower";
    }

    private void Update()
    {
        _shootTimer -= Time.deltaTime;
        _incomeTimer -= Time.deltaTime;

        TryShoot();
        TryIncome();
    }

    private void TryShoot()
    {
        if (_projectilePrefab == null)
        {
            return;
        }

        if (_shootTimer > 0f)
        {
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = transform.forward;

        // Ищем ВСЕ попадания по лучу вперёд
        RaycastHit[] hits = Physics.RaycastAll(origin, dir, _shootRange);

        if (hits.Length == 0)
        {
            // никого нет по лучу – не стреляем
            return;
        }

        float closestEnemyDistance = float.MaxValue;
        bool hasEnemy = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform hitTransform = hits[i].transform;

            if (!hitTransform.CompareTag("Enemy"))
            {
                continue;
            }

            // нашли врага на луче
            float distance = hits[i].distance;

            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                hasEnemy = true;
            }
        }

        // если по лучу нет ни одного Enemy – не стреляем
        if (!hasEnemy)
        {
            return;
        }

        // если враг есть в радиусе – стреляем (через растения/тайлы и т.п.)
        Instantiate(_projectilePrefab, origin, Quaternion.LookRotation(-dir));
        _shootTimer = _shootCooldown;
    }

    private void TryIncome()
    {
        if (!_isIncomeGenerator)
        {
            return;
        }

        if (_incomeTimer <= 0f)
        {
            GameManager.Instance.AddMoney(_incomeAmount);
            _incomeTimer = _incomeCooldown;
        }
    }
}
