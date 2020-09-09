using UnityEngine;
using UnityEngine.UI;

public class TutorialButtonController : MonoBehaviour
{
    public AudioClip clickSound; //ボタンのクリック音

    private AudioSource audioSource;
    private ScreenUIManager screenUIManager;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        screenUIManager = GameObject.Find("Script").GetComponent<ScreenUIManager>();
    }

    void Update() {

    }

    public void SelectTutorial() {
        audioSource.PlayOneShot(clickSound);
        string text = transform.GetChild(0).GetComponent<Text>().text;
        screenUIManager.SetExplainingTutorialTexts(text);
    }
}
