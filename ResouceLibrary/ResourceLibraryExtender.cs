using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class ResourceLibraryExtender : MonoBehaviour
    {
        public abstract BaseVirtualObject TryGetVirtualObjectFromStringIdentifier(string identifier, IBaseObject superObject);

    }
}