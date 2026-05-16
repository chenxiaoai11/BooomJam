using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class One : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clip;
    public Image interlude;
    string scene1;
    Color a;
    bool jump;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        a = interlude.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (jump)
        {
            a.a += Time.deltaTime;
            interlude.color = a;
        }
        if (interlude.color.a >= 1)
        {
            //Debug.Log("场景跳越");
            SceneManager.LoadScene(scene1);
        }
    }

    public void JumpScene(string scene)
    {
        jump = true;
        scene1 = scene;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayFSX()
    {
        int f = Random.Range(0, clip.Length);
        audioSource.clip = clip[f];
        audioSource.Play();
    }
}
