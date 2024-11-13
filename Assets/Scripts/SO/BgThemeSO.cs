using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/BgThemeSO", fileName = "BgThemeSO")]
public class BgThemeSO : ScriptableObject
{
    public List<IngameTheme> bgThemeList;

    public IngameTheme GetIngameTheme(BgTheme theme)
    {
        return bgThemeList.Find(t => t.theme == theme);
    }
}

[Serializable]
public class IngameTheme
{
    public BgTheme theme;
    public Sprite root;
    public Sprite rootCover;
    public Sprite sign;
    public Color textColor;
}
