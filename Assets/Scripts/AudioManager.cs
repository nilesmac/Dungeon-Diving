using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] Swordeffects;
    public Sound[] cutEffects;
    public Sound[] deathEffects;

    private AudioSource Source;
    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake()
    {
    

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in Swordeffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in cutEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in deathEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }



    }


    public void PlayRandom()
    {
        Sound s = Swordeffects[UnityEngine.Random.Range(0, Swordeffects.Length)];
        s.source.Play();
    }

    public void PlayRandomDeath()
    {
        Sound s = deathEffects[UnityEngine.Random.Range(0, Swordeffects.Length)];
        s.source.Play();
    }

    public void PlayRandomCuts()
    {
        Sound s = cutEffects[UnityEngine.Random.Range(0, cutEffects.Length)];
        s.source.Play();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.loop = false;
    }
}
