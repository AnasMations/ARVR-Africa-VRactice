using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Microsoft.CognitiveServices.Speech;

public class LoadKeys : MonoBehaviour 
{
    public static string OPEN_AI_API_KEY="", SPEECH_API_KEY="", SPEECH_API_REGION="", GOOGLE_SPEECH_API_KEY = "";
    
    public static void Load(keysController keys)
    {
        OPEN_AI_API_KEY = keys.openAIKey;
        GOOGLE_SPEECH_API_KEY = keys.GoogleSpeechKey;
        SPEECH_API_KEY = keys.SpeechKey;
        SPEECH_API_REGION = keys.SpeechRegion;
    }
        
}