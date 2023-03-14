using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeVisual : CardVisual
{
    public Upgrade upgrade;

    public override string Title => upgrade.visual.title;

    public override uint Value => upgrade.visual.value;

    public override string Description => upgrade.visual.description;

    public override Sprite Artwork => upgrade.visual.artwork;
}
