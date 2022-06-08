using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class ModificationNode : MonoBehaviour
    {
        protected readonly float heightOvershoot = 0.01f;
        protected readonly float widthOvershoot = 0.01f;

        public ModificationOrganizer LinkedOrganizer { get; private set; }

        protected virtual void setup(ModificationOrganizer linkedOrganizer)
        {
            LinkedOrganizer = linkedOrganizer;
            linkedOrganizer.AddModificationNode(this);
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

        public abstract bool ColliderActivationState { set; }

        public abstract void UpdatePosition();

        //public abstract void UpdateNodeSize();

    }
}