using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RawImageColorPicker : MonoBehaviour, IPointerClickHandler
{
    public RawImage rawImage; // 対象のRawImage

    public void OnPointerClick(PointerEventData eventData)
    {
        if (rawImage == null || rawImage.texture == null)
        {
            Debug.LogError("RawImageまたはテクスチャが設定されていません！");
            return;
        }

        // RawImageのRectTransformを取得
        RectTransform rectTransform = rawImage.rectTransform;

        // クリックされたスクリーン座標をローカル座標に変換
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

        // RectTransform内でのクリック座標を0〜1のUV座標に変換
        Rect rect = rectTransform.rect;
        float uvX = (localPoint.x - rect.xMin) / rect.width;
        float uvY = (localPoint.y - rect.yMin) / rect.height;

        // UV座標が範囲外なら無視
        if (uvX < 0 || uvX > 1 || uvY < 0 || uvY > 1)
        {
            Debug.Log("クリック位置がRawImageの範囲外です");
            return;
        }

        // UV座標をピクセル座標に変換
        Texture2D texture = rawImage.texture as Texture2D;
        int pixelX = Mathf.FloorToInt(uvX * texture.width);
        int pixelY = Mathf.FloorToInt(uvY * texture.height);

        // テクスチャから色を取得
        Color color = texture.GetPixel(pixelX, pixelY);

        Debug.Log($"クリックされた位置の色: {color}");
    }
}