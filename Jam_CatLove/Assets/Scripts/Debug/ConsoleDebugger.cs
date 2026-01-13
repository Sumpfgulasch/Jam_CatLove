using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ConsoleDebugger : MonoBehaviour
{
    private List<string> logs = new List<string>();
    private bool showConsole = false;
    private Vector2 scrollPosition;
    private int maxLogs = 50;
    private InputSystem_Actions inputActions;
    
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.ToggleConsole.started += ToggleConsole;
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.ToggleConsole.started -= ToggleConsole;
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"[{type}] {logString}";
        logs.Add(logEntry);
        
        if (logs.Count > maxLogs)
        {
            logs.RemoveAt(0);
        }
    }
    
    private void ToggleConsole(InputAction.CallbackContext context)
    {
        showConsole = !showConsole;
    }

    private void Update()
    {
        
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        // Console background
        float consoleHeight = Screen.height * 0.05f;
        GUI.Box(new Rect(0, 0, Screen.width*0.3f, consoleHeight), "");

        // Scrollable log area
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, consoleHeight - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (string log in logs)
        {
            GUILayout.Label(log);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}