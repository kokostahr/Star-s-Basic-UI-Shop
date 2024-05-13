using System.Collections;
using UnityEngine;

public abstract class ItemClass : ScriptableObject
{
    //data shared across every item

    public string itemName;
    public Sprite itemImage;
    public bool isStackable = true;

    //defining a function, but not defining its actual role yet...
    public abstract ItemClass GetItem();
    public abstract UpgradeClass GetUpgrade();
    public abstract MiscClass GetMisc();

}
