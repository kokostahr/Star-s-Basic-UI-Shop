using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new Upgrade Class", menuName = "Item/Upgrade")]
public class UpgradeClass : ItemClass
{
    //data specifically for upgrades.

    [Header("Upgrades")]
    public UpgradeType upgradeType;
    public enum UpgradeType
    {
        chest,
        backpack
    }

    public override ItemClass GetItem() {  return this; }
    public override UpgradeClass GetUpgrade() { return this; }
    public override MiscClass GetMisc() { return null; }
}
