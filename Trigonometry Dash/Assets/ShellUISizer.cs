using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShellUISizer : MonoBehaviour
{
    public float defaultWidth;
    public float defaultHeight; 

    public float totalChildWidth = 0;
    public float largestChildHeight = 0;
    public float childrenCount = 0;

    public float shadeSpread = 0.1f; //The difference in the color of each layer (nesting) of boxes
    public float spacing;

    public string type; //NOTES FOR LATER: possible types: "Container" "Operator" "Constant" "Variable" "Input"
    public string operatortype; //NOTES FOR LATER: possible types: "Addition/Subtraction" "Multiplication" "Division" "Exponent" "Root" "Absolute Value"

    //public GameObject parent;

    void Update()
    {
        SizerFunction();
        UIInputFunction();
    }

    public void SizerFunction()
    {
        childrenCount = 0; //transform.childCount;      //Number of children the box has
        var parentB = GetComponent<RectTransform>(); //Parent

        totalChildWidth = 0; //Total width of all child objects
        largestChildHeight = 0; //The height of the child with the largest height

        foreach(RectTransform child in parentB)
        {
            if (child != gameObject && child.tag == "ComponentBox")
            {
                childrenCount ++;

                float childWidth = child.sizeDelta.x * child.localScale.x;

                totalChildWidth += childWidth; //Adds the width of each child to the total width of all the children

                if (child.sizeDelta.y > largestChildHeight) //Whether this child has a bigger height than the current highest recorded height
                {
                largestChildHeight = child.sizeDelta.y; //Sets the new largest child height
                }

                child.GetComponent<Image>().color = new Color(0, child.parent.GetComponent<Image>().color.g - shadeSpread, 0); //Colors children a darker shade than their parent (to give layering effect for nesting)
            }
        }

        float noChildrenBoolNumber = 1;
        if(childrenCount>=1)
        {
            noChildrenBoolNumber = 0;
        }
        else
        {
            noChildrenBoolNumber = 1;
        }

        parentB.sizeDelta = new Vector2((defaultWidth*noChildrenBoolNumber + totalChildWidth + spacing * (childrenCount + 1)), defaultHeight*noChildrenBoolNumber + largestChildHeight + 10);

        //if (parent.GetComponent<ComponentBox>())
        //{
        GameObject parentC = transform.parent.gameObject;

        //Debug.Log("parent: " + parentC + "getComponentType : " + parentC.GetComponent<ComponentBox>().type);

        if (parentC.GetComponent<ComponentBox>())
        {
            if (parentC.GetComponent<ComponentBox>().type == ComponentBox.Type.Container && parentC.GetComponent<ComponentBox>().operatorType == ComponentBox.OperatorType.Exponent)
            {
                //Debug.Log("Child: " + transform.name + " | Second child of parent : " + transform.parent.GetComponentsInChildren<ComponentBox>()[2]);
                if (gameObject.transform.GetSiblingIndex() + 1 == parentC.transform.childCount)  //transform.parent.GetComponentsInChildren<ComponentBox>()[2] == gameObject.GetComponent<ComponentBox>())
                {
                    gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1);
                }
            }
        }
        //}

        float posRender = 0;
        float tick = 1;

        foreach(RectTransform child in parentB)
        {
            if (child != gameObject && child.tag == "ComponentBox")
            {
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(-posRender - spacing * tick - child.sizeDelta.x * child.localScale.x / 2 + parentB.sizeDelta.x / 2, 0); //x is negative to make it go left to right ingame and top down in hierarchy

                if (child.transform.parent.gameObject.GetComponent<ComponentBox>())
                {
                    if (child.transform.parent.gameObject.GetComponent<ComponentBox>().type == ComponentBox.Type.Container && child.transform.parent.gameObject.GetComponent<ComponentBox>().operatorType == ComponentBox.OperatorType.Exponent)
                    {
                        //Debug.Log("Child name : " + child.name + " | Child Index : " + child.transform.GetSiblingIndex() + " | Parent Child Count : " + child.transform.parent.childCount);
                        if (child.transform.GetSiblingIndex() + 1 == child.transform.parent.childCount) //transform.GetComponentsInChildren<ComponentBox>()[2] == child.gameObject.GetComponent<ComponentBox>())
                        {
                            child.GetComponent<RectTransform>().anchoredPosition = new Vector2(-posRender - spacing * tick - child.sizeDelta.x * child.localScale.x / 2 + parentB.sizeDelta.x / 2, parentB.sizeDelta.y * 0.15f); //(Might need tweaking)
                        }
                    }
                }

                posRender += child.sizeDelta.x;
                tick = tick + 1;
            }
        }

    }

    public void UIInputFunction()
    {
        GameObject parent = gameObject;
        if (parent.GetComponent<ComponentBox>())
        {
            if (parent.GetComponent<ComponentBox>().type == ComponentBox.Type.Container)
            {
                transform.gameObject.GetComponentInChildren<InputField>().enabled = false;
                transform.gameObject.GetComponentInChildren<InputField>().text = "";
                transform.gameObject.GetComponentInChildren<InputField>().interactable = false;
            }
            else if (transform.gameObject.GetComponent<ComponentBox>().type == ComponentBox.Type.Input)
            {
                transform.gameObject.GetComponentInChildren<InputField>().enabled = true;
                transform.gameObject.GetComponentInChildren<InputField>().contentType = InputField.ContentType.DecimalNumber;
                transform.gameObject.GetComponentInChildren<InputField>().interactable = true;
            }
            else if (transform.gameObject.GetComponent<ComponentBox>().type == ComponentBox.Type.Constant)
            {
                transform.gameObject.GetComponentInChildren<InputField>().enabled = true;
                transform.gameObject.GetComponentInChildren<InputField>().contentType = InputField.ContentType.DecimalNumber;
                transform.gameObject.GetComponentInChildren<InputField>().interactable = false;
            }
            else if (transform.gameObject.GetComponent<ComponentBox>().type == ComponentBox.Type.Variable)
            {
                transform.gameObject.GetComponentInChildren<InputField>().enabled = true;
                transform.gameObject.GetComponentInChildren<InputField>().contentType = InputField.ContentType.Name;
                transform.gameObject.GetComponentInChildren<InputField>().interactable = false;
            }
            else if (transform.gameObject.GetComponent<ComponentBox>().type == ComponentBox.Type.Operator)
            {
                transform.gameObject.GetComponentInChildren<InputField>().enabled = true;
                transform.gameObject.GetComponentInChildren<InputField>().contentType = InputField.ContentType.Standard;
                transform.gameObject.GetComponentInChildren<InputField>().interactable = false;
            }
        }
    }


}
