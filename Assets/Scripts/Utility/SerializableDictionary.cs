using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InGame.Utility
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : 
    Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            if (
                typeof(TKey).IsSubclassOf(typeof(UnityEngine.Object)) || 
                typeof(TKey) == typeof(UnityEngine.Object)
            ) {
                foreach (var element in this.Where(element => element.Key != null))
                {
                    keys.Add(element.Key);
                    values.Add(element.Value);
                }
            } 
            else
            {
                foreach (var element in this)
                {
                    keys.Add(element.Key);
                    values.Add(element.Value);
                }
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}