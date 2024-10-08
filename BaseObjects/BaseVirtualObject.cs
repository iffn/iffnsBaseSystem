﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public abstract class BaseVirtualObject : IBaseObject
    {
        BaseSupportObject baseSupportObject;

        public bool Failed
        {
            get
            {
                return baseSupportObject.failed;
            }
            set
            {
                baseSupportObject.failed = value;
            }
        }

        public void DestroyFailedSubObjects()
        {
            for (int i = 0; i < SubObjects.Count; i++)
            {
                if (SubObjects[i].Failed)
                {
                    SubObjects[i].DestroyObject();
                    i--;
                }
                else
                {
                    SubObjects[i].DestroyFailedSubObjects();
                }
            }
        }

        public string IdentifierString
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected Mailbox CurrentMailbox
        {
            get
            {
                return baseSupportObject.Mailbox;
            }
        }

        public List<MailboxLineSingle> SingleMailboxLines
        {
            get
            {
                return CurrentMailbox.SingleMailboxLines;
            }
        }

        public IBaseObject SuperObject
        {
            get
            {
                return baseSupportObject.SuperObject;
            }
        }

        public List<IBaseObject> SubObjects
        {
            get
            {
                return baseSupportObject.SubObjects;
            }
        }

        //Save and load system
        public List<string> JSONBuildParameters
        {
            get
            {
                List<string> returnValue = new List<string>();



                returnValue.AddRange(baseSupportObject.JSONBuildParameters);

                foreach (IBaseObject subObject in SubObjects)
                {
                    returnValue.AddRange(subObject.JSONBuildParameters);
                }

                return baseSupportObject.JSONBuildParameters;
            }

            set
            {
                baseSupportObject.JSONBuildParameters = value;
            }
        }

        //Constructor
        public BaseVirtualObject(IBaseObject superObject)
        {
            Setup(superObject);
        }

        //Base Functions
        public void Setup(IBaseObject superObject)
        {
            if (baseSupportObject == null)
            {
                baseSupportObject = new BaseSupportObject(baseObject: this, superObject: superObject);
            }
        }

        public abstract void ApplyBuildParameters();

        public abstract void PlaytimeUpdate();

        public abstract void InternalUpdate();

        protected void NonOrderedApplyBuildParameters()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.ApplyBuildParameters();
            }
        }

        protected void NonOrderedPlaytimeUpdate()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.PlaytimeUpdate();
            }
        }

        protected void NonOrderedInternalUpdate()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.InternalUpdate();
            }
        }

        public void RemoveSubObject(IBaseObject subObject)
        {
            CurrentMailbox.RemoveSubObject(subObject);
        }

        public virtual void DestroyObject()
        {
            DestroyObjectWithoutRemove();

            SuperObject.RemoveSubObject(this);
        }

        public void DestroyObjectWithoutRemove()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.DestroyObjectWithoutRemove();
            }
        }

        public abstract void ResetObject();

        protected void baseReset()
        {
            foreach (IBaseObject subObject in SubObjects)
            {
                subObject.DestroyObject(); //Also removes reference in super object but is somewhat slow since it first needs to find itself
            }
        }

        //Edit Button Functions
        public List<BaseEditButtonFunction> EditButtons
        {
            get
            {
                return baseSupportObject.EditButtonFunctions;
            }
        }

        protected void AddEditButtonFunctionToBeginning(BaseEditButtonFunction function)
        {
            baseSupportObject.AddEditButtonFunctionToBeginning(function);
        }

        protected void AddEditButtonFunctionToEnd(BaseEditButtonFunction function)
        {
            baseSupportObject.AddEditButtonFunctionToEnd(function);
        }

        protected void ResetEditButtons()
        {
            ResetEditButtons();
        }
    }
}