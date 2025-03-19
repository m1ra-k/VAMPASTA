// BURGER

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ChoiceMappingManager : MonoBehaviour
{
    public GameObject ChoicePrefab;
    public Sprite[] textboxes;
    public Color color;

    private DialogueSystemManager dialogueSystemManager;

    public void SetChoiceMapping(DialogueSystemManager dialogueSystemManager, List<ConditionalChoicesStruct> conditionalChoices) 
    {
        this.dialogueSystemManager = dialogueSystemManager;

        ModifyParentPositionY(conditionalChoices.Count);

        int index = 0;

        // when called, make a button for each choice, hook it up correctly according to its choiceMapping
        foreach (ConditionalChoicesStruct conditionalChoice in conditionalChoices) 
        {
            GameObject conditionalChoiceButton = Instantiate(ChoicePrefab);

            Image conditionalChoiceButtonImage = conditionalChoiceButton.GetComponent<Image>();
            conditionalChoiceButtonImage.alphaHitTestMinimumThreshold = 0.5f;

            conditionalChoiceButton.name = "Choice " + conditionalChoice.choiceMapping;
            conditionalChoiceButton.transform.SetParent(transform);
            conditionalChoiceButton.transform.position = new Vector2(0, conditionalChoiceButton.transform.position.y);
            conditionalChoiceButton.GetComponentInChildren<TextMeshProUGUI>().text = conditionalChoice.choiceDialogue;

            conditionalChoiceButtonImage.sprite = textboxes[index];

            conditionalChoiceButtonImage.color = color;

            conditionalChoiceButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(this.dialogueSystemManager, conditionalChoice.choiceMapping));

            StartCoroutine(FadeCreate(conditionalChoiceButton, conditionalChoiceButtonImage.sprite));   

            index++;
        }
    }

    public void DestroyAllConditionalChoiceButtonsAndContinue() 
    {
        foreach (Transform conditionalChoiceTransform in transform)
        {
            GameObject conditionalChoiceButton = conditionalChoiceTransform.gameObject;
            StartCoroutine(FadeDestroy(conditionalChoiceButton, conditionalChoiceButton.GetComponent<Image>().sprite));   
        }
    }

    void ModifyParentPositionY(int numChoices)
    {
        float modifiedPositionY = 0;

        switch (numChoices)
        {
            case 2:
                modifiedPositionY = 250;
                break;

            case 3:
                modifiedPositionY = 400;
                break;
                
            case 4:
                modifiedPositionY = 480;
                break;
        }

        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, modifiedPositionY);
    }

    void OnButtonClick(DialogueSystemManager dialogueSystemManager, int choiceMapping)
    {
        dialogueSystemManager.choiceMapping = choiceMapping;
        DestroyAllConditionalChoiceButtonsAndContinue();
    }

    IEnumerator FadeCreate(GameObject currentCharacterDisplay, Sprite sprite) 
    {
        yield return StartCoroutine(dialogueSystemManager.Fade(currentCharacterDisplay, sprite, -1, 1, isUIElement: true, getTextFromChild: true));      

        currentCharacterDisplay.GetComponent<Button>().interactable = true;
    }

    IEnumerator FadeDestroy(GameObject currentCharacterDisplay, Sprite sprite) 
    {
        yield return StartCoroutine(dialogueSystemManager.Fade(currentCharacterDisplay, sprite, 1, -1, isUIElement: true, getTextFromChild: true));      

        Destroy(currentCharacterDisplay); 
        dialogueSystemManager.choiceClicked = true;
    }
}
