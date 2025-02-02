using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HueSelector: MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private RawImage rawImage; // 対象のRawImage
    private Material material;
    public Material selectedColorMaterial;
    public Material svSelectorMaterial;
    private bool isSelecting = false;
    [SerializeField] private RectTransform circle;
    private float voidRadius;

    private void Awake() {
        rawImage = GetComponent<RawImage>();
        material = rawImage.material;
        Debug.Log(rawImage);
        Debug.Log(material);
    }

    private void Start() {
        voidRadius = material.GetFloat("_VoidRadius");
        SetHue(0.0f);
    }

    private Vector2 GetUVPosition(Vector2 screenPosition) 
    {
        // RawImageのRectTransformを取得
        RectTransform rectTransform = rawImage.rectTransform;

        // クリックされたスクリーン座標をローカル座標に変換
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPoint);

        // RectTransform内でのクリック座標をcenterを中心としたUV座標に変換
        Rect rect = rectTransform.rect;
        float uvX = (localPoint.x - rect.xMin) / rect.width - 0.5f;
        float uvY = (localPoint.y - rect.yMin) / rect.height - 0.5f;

        return new Vector2(uvX, uvY);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // if (rawImage == null || material == null)
        // {
        //     Debug.LogError("RawImageまたはmaterialが設定されていません！");
        //     return;
        // }

        // // RawImageのRectTransformを取得
        // RectTransform rectTransform = rawImage.rectTransform;

        // // クリックされたスクリーン座標をローカル座標に変換
        // Vector2 localPoint;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

        // // RectTransform内でのクリック座標をcenterを中心としたUV座標に変換
        // Rect rect = rectTransform.rect;
        // var center = material.GetVector("_Center");
        // float uvX = (localPoint.x - rect.xMin) / rect.width - center.x;
        // float uvY = (localPoint.y - rect.yMin) / rect.height - center.y;

        // // UV座標が範囲外なら無視
        // var dis = Mathf.Sqrt(uvX * uvX + uvY * uvY);
        // var radius = material.GetFloat("_VoidRadius");
        // if (dis < radius || dis > 0.5f)
        // {
        //     Debug.Log("クリック位置がRawImageの範囲外です");
        //     return;
        // }

        // // uv座標を角度に変換
        // float angle = Mathf.Atan2(uvY, uvX);
        // if (angle < 0) {
        //     angle += 2 * Mathf.PI;
        // }
        // angle /= 2 * Mathf.PI;

        // // float angle = (Mathf.Atan2(uvY, uvX) + Mathf.PI) / (2 * Mathf.PI);
        // selectedColorMaterial.SetFloat("_Hue", angle);
        // svSelectorMaterial.SetFloat("_Hue", angle);
        // Debug.Log($"Hue: {angle}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var uvPosition = GetUVPosition(eventData.position);
        var dis = Mathf.Sqrt(uvPosition.x * uvPosition.x + uvPosition.y * uvPosition.y);
        var radius = material.GetFloat("_VoidRadius");
        if (dis < radius || dis > 0.5f)
        {
            return;
        }
        isSelecting = true;
        StartCoroutine(SelectHueCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelecting = false;
    }

    private IEnumerator SelectHueCoroutine()
    {
        while (true)
        {
            if (isSelecting)
            {
                var mousePosition = Input.mousePosition;
                var uvPosition = GetUVPosition(mousePosition);
                var dis = Mathf.Sqrt(uvPosition.x * uvPosition.x + uvPosition.y * uvPosition.y);
                if (dis < voidRadius * 0.7 || dis > 0.5f * 1.3)
                {
                    yield return null;
                    continue;
                }

                float angle = Mathf.Atan2(uvPosition.y, uvPosition.x);
                if (angle < 0) {
                    angle += 2 * Mathf.PI;
                }
                angle /= 2 * Mathf.PI;

                SetHue(angle);
            } else {
                yield return null;
            }
            yield return null;
        }
    }

    // hueは0~1の値
    private void SetHue(float hue)
    {
        // shaderにセット
        selectedColorMaterial.SetFloat("_Hue", hue);
        svSelectorMaterial.SetFloat("_Hue", hue);

        // circleの位置を更新
        Vector2 circleUVPosition = new Vector2(
            (voidRadius + 0.5f) / 2 * Mathf.Cos(hue * 2 * Mathf.PI),
            (voidRadius + 0.5f) / 2 * Mathf.Sin(hue * 2 * Mathf.PI)
        );
        circle.anchoredPosition = new Vector2(
            circleUVPosition.x * rawImage.rectTransform.rect.width,
            circleUVPosition.y * rawImage.rectTransform.rect.height
        );
    }
}
