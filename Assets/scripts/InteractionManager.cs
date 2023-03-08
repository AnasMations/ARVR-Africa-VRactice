using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using CrazyMinnow.SALSA;


public class InteractionManager : MonoBehaviour
{ 
    int interval = 1;
    float nextTime =0;
    string prevMessage = "";
    string message = "-";
    bool stopped = false;
    bool isSynthesizing = false;

    bool delayAnimationIsWorking = false;
    public AudioSource audioSource;
   

    Animator anim;
    SynthesizeSpeech synthesizeSpeech;
    public RecognizeSpeech recognizeSpeech;
    // Emoter emoter;

    // Start is called before the first frame update


    void Start()
    {
        anim = GetComponent<Animator>();
        synthesizeSpeech = GetComponent<SynthesizeSpeech>();
       // emoter = GetComponent<Emoter>();
        



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
            if (recognizeSpeech != null) { 
            message = recognizeSpeech.getMessage();
            isSynthesizing = !recognizeSpeech.active;
            }
           
            stopped = StopedSpeaking(message);



            if (intro)
            {
                Interact(message, true);

                intro = false;
            }else if (message!=prevMessage && !audioSource.isPlaying && message.Length>1)
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
                        int ran = Random.RandomRange(1, 30)%4;
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

       prevMessage=message;
       nextTime += interval;
          }
        } 
    
    bool AnimatorIsPlaying(){
     return anim.GetCurrentAnimatorStateInfo(0).length >
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
  }
    private bool StopedSpeaking(string s)
    {
        return s.Contains("!") || s.Contains(".") || s.Contains("?");
    }

    private bool IsQuestion(string s)
    {
        return s.Contains("?");
    }

    public async Task Interact(string message, bool generate=true, string base_response="")
{

    await synthesizeSpeech.SynthesizeAudioAsync(message,generate,base_response);
}

}
