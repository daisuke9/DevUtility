using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DevUtilityWindow : EditorWindow
{
    private FloatField _timeScaleField;
    private Slider _timeScaleSlider;

    private Label _fpsLabel;
    private float deltaTime = 0f;


    
    [MenuItem("Window/DevUtility")]
    public static void ShowWindow()
    {
        var window = GetWindow<DevUtilityWindow>();
        window.titleContent = new GUIContent("DevUtility");
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        EditorApplication.update += UpdateFps;
    }
    
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
        EditorApplication.update -= UpdateFps;
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
        
        // パフォーマンス要素
        _fpsLabel = rootVisualElement.Q<Label>("fpsLabel");

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
    
    
    private void UpdateFps()
    {
        if (!Application.isPlaying) { return; }
        if (_fpsLabel == null) { return; }
        
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        var fps = 1f / deltaTime;
        _fpsLabel.text = $"FPS: {fps:F1}";
    }
    
}
