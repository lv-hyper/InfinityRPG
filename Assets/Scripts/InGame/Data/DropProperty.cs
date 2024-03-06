using UnityEngine;
using System;
using InGame.Data.Item;

namespace InGame.Data
{
    [Serializable]
    public class DropProperty
    {
        [SerializeField] public Item.Item item;
        [SerializeField] public float probablity;
    }
}