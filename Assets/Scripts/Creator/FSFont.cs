using TMPro;
using UnityEditor;

public enum FSFont
{
    DeadRevolution,
    Dumbledor,
    Geizer
}

static class FSFontMethods
{
    public static TMP_FontAsset ToAsset(this FSFont font) => AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(font switch
    {
        FSFont.DeadRevolution => "Assets/Fonts/Dead Revolution/Dead Revolution SDF.asset",
        FSFont.Dumbledor => "Assets/Fonts/Dumbledor/dum1 SDF.asset",
        FSFont.Geizer => "Assets/Fonts/Geizer/Geizer SDF.asset",
        _ => throw new System.Exception("Unknown Font!"),
    });
}