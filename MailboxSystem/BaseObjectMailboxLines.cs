using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class MailboxLineSingleSubObject
    {
        //readonly static char quote = '"';

        public string ValueName;

        IBaseObject subObject;

        readonly IBaseObject superObject;

        public IBaseObject SubObject
        {
            get
            {
                return subObject;
            }
            set
            {
                subObject = value;
            }
        }

        public MailboxLineSingleSubObject(string valueName, IBaseObject subObject, Mailbox objectHolder)
        {
            this.ValueName = valueName;
            this.subObject = subObject;
            objectHolder.AddSingleObjectLine(this);
            superObject = objectHolder.ObjectHolder;
        }

        public MailboxLineSingleSubObject(string valueName, Mailbox objectHolder)
        {
            this.ValueName = valueName;
            objectHolder.AddSingleObjectLine(this);
            superObject = objectHolder.ObjectHolder;
        }

        public bool TryRemoveSubObject(IBaseObject subObject)
        {
            if (this.subObject == subObject)
            {
                subObject = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> JSONStrings
        {

            get
            {
                List<string> returnList = new List<string>();

                returnList.Add(MyStringComponents.quote + "Type" + MyStringComponents.quote + ": " + MyStringComponents.quote + subObject.IdentifierString + MyStringComponents.quote + ",");

                returnList.AddRange(subObject.JSONBuildParameters);

                //Removing last "," should not be needed since already done by Mailbox
                //returnList[returnList.Count - 1] = returnList[returnList.Count - 1].Remove(returnList[returnList.Count - 1].Length - 1); //Remove last ","

                for (int line = 0; line < returnList.Count; line++)
                {
                    returnList[line] = returnList[line].Insert(0, MyStringComponents.tab);
                }

                returnList.Insert(0, MyStringComponents.quote + ValueName + MyStringComponents.quote + ":");
                returnList.Insert(1, "{");
                returnList.Add("},");

                return returnList;
            }
            set
            {
                subObject = StaticSaveAndLoadSystem.GetFullObjectFromJSONString(JSONString: value, superObject: superObject);

                if (subObject == null) superObject.Failed = true;
            }
        }
    }

    public class MailboxLineMultipleSubObject
    {
        //static readonly char quote = '"';

        public string ValueName;

        List<IBaseObject> subObjects;

        IBaseObject superObject;

        public int NumberOfObjects
        {
            get
            {
                return subObjects.Count;
            }
        }

        public void ClearAndDestroySubObjects()
        {
            foreach (IBaseObject subObject in subObjects)
            {
                if (subObject is IBaseObject)
                {
                    BaseGameObject subGameObject = subObject as BaseGameObject;

                    GameObject.Destroy(subGameObject);
                }
            }

            subObjects.Clear();
        }

        public List<IBaseObject> SubObjects
        {
            get
            {
                return subObjects;
                //return new List<IBaseObject>(subObjects);
            }
        }

        public MailboxLineMultipleSubObject(string valueName, Mailbox objectHolder)
        {
            this.ValueName = valueName;
            this.subObjects = new List<IBaseObject>();
            objectHolder.AddMultipleObjectLine(this);
            superObject = objectHolder.ObjectHolder;
        }

        public MailboxLineMultipleSubObject(string valueName, List<IBaseObject> subObjects, Mailbox objectHolder)
        {
            this.ValueName = valueName;
            this.subObjects = subObjects;
            objectHolder.AddMultipleObjectLine(this);
            superObject = objectHolder.ObjectHolder;
        }

        public void AddObject(IBaseObject newObject)
        {
            if (subObjects.Contains(newObject) == false)
            {
                subObjects.Add(newObject);
            }
        }

        public bool Contains(IBaseObject testObject)
        {
            return subObjects.Contains(testObject);
        }

        public void InsertAfterObject(IBaseObject newObject, IBaseObject oldObject)
        {
            int index = subObjects.IndexOf(oldObject);

            if (index != -1) //If found
            {
                subObjects.Insert(index: index + 1, item: newObject);
            }
            else //If not found
            {
                subObjects.Add(item: newObject);
            }
        }

        public void InsertBeforeObject(IBaseObject newObject, IBaseObject oldObject)
        {
            int index = subObjects.IndexOf(oldObject);

            if (index != -1) //If found
            {
                subObjects.Insert(index: index, item: newObject);
            }
            else //If not found
            {
                subObjects.Insert(index: 0, item: newObject);
            }
        }


        public bool TryRemoveSubObject(IBaseObject subObject)
        {
            bool worked = subObjects.Remove(subObject);

            return worked;
        }

        public List<string> JSONStrings
        {
            get
            {
                List<string> returnList = new List<string>();

                foreach (IBaseObject subObject in subObjects)
                {
                    List<string> subReturnList = new List<string>(subObject.JSONBuildParameters);

                    for (int line = 0; line < subReturnList.Count; line++)
                    {
                        subReturnList[line] = subReturnList[line].Insert(0, "\t\t");
                    }

                    subReturnList.Insert(0, "\t\t" + MyStringComponents.quote + "Type" + MyStringComponents.quote + ": " + MyStringComponents.quote + subObject.IdentifierString + MyStringComponents.quote + ",");

                    subReturnList.Insert(0, "\t{");

                    subReturnList.Add("\t},");

                    returnList.AddRange(subReturnList);
                }

                if (returnList.Count != 0)
                {
                    returnList[returnList.Count - 1] = returnList[returnList.Count - 1].Remove(returnList[returnList.Count - 1].Length - 1); //remove last comma from "},"
                }

                returnList.Insert(0, MyStringComponents.quote + ValueName + MyStringComponents.quote + ":");

                returnList.Insert(1, "[");
                returnList.Add("],");

                return returnList;
            }
            set
            {
                if (value.Count == 0) return; //Ignore empty fields

                int currentBracketCounter = 0;

                List<string> SubJSONList = new List<string>();

                int startingIndex;

                if (value[0] == "[")
                {
                    startingIndex = 1;
                }
                else
                {
                    startingIndex = 0;
                }

                for (int i = startingIndex; i < value.Count; i++)
                {
                    string currentLine = value[i];

                    if (currentLine == "{")
                    {
                        currentBracketCounter++;
                        if (currentBracketCounter == 1) continue;
                    }
                    else if (currentLine == "}," || currentLine == "}")
                    {
                        currentBracketCounter--;
                        if (currentBracketCounter == 0)
                        {
                            IBaseObject subObject = StaticSaveAndLoadSystem.GetFullObjectFromJSONString(JSONString: SubJSONList, superObject: superObject);

                            if(subObject != null)
                            {
                                if (subObjects.Contains(subObject) == false)
                                {
                                    subObjects.Add(subObject);
                                }
                            }

                            SubJSONList.Clear();
                            continue;
                        }
                    }
                    SubJSONList.Add(currentLine);
                }
            }
        }
    }
}