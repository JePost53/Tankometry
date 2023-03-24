using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShellUISizer : MonoBehaviour
{
    public float defaultWidth;
    public float defaultHeight;

     // Offset values for the size of the main container (the shellUI)
    public float size_widthOffset = 0;
    public float size_heightOffset = 10;


    public float totalChildWidth = 0;
    public float largestChildHeight = 0;
    public float childrenCount = 0;

    public float shadeSpread = 0.1f;    // The difference in the color of each layer (nesting) of boxes
    public float spacing;

    public string type;                 // Possible types: "Container" "Operator" "Constant" "Variable" "Input"
    public string operatortype;         // Possible types: "Addition/Subtraction" "Multiplication" "Division" "Exponent" "Root" "Absolute Value"

    void Update()
    {
        SizerFunction();
        UIInputFunction();
    }

    public void SizerFunction()
    {
        childrenCount = 0;      // Number of children the box has
        var parentRect = GetComponent<RectTransform>(); //Parent

        totalChildWidth = 0;    // Total width of all child objects
        largestChildHeight = 0; // The height of the child with the largest height

        foreach(RectTransform child in parentRect)
        {
            if (child != gameObject && child.tag == "ComponentBox")
            {
                childrenCount++;

                float childWidth = child.sizeDelta.x * child.localScale.x;

                totalChildWidth += childWidth;                  // Adds the width of each child to the total width of all the children

                if (child.sizeDelta.y > largestChildHeight)     // Whether this child has a bigger height than the current highest recorded height
                {
                    largestChildHeight = child.sizeDelta.y;     // Sets the new largest child height
                }

                 // Colors children a darker shade than their parent (to give layering effect for nesting)
                child.GetComponent<Image>().color = new Color(0, child.parent.GetComponent<Image>().color.g - shadeSpread, 0);
            }
        }

         // If there are no children, then set to the default height (maybe clamp would be better for this?)
        float noChildrenBoolNumber = 1;
        if(childrenCount>=1)
            noChildrenBoolNumber = 0;
        else
            noChildrenBoolNumber = 1;

         // Sets the size of the parent (this object)
        float xSize = defaultWidth*noChildrenBoolNumber + totalChildWidth + spacing * (childrenCount + 1) + size_widthOffset;
        float ySize = defaultHeight*noChildrenBoolNumber + largestChildHeight + size_heightOffset;
        parentRect.sizeDelta = new Vector2(xSize, ySize);


        GameObject parentC = transform.parent.gameObject;
        ComponentBox parentComponentBox = parentC.GetComponent<ComponentBox>();

        //Debug.Log("parent: " + parentC + "getComponentType : " + parentC.GetComponent<ComponentBox>().type);

        if (parentComponentBox && parentComponentBox.type == ComponentBox.Type.Container && parentComponentBox.operatorType == ComponentBox.OperatorType.Exponent)
        {
            //Debug.Log("Child: " + transform.name + " | Second child of parent : " + transform.parent.GetComponentsInChildren<ComponentBox>()[2]);
            if (gameObject.transform.GetSiblingIndex() + 1 == parentC.transform.childCount)  //transform.parent.GetComponentsInChildren<ComponentBox>()[2] == gameObject.GetComponent<ComponentBox>())
            {
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1);
            }
        }  

        float posRender = 0;
        float tick = 1;

         // Positions all the children component boxes and places them inside of each other accordingly
        foreach(RectTransform child in parentRect)
        {
            if (child != gameObject && child.tag == "ComponentBox")
            {
                float xPos = -posRender - spacing * tick - child.sizeDelta.x * child.localScale.x / 2 + parentRect.sizeDelta.x / 2;

                // if we are the top guy (the shellUI), make sure to offset the inner box a bit so it's not exactly in the middle
                if (parentC.GetComponent<ShellUISizer>() == null)
                    xPos -= 13;
                
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0); // x is negative to make it go left to right ingame and top down in hierarchy

                ComponentBox childComponentBox = child.transform.parent.gameObject.GetComponent<ComponentBox>();

                 // If this child box is an exponent operator, then move it to the corner accordingly
                if (childComponentBox
                && childComponentBox.type == ComponentBox.Type.Container 
                && childComponentBox.operatorType == ComponentBox.OperatorType.Exponent)
                {
                    //Debug.Log("Child name : " + child.name + " | Child Index : " + child.transform.GetSiblingIndex() + " | Parent Child Count : " + child.transform.parent.childCount);
                    if (child.transform.GetSiblingIndex() + 1 == child.transform.parent.childCount) //transform.GetComponentsInChildren<ComponentBox>()[2] == child.gameObject.GetComponent<ComponentBox>())
                    {
                        child.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, parentRect.sizeDelta.y * 0.15f); // (Might need tweaking)
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
