using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitCircle : MonoBehaviour
{
    public List<GameObject> hitCircles;
    private List<Button> hitCircleButtons;
    private AudioSource audioSource;

    void Start()
    {
        hitCircleButtons = new List<Button>();

        foreach (GameObject hitCircle in hitCircles)
        {
            hitCircleButtons.Add(hitCircle.GetComponent<Button>());
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        DetermineButtonPress(1);
        DetermineButtonPress(2);
    }

    private void DetermineButtonPress(int number)
    {
        KeyCode triggerKey = KeyCode.None;
        switch (number)
        {
            case 1:
                triggerKey = KeyCode.F;
                break;
            case 2:
                triggerKey = KeyCode.G;
                break;
        }

        if (Input.GetKeyDown(triggerKey)) 
        {    
            hitCircleButtons[number - 1].onClick.Invoke();
            audioSource.PlayOneShot(audioSource.clip);
        }

        // color
        if (Input.GetKey(triggerKey)) 
        {
            if (GetButtonColor(hitCircleButtons[number - 1]) != Color.red)
            {
                SetButtonColor(hitCircleButtons[number - 1], pressed: true);
            }
        }
        else 
        {
            if (GetButtonColor(hitCircleButtons[number - 1]) != Color.gray)
            {
                SetButtonColor(hitCircleButtons[number - 1], pressed: false);
            }
        }
    }
    
    private Color GetButtonColor(Button button)
    {
        return button.colors.normalColor;
    }

    private void SetButtonColor(Button hitCircleButton, bool pressed)
    {
        ColorBlock cb = hitCircleButton.colors;
        if (pressed) 
        {
            cb.normalColor = Color.red;
        }
        else
        {
            cb.normalColor = Color.gray;
        }

        hitCircleButton.colors = cb;
    }
}
