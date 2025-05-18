using UnityEngine;
using TMPro;

public class DebugUIMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject debugPanel;          // Panel to toggle
    public TextMeshProUGUI fpsText;        // Text to show FPS

    private bool isVisible = false;
    private float deltaTime = 0f;

    void Update()
    {
        // Toggle debug panel visibility with F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isVisible = !isVisible;
            if (debugPanel != null)
                debugPanel.SetActive(isVisible);
        }

        // FPS calculation
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        if (isVisible && fpsText != null)
        {
            fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
        }
    }
}
