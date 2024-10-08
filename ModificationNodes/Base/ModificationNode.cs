﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class ModificationNode : MonoBehaviour
    {
        protected readonly float heightOvershoot = 0.01f;
        protected readonly float widthOvershoot = 0.01f;

        public BaseGameObject LinkedObject { get; private set; }

        protected virtual void setup(BaseGameObject linkedObject)
        {
            LinkedObject = linkedObject;
            linkedObject.AddModificationNode(this);
        }

        public virtual void Show(bool activateCollider)
        {
            gameObject.SetActive(true);

            ColliderActivationState = activateCollider;

            UpdatePosition();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool ColliderActivationState
        {
            set
            {
                if (LinkedObject == null)
                {
                    //Debug.LogWarning("Setup not run on " + transform.name);
                    return;
                }

                LinkedObject.ColliderActivationState = value;
                NodeColliderState = value;
            }
        }

        protected abstract bool NodeColliderState { set; }

        public abstract void UpdatePosition();

        //public abstract void UpdateNodeSize();

    }
}