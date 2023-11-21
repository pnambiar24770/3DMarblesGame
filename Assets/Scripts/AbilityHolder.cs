using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public AbilityBase changeSize;

    enum AbilityState
    {
        ready,
        //active
    }
    AbilityState state = AbilityState.ready;

    public KeyCode growKey = KeyCode.Mouse0; // Left mouse button
    public KeyCode shrinkKey = KeyCode.Mouse1; // Right mouse button

    public KeyCode toggleSoftbodyKey = KeyCode.E; // E


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(growKey))
                {
                    changeSize.Activate(gameObject);
                    state = AbilityState.ready;
                }
                else if (Input.GetKeyDown(shrinkKey))
                {
                    changeSize.Activate(gameObject);
                    state = AbilityState.ready;
                }
                break;
            /*case AbilityState.active:
                // handling the active state
                break;*/
        }
    }
}
