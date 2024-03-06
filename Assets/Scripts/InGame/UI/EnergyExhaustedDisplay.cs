using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.UI
{
    public class EnergyExhaustedDisplay : MonoBehaviour
    {
        [SerializeField] GameObject energyExhaustedCanvas, playModeCanvas, character;

        static EnergyExhaustedDisplay instance = null;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        public void EnergyExhausted()
        {
            energyExhaustedCanvas.SetActive(true);
            playModeCanvas.SetActive(false);

            if(character != null)
                character.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
        }

        public void EnergyUnexhausted()
        {
            energyExhaustedCanvas.SetActive(false);
            playModeCanvas.SetActive(true);
            character.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }

        public static EnergyExhaustedDisplay GetInstance()
        {
            return instance;
        }
    }
}
