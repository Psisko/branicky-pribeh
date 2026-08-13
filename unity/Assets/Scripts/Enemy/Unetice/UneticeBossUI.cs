using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UneticeBossUI : MonoBehaviour
{
    [SerializeField] private Image fillSlider;
    [SerializeField] private UneticeBoss uneticeBoss;
    [SerializeField] private GameObject uneticeBossUIGO;
    private float UIhealth, UImaxHealth;
    private float lerpSpeed;

    // Use this for initialization
    void Start()
    {
        uneticeBossUIGO.SetActive(false);
        EventController controller = GetComponent<EventController>();
        controller.eventStart.AddListener(ShowHealth);
        controller.eventEnd.AddListener(HideHealth);

        uneticeBoss.livesChangeEvent.AddListener(UpdateLivesUI);


    }

    // Update is called once per frame
    private void Update()
    {
        lerpSpeed = 5f * Time.deltaTime;

        if(UIhealth > 0)
            fillSlider.fillAmount = Mathf.Lerp(fillSlider.fillAmount, UIhealth / UImaxHealth, lerpSpeed);

    }
    private void ShowHealth()
    {
        uneticeBossUIGO.SetActive(true);
        UpdateLivesUI(uneticeBoss.GetLives(), uneticeBoss.GetMaxLives());
        fillSlider.fillAmount = UIhealth / UImaxHealth;
    }

    private void HideHealth()
    {
        uneticeBossUIGO.SetActive(false);
    }
    private void UpdateLivesUI(int health, int maxHealth)
    {
        UIhealth = (float)health;
        UImaxHealth = (float)maxHealth;
    }

}