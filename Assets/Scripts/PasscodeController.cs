using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasscodeController : MonoBehaviour
{
    [SerializeField] TMP_InputField passcodeInput;
    public void Execute()
    {
        PlayerPrefs.SetString("passcode", passcodeInput.text);
    }
}
