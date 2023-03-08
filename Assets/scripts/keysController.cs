using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "keys", menuName = "Keys/Create", order = 1)]
public class keysController : ScriptableObject
{
    public string SpeechKey;
    public string GoogleSpeechKey;
    public string SpeechRegion;
    
    public string openAIKey;
}
