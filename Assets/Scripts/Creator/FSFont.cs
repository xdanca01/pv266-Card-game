using TMPro;
using UnityEditor;
using UnityEngine;

public enum FSFont
{
    DeadRevolution,
    Dumbledor,
    Geizer
}

static class FSFontMethods
{
    public static TMP_FontAsset ToAsset(this FSFont font) => Resources.Load<TMP_FontAsset>(font switch
    {
        FSFont.DeadRevolution => "Fonts/Dead Revolution/Dead Revolution SDF",
        FSFont.Dumbledor => "Fonts/Dumbledor/dum1 SDF",
        FSFont.Geizer => "Fonts/Geizer/Geizer SDF",
        _ => throw new System.Exception("Unknown Font!"),
    });
}