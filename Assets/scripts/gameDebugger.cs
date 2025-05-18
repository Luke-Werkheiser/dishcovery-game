#if UNITY_EDITOR
using UnityEngine;

public class gameDebugger : MonoBehaviour
{
    [Header("Frame Control")]
    public bool doFrameRateControl = false;
    public int targetFrameRate = 60;
    public float timeScale = 1f;

    [Header("Simulate Lag")]
    public bool simulateLag = false;
    public float lagTime = 0.03f; // 30ms = ~30 FPS

    [Header("Debug Options")]
    public bool showDebugLogs = false;

    private bool showUI = false;

    void Start()
    {
        ApplySettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            showUI = !showUI;

        if (simulateLag)
            System.Threading.Thread.Sleep((int)(lagTime * 1000));

        ApplySettings();
    }

    void ApplySettings()
    {
        if(doFrameRateControl){
        Application.targetFrameRate = targetFrameRate;
        Time.timeScale = timeScale;
        }
        if (showDebugLogs)
        {
            Debug.Log($"[GameDebugger] FPS: {Application.targetFrameRate}, TimeScale: {Time.timeScale}");
        }
    }

    void OnGUI()
    {
        if (!showUI) return;

        GUI.Box(new Rect(10, 10, 260, 160), "Game Debugger");

        GUILayout.BeginArea(new Rect(20, 40, 240, 140));

        GUILayout.Label("Target Frame Rate: " + targetFrameRate);
        targetFrameRate = (int)GUILayout.HorizontalSlider(targetFrameRate, 15, 300);

        GUILayout.Label("Time Scale: " + timeScale.ToString("F2"));
        timeScale = GUILayout.HorizontalSlider(timeScale, 0f, 2f);

        simulateLag = GUILayout.Toggle(simulateLag, "Simulate Lag");
        if (simulateLag)
        {
            GUILayout.Label("Lag Time: " + lagTime.ToString("F2") + "s");
            lagTime = GUILayout.HorizontalSlider(lagTime, 0f, 0.1f);
        }

        showDebugLogs = GUILayout.Toggle(showDebugLogs, "Show Debug Logs");

        GUILayout.EndArea();
    }
}
#endif
