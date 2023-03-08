using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Weelco;
public class inputfieldController : InputField
{
     GameObject keyboard;
    // Start is called before the first frame update
    public override void OnSelect(BaseEventData eventData)
    {
        openKeyboard();

    }

    public void openKeyboard()
    {
        keyboard = GameObject.FindGameObjectWithTag("keyboard");
        
        keyboard.GetComponent<VRKeyboardDemo>().inputFieldLabel = this;
        keyboard.transform.GetChild(0).gameObject.SetActive(true);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
