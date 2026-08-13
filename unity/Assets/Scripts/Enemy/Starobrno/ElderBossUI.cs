using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElderBossUI : MonoBehaviour
{
    [SerializeField] private Image fillSlider;
    [SerializeField] private ElderBoss elderBoss;
    [SerializeField] private GameObject elderBossUIGO;
    private float UIhealth, UImaxHealth;
    private float lerpSpeed;

    // Use this for initialization
    void Start()
    {
        elderBossUIGO.SetActive(false);
        EventController controller = GetComponent<EventController>();
        controller.eventStart.AddListener(ShowHealth);
        controller.eventEnd.AddListener(HideHealth);

        elderBoss.livesChangeEvent.AddListener(UpdateLivesUI);

    }

    // Update is called once per frame
    private void Update()
    {
        lerpSpeed = 5f * Time.deltaTime;

        if (UIhealth > 0)
            fillSlider.fillAmount = Mathf.Lerp(fillSlider.fillAmount, UIhealth / UImaxHealth, lerpSpeed);
        else
            fillSlider.fillAmount = 0;

    }
    private void ShowHealth()
    {
        elderBossUIGO.SetActive(true);
        UpdateLivesUI(elderBoss.GetLives(), elderBoss.GetMaxLives());
        fillSlider.fillAmount = UIhealth / UImaxHealth;
    }

    private void HideHealth()
    {
        elderBossUIGO.SetActive(false);
    }
    private void UpdateLivesUI(int health, int maxHealth)
    {
        UIhealth = (float)health;
        UImaxHealth = (float)maxHealth;
    }

}