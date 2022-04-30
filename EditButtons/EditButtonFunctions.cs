using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEditButtonFunction
{
    public string ButtonName { get; }

    public delegate void ButtonFunction();

    public ButtonFunction Function { get; }

    protected BaseEditButtonFunction(string buttonName, ButtonFunction buttonFunction)
    {
        this.ButtonName = buttonName;
        this.Function = buttonFunction;

        AdditionalFunctions = new List<ButtonFunction>();
    }

    List<ButtonFunction> AdditionalFunctions;
}

public class SingleButtonBaseEditFunction : BaseEditButtonFunction
{
    public SingleButtonBaseEditFunction(string buttonName, ButtonFunction buttonFunction) : base(buttonName: buttonName, buttonFunction: buttonFunction)
    {

    }
}

