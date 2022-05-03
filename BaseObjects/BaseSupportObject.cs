using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class BaseSupportObject
    {
        private IBaseObject linkedBaseObject;
        public IBaseObject LinkedBaseObject
        {
            get
            {
                return linkedBaseObject;
            }
        }

        //Hierarchy stuff
        IBaseObject superObject;
        public IBaseObject SuperObject
        {
            get
            {
                return superObject;
            }
        }

        //List<IBaseObject> subObjects;
        public List<IBaseObject> SubObjects
        {
            get
            {
                return Mailbox.SubObjects;

                //return subObjects;
            }
        }

        public enum HierarchyTypes
        {
            Bottom,
            Middle,
            Top,
            Single
        }

        public HierarchyTypes HierarchyType
        {
            get
            {
                if (SuperObject == null)
                {
                    if (SubObjects.Count == 0)
                    {
                        return HierarchyTypes.Single;
                    }
                    else
                    {
                        return HierarchyTypes.Top;
                    }
                }
                else
                {
                    if (SubObjects.Count == 0)
                    {
                        return HierarchyTypes.Bottom;
                    }
                    else
                    {
                        return HierarchyTypes.Middle;
                    }
                }
            }
        }


        //Mailbox System
        Mailbox mailbox;
        public Mailbox Mailbox
        {
            get
            {
                return mailbox;
            }
        }

        //Build parameters
        protected MailboxLineString buildParameterName;

        public string Name
        {
            get
            {
                return buildParameterName.Val;
            }
            set
            {
                buildParameterName.Val = value;
            }
        }

        //Save and load system
        public List<string> JSONBuildParameters
        {
            get
            {
                return Mailbox.JSONBuildParameters;
            }

            set
            {
                Mailbox.JSONBuildParameters = value;
            }
        }

        //Edit Button Functions
        List<BaseEditButtonFunction> editButtonFunctions;

        public List<BaseEditButtonFunction> EditButtonFunctions
        {
            get
            {
                return new List<BaseEditButtonFunction>(editButtonFunctions);
            }
        }

        public void AddEditButtonFunctionToBeginning(BaseEditButtonFunction function)
        {
            editButtonFunctions.Insert(index: 0, item: function);
        }

        public void AddEditButtonFunctionToEnd(BaseEditButtonFunction function)
        {
            editButtonFunctions.Add(item: function);
        }

        public void ResetEditButtons()
        {
            editButtonFunctions.Clear();
        }

        //Constructor
        public BaseSupportObject(IBaseObject baseObject, string name, IBaseObject superObject)
        {
            linkedBaseObject = baseObject;

            //Create mailbox objects
            mailbox = new Mailbox(linkedBaseObject);
            //subObjects = new List<IBaseObject>();
            this.superObject = superObject;

            buildParameterName = new MailboxLineString("Name", Mailbox, Mailbox.ValueType.buildParameter);

            editButtonFunctions = new List<BaseEditButtonFunction>();
        }
    }
}