using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{

    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public Animator Transition;
    public float transitionTime = 1f;
    

    public GameObject continueButton;
    public Animator textDisplayAnim;

    AudioSource audioData;

    private void Start()
    {
        StartCoroutine(Type());
        audioData = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(textDisplay.text == sentences[index])
        {
            continueButton.SetActive(true);
        }

    }

    IEnumerator Type()
    {
        
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        int scenceNum = SceneManager.GetActiveScene().buildIndex;
        textDisplayAnim.SetTrigger("Change");
        audioData.Play(0);
        continueButton.SetActive(false);
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
            continueButton.SetActive(false);
            if (scenceNum == 1 || scenceNum == 2)
                StartCoroutine("PlayTransition");
        }
    }

    IEnumerator PlayTransition()
    {
        Transition.SetTrigger("Transition");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //SceneManager.LoadScene(2);
    }

    public void Skip()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
