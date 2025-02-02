using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TextureGenerator : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public int textureWidth = 1024;
    public int textureHeight = 1024;
    [SerializeField] private Material selectedColorMaterial;
    private RawImage rawImage; // 対象のRawImage
    private bool isPainting = false;

    private void Awake() {
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        // Texture2Dを生成
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        // ピクセルデータを設定
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Color color = new Color(1.0f, 1.0f, 1.0f);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        // このスクリプトがアタッチされているオブジェクトのRawImageコンポーネントに適用
        RawImage rawImage = GetComponent<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = texture;
        }
        else
        {
            Debug.LogError("RawImage component not found on this GameObject.");
        }
    }

    public void Paint(float u, float v, float radius=5.0f)
    {
        float h = selectedColorMaterial.GetFloat("_Hue");
        float s = selectedColorMaterial.GetFloat("_Saturation");
        float v_ = selectedColorMaterial.GetFloat("_Value");
        Color color = Color.HSVToRGB(h, s, v_);
        
        if (rawImage == null)
        {
            Debug.LogError("RawImage component not found on this GameObject.");
            return;
        }

        Texture2D texture = rawImage.texture as Texture2D;
        if (texture == null)
        {
            Debug.LogError("Texture2D not found on RawImage component.");
            return;
        }

        // テクスチャ座標を計算
        int centerx = (int)(u * texture.width);
        int centery = (int)(v * texture.height);

        // テクスチャ座標を中心とした円形領域に色を塗る
        for (int y = centery - (int)radius; y <= centery + (int)radius; y++)
        {
            for (int x = centerx - (int)radius; x <= centerx + (int)radius; x++)
            {
                if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                {
                    continue;
                }

                float dx = x - centerx;
                float dy = y - centery;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        // ピクセルデータを設定
        // texture.SetPixel(x, y, color);
        texture.Apply();
    }

    private Vector2 GetUVPosition(Vector2 screenPosition)
    {
        // RawImageのRectTransformを取得
        RectTransform rectTransform = rawImage.rectTransform;

        // クリックされたスクリーン座標をローカル座標に変換
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPoint);

        // RectTransform内でのクリック座標をUV座標に変換
        Rect rect = rectTransform.rect;
        float uvX = (localPoint.x - rect.xMin) / rect.width;
        float uvY = (localPoint.y - rect.yMin) / rect.height;

        return new Vector2(uvX, uvY);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Vector2 uvPosition = getUVPosition(eventData.position);
        // float uvX = uvPosition.x;
        // float uvY = uvPosition.y;

        // // UV座標が範囲外なら無視
        // if (uvX < 0 || uvX > 1 || uvY < 0 || uvY > 1)
        // {
        //     Debug.Log("クリック位置がRawImageの範囲外です");
        //     return;
        // }

        // // テクスチャに色を塗る
        // Paint(uvX, uvY, Color.black);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var uvPosition = GetUVPosition(eventData.position);
        if (uvPosition.x < 0 || uvPosition.x > 1 || uvPosition.y < 0 || uvPosition.y > 1)
        {
            return;
        }

        isPainting = true;
        StartCoroutine(PaintCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPainting = false;
        
    }

    private void Update()
    {
        if (isPainting)
        {
            // var mousePosition = Input.mousePosition;
            // var uvPosition = getUVPosition(mousePosition);
            // if (uvPosition.x < 0 || uvPosition.x > 1 || uvPosition.y < 0 || uvPosition.y > 1)
            // {
            //     return;
            // }
            // Paint(uvPosition.x, uvPosition.y, Color.black);
        }
    }

    private IEnumerator PaintCoroutine()
    {
        while (true)
        {
            if (isPainting)
            {
                Debug.Log("StartTime: " + Time.time);
                var mousePosition = Input.mousePosition;
                var uvPosition = GetUVPosition(mousePosition);
                if (uvPosition.x < 0 || uvPosition.x > 1 || uvPosition.y < 0 || uvPosition.y > 1)
                {
                    yield return null;
                    continue;
                }
                Paint(uvPosition.x, uvPosition.y);
                Debug.Log("Time: " + Time.time + ", uvPosition: " + uvPosition.x + ", " + uvPosition.y);
            } else {
                yield break;
            }
            yield return null;
        }
    }
}