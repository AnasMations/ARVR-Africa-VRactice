using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetString("jobTitle",""); 
        PlayerPrefs.SetString("ExamTopic", "");
    }
    public void setJobTitle(inputfieldController v)
    {
        PlayerPrefs.SetString("jobTitle", v.text);

    }
    public void setExamTopic(inputfieldController v)
    {
        PlayerPrefs.SetString("ExamTopic", v.text);

    }
    public void goScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
