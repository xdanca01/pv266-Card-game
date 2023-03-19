using System;
using UnityEngine;

public enum FSColor
{
    Yellow = 0xBF9F00,
    Orange = 0xBF6000,
    Red = 0xBF0000,
    Blue = 0x00A0FF,
    Violet = 0xAE00FF,
    Black = 0x0,
    DarkGray = 0x525252,
    LightGray = 0x808080,
    White = 0xFFFFFF
}

static class FSColorMethods
{
    public static Color ToColor(this FSColor color, float alpha) => new(
        (((int)color & 0xFF0000) >> 16) / 255.0f,
        (((int)color & 0x00FF00) >> 8) / 255.0f,
        ((int)color & 0x0000FF) / 255.0f,
        alpha
    );
    public static Color ToColor(this FSColor color) => ToColor(color, 1.0f);
    public static FSColor ToFSColor(this AbilityType ability) => ability switch
    {
        AbilityType.LightAttack => FSColor.Yellow,
        AbilityType.HeavyAttack => FSColor.Orange,
        AbilityType.Heal => FSColor.Red,
        _ => throw new NotImplementedException()
    };
}
