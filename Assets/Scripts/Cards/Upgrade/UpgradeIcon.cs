using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeIcon : CardIcon
{
    public Upgrade upgrade;

    [SerializeField]
    private Color badgeColor;

    public override string MainText => upgrade.icon.mainText;

    public override string AdditionalText => upgrade.icon.additionalText;

    public override Sprite Badge => upgrade.icon.badge;

    public override Color BadgeColor => badgeColor;
}
