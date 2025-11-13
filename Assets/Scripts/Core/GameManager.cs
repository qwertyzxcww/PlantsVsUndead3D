using System.Collections;
using TMPro;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _heartsText;

    [Header("Waves")]
    [SerializeField] private EnemyData[] _enemyTypes;
    [SerializeField] private int[] _enemyCounts;
    [SerializeField] private GameObject[] _wavesPanels;

    [Header("End Panels")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    [Header("Gameplay Settings")]
    [SerializeField] private float _initialDelay = 3f;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private float _panelDuration = 2f;
    [SerializeField] private float _startMoney = 100f;
    [SerializeField] private int _startHearts = 3;

    private float _money;
    private int _lives;

    private int _currentWave;
    private int _spawnedInWave;
    private float _spawnTimer;
    private bool _spawning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        _money = _startMoney;
        _lives = _startHearts;

        UpdateMoneyUI();
        UpdateLivesUI();

        StartCoroutine(ShowWavePanelAndStart(0, _initialDelay));
    }

    private void Update()
    {
        if (_spawning && _currentWave < _enemyTypes.Length)
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f)
            {
                SpawnEnemy();
                _spawnedInWave++;

                if (_spawnedInWave >= _enemyCounts[_currentWave])
                {
                    _spawning = false;
                    StartCoroutine(ShowWavePanelAndStart(_currentWave + 1, _panelDuration));
                }
                else
                {
                    _spawnTimer = _spawnInterval;
                }
            }
        }

        // Победа
        if (!_spawning && _currentWave >= _enemyTypes.Length)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                _winPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    private IEnumerator ShowWavePanelAndStart(int nextWaveIndex, float delay)
    {
        GameObject panel = null;

        if (nextWaveIndex == 0)
        {
            panel = _wavesPanels[0];
        }
        else if (nextWaveIndex < _enemyTypes.Length - 1)
        {
            panel = _wavesPanels[1];
        }
        else if (nextWaveIndex == _enemyTypes.Length - 1)
        {
            panel = _wavesPanels[2];
        }

        if (panel != null)
        {
            panel.SetActive(true);
            yield return new WaitForSeconds(delay);
            panel.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        _currentWave = nextWaveIndex;
        _spawnedInWave = 0;
        _spawnTimer = _spawnInterval;
        _spawning = (_currentWave < _enemyTypes.Length);
    }

    private void SpawnEnemy()
    {
        EnemyData data = _enemyTypes[_currentWave];

        int col = Random.Range(-2, 3);
        Vector3 spawnPos = new Vector3(col, 1f, 2f); // ← ты просил Z = 2

        // ❗ ИСПРАВЛЕНИЕ ГЛАВНОЙ ПРОБЛЕМЫ:
        GameObject go = Instantiate(data.prefab, spawnPos, Quaternion.identity);

        Unit unit = go.GetComponent<Unit>();
        EnemyController ec = go.GetComponent<EnemyController>();

        if (unit != null)
        {
            unit.Health = data.health;
        }

        if (ec != null)
        {
            ec.MovementSpeed = data.movementSpeed;
            ec.Damage = data.damage;
            ec.DamageCooldown = data.damageCooldown;
            ec.Worth = data.worth;
        }
    }

    public void LoseLife()
    {
        _lives--;
        UpdateLivesUI();

        if (_lives <= 0)
        {
            Time.timeScale = 0f;
            _losePanel.SetActive(true);
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if (_money < amount)
        {
            return false;
        }

        _money -= amount;
        UpdateMoneyUI();
        return true;
    }

    public void AddMoney(float amount)
    {
        _money += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (_moneyText != null)
        {
            _moneyText.text = Mathf.FloorToInt(_money).ToString();
        }
    }

    private void UpdateLivesUI()
    {
        if (_heartsText != null)
        {
            _heartsText.text = _lives.ToString();
        }
    }
}
