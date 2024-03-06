using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPersistence : MonoBehaviour
{
    static List<SetPersistence> persistentObjectList;

    List<SetPersistence> GetPersistentObjectList(){
        if(persistentObjectList == null)
            persistentObjectList = new List<SetPersistence>();
        return persistentObjectList;
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        GetPersistentObjectList().Add(this);
    }

    public static void DestroyAll()
    {
        foreach(var obj in persistentObjectList)
        {
            Destroy(obj.gameObject);
        }

        persistentObjectList.Clear();
    }

    private void OnDestroy() {
        persistentObjectList.Remove(this);
    }
}
