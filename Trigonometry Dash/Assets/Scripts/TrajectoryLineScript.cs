//using B83.ExpressionParser;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TrajectoryLineScript : MonoBehaviour
{

    public GameObject lineRenderThing;
    public float x;
    public float y;
    public float xMax = 25.00f;
    public float coordRender;
    public string textExpression;
    public GameObject textbox;
    public float dotFrequency;
    public GameObject RestrictionLine;
    public bool moveRestriction = true;
    private Vector3 mousePos;
    public Transform lineVector;
    public Transform player;
    public float maxX;
    private int dot;
    public float drawSpeed;
    public Vector3[] coordinates;
    public Vector3[] lineCoords;
    public float lim1;
    public float lim2;
    public int pID = 0;
    public GameObject turret;
    public GameObject barrelB;
    public HingeJoint2D barHinge;
    private JointAngleLimits2D limits;
    private JointMotor2D motor;
    public Transform barPos;
    public GameObject hull;
    public float aDirection;
    private float hullDirection;
    public float newAngle;
    public float oldAngle;
    public Transform firePos;
    public LineRenderer lRend;
    public int barLoops;
    public GameObject equationBox;
    public GameObject topBox;

    // Start is called before the first frame update
    void Start()
    {
        lRend = lineRenderThing.GetComponent<LineRenderer>();
        barHinge = turret.GetComponent<HingeJoint2D>();
        motor = barHinge.motor;
        //limits = barHinge.limits;
    }

    // Update is called once per frame
    void Update()
    {
        //RestrictionMove(); //TEMPORARILY DISABLED
        //FindString(); //TEMP DISABLED
        //SetRestriction(); //TEMP DISABLED
        Aim(); 
    }
    public float equationValue = 0;



    // ALL OF THE MATH OPERATIONS!!!!
    // Given an x value, go through all of the equation components in this projectile and perform the necessary operations
    
    // How it works:
    // Search the list of boxes for the deepest box that hasn't been solved (bool "hasSolved" ==false), with a loop
    // Find the value of deepest box, have different function for types "Container", "Constant", "Variable", "Input", which solve differently
    // Repeat search for new deepest box, and solve value, and so on, until it finds the value of the big box, which is y

    // (Search for deepest container, in practice it may be the deepest of the first container (if there are two containers on the same level), and then it will go down any other container trails in order) 
    // Find a container from the list of boxes
    // Once container found, search it for another container, and so on, with a loop, until the deepest container is found
    // Find the value of the deepest container, and then set its value variable
    // Repeat the process of checking for the deepest container

    private float ContainerBoxSearch(float x) //, out float y)
    {
        //ComponentBox[] componentBoxes = equationBox.GetComponentsInChildren<ComponentBox>();

        if (equationBox.GetComponentInChildren<ComponentBox>() == null)
        {
            Debug.Log("No weapon loaded!!!");
            return 0;
        }

        topBox = equationBox.GetComponentInChildren<ComponentBox>().gameObject;

        topBox.GetComponent<ComponentBox>().Unsolve();

        Transform currentBox = topBox.transform;

        while (topBox.GetComponent<ComponentBox>().hasSolved == false)
        {
            bool foundDeepest = false;
            while (foundDeepest == false)
            {
                Transform currentDeepest = currentBox;
                //Debug.Log("Searching for deepest in : " + currentBox);
                foreach (Transform child in currentBox)
                {
                    //Debug.Log("Checking child : " + child);
                    if (currentBox.GetComponentsInChildren<ComponentBox>().Length > 1)
                    {
                        if (child.GetComponent<ComponentBox>() != null)
                        {
                            if (child.GetComponent<ComponentBox>().hasSolved == false)
                            {
                                //Debug.Log("Found new deepest child : " + child.GetComponent<ComponentBox>());
                                currentBox = child.GetComponent<ComponentBox>().transform;
                                break;
                            }
                        }
                    }
                }
                if (currentBox == currentDeepest)
                {
                    //Debug.Log("Found deepest! : " + currentBox);
                    foundDeepest = true;
                }
            }

            //Debug.Log("CurrentBox : " + currentBox);

            if (currentBox.GetComponent<ComponentBox>().type == ComponentBox.Type.Constant || currentBox.GetComponent<ComponentBox>().type == ComponentBox.Type.Input)
            {
                if (currentBox.GetComponentInChildren<InputField>().text != "")
                {
                    //Debug.Log("Input string : " + currentBox.GetComponentInChildren<InputField>().text);
                    currentBox.gameObject.GetComponent<ComponentBox>().value = float.Parse(currentBox.GetComponentInChildren<InputField>().text);
                }
                else
                {
                    currentBox.GetComponent<ComponentBox>().value = 0;
                }
                //Debug.Log("CurrentBox is Constant! Value is " + currentBox.GetComponent<ComponentBox>().value);
            }
            else if (currentBox.GetComponent<ComponentBox>().type == ComponentBox.Type.Container)
            {
                ComponentBox currentBoxComp = currentBox.GetComponent<ComponentBox>();
                currentBoxComp.value = 0; //currentBox.GetComponent<ComponentBox>().value = 0;

                if (currentBoxComp.operatorType == ComponentBox.OperatorType.Addition)
                {
                    currentBox.GetComponent<ComponentBox>().value = AdditionOperator(currentBox);
                }
                else if (currentBoxComp.operatorType == ComponentBox.OperatorType.Multiplication)
                {
                    currentBox.GetComponent<ComponentBox>().value = MultiplicationOperator(currentBox);
                }
                else if (currentBoxComp.operatorType == ComponentBox.OperatorType.Exponent)
                {
                    currentBox.GetComponent<ComponentBox>().value = Mathf.Pow(currentBox.GetComponentsInChildren<ComponentBox>()[1].value, currentBox.GetComponentsInChildren<ComponentBox>()[2].value);
                }

                //foreach (Transform child in currentBox)
                //{
                //if (child.GetComponent<ComponentBox>() != null)
                //{
                //currentBox.GetComponent<ComponentBox>().value += child.GetComponent<ComponentBox>().value;
                //}
                //}
                //Debug.Log("CurrentBox is Container! Value is " + currentBox.GetComponent<ComponentBox>().value);
            }
            else if (currentBox.GetComponent<ComponentBox>().type == ComponentBox.Type.Operator)
            {
                currentBox.GetComponent<ComponentBox>().value = 0;
                if (currentBox.GetComponent<ComponentBox>().operatorType == ComponentBox.OperatorType.Addition) //Makes positive
                {
                    currentBox.GetComponent<ComponentBox>().value = 1;
                }
                else if(currentBox.GetComponent<ComponentBox>().operatorType == ComponentBox.OperatorType.Subtraction) //Makes negative
                {
                    currentBox.GetComponent<ComponentBox>().value = -1;
                }
                //Debug.Log("CurrentBox is Operator! Value is " + currentBox.GetComponent<ComponentBox>().value);
            }
            else if (currentBox.GetComponent<ComponentBox>().type == ComponentBox.Type.Variable)
            {
                currentBox.GetComponent<ComponentBox>().value = x;
                if (currentBox.GetComponent<ComponentBox>().flipsSignWithX && x > 0)
                    currentBox.GetComponent<ComponentBox>().value *= -1;
                //Debug.Log("CurrentBox is Variable! Value is" + currentBox.GetComponent<ComponentBox>().value);
            }
            currentBox.GetComponent<ComponentBox>().hasSolved = true;
            currentBox = topBox.transform;
        }

        //if(aDirection == -1)

        return topBox.GetComponent<ComponentBox>().value;
    }

    private float AdditionOperator(Transform box)
    {
        float totalValue = 0; //Sets total to 0
        ComponentBox.OperatorType operatorType = ComponentBox.OperatorType.Addition; //The type of operator it is using, whether addition or subtraction, by default addition

        foreach (Transform child in box) //Loops through children of container
        {
            if (child.GetComponent<ComponentBox>() != null) //If child is actually box
            {
                if (child.GetComponent<ComponentBox>().type == ComponentBox.Type.Operator) //If the box is an operator
                {
                     // Sets operator type to the type of operator, determines addition or subtraction for the next number
                    operatorType = child.GetComponent<ComponentBox>().operatorType;
                    //Debug.Log("Child " + child.name + " is OPERATOR! OperatorType is now : " + operatorType + " because child" + child.name + " is " + child.GetComponent<ComponentBox>().operatorType);
                }
                else //else if(child.type == ComponentBox.Type.Constant)
                {
                    if (operatorType == ComponentBox.OperatorType.Addition)
                    {
                        totalValue += child.GetComponent<ComponentBox>().value;
                        //Debug.Log("Child " + child.name + " is CONSTANT! ADDED child value " + child.GetComponent<ComponentBox>().value + " to new total value " + totalValue + " because operatorType is " + operatorType);
                    }
                    else if (operatorType == ComponentBox.OperatorType.Subtraction)
                    {
                        totalValue -= child.GetComponent<ComponentBox>().value;
                        //Debug.Log("Child " + child.name + " is CONSTANT! SUBTRACTED child value " + child.GetComponent<ComponentBox>().value + " from new total value " + totalValue + " because operatorType is " + operatorType);
                    }
                }
            }
        }

        return totalValue;
    }

    private float MultiplicationOperator(Transform box)
    {
        float totalValue = box.GetComponentsInChildren<ComponentBox>()[1].value; //Gets first number
        //Debug.Log("Starting value of multiplication: " + totalValue + " From object : " + box.GetComponentsInChildren<ComponentBox>()[1].name);

        //ComponentBox.OperatorType operatorType = ComponentBox.OperatorType.Addition;

        foreach (Transform child in box)
        {
            if(child.GetComponent<ComponentBox>() != null)
            {
                if (child.GetComponent<ComponentBox>() != box.GetComponentsInChildren<ComponentBox>()[1]) //Makes sure it isn't the first number
                {
                    if ((child.GetComponent<ComponentBox>().type == ComponentBox.Type.Operator && child.GetComponent<ComponentBox>().operatorType == ComponentBox.OperatorType.Multiplication) == false) //Multiplication operators == 0, so they would make the equation 0 (change??)
                    {
                        //Debug.Log("MULTIPLYING VALUES : A " + totalValue + " B " + child.GetComponent<ComponentBox>().value + " From child " + child.name);

                        totalValue = totalValue * child.GetComponent<ComponentBox>().value;
                    }
                    else
                    {
                        //Debug.Log("Skipping CHILD " + child.name + "From multiplication because of last query");
                    }
                } 
            }
        }

        return totalValue;
    }




    private void FindString() //Obsolete??
    {
        if (textExpression != textbox.GetComponent<InputField>().text) //|| coordinates[0].y != player.position.y) //If new expression or player moved y
        {
            textExpression = textbox.GetComponent<InputField>().text;
            LineGenerator();
            LineRender();
        }
        LineRender();
        TurnTurret();

        //xMax = (lineVector.position.x - player.position.x);
        //xMax = xMax + (xMax / Mathf.Abs(xMax)) * 0.1f;
        //if (maxX != xMax) //If new restriction
        //{
        //maxX = xMax;
        //LineRender();
        //}
    }

    public void SetRestriction()
    {

        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z; //Camera.main.nearClipPlane;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos); //Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Mathf.Abs(mousePos.x - RestrictionLine.transform.position.x) < 1)
            {
                moveRestriction = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            moveRestriction = false;
        }
    }

    private void RestrictionMove()
    {
        if (moveRestriction == true)
        {
            //mousePos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineVector.position = new Vector3(mousePos.x, lineVector.position.y, lineVector.position.z);
        }
    }

    private void Aim()
    {
        lineCoords = new Vector3[lRend.positionCount];
        lRend.GetPositions(lineCoords);
        if (lineCoords.Length > 5)
        {
            Vector3 point1 = (lineCoords[1] - lineCoords[5]).normalized; //lineCoords[1]; //barPos.right;
            Vector3 point2 = lineCoords[5];// + hull.transform.position;
                                            //oldAngle = newAngle;
                                            //newAngle = Vector2.SignedAngle(point1, point2) - hull.transform.eulerAngles.z + barLoops * 360;
                                            //Debug.Log("Raw newAngle: " + newAngle + " Slope Angle: " + Vector2.SignedAngle(point1, point2)); //+ Vector3.AngleBetween(point1,point2));
            Vector2 dir = point1 - point2;
            newAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - hull.transform.eulerAngles.z;

            //if (oldAngle - newAngle > 330)
            //{
            //barLoops += 1;
            //newAngle += 360;
            //}
            //else if(oldAngle - newAngle < -330)
            //{
            //barLoops -= 1;
            //newAngle += -360;
            //}

            //newAngle += barLoops * 360;

            //if(oldAngle - newAngle < -180)
            //{
            //newAngle -= 360;
            //}
            //else if(oldAngle - newAngle > 180)
            //{
            //newAngle += 360;
            //}
            if (aDirection == -1)
            {
                newAngle -= 180;
            }

            if (newAngle < -90)
            {
                for (int i = 0; newAngle < -90; i++)
                {
                    if (newAngle + i * 180 > -90)
                    {
                        newAngle += i * 180;
                    }
                }
            }
            else if (newAngle > 90)
            {
                for (int i = 0; newAngle > 90; i++)
                {
                    if (newAngle + i * -180 > 90)
                    {
                        newAngle += i * -180;
                    }
                }
            }

            if (barHinge.jointAngle < newAngle)
            {
                limits.min = -90;
                limits.max = newAngle;
                barHinge.limits = limits;
                motor.motorSpeed = 1.5f * (newAngle - barHinge.jointAngle); //30;
                barHinge.motor = motor;
            }
            else if (barHinge.jointAngle > newAngle)
            {
                limits.min = newAngle;
                limits.max = 90;
                barHinge.limits = limits;
                motor.motorSpeed = 1.5f * (newAngle - barHinge.jointAngle); //-30;
                barHinge.motor = motor;
            }
        }

        //Debug.Log("Angle:" + newAngle + "||| Vectors:" + point1 + "----" + point2 + "||||| Old and New Angles: " + oldAngle + "; " + newAngle + "||" + "Loops: " + barLoops);
        //limits.max = newAngle; //tempOffset - Vector2.SignedAngle(point1, coordinates[20]);
        //limits.min = newAngle; //tempOffset - Vector2.SignedAngle(point1, coordinates[20]);
        //barHinge.limits = limits;
    }

    public void CallLineGenerator()
    {
        StartCoroutine(LineGenerator2());
    }

    public IEnumerator LineGenerator2()
    {
        Debug.Log("LineGenerator2.0 generating a new line!");
        pID += 1;
        int fID = pID;
        x = 0;
        dot = 0;
        coordinates = new Vector3[Math.Abs((int)(coordRender / dotFrequency))];
        lRend.positionCount = 0;

        //Debug.Log("Draw speed: " + drawSpeed / coordinates.Length + " dots per second");

        foreach(Vector3 position in coordinates)
        {
            if (pID == fID)
            {
                yield return new WaitForSeconds(drawSpeed / coordinates.Length); //WaitForFixedUpdate();

                y = ContainerBoxSearch(x);

                lRend.positionCount = dot + 1;
                lRend.SetPosition(dot, new Vector3(x, y, 0));

                dot++;
                if (aDirection == 1) // * transform.parent.GetComponentInChildren<TankMove>().direction == 1)
                {
                    //lineRenderThing.transform.localScale = new Vector3(-1, 1, 1); 
                    x -= dotFrequency;
                }
                else
                {
                    //lineRenderThing.transform.localScale = new Vector3(1, 1, 1);
                    x += dotFrequency;
                }
                //x += dotFrequency;

                //Debug.Log("POINT NUMBER " + dot + " SET! X = " + x + " Y = " + y);
            }
            else
            {
                lRend.positionCount = 0;
                Debug.Log("BREAK!");
                break;
            }
            if(dot == 31) // < 30 && dot > 20)
            {
                Debug.Log("Aiming!");
                Aim();
            }

        }
        if (fID == pID)
        {
            Aim();
        }

        yield return null;
    }

    public IEnumerator LineGenerator() //OUTDATED //public void LineGenerator()                                                                            //Creates an array of points when a new expression is made
    {
        Debug.Log("Generate new line!");
        pID = +1;
        int fID = pID;
        x = barPos.position.x - coordRender;                                                                //Resets x to zero when new graph made
        dot = 0;                                                                                            //Resets dot count to 0 when new graph made
        coordinates = new Vector3[Math.Abs((int)(coordRender * 2 / dotFrequency))];                         //Sets length of coord array, is doubled to account for both negative and positive

        for (float i = 0; i < coordinates.Length && i >= 0 && dot < coordinates.Length && pID == fID; i = i + dotFrequency) //Creates points for graph
        {
            //var parser = new ExpressionParser();
            //B83.ExpressionParser.Expression exp = parser.EvaluateExpression(textExpression + " + (0 * x)"); //Gets y, also adds 0x to not error when no x found
            //exp.Parameters["x"].Value = x;                                                                  //Defines x variable in the string
            //y = exp.Value;                                                                                  //Sets y

            equationBox.GetComponent<ComponentBox>().Unsolve();
            //equationBox.GetComponent<ComponentBox>().hasSolved = false;
            //equationBox.GetComponent<ComponentBox>().value = 0;
            y = ContainerBoxSearch(x);
            
            //Debug.Log("X : " + x + " Y : " + y);
            coordinates[dot] = new Vector3(x, y, 0); //(x + player.position.x, (float)y + player.position.y, 0);         //Sets a point with the calculated x and y, and also adds player position
            x = x + dotFrequency*aDirection;                                                                           //Increments x
            dot = dot + 1;                                                                                  //Increments the dot count to tell which point is being made

            Debug.Log("POINT SET! X = " + x + " Y = " + y);

            yield return new WaitForFixedUpdate();
            //yield return null;
        }
        LineRender();

        yield return null;
    }

    private void LineRender()
    {
        //var parser = new ExpressionParser();
        float start;
        int direction;

        if(aDirection == -1) //(hull.transform.localScale.x == -1)  //(lineVector.position.x > player.position.x)
        {
            start = 0;
            direction = 1;
            lim1 = firePos.localPosition.x; //player.position.x;
            lim2 = firePos.localPosition.x + coordRender * direction; //lineVector.position.x; //xMax
        }
        else
        {
            start = coordinates.Length - 2;
            direction = -1;
            lim1 = firePos.localPosition.x + coordRender * direction; //lineVector.position.x; //xMax;
            lim2 = firePos.localPosition.x; // player.position.x;
        }

        lRend.positionCount = 0;
        lRend.positionCount = Math.Abs((int)(xMax / dotFrequency)) + (int)0.1;
        //dot = 0;
        int point = 0;

        for (int i = (int)start; i < coordinates.Length && i >= 0; i = i + direction) //i < coordRender * 2 / dotFrequency - 1
        {
            //dot = dot + 1;
            if (coordinates[i].x > lim1 && coordinates[i].x < lim2 && point < lRend.positionCount)
            {
                lRend.SetPosition(point, coordinates[i]);
                point = point + 1;
            }
        }
        Aim();
        
    }

    public void ChangeDirection()
    {
        if (aDirection == -1)
        {
            aDirection = 1;
            firePos.eulerAngles = new Vector3(0,0,0);
            //turret.transform.localScale = new Vector3(1 * hull.transform.localScale.x, 1, 1);
        }
        else
        {
            aDirection = -1;
            firePos.eulerAngles = new Vector3(0, 0, 180);
            //turret.transform.localScale = new Vector3(-1 * hull.transform.localScale.x, 1, 1);
        }
        //Debug.Log("Turn turret!" + "oldangle: " + newAngle + " loops: " + barLoops);
        //TurnTurret();
        //pID++;
        StartCoroutine(LineGenerator2());
    }

    public void TurnTurret()
    {
        ChangeDirection();
        turret.transform.localScale = new Vector3(aDirection * hull.transform.localScale.x, 1, 1);
        firePos.localScale = new Vector3(aDirection * hull.transform.localScale.x, 1, 1);
    }
}
 

