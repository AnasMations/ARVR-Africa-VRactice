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






public class GeneratedText : MonoBehaviour
{   [HideInInspector]
    public string response = "";
    string prompt;
    string[] prompts;
    public string[] promptsEng;
    string previousPrompt="";
    public string jobDescription = "";
    string[] talker;
    public string[] talkerEng;
    string openAIDomain ="";
    int index = 0;
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
        string pattern = @"\b_jobDescrition\b";
        //for progression
        //  prompt = prompts[index];
        prompt = prompts[0];
        if (PlayerPrefs.GetString("jobTitle") != "")
        {
            prompt = Regex.Replace(prompt, pattern, PlayerPrefs.GetString("jobTitle"));
        }
        else
        {
            prompt = Regex.Replace(prompt, pattern, jobDescription);

        }
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
                        // var clean_response = Regex.Replace(response, @"\t|\n|\r", "");
                        // eyeController.randomLook();
                        // var expression_input = $"\n\n Speaker :{playerInput} \n Listener:" +clean_response + " Emotion:";
                        // expController.react(expression_input);


                    }
                }

            }
/*
            openAiData openAIdataCF = new openAiData();
            openAIdataCF.prompt = $"<|endoftext|>[{response}]\n--\nLabel:";
            openAIdataCF.model = "content-filter-alpha";
            openAIdataCF.max_tokens = 1;
            openAIdataCF.temperature=0;
            openAIdataCF.top_p=1;
            openAIdataCF.logprobs=10;

            openAIdataCF.user = PlayerPrefs.GetString("UserId");
            string jsonCF = JsonUtility.ToJson(openAIdataCF);


        using (UnityWebRequest www = UnityWebRequest.Put("https://api.openai.com/v1/completions", jsonCF))
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
                    openAiResponse openAIresponseCF = JsonUtility.FromJson<openAiResponse>(www.downloadHandler.text);

                    if (openAIresponseCF.choices.Length > 0)
                    {
                        
                        string output_label = openAIresponseCF.choices[0].text;
                        if (output_label=="2" ){
                            
                            response =  base_response;
                        }
                        else
                        {
                        
                        
                        var clean_response = Regex.Replace(response, @"\t|\n|\r", "");
                        var expression_input = $"\n\n Speaker :{playerInput} \n Listener:" +clean_response + " Emotion:";
                     
                        }
                        


                    }

                }
            }
       
        
        */
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
        response="";
        
    }

}