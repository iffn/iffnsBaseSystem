using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class LinearModificationNode : FloatingModificationNode
    {
        MailboxLineRanged distanceValue;
        Vector3 localCenter;
        Vector3 axis;

        public override Vector3 AbsolutePosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = new Vector3(localCenter.x, value.y, localCenter.z);

                Vector3 offset = transform.localPosition - localCenter;

                offset = new Vector3(offset.x, 0, offset.z); //ToDo: Do it relative to axis instad of just up

                distanceValue.Val = offset.y;

                LinkedObject.ApplyBuildParameters();
            }
        }

        public void Setup(BaseGameObject linkedObject, MailboxLineRanged distanceValue, Vector3 localCenter, Vector3 axis)
        {
            base.setup(linkedObject: linkedObject);

            this.distanceValue = distanceValue;
            this.localCenter = localCenter;
            this.axis = axis;

            transform.parent = linkedObject.transform;
            transform.localPosition = Vector3.zero;

            if (axis != Vector3.up)
            {
                Debug.Log("Warning with Radius Modification Node: Currently only Up axis supported, new value = " + axis);
            }
        }

        protected override bool NodeColliderState
        {
            set
            {
                transform.GetComponent<Collider>().enabled = value;
            }
        }

        public override void UpdatePosition()
        {

        }
    }
}