using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PickPixelColor : MonoBehaviour
{
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private void Awake() {
        // GraphicRaycasterとEventSystemをグローバルに検索して取得
        graphicRaycaster = FindAnyObjectByType<GraphicRaycaster>();
        eventSystem = FindAnyObjectByType<EventSystem>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリック
        {
            var renderer = DetectRendererUnderMouse();
            Debug.Log($"color: {renderer.GetColor()}");
            // if (renderer != null) {
            //     // テクスチャ座標を取得
            //     Texture2D texture = renderer.material.mainTexture as Texture2D;

            //     // クリックした座標をUV座標に変換
            //     Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     Vector2 mousePos2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);   
            //     Vector2 localPoint = mousePos2D - (Vector2)renderer.transform.position;
            //     Vector2 uv = new Vector2(
            //         (localPoint.x / renderer.bounds.size.x) + 0.5f,
            //         (localPoint.y / renderer.bounds.size.y) + 0.5f
            //     );

            //     // UV座標からピクセルの色を取得
            //     int x = Mathf.FloorToInt(uv.x * texture.width);
            //     int y = Mathf.FloorToInt(uv.y * texture.height);
            //     Color color = texture.GetPixel(x, y);

            //     Debug.Log($"クリックした位置の色: {color}");
            // }

            // DetectUIUnderMouse();
            // Debug.Log("クリックされました");

            // // スクリーン座標をワールド座標に変換
            // Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Vector2 mousePos2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
            // Debug.Log($"クリックした位置: {mousePos2D}");

            // // レイキャスト（2D）
            // RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            // if (hit.collider != null)
            // {
            //     Debug.Log($"ヒットしたオブジェクト: {hit.collider.gameObject.name}");

            //     // ヒットしたオブジェクトのRendererを取得
            //     SpriteRenderer spriteRenderer = hit.collider.GetComponent<SpriteRenderer>();

            //     if (spriteRenderer != null)
            //     {
            //         // テクスチャ座標を取得
            //         Texture2D texture = spriteRenderer.sprite.texture;

            //         // ローカル座標をUV座標に変換
            //         Vector2 localPoint = hit.point - (Vector2)spriteRenderer.transform.position;
            //         Vector2 uv = new Vector2(
            //             (localPoint.x / spriteRenderer.bounds.size.x) + 0.5f,
            //             (localPoint.y / spriteRenderer.bounds.size.y) + 0.5f
            //         );

            //         // UV座標からピクセルの色を取得
            //         int x = Mathf.FloorToInt(uv.x * texture.width);
            //         int y = Mathf.FloorToInt(uv.y * texture.height);
            //         Color color = texture.GetPixel(x, y);

            //         Debug.Log($"クリックした位置の色: {color}");
            //     }
            // }
        }
    }

    void DetectUIUnderMouse()
    {
        // マウスの位置をスクリーン座標として取得
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        // Raycastの結果を格納するリスト
        List<RaycastResult> results = new List<RaycastResult>();

        // GraphicRaycasterでUI要素を検出
        graphicRaycaster.Raycast(pointerEventData, results);

        // 結果を処理
        foreach (RaycastResult result in results)
        {
            Debug.Log($"クリックしたUI: {result.gameObject.name}");
        }

        if (results.Count == 0)
        {
            Debug.Log("クリック位置にUIはありません。");
        }
    }

    CanvasRenderer DetectRendererUnderMouse() {
        // マウスの位置をスクリーン座標として取得
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        // Raycastの結果を格納するリスト
        List<RaycastResult> results = new List<RaycastResult>();

        // GraphicRaycasterでUI要素を検出
        graphicRaycaster.Raycast(pointerEventData, results);

        // 結果を処理
        foreach (RaycastResult result in results)
        {
            Debug.Log($"クリックしたUI: {result.gameObject.name}");
            // gameObjectが持っているコンポーネントをすべて表示
            foreach (var component in result.gameObject.GetComponents<Component>())
            {
                Debug.Log($"Component: {component}");
            }
            if (result.gameObject.TryGetComponent(out CanvasRenderer spriteRenderer))
            {
                return spriteRenderer;
            }
        }

        return null;
    }
}
