using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLibraryIntegrator : MonoBehaviour
{
    public Color ModificationNodeObjectMoverColor;
    public Color ModificationNodeSizeAdjusterColor;

    public void Setup()
    {
        ColorLibrary.Setp(integrator: this);
    }
}

public static class ColorLibrary
{
    static ColorLibraryIntegrator integrator;

    public static void Setp(ColorLibraryIntegrator integrator)
    {
        ColorLibrary.integrator = integrator;
    }

    public static Color ModificationNodeObjectMoverColor
    {
        get
        {
            return integrator.ModificationNodeObjectMoverColor;
        }
    }

    public static Color ModificationNodeSizeAdjusterColor
    {
        get
        {
            return integrator.ModificationNodeSizeAdjusterColor;
        }
    }
}
