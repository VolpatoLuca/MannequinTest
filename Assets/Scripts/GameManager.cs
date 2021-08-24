using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CubeSettings[] _settings;

    public static CubeSettings currentSetting;
    public static List<Transform> interactors = new List<Transform>();
    private int settingsIndex = 0;

    private void Awake()
    {
        currentSetting = _settings[settingsIndex];
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            settingsIndex++;
            if (settingsIndex >= _settings.Length) settingsIndex = 0;
            currentSetting = _settings[settingsIndex];
        }
    }
}
