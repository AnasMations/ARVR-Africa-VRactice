using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.XR.Interaction.Toolkit;
public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    private InputDevice rightDevice, leftDevice;

    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach (var item in devices)
        {
            if (item.name.Contains("Right"))
            {
                rightDevice = item;
                continue;
            }
            else if (item.name.Contains("Left"))
            {
                leftDevice = item;
                continue;
            }
        }
    }
    bool menuOpened = false;

    void Update()
    {
        leftDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPress);

        if (!menuOpened && (menuButtonPress || Input.GetKeyDown(KeyCode.P)))
        {
            Pause();
        }
    }

    public void Pause()
    {
        menuOpened = true;
        pauseMenuCanvas.SetActive(true);
    }

    public void Resume()
    {
        menuOpened = false;
        pauseMenuCanvas.SetActive(false);
    }
    public sceneManager sceManager;
    public void Exit()
    {
        sceManager.goScene(0);
        //TODO leave scene
    }

}
