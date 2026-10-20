using UnityEngine;

/// <summary>
/// 致谢页：Read again（重读故事）/ Back to Home（返回主页）。
/// </summary>
public static class TDF_Credits
{
    public static void Build(TDF_GameManager gm)
    {
        var page = gm.Page;
        TDF_UIFactory.InvisibleButton("ReadAgain", page.rootRT, page.SizeFor(TDF_Zones.CreditsReadAgain), page.ImgToAnchoredPos(TDF_Zones.CreditsReadAgain),
            () => gm.Go(TDFPage.Story1));
        TDF_UIFactory.InvisibleButton("BackHome", page.rootRT, page.SizeFor(TDF_Zones.CreditsBackHome), page.ImgToAnchoredPos(TDF_Zones.CreditsBackHome),
            () => gm.Go(TDFPage.Home));
    }
}
