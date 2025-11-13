using UnityEngine;

/// <summary>
/// Управляет установкой растений и удалением (лопатой).
/// Хранит выбранный тип растения.
/// </summary>
public sealed class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [Header("Список растений в магазине")]
    [SerializeField] private PlantData[] _plantTypes;

    // Выбранное растение (-1 если ничего не выбрано)
    private int _selectedPlant = -1;

    // Активирован ли режим лопаты
    private bool _shovelMode;

    private void Awake()
    {
        // Синглтон BuildManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // ЛКМ не нажата — не обрабатываем
        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }

        // Пускаем луч из камеры в мир
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Если никуда не попали — выходим
        if (Physics.Raycast(ray, out hit, 100f) == false)
        {
            return;
        }

        // ---------------------------------------------------------
        // УДАЛЕНИЕ РАСТЕНИЯ (режим лопаты)
        // ---------------------------------------------------------
        if (_shovelMode && hit.transform.CompareTag("Tower"))
        {
            Transform tile = hit.transform.parent;

            // Включаем коллайдер плитки
            if (tile != null)
            {
                Collider col = tile.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                }
            }

            // Удаляем растение
            Destroy(hit.transform.gameObject);

            _shovelMode = false;
            return;
        }

        // ---------------------------------------------------------
        // УСТАНОВКА РАСТЕНИЯ
        // ---------------------------------------------------------
        if (_shovelMode == false && _selectedPlant >= 0 && hit.transform.CompareTag("Tile"))
        {
            PlantData data = _plantTypes[_selectedPlant];

            // Проверяем, хватает ли денег
            bool payment = GameManager.Instance.TrySpendMoney(data.cost);
            if (payment == false)
            {
                return;
            }

            // Чтобы на одну плитку нельзя было ставить два растения
            hit.collider.enabled = false;

            // Позиция растения
            Vector3 pos = hit.transform.position + Vector3.up * data.placementYOffset;

            GameObject plant = Instantiate(data.prefab, pos, Quaternion.identity);
            plant.transform.SetParent(hit.transform);

            // Присваиваем здоровье
            Unit unit = plant.GetComponent<Unit>();
            if (unit != null)
            {
                unit.Health = data.health;
            }

            // Передаём параметры TowerController
            TowerController tower = plant.GetComponent<TowerController>();
            if (tower != null)
            {
                tower.ProjectilePrefab = data.projectilePrefab;
                tower.ShootCooldown = data.shootCooldown;
                tower.ShootRange = data.shootRange;
                tower.IsIncomeGenerator = data.isIncomeGenerator;
                tower.IncomeCooldown = data.incomeCooldown;
                tower.IncomeAmount = data.incomeAmount;
            }
        }
    }

    /// <summary>
    /// Выбор растения (кнопки магазина)
    /// </summary>
    public void SelectPlant(int index)
    {
        _selectedPlant = index;
        _shovelMode = false;
    }

    /// <summary>
    /// активация лопаты
    /// </summary>
    public void SelectShovel()
    {
        _shovelMode = true;
        _selectedPlant = -1;
    }
}
