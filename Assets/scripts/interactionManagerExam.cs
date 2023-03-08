
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using CrazyMinnow.SALSA;
using TMPro;

public class interactionManagerExam : MonoBehaviour
{

    public TextMeshProUGUI Question;


    int interval = 1;
    float nextTime = 0;
    string prevMessage = "";
    string message = "-";
    bool stopped = false;
    bool isSynthesizing = false;

    bool delayAnimationIsWorking = false;
    public AudioSource audioSource;
    Animator anim;
    SynthesizeSpeechExam synthesizeSpeech;
    GeneratedQuestions generatedText;
    public RecognizeSpeech recognizeSpeech;
    // Emoter emoter;

    // Start is called before the first frame update


    void Start()
    {
        anim = GetComponent<Animator>();
        synthesizeSpeech = GetComponent<SynthesizeSpeechExam>();
        generatedText = GetComponent<GeneratedQuestions>();
        // emoter = GetComponent<Emoter>();




    }
    public void stopAnswering()
    {
        stopped = true;
    }
    void disableDelayAnimationIsWorking()
    {
        delayAnimationIsWorking = false;
    }
    bool intro = true;
    void Update()
    {
        // Read player input every second.
        if (Time.time >= nextTime)
        {
            if (recognizeSpeech != null)
            {
                message = recognizeSpeech.getMessage();
                isSynthesizing = !recognizeSpeech.active;
            }

           


            if (intro)
            {
                message = synthesizeSpeech.introEng[0];
                print(message);
                Interact(message, true);

                intro = false;
            }
            else if (message != prevMessage &&  message.Length > 1)
            {

                // Player has stopped speaking
                if (stopped)
                {
                    if (!isSynthesizing)
                    {
                        isSynthesizing = true;
                        // Interact(shortWords[Random.RandomRange(0, shortWords.Length - 1)], false);

                    }


                    Interact(message, true);




                }
                else
                {
                    if (!delayAnimationIsWorking)
                    {
                        delayAnimationIsWorking = true;
                        //generate random animation
                        int ran = Random.RandomRange(1, 30) % 4;
                        if (ran == 0)
                        {
                            ran++;
                        }
                        string rand = ran.ToString();
                        anim.SetTrigger(rand);
                        Invoke("disableDelayAnimationIsWorking", 2.5f);
                    }

                }

            }

            prevMessage = message;
            nextTime += interval;
        }
    }
    public async void answerQuestion(TextMeshProUGUI answer)
    {
        recognizeSpeech.active = false;
        await generatedText.GetText(answer.text, true, "");
    }
    public async void startExam()
    {
        await generatedText.GetText("yes", true,"");
    }
    public async Task Interact(string message, bool generate = true, string base_response = "")
    {
        Question.text = message;
        print(message);
        //await synthesizeSpeech.SynthesizeAudioAsync(message, generate, base_response);
    }

}
