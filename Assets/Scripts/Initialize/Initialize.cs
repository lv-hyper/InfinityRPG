using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Initialize
{
    class InitOnGameStart : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void FirstInitialize()
        {
            PlayerPrefs.SetInt("collectionPage", 1);//Initializing collection page to 1. Doesn't reset unless game off.
            PlayerPrefs.SetString("CollectionDiff", "normal");//Initializing collection difficulty to normal. Doesn't reset unelss game off, too.
            PlayerPrefs.SetInt("showAll", 0);
            PlayerPrefs.SetInt("sortingOrder", 1);
            PlayerPrefs.SetInt("element", 0);
            PlayerPrefs.SetInt("sortingOptionNum", 0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
