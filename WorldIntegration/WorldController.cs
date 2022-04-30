using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldController : BaseGameObject
{
    [SerializeField] ResourceLibraryIntegrator CurrentResourceLibraryIntegrator = null; //ToDo: Rename to Human building Resource Library
    [SerializeField] ColorLibraryIntegrator CurrentColorLibraryIntegrator = null;
    [SerializeField] ModificationNodeLibraryIntegrator CurrentModificationNodeLibraryIntegrator = null; //ToDo: Move to Builder library
    [SerializeField] UnityPrefabLibaryIntegrator CurrentUnityPrefabLibaryIntegrator = null;
    [SerializeField] MaterialLibraryIntegrator CurrentMaterialLibraryIntegrator = null;

    public static string identifierString = "World object";

    public override string IdentifierString
    {
        get
        {
            return identifierString;
        }
    }

    public override void PlaytimeUpdate()
    {
        NonOrderedPlaytimeUpdate();
    }

    public override void InternalUpdate()
    {
        foreach (IBaseObject subObject in SubObjects)
        {
            subObject.InternalUpdate();
        }
    }

    public override void Setup(IBaseObject superObject)
    {
        base.Setup(superObject);

        CurrentResourceLibraryIntegrator.Setup();
        CurrentColorLibraryIntegrator.Setup();
        CurrentModificationNodeLibraryIntegrator.Setup();
        CurrentUnityPrefabLibaryIntegrator.Setup();
        CurrentMaterialLibraryIntegrator.Setup();
    }

    public override void ApplyBuildParameters()
    {
        
    }

    // Start is called before the first frame update
    protected abstract void Start();

    // Update is called once per frame
    protected abstract void Update();
}
