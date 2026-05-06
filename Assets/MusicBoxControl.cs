using UnityEngine;

public class MusicBoxControl : MonoBehaviour
{
    [Header("Настройки механизма")]
    public Transform mechanism;
    public float rotationSpeed = 100f;
    private bool isRunning = false;

    [Header("Настройки крышки")]
    public Transform lidHinge;
    public float openAngle = -110f;
    private bool isLidOpen = false;
    private float currentLidAngle = 0f;

    [Header("Камеры")]
    public GameObject mainCamera;
    public GameObject insideCamera;

    [Header("Звук")]
    public AudioSource musicSource;

    [Header("Статистика (Счетчик оборотов)")]
    public float totalDegreesTurned = 0f; // Всего пройдено градусов за запуск
    public int fullRevolutions = 0;       // Количество полных оборотов (по 360 градусов)

    void Update()
    {
        // 1. Управление запуском (Пробел)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRunning = !isRunning; // Переключаем состояние (Вкл/Выкл)

            if (isRunning)
            {
                // МЕХАНИЗМ ЗАПУЩЕН
                totalDegreesTurned = 0f; // Обнуляем счетчик при новом старте
                fullRevolutions = 0;
                Debug.Log("Запуск! Счетчик оборотов сброшен.");

                if (musicSource != null) musicSource.Play();
            }
            else
            {
                // МЕХАНИЗМ ОСТАНОВЛЕН
                Debug.Log($"Остановка! Сделано полных оборотов: {fullRevolutions}");

                if (musicSource != null) musicSource.Pause();
            }
        }

        // 2. ОТКРЫТЬ КРЫШКУ И ВКЛЮЧИТЬ ВИД СВЕРХУ (L или ПКМ)
        if (Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1))
        {
            isLidOpen = !isLidOpen;

            if (mainCamera != null && insideCamera != null)
            {
                mainCamera.SetActive(!isLidOpen);
                insideCamera.SetActive(isLidOpen);
            }
        }

        // 3. Работа механизма и ПОДСЧЕТ ОБОРОТОВ
        if (isRunning && mechanism != null)
        {
            // Вычисляем, на сколько градусов мы поворачиваемся в этом конкретном кадре
            float step = rotationSpeed * Time.deltaTime;

            // Крутим механизм
            mechanism.Rotate(Vector3.right * step);

            // Добавляем этот шаг к нашему счетчику пройденных градусов
            // Используем Mathf.Abs, чтобы считалось правильно, даже если барабан крутится в обратную сторону (скорость с минусом)
            totalDegreesTurned += Mathf.Abs(step);

            // Вычисляем полные обороты: делим все градусы на 360 и отбрасываем остаток
            fullRevolutions = Mathf.FloorToInt(totalDegreesTurned / 360f);

            // Настройка звука
            if (musicSource != null)
            {
                musicSource.pitch = Mathf.Abs(rotationSpeed / 100f);
            }
        }

        // 4. Плавное движение крышки
        if (lidHinge != null)
        {
            float target = isLidOpen ? openAngle : 0f;
            currentLidAngle = Mathf.MoveTowards(currentLidAngle, target, 120f * Time.deltaTime);
            lidHinge.localRotation = Quaternion.Euler(currentLidAngle, 0, 0);
        }

        // 5. Изменение скорости (Стрелки)
        if (Input.GetKey(KeyCode.UpArrow)) rotationSpeed += 5f;
        if (Input.GetKey(KeyCode.DownArrow)) rotationSpeed -= 5f;
    }
}