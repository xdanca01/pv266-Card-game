// upgrade is an attachable effect for a unit
public interface IUpgrade
{
    // what effect the upgrade provides?
    IEffect Effect { get; }
    // what color
    FSColor Color { get; }
}
