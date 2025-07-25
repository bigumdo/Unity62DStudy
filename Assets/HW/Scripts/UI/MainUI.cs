using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using YUI;
using YUI.Cores;

public class MainUI : ToolkitUI
{
    private VisualElement _root;

    private Label _title;
    private Label _start;
    private Label _setting;
    private Label _exit;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.AddUI(this);

        if (visualTreeAsset != null)
        {
            _root = visualTreeAsset.CloneTree();
            _root.style.flexGrow = 1;
        }
        else
            Debug.LogError("Main Asset이 설정되지 않았습니다.");

    }
    private void OnEnable()
    {
        Open();
    }

    public override void Close()
    {
        _title.AddToClassList("title-disappear");
        _start.AddToClassList("disappear");
        _setting.AddToClassList("disappear");
        _exit.AddToClassList("disappear");
    }

    public override void Open()
    {
        if (_root != null)
        {
            root.Q("container").Add(_root);

            // VisualElements를 가져오기
            _title = _root.Q<Label>("title");
            _start = _root.Q<Label>("start");
            _setting = _root.Q<Label>("setting");
            _exit = _root.Q<Label>("exit");

            // 클릭 이벤트 등록
            _start?.RegisterCallback<ClickEvent>(evt =>
            {
                Close();

                UIManager.Instance.GetUI<FadeUI>().Open();

                UIManager.Instance.Fade("TitleToTuto");

            });

            _setting?.RegisterCallback<ClickEvent>(evt =>
            {
                Close();
                UIManager.Instance.ShowUI<SettingUI>();
            });

            _exit?.RegisterCallback<ClickEvent>(evt =>
            {
                Application.Quit();
            });

        }

        StartCoroutine(AddClass());
    }

    private IEnumerator AddClass()
    {
        yield return new WaitForSeconds(.1f);

        _title?.RemoveFromClassList("title-disappear");
        _start.RemoveFromClassList("disappear");
        _setting.RemoveFromClassList("disappear");
        _exit.RemoveFromClassList("disappear");
    }


}
