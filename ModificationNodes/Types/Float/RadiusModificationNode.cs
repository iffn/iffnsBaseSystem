using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class RadiusModificationNode : FloatingModificationNode
    {
        MailboxLineRanged radiusValue;
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
                transform.position = value;

                Vector3 offset = transform.localPosition - localCenter;

                offset = new Vector3(offset.x, 0, offset.z); //ToDo: Do it relative to axis instad of just up

                radiusValue.Val = offset.magnitude;

                //LinkedOrganizer.ApplyBuildParameters();
            }
        }

        public void Setup(ModificationOrganizer linkedOrganizer, MailboxLineRanged radiusValue, Vector3 localCenter, Vector3 axis)
        {
            base.setup(linkedOrganizer: linkedOrganizer);

            this.radiusValue = radiusValue;
            this.localCenter = localCenter;
            this.axis = axis;

            transform.parent = linkedOrganizer.LinkedObject.transform;
            transform.localPosition = Vector3.zero;

            if (axis != Vector3.up)
            {
                Debug.Log("Warning with Radius Modification Node: Currently only Up axis supported, new value = " + axis);
            }
        }

        public override bool ColliderActivationState
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