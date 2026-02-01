using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enablegames.Suki;
using UnityEngine.UI;

public class SchemaNameDisplay : MonoBehaviour
{
    TMP_Text sukiSchema_Text = null;

    public bool checkInUpdate = false;

    Text sukiProfileText = null;


    void Awake()
    {
        sukiSchema_Text = GetComponent<TMP_Text>();
    }



    private void Start()
    {
        UpdateText();
    }



    private void Update()
    {
        if(checkInUpdate)
        {
            UpdateText();
        }
    }



    public void UpdateText()
    {
        if (sukiProfileText == null)
        {
            GameObject sptObj = GameObject.Find("SukiProfileText");
            if (sptObj != null)
                sukiProfileText = sptObj.GetComponent<Text>();
        }
        if(sukiProfileText != null)
        {
            sukiSchema_Text.text = sukiProfileText.text.Replace("Body Profile: ", "Using ");
            return;
        }

        string schema = SukiSchemaList.currentSukiFile;

        if (sukiSchema_Text == null)
            sukiSchema_Text = GetComponent<TMP_Text>();

        if (schema != null)
        {
            Debug.Log($"Schema name: {schema}");
            sukiSchema_Text.GetComponent<TMP_Text>().text = "Using " + schema.Remove(schema.Length - 5, 5);
        }
        else
        {
            Debug.Log($"Schema name: {schema} is not selected!");
            sukiSchema_Text.GetComponent<TMP_Text>().text = "Using default";
        }
    }
}
