using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class XmasEasterEgg : MonoBehaviour
{
    public float chance;
    public AudioClip easterEggAudio;

    void Start() {
        if (Random.Range(0, 1f) <= chance && easterEggAudio != null) {
            Debug.Log("It's tiiiiiimeeeeee...");
            GetComponent<AudioSource>().clip = easterEggAudio;
        }
    }
}
