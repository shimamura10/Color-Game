using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    private RawImage rawImage;

    private void Awake() {
        // MainImageという名前のrawImageを取得
        rawImage = GameObject.Find("MainImage").GetComponent<RawImage>();
    }

    void Start()
    {
        // 1. Texture2Dを作成
        Texture2D texture = new Texture2D(width, height);

        // 2. 各ピクセルの色を設定
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 任意のRGB値 (ここではグラデーションを例にしています)
                float r = (float)x / width;
                float g = (float)y / height;
                float b = 0.5f;
                Color color = new Color(r, g, b);

                texture.SetPixel(x, y, color);
            }
        }

        // 3. テクスチャを適用
        texture.Apply();

        // 4. マテリアルに設定
        rawImage.texture = texture;
    }
}
