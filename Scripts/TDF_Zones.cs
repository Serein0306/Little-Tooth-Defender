using UnityEngine;

public static class TDF_Zones
{
    public const bool DEBUG = false;

    // 顶部导航栏（所有故事页统一）
    public static Rect NavBack     = new Rect(0.030f, 0.953f, 0.250f, 0.042f);
    public static Rect NavRead     = new Rect(0.300f, 0.953f, 0.230f, 0.042f);
    public static Rect NavVolume   = new Rect(0.550f, 0.953f, 0.220f, 0.042f);
    public static Rect NavProgress = new Rect(0.780f, 0.953f, 0.200f, 0.042f);

    // Home 页按钮（左对齐竖排）
    public static Rect HomeStart    = new Rect(0.080f, 0.728f, 0.330f, 0.056f);
    public static Rect HomeContinue = new Rect(0.080f, 0.662f, 0.300f, 0.047f);
    public static Rect HomeSettings = new Rect(0.080f, 0.596f, 0.300f, 0.047f);

    // Story1: 5 只糖虫
    public static Rect[] Story1Bugs =
    {
        new Rect(0.080f, 0.643f, 0.170f, 0.094f), // 左上
        new Rect(0.370f, 0.662f, 0.200f, 0.094f), // 中上
        new Rect(0.720f, 0.653f, 0.200f, 0.085f), // 右上
        new Rect(0.280f, 0.521f, 0.180f, 0.094f), // 左下
        new Rect(0.620f, 0.521f, 0.180f, 0.094f), // 右下
    };

    // Story2: 刷牙
    public static Rect Story2Brush = new Rect(0.130f, 0.042f, 0.330f, 0.056f); // Brush 按钮
    public static Rect Story2Rinse = new Rect(0.550f, 0.042f, 0.330f, 0.056f); // Rinse 按钮
    public static Rect[] Story2Bugs = { new Rect(0.720f, 0.155f, 0.250f, 0.075f) }; // 右下角糖虫
    public static Rect Story2BrushHome = new Rect(0.000f, 0.343f, 0.500f, 0.376f); // 牙刷拖拽区

    // Story3: 食物选择
    public static Rect Story3Apple  = new Rect(0.050f, 0.577f, 0.280f, 0.122f);
    public static Rect Story3Carrot = new Rect(0.330f, 0.484f, 0.330f, 0.207f);
    public static Rect Story3Candy  = new Rect(0.670f, 0.624f, 0.280f, 0.094f);
    public static Rect Story3Cake   = new Rect(0.700f, 0.465f, 0.250f, 0.113f);

    // Story4: Boss + 牙线
    public static Rect Story4Boss       = new Rect(0.550f, 0.606f, 0.420f, 0.188f); // 绿色糖虫
    public static Rect Story4FlossHome  = new Rect(0.030f, 0.624f, 0.330f, 0.132f); // 牙线圈拖拽区

    // Credits
    public static Rect CreditsReadAgain = new Rect(0.550f, 0.465f, 0.400f, 0.066f);
    public static Rect CreditsBackHome  = new Rect(0.550f, 0.380f, 0.400f, 0.066f);
}
