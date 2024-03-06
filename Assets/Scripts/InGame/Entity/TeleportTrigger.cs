using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] string triggerId;
    [SerializeField] string destMap;

    public string GetTriggerID(){return triggerId;}
    public string GetDestMap(){return destMap;}

}
