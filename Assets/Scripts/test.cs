using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CustomDictionary<TK, TV> : Dictionary<TK, TV>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TK> keys = new List<TK> ();
    [SerializeField]
    private List<TV> values = new List<TV> ();

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach(var kvPair in this)
        {
            keys.Add(kvPair.Key);
            values.Add(kvPair.Value);
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();

        if(keys.Count > values.Count)
        {
            var difference = keys.Count - values.Count;

            for(int i= 0; i < difference; i++)
            {
                keys.RemoveAt(keys.Count - 1);
            }
        }

        else if (keys.Count < values.Count)
        {
            var difference = values.Count - keys.Count;

            for (int i = 0; i < difference; i++)
            {
                values.RemoveAt(values.Count - 1);
            }
        }

        for (int i=0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}

public class Test : MonoBehaviour
{
    [SerializeField] CustomDictionary<string, int> data;

    public void AddData(string key, int value)
    {
        data.Add(key, value);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Add Dictionary Element"))
        {
            var test = target as Test;

            test.AddData("key", 0);
        }
    }
}

#endif


