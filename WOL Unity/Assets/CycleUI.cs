using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CycleUI : MonoBehaviour
{
    public Selectable[] UISelectables;
    public EventSystem eventSystem;

    public void CycleNwextUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            for (int i = 0; i < UISelectables.Length; i++)
            {
                if (UISelectables[i].gameObject == eventSystem.currentSelectedGameObject)
                {
                    UISelectables[(i + 1) % UISelectables.Length].Select();
                    break;
                }
            }
        }
    }
    public void MaxedChar(TMP_InputField feild)
    {
        int maxchar = feild.characterLimit;
        int curlength = feild.text.Length;
        if (curlength >= maxchar)
        {
            //CycleNwextUI();
        }
    }
}
