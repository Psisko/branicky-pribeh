using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject spotlightPrefab; // Drag your spotlight prefab here
    private GameObject currentSpotlight;

    private void Start()
    {
        // Disable the spotlight initially
        if (spotlightPrefab != null)
        {
            spotlightPrefab.SetActive(false);
        }
    }

    public void OnButtonHoverEnter(Button button)
    {
        // Get the position of the button and create a spotlight there
        Vector3 buttonPosition = button.transform.position;
        buttonPosition.z = 80;
        buttonPosition.y -= 1.5f;

        if (spotlightPrefab != null)
        {
            // Destroy previous spotlight if it exists
            if (currentSpotlight != null)
            {
                Destroy(currentSpotlight);
            }

            // Instantiate a new spotlight at the button's position
            currentSpotlight = Instantiate(spotlightPrefab, buttonPosition, Quaternion.identity);
            currentSpotlight.SetActive(true);
        }
    }

    public void OnButtonHoverExit()
    {
        // Disable the spotlight when the mouse exits the button
        if (currentSpotlight != null)
        {
            Destroy(currentSpotlight);
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("1-1-unetice-outdoor");
    }

    public void SettingsMenu()
    {
        
    }

    public void ExitGame() {
        Application.Quit();
    }
}
