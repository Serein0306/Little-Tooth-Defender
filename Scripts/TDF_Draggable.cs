using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 通用拖拽组件：把 RectTransform 跟随手指/鼠标移动，并回报画布坐标。
/// 用于故事 2 的牙刷和故事 4 的牙线。
/// </summary>
public class TDF_Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform canvasRT;
    public System.Action<Vector2> onDrag; // 画布锚点坐标（以画布中心为原点）
    public System.Action onEnd;

    RectTransform _rt;

    void Awake()
    {
        _rt = (RectTransform)transform;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        Vector2 local;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, e.position, e.pressEventCamera, out local))
            _rt.anchoredPosition = local;
    }

    public void OnDrag(PointerEventData e)
    {
        Vector2 local;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, e.position, e.pressEventCamera, out local))
        {
            _rt.anchoredPosition = local;
            if (onDrag != null) onDrag(local);
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (onEnd != null) onEnd();
    }
}
