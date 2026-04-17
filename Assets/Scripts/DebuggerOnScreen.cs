using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DebuggerOnScreen : MonoBehaviour
{
    public TextMeshProUGUI errorText;
    public float messageLifetime = 5f;

    private List<string> messages = new List<string>();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            AddMessage(logString);
        }
    }

    void AddMessage(string message)
    {
        messages.Add(message);
        UpdateUI();

        StartCoroutine(RemoveMessageAfterDelay(message, messageLifetime));
    }

    IEnumerator RemoveMessageAfterDelay(string message, float delay)
    {
        yield return new WaitForSeconds(delay);

        messages.Remove(message);
        UpdateUI();
    }

    void UpdateUI()
    {
        errorText.text = string.Join("\n", messages);
    }
}