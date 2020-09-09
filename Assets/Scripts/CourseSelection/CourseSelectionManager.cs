using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CourseSelectionManager : MonoBehaviour, IPointerClickHandler
{
    public AudioClip decideCourseNumberSound;

    private ScreenUIManager screenUIManager;
    private AudioSource audioSource;

    void Start() {
        screenUIManager = GameObject.Find("Script").GetComponent<ScreenUIManager>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (ScreenUIManager.canClick) {
            audioSource.PlayOneShot(decideCourseNumberSound);
            int courseNumber =
                int.Parse(gameObject.transform.GetChild(0).gameObject.transform.GetComponentInChildren<Text>().text); //コース番号を格納
            screenUIManager.SetExplainingCourseTexts(courseNumber);
        }
    }
}
