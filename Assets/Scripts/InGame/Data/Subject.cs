using UnityEngine;
using InGame.UI;
using System.Collections;
using System.Collections.Generic;

namespace InGame.Data
{
    public interface Subject {
        List<IDisplayComponent> displayComponents{get; set;}
        public void AddDisplayComponent(IDisplayComponent component);
    }
}
