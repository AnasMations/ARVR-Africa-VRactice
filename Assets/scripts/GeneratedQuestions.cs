using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;



using TMPro;


public class GeneratedQuestions : MonoBehaviour
{
    [HideInInspector]
    public string response = "";
    string prompt;
    string[] prompts;
    public string[] promptsEng;
    string previousPrompt = "";
    public string examTopic = "";
    string[] talker;
    public string[] talkerEng;
    string openAIDomain = "";
    int index = 0;
    public TextMeshProUGUI question;
    public GameObject submit;
    private void Awake()
    {



        openAIDomain = "https://api.openai.com/v1/engines/text-curie-001/completions";
        talker = talkerEng;
        prompts = promptsEng;

        prompt = prompts[0];
        updatePrompt();

    }
    private void Start()
    {
    }
    void Update()
    {
    }
    public void updatePrompt()
    {
        response = "";
        string pattern = @"\b_examTopic\b";
        //for progression
        //  prompt = prompts[index];
        prompt = prompts[0];
        if (PlayerPrefs.GetString("ExamTopic") != "")
        {
            prompt = Regex.Replace(prompt, pattern, PlayerPrefs.GetString("ExamTopic"));
        }
        else
        {
            prompt = Regex.Replace(prompt, pattern, examTopic);

        }
        //prompt += "\n\n " + previousPrompt;

    }

    public async Task GetText(string playerInput, bool generate, string base_response)
    {
        print("start generated : " + Time.time);
        string promptInput = "";


        promptInput = prompt + $"\n\n{talker[1]} {playerInput} \n{talker[0]}";
        print($"prompt input: {promptInput}");

        if (generate)
        {
            openAiData openAIdata = new openAiData();
            openAIdata.prompt = promptInput;
            openAIdata.stop = talker;
            print(PlayerPrefs.GetString("UserId"));
            openAIdata.user = PlayerPrefs.GetString("UserId");
            string json = JsonUtility.ToJson(openAIdata);
            //https://api.openai.com/v1/engines/text-curie-001/completions for English
            using (UnityWebRequest www = UnityWebRequest.Put(openAIDomain, json))
            {
                www.method = "POST";
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Bearer " + LoadKeys.OPEN_AI_API_KEY.ToString());

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
                    openAiResponse openAIresponse = JsonUtility.FromJson<openAiResponse>(www.downloadHandler.text);

                    if (openAIresponse.choices.Length > 0)
                    {

                        response = openAIresponse.choices[0].text;

                        print(response);
                        question.text = response;
                        submit.SetActive(true);
                    }
                }

            }
           
        }
        else
        {
            response = base_response;

        }

        prompt = promptInput + response;

    }
    

    public string GetResponse()
    {
        return response;
    }

    public string GetPrompt()
    {

        return prompt;
    }

    void Disable()
    {
        response = "";

    }

}