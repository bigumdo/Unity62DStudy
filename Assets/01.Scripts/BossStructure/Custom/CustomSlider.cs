using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomSlider : VisualElement, INotifyValueChanged<float>
{
    private VisualElement _tracker;
    private VisualElement _progress;
    private VisualElement _dragger;

    private bool _isDragging = false;
    private float _normalizedValue = 0f;

    private float _lowValue = 0f;
    private float _highValue = 100f;
    private float _rawValue = 0f; // 사용자가 설정한 실제 값

    [UxmlAttribute("low-value")]
    public float LowValue
    {
        get => _lowValue;
        set
        {
            _lowValue = value;
            RecalculateFromRawValue();
        }
    }

    [UxmlAttribute("high-value")]
    public float HighValue
    {
        get => _highValue;
        set
        {
            _highValue = value;
            RecalculateFromRawValue();
        }
    }

    [UxmlAttribute("value")]
    public float value
    {
        get => Mathf.Lerp(_lowValue, _highValue, _normalizedValue);
        set
        {
            float oldValue = this.value;
            float newNormalized = Mathf.InverseLerp(_lowValue, _highValue, value);
            newNormalized = Mathf.Clamp01(newNormalized);

            if (!Mathf.Approximately(oldValue, value))
            {
                _rawValue = value;
                _normalizedValue = newNormalized;

                Debug.Log($"[CustomSlider] value changed: {oldValue} → {value}");

                UpdateVisuals();

                using var evt = ChangeEvent<float>.GetPooled(oldValue, value);
                evt.target = this;
                SendEvent(evt);
            }
        }
    }



    private void RecalculateFromRawValue()
    {
        if (_lowValue == _highValue) return;

        _normalizedValue = Mathf.InverseLerp(_lowValue, _highValue, _rawValue);
        _normalizedValue = Mathf.Clamp01(_normalizedValue);

        UpdateVisuals();
    }

    public CustomSlider()
    {
        style.flexGrow = 1;
        style.flexDirection = FlexDirection.Row;

        _tracker = new VisualElement { name = "tracker" };
        _tracker.AddToClassList("tracker");
        hierarchy.Add(_tracker);

        _progress = new VisualElement { name = "progress" };
        _progress.AddToClassList("progress");
        _tracker.Add(_progress);

        _dragger = new VisualElement { name = "dragger" };
        _dragger.AddToClassList("dragger");
        _tracker.Add(_dragger);

        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<PointerMoveEvent>(OnPointerMove);
        RegisterCallback<PointerUpEvent>(OnPointerUp);

        RegisterCallback<GeometryChangedEvent>(_ => UpdateVisuals());
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        _isDragging = true;
        SetValueFromPosition(evt.localPosition.x);
        this.CapturePointer(evt.pointerId);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (_isDragging)
            SetValueFromPosition(evt.localPosition.x);
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        _isDragging = false;
        this.ReleasePointer(evt.pointerId);
    }

    private void SetValueFromPosition(float x)
    {
        float width = contentRect.width;
        _normalizedValue = Mathf.Clamp01(x / width);
        _rawValue = Mathf.Lerp(_lowValue, _highValue, _normalizedValue); // 동기화
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        float width = contentRect.width;
        float posX = Mathf.Round(width * _normalizedValue);

        _progress.style.width = new Length(posX, LengthUnit.Pixel);
        _dragger.style.left = new Length(posX - (_dragger.resolvedStyle.width / 2), LengthUnit.Pixel);


    }

    public void SetValueWithoutNotify(float newValue)
    {
        _rawValue = newValue;
        RecalculateFromRawValue(); // normalizedValue 계산 + UpdateVisuals()
    }
}
