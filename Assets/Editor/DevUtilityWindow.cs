using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DevUtilityWindow : EditorWindow
{
    private int _defaultTargetFrameRate;
    private float _deltaTime;

    private Label _fpsLabel;
    private Label _targetFrameRateLabel;
    private FloatField _timeScaleField;
    private Slider _timeScaleSlider;

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        EditorApplication.update += UpdateFps;

        _defaultTargetFrameRate = Application.targetFrameRate;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
        EditorApplication.update -= UpdateFps;

        Application.targetFrameRate = _defaultTargetFrameRate;
    }

    public void CreateGUI()
    {
        // UXML読み込み
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DevUtility.uxml");
        visualTree.CloneTree(rootVisualElement);

        InitializeTimeScaleSection();
        InitializePerformanceSection();
    }

    private void OnInspectorUpdate()
    {
        if (_targetFrameRateLabel != null)
        {
            _targetFrameRateLabel.text = $"Target Frame Rate: {Application.targetFrameRate}";
        }
    }

    [MenuItem("Window/DevUtility")]
    public static void ShowWindow()
    {
        var window = GetWindow<DevUtilityWindow>();
        window.titleContent = new GUIContent("DevUtility");
    }

    private void OnPlaymodeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode ||
            state == PlayModeStateChange.ExitingEditMode)
        {
            Time.timeScale = 1f;
            UpdateTimeScaleValue();

            Application.targetFrameRate = _defaultTargetFrameRate;
        }
    }

    // タイムスケールセクションの初期化
    private void InitializeTimeScaleSection()
    {
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

    private float RoundedTimeScale(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }


    private void InitializePerformanceSection()
    {
        // パフォーマンス要素
        _fpsLabel = rootVisualElement.Q<Label>("fpsLabel");

        _targetFrameRateLabel = rootVisualElement.Q<Label>("targetFrameRateLabel");
        var targetFrameRate5Button = rootVisualElement.Q<Button>("targetFrameRate5Button");
        var targetFrameRate30Button = rootVisualElement.Q<Button>("targetFrameRate30Button");
        var targetFrameRate60Button = rootVisualElement.Q<Button>("targetFrameRate60Button");
        var targetFrameRate120Button = rootVisualElement.Q<Button>("targetFrameRate120Button");
        var targetFrameRateUnlimitedButton = rootVisualElement.Q<Button>("targetFrameRateUnlimitedButton");

        targetFrameRate5Button.clicked += () =>
        {
            Application.targetFrameRate = 5;
            UpdateTargetFrameRateLabel();
        };
        targetFrameRate30Button.clicked += () =>
        {
            Application.targetFrameRate = 30;
            UpdateTargetFrameRateLabel();
        };
        targetFrameRate60Button.clicked += () =>
        {
            Application.targetFrameRate = 60;
            UpdateTargetFrameRateLabel();
        };
        targetFrameRate120Button.clicked += () =>
        {
            Application.targetFrameRate = 120;
            UpdateTargetFrameRateLabel();
        };
        targetFrameRateUnlimitedButton.clicked += () =>
        {
            Application.targetFrameRate = -1;
            UpdateTargetFrameRateLabel();
        };

        UpdateTargetFrameRateLabel();
    }

    private void UpdateFps()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_fpsLabel == null)
        {
            return;
        }

        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        var fps = 1f / _deltaTime;
        _fpsLabel.text = $"FPS: {fps:F1}";
    }

    private void UpdateTargetFrameRateLabel()
    {
        if (_targetFrameRateLabel == null)
        {
            return;
        }

        _targetFrameRateLabel.text = $"Target Frame Rate: {Application.targetFrameRate}";
    }
}
