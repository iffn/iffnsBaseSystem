using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class FloatingModificationNode : ModificationNode
    {
        public abstract Vector3 AbsolutePosition { get; set; }
    }
}