using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSelectorScript : MonoBehaviour
{
    public GameObject shellPanel;
    public GameObject scrollRect;
    public GameObject equationBox;
    public float defaultHeight;

    public float childrenCount = 0;
    public float spacing;
    public float totalChildHeight = 0;
    public GameObject selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UIOrganizer();
    }

    public void ToggleShellPanel()
    {
        //Sets shellpanel gameobject to active
        if(shellPanel.activeSelf == false)
        {
            shellPanel.SetActive(true);
        }
        else
        {
            shellPanel.SetActive(false);
        }
    }

    void UIOrganizer()
    {
        childrenCount = 0; //Number of children the box has, by default 0
        var parent = shellPanel.GetComponent<RectTransform>();
        var panel = scrollRect.GetComponent<RectTransform>(); //Parent

        totalChildHeight = 0; //Total width of all child objects

        foreach (RectTransform child in panel)
        {
            if (child != panel)// && child.GetComponentInChildren<ComponentBox>()) //child.tag == "ComponentBox")
            {
                childrenCount++;

                float childHeight = child.sizeDelta.y;

                totalChildHeight += childHeight; //Adds the width of each child to the total width of all the children
            }
        }

        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(panel.sizeDelta.x, totalChildHeight + spacing * (childrenCount + 1) + defaultHeight);
        //Debug.Log("PARENT : " + parent);

        float posRender = 0;
        float tick = 1;

        foreach (RectTransform child in panel)
        {
            if (child != panel) //child.GetComponentInChildren<ComponentBox>()) //child.tag == "ComponentBox")
            {
                //Debug.Log("CHILD : " + child.name);
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(panel.sizeDelta.x / 2, -posRender - spacing * tick - child.sizeDelta.y / 2); // - panel.sizeDelta.y/2); //y is positive to make it go top to bottom ingame and top down in hierarchy

                posRender += child.sizeDelta.y;
                tick = tick + 1;
            }
        }
    }

    public void ShellSelected(GameObject button)
    {
        //Debug.Log("BUTTON!!!!!!!");
        
        if(equationBox.GetComponentInChildren<ShellUISizer>() && selectedButton != null)
        {
            GameObject selectedShell = equationBox.GetComponentInChildren<ShellUISizer>().gameObject;
            selectedShell.transform.parent = selectedButton.transform;
            selectedShell.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        }

        GameObject shell = button.GetComponentInChildren<ShellUISizer>().gameObject;

        shell.transform.parent = equationBox.transform;
        shell.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        selectedButton = button;

        shellPanel.SetActive(false);

        GameObject gameManager = GameObject.Find("_GameManager");

        gameManager.GetComponent<GameManagerScript>().CallLineGenerator();
    }
}
