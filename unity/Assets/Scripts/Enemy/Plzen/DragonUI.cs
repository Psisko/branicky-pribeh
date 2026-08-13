using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DragonUI : MonoBehaviour
{
    [SerializeField] private Image fillSlider;
    [SerializeField] private DragonController dragonBoss;
    [SerializeField] private GameObject dragonBossUIGO;
    private float UIhealth, UImaxHealth;
    private float lerpSpeed;

    void Start()
    {
        dragonBossUIGO.SetActive(false);
        EventController controller = GetComponent<EventController>();
        controller.eventStart.AddListener(ShowHealth);
        controller.eventEnd.AddListener(HideHealth);

        dragonBoss.livesChangeEvent.AddListener(UpdateLivesUI);


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
        dragonBossUIGO.SetActive(true);
        UpdateLivesUI(dragonBoss.GetLives(), dragonBoss.GetMaxLives());
        fillSlider.fillAmount = UIhealth / UImaxHealth;
    }

    private void HideHealth()
    {
        dragonBossUIGO.SetActive(false);
    }
    private void UpdateLivesUI(int health, int maxHealth)
    {
        UIhealth = (float)health;
        UImaxHealth = (float)maxHealth;
    }

}