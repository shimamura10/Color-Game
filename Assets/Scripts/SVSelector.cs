using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SVSelector: MonoBehaviour, IPointerClickHandler
{
    private RawImage rawImage; // 対象のRawImage
    private Material material;
    public Material selectedColorMaterial;


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

        // RectTransform内でのクリック座標をUV座標に変換
        Rect rect = rectTransform.rect;
        float uvX = (localPoint.x - rect.xMin) / rect.width;
        float uvY = (localPoint.y - rect.yMin) / rect.height;

        // UV座標が範囲外なら無視
        if (uvX < 0 || uvX > 1 || uvY < 0 || uvY > 1)
        {
            Debug.Log("クリック位置がRawImageの範囲外です");
            return;
        }

        // 彩度と明度を設定
        selectedColorMaterial.SetFloat("_Saturation", uvX);
        selectedColorMaterial.SetFloat("_Value", uvY);
        Debug.Log($"Saturation: {uvX}, Value: {uvY}");
    }
}
