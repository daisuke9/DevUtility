using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DevUtilityWindow : EditorWindow
{
    [MenuItem("Window/DevUtility")]
    public static void ShowWindow()
    {
        var window = GetWindow<DevUtilityWindow>();
        window.titleContent = new GUIContent("DevUtility");
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
    }
    
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
    }
    
    
    private void OnPlaymodeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode ||
            state == PlayModeStateChange.ExitingEditMode)
        {
            Time.timeScale = 1f;
            UpdateTimeScaleValue();
        }
    }


    private FloatField _timeScaleField;
    private Slider _timeScaleSlider;


    public void CreateGUI()
    {
        Debug.Log($"Current Time.timeScale: {Time.timeScale}");

        
        // UXML読み込み
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DevUtility.uxml");
        visualTree.CloneTree(rootVisualElement);

        // タイムスケール要素
        _timeScaleField = rootVisualElement.Q<FloatField>("timeScaleField");
        _timeScaleSlider = rootVisualElement.Q<Slider>("timeScaleSlider");
        var timeScaleResetButton = rootVisualElement.Q<Button>("timeScaleResetButton");
        
        _timeScaleField.SetValueWithoutNotify(Time.timeScale);
        _timeScaleSlider.SetValueWithoutNotify(Time.timeScale);
        
        // フィールド
        _timeScaleField.RegisterValueChangedCallback(evt =>
        {
            Time.timeScale = evt.newValue;
            UpdateTimeScaleValue();
        });
        
        // スライダー
        _timeScaleSlider.RegisterValueChangedCallback(evt =>
        {
            Time.timeScale = RoundedTimeScale(evt.newValue);
            UpdateTimeScaleValue();
        });

        // リセットボタン
        timeScaleResetButton.clicked += () =>
        {
            Time.timeScale = 1f;
            UpdateTimeScaleValue();
        };
    }
    
    private void UpdateTimeScaleValue()
    {
        _timeScaleField?.SetValueWithoutNotify(Time.timeScale);
        _timeScaleSlider?.SetValueWithoutNotify(Time.timeScale);
    }

    private float RoundedTimeScale(float value) => Mathf.Round(value * 10f) / 10f;
}
