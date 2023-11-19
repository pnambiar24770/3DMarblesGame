using System.Collections;
using UnityEngine;

public class AbilityBase : ScriptableObject
{
    public new string name;
    public float activeTime;

    public virtual void Activate(GameObject parent) { }

    public virtual void Activate(GameObject parent, bool shrink) { }
}
