using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YUI;
using YUI.Cores;

public class SettingUI : ToolkitUI
{
    private VisualElement _root;
    private VisualElement _settingRoot;
    private VisualElement _fade;
    private List<(Button, Action)> _btnActions = new List<(Button, Action)>();
    private List<(Label, Action)> _sliderClickActions = new List<(Label, Action)>();
    private List<(Slider, Action<ChangeEvent<float>>)> _sliderActions = new List<(Slider, Action<ChangeEvent<float>>)>();
    private Button _exitBtn;
    private Label _exit;
    public Slider _master;
    public Slider _bgm;
    public Slider _sfx;
    private VisualElement _masterProgress;
    private VisualElement _bgmProgress;
    private VisualElement _sfxProgress;
    private VisualElement _settingWindow;

    private DropdownField _resolutionDropdown;
    private readonly List<Vector2Int> _supportedResolutions = new List<Vector2Int> { new Vector2Int(1920, 1080), new Vector2Int(1280, 720), new Vector2Int(1600, 900) };

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.AddUI(this);

        if (visualTreeAsset != null)
        {
            _root = visualTreeAsset.CloneTree();
            _root.style.flexGrow = 1;
        }
    }

    private void OnEnable()
    {

    }

    public void InitUI()
    {
        InitElements();
        RegisterBtnEvents();
        RegisterSliderEvents();
        RegisterResolutionDropdown();

        UpdateProgress(_masterProgress, _master);
        UpdateProgress(_bgmProgress, _bgm);
        UpdateProgress(_sfxProgress, _sfx);
    }

    private void InitElements()
    {
        _settingRoot = _root.Q("setting-container");
        _fade = _settingRoot.Q("fade-pannel");

        _exitBtn = _settingRoot.Q<Button>("exit-btn");
        _exit = _settingRoot.Q<Label>("exit");

        _master = _settingRoot.Q<Slider>("master-slider");
        _bgm = _settingRoot.Q<Slider>("bgm-slider");
        _sfx = _settingRoot.Q<Slider>("sfx-slider");

        _masterProgress = new VisualElement();
        _masterProgress.name = "master-progress";
        _masterProgress.AddToClassList("progress");
        _master.Q("unity-tracker")?.Add(_masterProgress);

        _bgmProgress = new VisualElement();
        _bgmProgress.name = "bgm-progress";
        _bgmProgress.AddToClassList("progress");
        _bgm.Q("unity-tracker")?.Add(_bgmProgress);

        _sfxProgress = new VisualElement();
        _sfxProgress.name = "sfx-progress";
        _sfxProgress.AddToClassList("progress");
        _sfx.Q("unity-tracker")?.Add(_sfxProgress);

        _resolutionDropdown = _settingRoot.Q<DropdownField>("resolution-dropdown");
        _settingWindow = _root.Q("window");
    }

    #region AddActions
    private void AddSliderValueChanged(Slider master, Action<ChangeEvent<float>> action)
        => _sliderActions.Add((master, action));
    private void AddButtonAction(Button btn, Action action)
        => _btnActions.Add((btn, action));
    private void AddLabelAction(Label exit, Action onExitLabelClickEvent)
    => _sliderClickActions.Add((exit, onExitLabelClickEvent));

    #endregion

    #region InitEvents

    private void RegisterBtnEvents()
    {
        AddButtonAction(_exitBtn, OnExitBtnClickEvent);
        AddLabelAction(_exit, OnExitLabelClickEvent);

        foreach ((Button, Action) item in _btnActions)
            item.Item1.clicked += item.Item2;
    }
    private void OnExitLabelClickEvent()
    {
        Debug.Log("나가기");
        Application.Quit();
    }


    private void RegisterSliderEvents()
    {
        AddSliderValueChanged(_master, OnMasterSliderValueChangeEvent);
        AddSliderValueChanged(_bgm, OnBgmSliderValueChangeEvent);
        AddSliderValueChanged(_sfx, OnSfxSliderValueChangeEvent);

        foreach ((Slider, Action<ChangeEvent<float>>) item in _sliderActions)
            item.Item1.RegisterValueChangedCallback(evt =>
            {
                item.Item2(evt);
            });

        foreach((Label, Action) iten in _sliderClickActions)
        {
            iten.Item1.RegisterCallback<ClickEvent>(evt =>
            {
                iten.Item2();
            });
        }
    }

    private void RegisterResolutionDropdown()
    {
        if (_resolutionDropdown == null)
            return;

        List<string> options = new List<string>();
        foreach (var res in _supportedResolutions)
            options.Add($"{res.x} x {res.y}");

        _resolutionDropdown.choices = options;
        _resolutionDropdown.index = 0;
        _resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            Vector2Int selectedRes = _supportedResolutions[_resolutionDropdown.index];
            Screen.SetResolution(selectedRes.x, selectedRes.y, true);
            Debug.Log($"Resolution changed to {selectedRes.x} x {selectedRes.y}");
        });
    }

    #endregion

    #region SliderValueChangedEvents
    private void OnMasterSliderValueChangeEvent(ChangeEvent<float> evt)
    {
        Debug.Log("Master Slider의 값이 변경 되었습니다.");
        UpdateProgress(_masterProgress, _master);
    }
    private void OnSfxSliderValueChangeEvent(ChangeEvent<float> evt)
    {
        Debug.Log("Sfx Slider의 값이 변경 되었습니다.");
        UpdateProgress(_sfxProgress, _sfx);
    }
    private void OnBgmSliderValueChangeEvent(ChangeEvent<float> evt)
    {
        Debug.Log("Bgm Slider의 값이 변경 되었습니다.");
        UpdateProgress(_bgmProgress, _bgm);
    }
    #endregion

    #region ButtonClickedEvents

    private void OnExitBtnClickEvent()
    {
        StartCoroutine(ExitRoutine());

    }

    #endregion

    public override void Close()
    {
        _settingWindow.RemoveFromClassList("appear");
        _fade.RemoveFromClassList("fade");
    }

    public override void Open()
    {
        if (_root != null)
        {
            root.Q("container").Add(_root);

            _root.style.position = Position.Absolute;
            _root.style.width = new Length(100, LengthUnit.Percent);
            _root.style.height = new Length(100, LengthUnit.Percent);

            InitUI();

            SoundManager.Instance.InitSliders();
        }

        StartCoroutine(AddClass());
    }

    private IEnumerator AddClass()
    {
        _settingWindow.RemoveFromClassList("appear");
        _fade.RemoveFromClassList("fade");
        yield return new WaitForSeconds(0.2f);
        _settingWindow.AddToClassList("appear");
        _fade.AddToClassList("fade");

        UpdateProgress(_masterProgress, _master);
        UpdateProgress(_sfxProgress, _sfx);
        UpdateProgress(_bgmProgress, _bgm);
    }

    private IEnumerator ExitRoutine()
    {
        Close();
        UIManager.Instance.ShowUI<MainUI>();
        yield return new WaitForSeconds(0.5f);
    }

    private void UpdateProgress(VisualElement progress, Slider slider)
    {
        float percent = Mathf.InverseLerp(slider.lowValue, slider.highValue, slider.value);

        VisualElement dragcontainer = slider.Q("unity-tracker");
        float width = dragcontainer.resolvedStyle.width;
        float posX = Mathf.Round(width * percent);

        Debug.Log(posX);

        progress.style.width = new Length(posX, LengthUnit.Pixel);
    }
}
