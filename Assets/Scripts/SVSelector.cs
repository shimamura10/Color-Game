using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SVSelector: MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private RawImage rawImage; // 対象のRawImage
    private Material material;
    public Material selectedColorMaterial;
    [SerializeField] private RectTransform circle;
    private bool isSelecting = false;


    private void Awake() {
        rawImage = GetComponent<RawImage>();
        material = rawImage.material;
        Debug.Log(rawImage);
        Debug.Log(material);
    }

    private void Start() {
        SetSV(0.0f, 0.0f);
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
        float uvX = (localPoint.x - rect.xMin) / rect.width;
        float uvY = (localPoint.y - rect.yMin) / rect.height;

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

        // // RectTransform内でのクリック座標をUV座標に変換
        // Rect rect = rectTransform.rect;
        // float uvX = (localPoint.x - rect.xMin) / rect.width;
        // float uvY = (localPoint.y - rect.yMin) / rect.height;

        // // UV座標が範囲外なら無視
        // if (uvX < 0 || uvX > 1 || uvY < 0 || uvY > 1)
        // {
        //     Debug.Log("クリック位置がRawImageの範囲外です");
        //     return;
        // }

        // // 彩度と明度を設定
        // SetSV(uvX, uvY);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var uvPosition = GetUVPosition(eventData.position);
        if (uvPosition.x < 0 || uvPosition.x > 1 || uvPosition.y < 0 || uvPosition.y > 1)
        {
            return;
        }
        isSelecting = true;
        StartCoroutine(SelectSVCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelecting = false;
    }

    private IEnumerator SelectSVCoroutine()
    {
        while (isSelecting)
        {
            var uvPosition = GetUVPosition(Input.mousePosition);
            SetSV(uvPosition.x, uvPosition.y);
            yield return null;
        }
    }

    private void SetSV(float s, float v) {
        s = Mathf.Clamp(s, 0, 1);
        v = Mathf.Clamp(v, 0, 1);
        selectedColorMaterial.SetFloat("_Saturation", s);
        selectedColorMaterial.SetFloat("_Value", v);
        Debug.Log($"Saturation: {s}, Value: {v}");

        // サークルの位置を更新
        circle.anchoredPosition = new Vector2(
            (s - 0.5f) * rawImage.rectTransform.rect.width,
            (v - 0.5f) * rawImage.rectTransform.rect.height
        );
    }
}
