﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class WorldController : BaseGameObject
    {
        [SerializeField] ColorLibraryIntegrator CurrentColorLibraryIntegrator = null;
        [SerializeField] UnityPrefabLibaryIntegrator CurrentUnityPrefabLibaryIntegrator = null;

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

            CurrentColorLibraryIntegrator.Setup();
            CurrentUnityPrefabLibaryIntegrator.Setup();
            
        }

        public override void ApplyBuildParameters()
        {

        }

        // Start is called before the first frame update
        protected abstract void Start();

        // Update is called once per frame
        protected abstract void Update();

        public static class LayerManager
        {
            public enum Layers
            {
                Default,
                TransparentFX,
                IgnoreRaycast,
                Undefined,
                Water,
                UI,
                SelectOnly,
                NoSelection
            }

            public static int GetLayerNumber(Layers layer)
            {
                return (int)layer;
            }

            public static int SelectionLayerMask
            {
                get
                {
                    int returnValue = (1 << GetLayerNumber(Layers.SelectOnly)) | (1 << GetLayerNumber(Layers.Default));

                    //Debug.Log(System.Convert.ToString(returnValue, 2).PadLeft(32, '0'));

                    return returnValue;
                }
            }
        }
    }
}