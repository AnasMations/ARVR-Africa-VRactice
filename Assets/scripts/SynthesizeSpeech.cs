using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.Networking;
// using System;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Linq;
using System.IO;
using System;
using System.Net;
public class SynthesizeSpeech : MonoBehaviour
{
    public RecognizeSpeech recognizeSpeech;
    public AudioSource SynthesisAudioSource;
    private SpeechSynthesizer synthesizer;
    GeneratedText generatedText;
    string voiceGoogle = "";
    public bool isMan = true;
    public bool isRobot = false;
    private float startTimeOfPlayAudio;

    string[] intro;
    public string[] introEng;

    void Start()
    {
        generatedText = GetComponent<GeneratedText>();

            intro = introEng;
       
        
        if (isMan)
        {
           voiceGoogle = "en-US-Wavenet-D";

        }
        else
        {
            voiceGoogle = "en-US-Wavenet-C";

        }
        


    }
    int index = 0;
    public async Task SynthesizeAudioAsync(string s, bool generate, string base_response)
    {

        if (recognizeSpeech != null)
            recognizeSpeech.active = false;

     

        if (index == 0)
        {
            s = intro[0];
            index++;
        }
        else
        {

             if (generate)
             {

                 await generatedText.GetText(s, generate, base_response);

                 s = generatedText.GetResponse().ToString();

             }
        }
       

        bool gotAudio = false;

        string ssml;


       
        //google sdk
       // print(s);
        ssml = $" < speak >{s}</ speak > ";
       
        //print("start google : " + Time.time);
        AudioConfiguration audioConfig = new AudioConfiguration("MP3", 0, 1);
        InputData input = new InputData(ssml);
        Voice voi = new Voice("en-US", voiceGoogle);

        GoogleSpeechBody body = new GoogleSpeechBody(audioConfig,input,voi) ;
        string requestBody = JsonUtility.ToJson(body);
        //print(requestBody);

        using (UnityWebRequest www = UnityWebRequest.Put("https://texttospeech.googleapis.com/v1/text:synthesize?key="+LoadKeys.GOOGLE_SPEECH_API_KEY, requestBody))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }

            else
            {
                GoogleSpeechResponse response=JsonUtility.FromJson<GoogleSpeechResponse>(www.downloadHandler.text);
             
                File.WriteAllBytes(Application.persistentDataPath + "/somefile.mp3", Convert.FromBase64String(response.audioContent));

                gotAudio = true;


            }
        }
        if (gotAudio)
        {
            StartCoroutine(GetAudioClip());
        }


    
    }

    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + Application.persistentDataPath + "/somefile.mp3", AudioType.MPEG))
        {
            yield return www.Send();
            
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                SynthesisAudioSource.clip = myClip;

               SynthesisAudioSource.Play();
                StartCoroutine(finishSpeaking());

            }
        }
    }
   
    
    
    IEnumerator finishSpeaking()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.100f);
            if ((SynthesisAudioSource.clip.length-(Time.time - startTimeOfPlayAudio))<1.0f)
                if (!SynthesisAudioSource.isPlaying)
                {
                if (recognizeSpeech != null)
                    recognizeSpeech.active = true;

                    break;

            }
        }
    }
}