// A sample class from the interwebs that was designed to interpret string equations and produce a result.
// This was used during the early stages of developing the trajectory system, but has since been replaced by my own parsing system (literally built into the game via container blocks)

 /*
namespace B83.ExpressionParser
{
    public interface IValue
    {
        double Value { get; }
    }
    public class Number : IValue
    {
        private double m_Value;
        public double Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        public Number(double aValue)
        {
            m_Value = aValue;
        }
        public override string ToString()
        {
            return "" + m_Value + "";
        }
    }
    public class OperationSum : IValue
    {
        private IValue[] m_Values;
        public double Value
        {
            get { return m_Values.Select(v => v.Value).Sum(); }
        }
        public OperationSum(params IValue[] aValues)
        {
            // collapse unnecessary nested sum operations.
            List<IValue> v = new List<IValue>(aValues.Length);
            foreach (var I in aValues)
            {
                var sum = I as OperationSum;
                if (sum == null)
                    v.Add(I);
                else
                    v.AddRange(sum.m_Values);
            }
            m_Values = v.ToArray();
        }
        public override string ToString()
        {
            return "( " + string.Join(" + ", m_Values.Select(v => v.ToString()).ToArray()) + " )";
        }
    }
    public class OperationProduct : IValue
    {
        private IValue[] m_Values;
        public double Value
        {
            get { return m_Values.Select(v => v.Value).Aggregate((v1, v2) => v1 * v2); }
        }
        public OperationProduct(params IValue[] aValues)
        {
            m_Values = aValues;
        }
        public override string ToString()
        {
            return "( " + string.Join(" * ", m_Values.Select(v => v.ToString()).ToArray()) + " )";
        }

    }
    public class OperationPower : IValue
    {
        private IValue m_Value;
        private IValue m_Power;
        public double Value
        {
            get { return System.Math.Pow(m_Value.Value, m_Power.Value); }
        }
        public OperationPower(IValue aValue, IValue aPower)
        {
            m_Value = aValue;
            m_Power = aPower;
        }
        public override string ToString()
        {
            return "( " + m_Value + "^" + m_Power + " )";
        }

    }
    public class OperationNegate : IValue
    {
        private IValue m_Value;
        public double Value
        {
            get { return -m_Value.Value; }
        }
        public OperationNegate(IValue aValue)
        {
            m_Value = aValue;
        }
        public override string ToString()
        {
            return "( -" + m_Value + " )";
        }

    }
    public class OperationReciprocal : IValue
    {
        private IValue m_Value;
        public double Value
        {
            get { return 1.0 / m_Value.Value; }
        }
        public OperationReciprocal(IValue aValue)
        {
            m_Value = aValue;
        }
        public override string ToString()
        {
            return "( 1/" + m_Value + " )";
        }

    }

    public class MultiParameterList : IValue
    {
        private IValue[] m_Values;
        public IValue[] Parameters { get { return m_Values; } }
        public double Value
        {
            get { return m_Values.Select(v => v.Value).FirstOrDefault(); }
        }
        public MultiParameterList(params IValue[] aValues)
        {
            m_Values = aValues;
        }
        public override string ToString()
        {
            return string.Join(", ", m_Values.Select(v => v.ToString()).ToArray());
        }
    }

    public class CustomFunction : IValue
    {
        private IValue[] m_Params;
        private System.Func<double[], double> m_Delegate;
        private string m_Name;
        public double Value
        {
            get
            {
                if (m_Params == null)
                    return m_Delegate(null);
                return m_Delegate(m_Params.Select(p => p.Value).ToArray());
            }
        }
        public CustomFunction(string aName, System.Func<double[], double> aDelegate, params IValue[] aValues)
        {
            m_Delegate = aDelegate;
            m_Params = aValues;
            m_Name = aName;
        }
        public override string ToString()
        {
            if (m_Params == null)
                return m_Name;
            return m_Name + "( " + string.Join(", ", m_Params.Select(v => v.ToString()).ToArray()) + " )";
        }
    }
    public class Parameter : Number
    {
        public string Name { get; private set; }
        public override string ToString()
        {
            return Name + "[" + base.ToString() + "]";
        }
        public Parameter(string aName) : base(0)
        {
            Name = aName;
        }
    }

    public class Expression : IValue
    {
        public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
        public IValue ExpressionTree { get; set; }
        public double Value
        {
            get { return ExpressionTree.Value; }
        }
        public double[] MultiValue
        {
            get
            {
                var t = ExpressionTree as MultiParameterList;
                if (t != null)
                {
                    double[] res = new double[t.Parameters.Length];
                    for (int i = 0; i < res.Length; i++)
                        res[i] = t.Parameters[i].Value;
                    return res;
                }
                return null;
            }
        }
        public override string ToString()
        {
            return ExpressionTree.ToString();
        }
        public ExpressionDelegate ToDelegate(params string[] aParamOrder)
        {
            var parameters = new List<Parameter>(aParamOrder.Length);
            for (int i = 0; i < aParamOrder.Length; i++)
            {
                if (Parameters.ContainsKey(aParamOrder[i]))
                    parameters.Add(Parameters[aParamOrder[i]]);
                else
                    parameters.Add(null);
            }
            var parameters2 = parameters.ToArray();

            return (p) => Invoke(p, parameters2);
        }
        public MultiResultDelegate ToMultiResultDelegate(params string[] aParamOrder)
        {
            var parameters = new List<Parameter>(aParamOrder.Length);
            for (int i = 0; i < aParamOrder.Length; i++)
            {
                if (Parameters.ContainsKey(aParamOrder[i]))
                    parameters.Add(Parameters[aParamOrder[i]]);
                else
                    parameters.Add(null);
            }
            var parameters2 = parameters.ToArray();


            return (p) => InvokeMultiResult(p, parameters2);
        }
        double Invoke(double[] aParams, Parameter[] aParamList)
        {
            int count = System.Math.Min(aParamList.Length, aParams.Length);
            for (int i = 0; i < count; i++)
            {
                if (aParamList[i] != null)
                    aParamList[i].Value = aParams[i];
            }
            return Value;
        }
        double[] InvokeMultiResult(double[] aParams, Parameter[] aParamList)
        {
            int count = System.Math.Min(aParamList.Length, aParams.Length);
            for (int i = 0; i < count; i++)
            {
                if (aParamList[i] != null)
                    aParamList[i].Value = aParams[i];
            }
            return MultiValue;
        }
        public static Expression Parse(string aExpression)
        {
            return new ExpressionParser().EvaluateExpression(aExpression);
        }

        public class ParameterException : System.Exception { public ParameterException(string aMessage) : base(aMessage) { } }
    }
    public delegate double ExpressionDelegate(params double[] aParams);
    public delegate double[] MultiResultDelegate(params double[] aParams);



    public class ExpressionParser
    {
        private List<string> m_BracketHeap = new List<string>();
        private Dictionary<string, System.Func<double>> m_Consts = new Dictionary<string, System.Func<double>>();
        private Dictionary<string, System.Func<double[], double>> m_Funcs = new Dictionary<string, System.Func<double[], double>>();
        private Expression m_Context;

        public ExpressionParser()
        {
            var rnd = new System.Random();
            m_Consts.Add("PI", () => System.Math.PI);
            m_Consts.Add("e", () => System.Math.E);
            m_Funcs.Add("sqrt", (p) => System.Math.Sqrt(p.FirstOrDefault()));
            m_Funcs.Add("abs", (p) => System.Math.Abs(p.FirstOrDefault()));
            m_Funcs.Add("ln", (p) => System.Math.Log(p.FirstOrDefault()));
            m_Funcs.Add("floor", (p) => System.Math.Floor(p.FirstOrDefault()));
            m_Funcs.Add("ceiling", (p) => System.Math.Ceiling(p.FirstOrDefault()));
            m_Funcs.Add("round", (p) => System.Math.Round(p.FirstOrDefault()));

            m_Funcs.Add("sin", (p) => System.Math.Sin(p.FirstOrDefault()));
            m_Funcs.Add("cos", (p) => System.Math.Cos(p.FirstOrDefault()));
            m_Funcs.Add("tan", (p) => System.Math.Tan(p.FirstOrDefault()));

            m_Funcs.Add("asin", (p) => System.Math.Asin(p.FirstOrDefault()));
            m_Funcs.Add("acos", (p) => System.Math.Acos(p.FirstOrDefault()));
            m_Funcs.Add("atan", (p) => System.Math.Atan(p.FirstOrDefault()));
            m_Funcs.Add("atan2", (p) => System.Math.Atan2(p.FirstOrDefault(), p.ElementAtOrDefault(1)));
            //System.Math.Floor
            m_Funcs.Add("min", (p) => System.Math.Min(p.FirstOrDefault(), p.ElementAtOrDefault(1)));
            m_Funcs.Add("max", (p) => System.Math.Max(p.FirstOrDefault(), p.ElementAtOrDefault(1)));
            m_Funcs.Add("rnd", (p) =>
            {
                if (p.Length == 2)
                    return p[0] + rnd.NextDouble() * (p[1] - p[0]);
                if (p.Length == 1)
                    return rnd.NextDouble() * p[0];
                return rnd.NextDouble();
            });
        }

        public void AddFunc(string aName, System.Func<double[], double> aMethod)
        {
            if (m_Funcs.ContainsKey(aName))
                m_Funcs[aName] = aMethod;
            else
                m_Funcs.Add(aName, aMethod);
        }

        public void AddConst(string aName, System.Func<double> aMethod)
        {
            if (m_Consts.ContainsKey(aName))
                m_Consts[aName] = aMethod;
            else
                m_Consts.Add(aName, aMethod);
        }
        public void RemoveFunc(string aName)
        {
            if (m_Funcs.ContainsKey(aName))
                m_Funcs.Remove(aName);
        }
        public void RemoveConst(string aName)
        {
            if (m_Consts.ContainsKey(aName))
                m_Consts.Remove(aName);
        }

        int FindClosingBracket(ref string aText, int aStart, char aOpen, char aClose)
        {
            int counter = 0;
            for (int i = aStart; i < aText.Length; i++)
            {
                if (aText[i] == aOpen)
                    counter++;
                if (aText[i] == aClose)
                    counter--;
                if (counter == 0)
                    return i;
            }
            return -1;
        }

        void SubstitudeBracket(ref string aExpression, int aIndex)
        {
            int closing = FindClosingBracket(ref aExpression, aIndex, '(', ')');
            if (closing > aIndex + 1)
            {
                string inner = aExpression.Substring(aIndex + 1, closing - aIndex - 1);
                m_BracketHeap.Add(inner);
                string sub = "&" + (m_BracketHeap.Count - 1) + ";";
                aExpression = aExpression.Substring(0, aIndex) + sub + aExpression.Substring(closing + 1);
            }
            else throw new ParseException("Bracket not closed!");
        }

        IValue Parse(string aExpression)
        {
            aExpression = aExpression.Trim();
            int index = aExpression.IndexOf('(');
            while (index >= 0)
            {
                SubstitudeBracket(ref aExpression, index);
                index = aExpression.IndexOf('(');
            }
            if (aExpression.Contains(','))
            {
                string[] parts = aExpression.Split(',');
                List<IValue> exp = new List<IValue>(parts.Length);
                for (int i = 0; i < parts.Length; i++)
                {
                    string s = parts[i].Trim();
                    if (!string.IsNullOrEmpty(s))
                        exp.Add(Parse(s));
                }
                return new MultiParameterList(exp.ToArray());
            }
            else if (aExpression.Contains('+'))
            {
                string[] parts = aExpression.Split('+');
                List<IValue> exp = new List<IValue>(parts.Length);
                for (int i = 0; i < parts.Length; i++)
                {
                    string s = parts[i].Trim();
                    if (!string.IsNullOrEmpty(s))
                        exp.Add(Parse(s));
                }
                if (exp.Count == 1)
                    return exp[0];
                return new OperationSum(exp.ToArray());
            }
            else if (aExpression.Contains('-'))
            {
                string[] parts = aExpression.Split('-');
                List<IValue> exp = new List<IValue>(parts.Length);
                if (!string.IsNullOrEmpty(parts[0].Trim()))
                    exp.Add(Parse(parts[0]));
                for (int i = 1; i < parts.Length; i++)
                {
                    string s = parts[i].Trim();
                    if (!string.IsNullOrEmpty(s))
                        exp.Add(new OperationNegate(Parse(s)));
                }
                if (exp.Count == 1)
                    return exp[0];
                return new OperationSum(exp.ToArray());
            }
            else if (aExpression.Contains('*'))
            {
                string[] parts = aExpression.Split('*');
                List<IValue> exp = new List<IValue>(parts.Length);
                for (int i = 0; i < parts.Length; i++)
                {
                    exp.Add(Parse(parts[i]));
                }
                if (exp.Count == 1)
                    return exp[0];
                return new OperationProduct(exp.ToArray());
            }
            else if (aExpression.Contains('/'))
            {
                string[] parts = aExpression.Split('/');
                List<IValue> exp = new List<IValue>(parts.Length);
                if (!string.IsNullOrEmpty(parts[0].Trim()))
                    exp.Add(Parse(parts[0]));
                for (int i = 1; i < parts.Length; i++)
                {
                    string s = parts[i].Trim();
                    if (!string.IsNullOrEmpty(s))
                        exp.Add(new OperationReciprocal(Parse(s)));
                }
                return new OperationProduct(exp.ToArray());
            }
            else if (aExpression.Contains('^'))
            {
                int pos = aExpression.IndexOf('^');
                var val = Parse(aExpression.Substring(0, pos));
                var pow = Parse(aExpression.Substring(pos + 1));
                return new OperationPower(val, pow);
            }
            int pPos = aExpression.IndexOf("&");
            if (pPos > 0)
            {
                string fName = aExpression.Substring(0, pPos);
                foreach (var M in m_Funcs)
                {
                    if (fName == M.Key)
                    {
                        var inner = aExpression.Substring(M.Key.Length);
                        var param = Parse(inner);
                        var multiParams = param as MultiParameterList;
                        IValue[] parameters;
                        if (multiParams != null)
                            parameters = multiParams.Parameters;
                        else
                            parameters = new IValue[] { param };
                        return new CustomFunction(M.Key, M.Value, parameters);
                    }
                }
            }
            foreach (var C in m_Consts)
            {
                if (aExpression == C.Key)
                {
                    return new CustomFunction(C.Key, (p) => C.Value(), null);
                }
            }
            int index2a = aExpression.IndexOf('&');
            int index2b = aExpression.IndexOf(';');
            if (index2a >= 0 && index2b >= 2)
            {
                var inner = aExpression.Substring(index2a + 1, index2b - index2a - 1);
                int bracketIndex;
                if (int.TryParse(inner, out bracketIndex) && bracketIndex >= 0 && bracketIndex < m_BracketHeap.Count)
                {
                    return Parse(m_BracketHeap[bracketIndex]);
                }
                else
                    throw new ParseException("Can't parse substitude token");
            }
            double doubleValue;
            if (double.TryParse(aExpression, out doubleValue))
            {
                return new Number(doubleValue);
            }
            if (ValidIdentifier(aExpression))
            {
                if (m_Context.Parameters.ContainsKey(aExpression))
                    return m_Context.Parameters[aExpression];
                var val = new Parameter(aExpression);
                m_Context.Parameters.Add(aExpression, val);
                return val;
            }

            throw new ParseException("Reached unexpected end within the parsing tree");
        }

        private bool ValidIdentifier(string aExpression)
        {
            aExpression = aExpression.Trim();
            if (string.IsNullOrEmpty(aExpression))
                return false;
            if (aExpression.Length < 1)
                return false;
            if (aExpression.Contains(" "))
                return false;
            if (!"abcdefghijklmnopqrstuvwxyz§$".Contains(char.ToLower(aExpression[0])))
                return false;
            if (m_Consts.ContainsKey(aExpression))
                return false;
            if (m_Funcs.ContainsKey(aExpression))
                return false;
            return true;
        }

        public Expression EvaluateExpression(string aExpression)
        {
            var val = new Expression();
            m_Context = val;
            val.ExpressionTree = Parse(aExpression);
            m_Context = null;
            m_BracketHeap.Clear();
            return val;
        }

        public double Evaluate(string aExpression)
        {
            return EvaluateExpression(aExpression).Value;
        }
        public static double Eval(string aExpression)
        {
            return new ExpressionParser().Evaluate(aExpression);
        }

        public class ParseException : System.Exception { public ParseException(string aMessage) : base(aMessage) { } }
    }
}*/
