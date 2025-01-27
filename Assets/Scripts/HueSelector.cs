using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HueSelector: MonoBehaviour, IPointerClickHandler
{
    private RawImage rawImage; // 対象のRawImage
    private Material material;
    public Material selectedColorMaterial;
    public Material svSelectorMaterial;


    private void Awake() {
        rawImage = GetComponent<RawImage>();
        material = rawImage.material;
        Debug.Log(rawImage);
        Debug.Log(material);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (rawImage == null || material == null)
        {
            Debug.LogError("RawImageまたはmaterialが設定されていません！");
            return;
        }

        // RawImageのRectTransformを取得
        RectTransform rectTransform = rawImage.rectTransform;

        // クリックされたスクリーン座標をローカル座標に変換
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

        // RectTransform内でのクリック座標をcenterを中心としたUV座標に変換
        Rect rect = rectTransform.rect;
        var center = material.GetVector("_Center");
        float uvX = (localPoint.x - rect.xMin) / rect.width - center.x;
        float uvY = (localPoint.y - rect.yMin) / rect.height - center.y;

        // UV座標が範囲外なら無視
        var dis = Mathf.Sqrt(uvX * uvX + uvY * uvY);
        var radius = material.GetFloat("_VoidRadius");
        if (dis < radius || dis > 0.5f)
        {
            Debug.Log("クリック位置がRawImageの範囲外です");
            return;
        }

        // uv座標を角度に変換
        float angle = Mathf.Atan2(uvY, uvX);
        if (angle < 0) {
            angle += 2 * Mathf.PI;
        }
        angle /= 2 * Mathf.PI;

        // float angle = (Mathf.Atan2(uvY, uvX) + Mathf.PI) / (2 * Mathf.PI);
        selectedColorMaterial.SetFloat("_Hue", angle);
        svSelectorMaterial.SetFloat("_Hue", angle);
        Debug.Log($"Hue: {angle}");
    }
}
