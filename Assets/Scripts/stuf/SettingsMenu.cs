using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    bool isSettingsOpen = false;

    [SerializeField] TMP_Text helpCloudText;
    [SerializeField] TMP_Text meanCloudText;
    [SerializeField] HazardSpawner hazardSpawner;
    [SerializeField] GameManager gameManager;


    public void SettingsButton()
    {
        if (!isSettingsOpen)
        {
            settingsMenu.SetActive(true);
            isSettingsOpen = true;
        }
        else
        {
            settingsMenu.SetActive(false);
            isSettingsOpen = false;
        }
    }

    public void ToggleHeightDamage(bool value)
    {
        gameManager.allowHeightBurn = value;
    }
    public void AllowWingDecay(bool value)
    {
        gameManager.allowStabilityChange = value;
    }
/*    public void HelpfulCloudAmount(float amount)
    {
        hazardSpawner.helpIntensity = amount;
        helpCloudText.text = amount.ToString();
    }
    public void HarmfulCloudAmount(float amount)
    {
        hazardSpawner.harmIntensity = amount;
        meanCloudText.text = amount.ToString();
    }*/

}