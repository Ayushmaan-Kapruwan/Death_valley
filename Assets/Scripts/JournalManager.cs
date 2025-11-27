using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class JournalManager : MonoBehaviour
{
    [SerializeField] private GameObject journalPage;
    [SerializeField] private TMP_Text clueTextBox;
    [SerializeField] private string[] noClueText;
    private bool openBook;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            OpenJournalBook();
        }
    }

    public void OpenJournalBook()
    {
        openBook = !openBook;
        Debug.Log($"Journal opened: {openBook}");
        CreatePage();
        WriteQuests();
    }

    private void CreatePage()
    {
        if (journalPage != null)
        {
            if (openBook)
            {
                journalPage.SetActive(true);
            }
            else
            {
                journalPage.SetActive(false);
            }
        }
    }

    private void WriteQuests()
    {
        Debug.Log($"WriteQuests called. Clues in MainManager: {MainManager.mainManager.clueNames.Count}");
        
        if (clueTextBox != null)
        {
            if (MainManager.mainManager.clueNames.Count == 0)
            {
                Debug.Log("No clues found, showing placeholder text");
                if (noClueText != null)
                {
                    int randomNumber = (Random.Range(0, noClueText.Length));
                    clueTextBox.text = noClueText[randomNumber];
                }
            }
            else
            {
                StringBuilder stringBuilder = new();
                foreach (string clue in MainManager.mainManager.clueNames)
                {
                    Debug.Log($"Adding clue to journal: {clue}");
                    stringBuilder.AppendLine(clue);
                }
                string finalText = stringBuilder.ToString();
                clueTextBox.text = finalText;
                Debug.Log($"Journal text set to: '{finalText}'");
            }

            clueTextBox.rectTransform.sizeDelta = new Vector2(clueTextBox.rectTransform.sizeDelta.x, clueTextBox.preferredHeight);
        }
        else
        {
            Debug.LogError("ClueTextBox is NULL!");
        }
    }
}
