using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new Upgrade Class", menuName = "Item/Misc")]
public class MiscClass : ItemClass
{
    [Header("Misc Items")]
    public MiscType miscType;
    public enum MiscType
    {
        electronics, 
        gamingGear,
        randomShiet
    }

    //data specifically for misc. items

    public override ItemClass GetItem() { return this; }
    public override UpgradeClass GetUpgrade() { return null; }
    public override MiscClass GetMisc() {  return this; }

}
