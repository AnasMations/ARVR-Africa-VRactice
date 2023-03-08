//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using System.Linq;
using UnityEngine.Animations.Rigging;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
using System.Collections;
#endif
using TMPro;
public class RecognizeSpeech : MonoBehaviour
{
    public static RecognizeSpeech instance;
    public TextMeshProUGUI answer;
    private void Awake()
    {

        LoadKeys.Load(keys);
      /*
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
      */
    }
    // create instance
   public bool active = false;
    private bool micPermissionGranted = false;
    SpeechRecognizer recognizer;
    SpeechConfig config;
    AudioConfig audioInput;
    PushAudioInputStream pushStream;
    //public IntroController introControllerRecepit;

    private object threadLocker = new object();
    private bool recognitionStarted = false;
    private bool recognitionStopped = false;
    private string message;
    int lastSample = 0;
    int message_length = 0;
    AudioSource audioSource;
    public string language = "en-US";

    [HideInInspector]
    public int index = 0;

#if PLATFORM_ANDROID || PLATFORM_IOS
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    private byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
    {
        MemoryStream dataStream = new MemoryStream();
        int x = sizeof(Int16);
        Int16 maxValue = Int16.MaxValue;
        int i = 0;
        while (i < data.Length)
        {
            dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
            ++i;
        }
        byte[] bytes = dataStream.ToArray();
        dataStream.Dispose();
        return bytes;
    }

    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            message = e.Result.Text;
            Debug.Log("RecognizingHandler: " + message);
         }

    }
    public string ans = "";
    private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {

            message = e.Result.Text;
            ans = message;
            Debug.Log("RecognizedHandler: " + message);
           
           
        }
       

    }
    private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        lock (threadLocker)
        {
            Debug.Log(e.ErrorCode.ToString());
            if(e.ErrorCode.ToString()== "ConnectionFailure")
            {
                
            }

            message = e.ErrorDetails.ToString();
            Debug.Log("CanceledHandler: " + message);
        }
    }

    public bool FindWord(string word)
    {
        return message.Contains(word.ToLower());
    }

    public string getMessage()
    {
        return message;
    }

    public int Speaking()
    {
        message_length = message.Length;
        message = "";
        return message_length;
    }
    public async void startRecognition ()
    {
        

        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false); ;
        lock (threadLocker)
        {
            recognitionStarted = true;
            Debug.Log("RecognitionStarted: " + recognitionStarted.ToString());
        }

    }

    public async void stopRecognition()
    {

        if (recognitionStarted) {
        
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(true);
            print("stopped");
            if (Microphone.IsRecording(Microphone.devices[0]))
            {
                Debug.Log("Microphone.End: " + Microphone.devices[0]);
                Microphone.End(null);
                lastSample = 0;
            }

            lock (threadLocker)
            {
                recognitionStarted = false;
                Debug.Log("RecognitionStarted: " + recognitionStarted.ToString());
            }
        }
    
    }
    public keysController keys;


    public void configure()
    {
       

        {
            // Continue with normal initialization, Text and Button objects are present.
#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            message = "";
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#elif PLATFORM_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
#else
            micPermissionGranted = true;
            message = "";
#endif
            config = SpeechConfig.FromSubscription(LoadKeys.SPEECH_API_KEY, LoadKeys.SPEECH_API_REGION);
          
            //language = "nb-NO";
            config.SpeechRecognitionLanguage = "en-US";

            pushStream = AudioInputStream.CreatePushStream();
            audioInput = AudioConfig.FromStreamInput(pushStream);
            recognizer = new SpeechRecognizer(config, audioInput);


            // recognizer = new SpeechRecognizer(config);


            recognizer.Recognizing += RecognizingHandler;
            recognizer.Recognized += RecognizedHandler;
            recognizer.Canceled += CanceledHandler;


            foreach (var device in Microphone.devices)
            {
                Debug.Log("DeviceName: " + device);
            }
            audioSource = GameObject.Find("MyAudioSource").GetComponent<AudioSource>();
            startRecognition();
         //   if(introControllerRecepit!=null)
           // introControllerRecepit.enabled = true;
        }
    }
    void Start()
    {
        configure();
    }

    void Disable()
    {
        recognizer.Recognizing -= RecognizingHandler;
        recognizer.Recognized -= RecognizedHandler;
        recognizer.Canceled -= CanceledHandler;
        pushStream.Close();
        recognizer.Dispose();
        message="";
        
    }

    void OnEnable()
    {
        message="";
    }

    void FixedUpdate()
    {
       
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "";
        }
#elif PLATFORM_IOS
        if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            micPermissionGranted = true;
            message = "";
        }
#endif

        if (active)
        {
           
            if (!Microphone.IsRecording(Microphone.devices[0]))
            {
                audioSource.clip = Microphone.Start(Microphone.devices[0], true, 200, 16000);
            }

            if (Microphone.IsRecording(Microphone.devices[0]) && recognitionStarted)
            {

                int pos = Microphone.GetPosition(Microphone.devices[0]);
                int diff = pos - lastSample;

                if (diff > 0)
                {
                    float[] samples = new float[diff * audioSource.clip.channels];

                    audioSource.clip.GetData(samples, lastSample);

                    byte[] ba = ConvertAudioClipDataToInt16ByteArray(samples);
                    if (ba.Length != 0)
                    {
                         //Debug.Log("pushStream.Write pos:" + Microphone.GetPosition(Microphone.devices[0]).ToString() + " length: " + ba.Length.ToString());
                        pushStream.Write(ba);
                    }
                }
                lastSample = pos;
            }
            else if (!Microphone.IsRecording(Microphone.devices[0]) && !recognitionStarted)
            {
                //  GameObject.Find("MyButton").GetComponentInChildren<Text>().text = "Start";
            }
            if (previousMessage != ans)
            {
                if (answer != null) { 
                answer.text += ans;
                previousMessage = ans;
                }
            }
        }
        else
        {
          
            Microphone.End(null);
            lastSample = 0;
            if (answer != null)
            {
                answer.text = "";
            }
        }
       

        
    }
    string previousMessage = "";
    
}