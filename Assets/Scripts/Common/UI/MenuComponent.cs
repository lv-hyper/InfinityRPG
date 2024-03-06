using System;
using UnityEngine;

namespace Common.UI
{
    public abstract class MenuComponent : MonoBehaviour
    {
        public abstract void OpenChild(string menuName);
        public abstract void Open();
        public abstract void Close();
        
        public abstract string Name { get; }
    }
}