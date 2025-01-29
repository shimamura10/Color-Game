using UnityEngine;
using static UnityEngine.Mathf;

// 色計算のクラス。Vector3型のRGB値を使って色の計算を行う。
// aが基本色で、bが合成色。RGB値は0～1の範囲で指定する。
// 計算式は下記サイトの引用。
// https://tips.clip-studio.com/ja-jp/articles/4162
// https://blog.nijibox.jp/article/photoshop_blendingmode_02/
public class ColorCalculate
{
    // 反転
    public static Vector3 Invert(Vector3 a)
    {
        return new Vector3(1 - a.x, 1 - a.y, 1 - a.z);
    }

    // 暗い色になるもの
    // 比較（暗）：暗い方の色を返す
    public static Vector3 CompareDarken(Vector3 a, Vector3 b)
    {
        return Vector3.Min(a, b);
    }

    // 乗算
    public static Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    
    // 焼き込みカラー：乗算よりコントラストが強くなる
    public static Vector3 BurnColor(Vector3 a, Vector3 b)
    {
        return Invert(Divide(Invert(a), b));
    }

    // 焼き込みリニア：
    public static Vector3 BurnLinear(Vector3 a, Vector3 b)
    {
        return Invert(Add(Invert(a), Invert(b)));
    }

    // 減算：a - b
    public static Vector3 Subtract(Vector3 a, Vector3 b)
    {
        return new Vector3(Max(0, a.x - b.x), Max(0, a.y - b.y), Max(0, a.z - b.z));
    }

    // カラー比較（暗）：RGBの合計値が小さい方の色を返す
    public static Vector3 CompareColorDark(Vector3 a, Vector3 b)
    {
        return (a.x + a.y + a.z) < (b.x + b.y + b.z) ? a : b;
    }


    // 明るくする
    // 比較（明）：明るい方の色を返す
    public static Vector3 CompareBrighten(Vector3 a, Vector3 b)
    {
        return Vector3.Max(a, b);
    }

    // スクリーン：乗算の逆
    public static Vector3 Screen(Vector3 a, Vector3 b)
    {
        return Invert(Multiply(Invert(a), Invert(b)));
    }

    // 覆い焼きカラー：焼き込みカラーの逆
    public static Vector3 DodgeBurnColor(Vector3 a, Vector3 b)
    {
        return Divide(a, Invert(b));
    }

    // 覆い焼きリニア：焼き込みリニアの逆。加算と同じ。
    public static Vector3 DodgeBurnLinear(Vector3 a, Vector3 b)
    {
        return Add(a, b);
    }

    // 加算（発光）：加算の亜種
    // public static Vector3 AddLuminescence(Vector3 a, Vector3 b)
    
    // カラー比較（明）：RGBの合計値が大きい方の色を返す
    public static Vector3 CompareColorBright(Vector3 a, Vector3 b)
    {
        return (a.x + a.y + a.z) > (b.x + b.y + b.z) ? a : b;
    }


    // コントラストを出すもの
    // オーバーレイ：乗算とスクリーンの中間。基本色が0.5以上でスクリーン。0.5未満で乗算
    public static Vector3 Overlay(Vector3 a, Vector3 b)
    {
        Vector3 c = 2 * Multiply(a, b);
        if (a.x > 0.5f) { c.x = 1 - 2 * (1 - a.x) * (1 - b.x); }
        if (a.y > 0.5f) { c.y = 1 - 2 * (1 - a.y) * (1 - b.y); }
        if (a.z > 0.5f) { c.z = 1 - 2 * (1 - a.z) * (1 - b.z); }
        return c;
    }

    // ソフトライト：実装はばらつきがある。合成色が0.5以上で覆い焼きチック。0.5未満で焼き込みチック。
    // public static Vector3 SoftLight(Vector3 a, Vector3 b)
    
    // ハードライト：合成色が0.5以上でスクリーン。0.5未満で乗算
    public static Vector3 HardLight(Vector3 a, Vector3 b)
    {
        Vector3 c = 2 * Multiply(a, b);
        if (b.x > 0.5f) { c.x = 1 - 2 * (1 - a.x) * (1 - b.x); }
        if (b.y > 0.5f) { c.y = 1 - 2 * (1 - a.y) * (1 - b.y); }
        if (b.z > 0.5f) { c.z = 1 - 2 * (1 - a.z) * (1 - b.z); }
        return c;
    }

    // ビビッドライト：焼き込み（カラー）と覆い焼き（カラー）の組み合わせ
    // public static Vector3 VividLight(Vector3 a, Vector3 b)

    // リニアライト：焼き込み（リニア）と覆い焼き（リニア）の組み合わせ
    // public static Vector3 LinearLight(Vector3 a, Vector3 b)
    
    // ピンライト：合成色が0.5以上で比較（明）。0.5未満で比較（暗）
    public static Vector3 PinLight(Vector3 a, Vector3 b)
    {
        Vector3 c = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            if (b[i] > 0.5f) 
            {
                c[i] = a[i] < 1 - 2 * b[i] ? a[i] : 2 * b[i];
            } 
            else 
            {
                c[i] = a[i] < 2 * b[i] - 1 ? 2 * b[i] - 1 : a[i];
            }
        }
        return c;
    }

    // 除算：a / b
    public static Vector3 Divide(Vector3 a, Vector3 b)
    {
        b.x = b.x == 0 ? 0.001f : b.x;
        b.y = b.y == 0 ? 0.001f : b.y;
        b.z = b.z == 0 ? 0.001f : b.z;
        return new Vector3(Min(1, a.x / b.x), Min(1, a.y / b.y), Min(1, a.z / b.z));
    }

    // 加算：a + b
    public static Vector3 Add(Vector3 a, Vector3 b)
    {
        return new Vector3(Min(1, a.x + b.x), Min(1, a.y + b.y), Min(1, a.z + b.z));
    }

}
