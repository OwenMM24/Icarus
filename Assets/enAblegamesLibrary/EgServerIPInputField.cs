using UnityEngine;
using TMPro;
using System.Linq;

public class EgServerIPInputField : MonoBehaviour
{
    private TMP_InputField field;

    private void Start()
    {
        field = GetComponent<TMP_InputField>();

        if (PlayerPrefs.HasKey("EgServerIPInputField"))
        {
            field.text = PlayerPrefs.GetString("EgServerIPInputField");
        }
        else
        {
            PlayerPrefs.SetString("EgServerIPInputField", "localhost");
            field.text = "localhost";
        }

        field.onValueChanged.AddListener(Set);
        field.onEndEdit.AddListener(Finished);
    }

    private void Set(string value)
    {
        string address = field.text.Replace(" ", "").ToLower();

        if (address.Count(c => c == '.') != 3 && address.Count(c => c == ':') != 7 && address != "localhost")
        {
            return;
        }

        Debug.Log(address);

        PlayerPrefs.SetString("EgServerIPInputField", address);
    }

    private void Finished(string value)
    {
        if (field.text.Count(c => c == '.') != 3 && field.text.Count(c => c == ':') != 7 && field.text != "localhost")
        {
            field.SetTextWithoutNotify(PlayerPrefs.GetString("EgServerIPInputField"));
        }
    }
}
