using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class viewPortMaskScript : MonoBehaviour
{
    static public bool do_update = false;
    static public bool last_update = true;
 
    IMaskable[] maskables = null;
    void OnEnable()
    {
        maskables = GetComponentsInChildren<IMaskable>();
    }
 
    void Update()
    {
        if (do_update)
        {
            if (!last_update)
                maskables = GetComponentsInChildren<IMaskable>();
            foreach (IMaskable msk in maskables)
                msk.RecalculateMasking();
        }
    }
    void LateUpdate()
    {
        last_update = do_update;
    }
}