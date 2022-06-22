using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class SingleStringComponents
    {
        public bool IsValid { get; private set; } = false;

        public string ValueName;

        public string ValueString;

        public SingleStringComponents()
        {

        }

        public SingleStringComponents(string valueName, string valueString)
        {
            this.ValueName = valueName;
            this.ValueString = valueString;
            IsValid = true;
        }

        public string JSONString
        {
            get
            {
                string returnString = MyStringComponents.quote + ValueName + MyStringComponents.quote + ": " + ValueString;

                return returnString;
            }

            set
            {
                if (value[0] != '"')
                {
                    Debug.LogWarning("Error when assigning json string: String does not start with quote. JSon line = " + value);
                    IsValid = false;
                    return;
                }

                int colonLocation = value.IndexOf("\":");

                if (colonLocation < 4 || colonLocation < 3)
                {
                    Debug.LogWarning("Error when assigning json string: Colon arrangement not found in the correct location. JSon line = " + value);
                    IsValid = false;
                    return;
                }

                ValueName = value.Substring(1, colonLocation - 1);

                if (value.Length - colonLocation < 3)
                {
                    IsValid = false;
                    return;
                }

                ValueString = value.Substring(colonLocation + 3);

                if (ValueString[ValueString.Length - 1] == ',')
                {
                    ValueString = ValueString.Remove(ValueString.Length - 1);
                }

                //ValueString = ValueString.Replace(myStringComponents.quote.ToString(), "");

                IsValid = true;
            }
        }
    }

    public abstract class MailboxLineSingle
    {
        //protected const string quote = "\"";

        SingleStringComponents stringComponents;
        Mailbox objectHolder;
        Mailbox.ValueType valueType;

        protected abstract string typeName { get; }
        protected abstract string JSONValueString { get; set; }

        public string Name
        {
            get
            {
                return stringComponents.ValueName;
            }
        }

        public string ValueString
        {
            get
            {
                return stringComponents.ValueString;
            }
        }

        public MailboxLineSingle(string valueName, Mailbox objectHolder, Mailbox.ValueType valueType)
        {
            this.objectHolder = objectHolder;
            this.valueType = valueType;

            objectHolder.AddParameterLine(this, valueType);

            stringComponents = new SingleStringComponents(valueName: valueName, valueString: null);
        }

        public void Destroy()
        {
            objectHolder.RemoveParameterLine(removalLine: this, valueType: valueType);
        }

        public bool TryAssignValue(SingleStringComponents components)
        {
            if (!components.ValueName.Equals(stringComponents.ValueName)) return false;
            JSONValueString = components.ValueString;
            return true;
        }

        public string JSONOutputString
        {
            get
            {
                stringComponents.ValueString = JSONValueString;

                return stringComponents.JSONString;
            }
        }
    }

    public class Mailbox
    {
        public enum ValueType
        {
            mailboxLine,
            buildParameter
        }

        List<MailboxLineSingle> InputParameters;
        List<MailboxLineSingle> BuildParameters;
        List<MailboxLineSingleSubObject> SingleSubObjects;
        List<MailboxLineMultipleSubObject> MultipleSubObjects;

        public IBaseObject ObjectHolder { get; private set; }

        public List<IBaseObject> SubObjects
        {
            get
            {
                List<IBaseObject> returnList = new List<IBaseObject>();

                foreach (MailboxLineSingleSubObject line in SingleSubObjects)
                {
                    returnList.Add(line.SubObject);
                }

                foreach (MailboxLineMultipleSubObject line in MultipleSubObjects)
                {
                    returnList.AddRange(line.SubObjects);
                }

                return returnList;
            }
        }

        public List<MailboxLineSingle> SingleMailboxLines
        {
            get
            {
                return BuildParameters;
            }
        }

        public Mailbox(IBaseObject objectHolder)
        {
            this.ObjectHolder = objectHolder;

            InputParameters = new List<MailboxLineSingle>();
            BuildParameters = new List<MailboxLineSingle>();
            SingleSubObjects = new List<MailboxLineSingleSubObject>();
            MultipleSubObjects = new List<MailboxLineMultipleSubObject>();
        }

        public void AddParameterLine(MailboxLineSingle newLine, ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.mailboxLine:
                    //if (mailboxLines == null) mailboxLines = new List<MailboxLine>();
                    InputParameters.Add(newLine);
                    break;
                case ValueType.buildParameter:
                    //if (buildParameters == null) buildParameters = new List<MailboxLine>();
                    BuildParameters.Add(newLine);
                    break;
            }
        }

        public void RemoveParameterLine(MailboxLineSingle removalLine, ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.mailboxLine:
                    InputParameters.Remove(removalLine);
                    break;
                case ValueType.buildParameter:
                    BuildParameters.Remove(removalLine);
                    break;
            }
        }

        public void AddSingleObjectLine(MailboxLineSingleSubObject singleObject)
        {
            SingleSubObjects.Add(singleObject);

            ObjectHolder.SubObjects.Add(singleObject.SubObject);
        }

        public void AddMultipleObjectLine(MailboxLineMultipleSubObject mutlipleObject)
        {
            MultipleSubObjects.Add(mutlipleObject);
            foreach (IBaseObject subObject in mutlipleObject.SubObjects)
            {
                ObjectHolder.SubObjects.Add(subObject);
            }
        }

        public void RemoveSubObject(IBaseObject subObject)
        {
            bool found = false;

            foreach (MailboxLineSingleSubObject singleLine in SingleSubObjects)
            {
                bool newFound = singleLine.TryRemoveSubObject(subObject);

                if (newFound) found = newFound;
            }

            foreach (MailboxLineMultipleSubObject multipleLine in MultipleSubObjects)
            {
                bool newFound = multipleLine.TryRemoveSubObject(subObject);

                if (newFound) found = newFound;
            }

            if (!found) Debug.LogWarning("Error when removing SubObject: SubObject not found. Type = " + subObject.IdentifierString);
        }

        public List<string> JSONBuildParameters
        {
            get
            {
                List<string> outputList = new List<string>();

                foreach (MailboxLineSingle currentLine in BuildParameters)
                {
                    outputList.Add(currentLine.JSONOutputString + ",");
                }

                foreach (MailboxLineSingleSubObject currentObject in SingleSubObjects)
                {
                    outputList.AddRange(currentObject.JSONStrings);
                }

                foreach (MailboxLineMultipleSubObject currentObjects in MultipleSubObjects)
                {
                    outputList.AddRange(currentObjects.JSONStrings);
                }

                outputList[outputList.Count - 1] = outputList[outputList.Count - 1].Remove(outputList[outputList.Count - 1].Length - 1); //remove last ,

                return outputList;
            }
            set
            {
                for (int line = 0; line < value.Count; line++)
                {
                    string parameterLine = value[line];

                    SingleStringComponents stringComponents = new SingleStringComponents
                    {
                        JSONString = parameterLine
                    };

                    if (stringComponents.ValueString != null) //ToDo: Switch to states
                    {
                        //Signle line
                        bool found = false;

                        foreach (MailboxLineSingle buildParameter in BuildParameters)
                        {
                            found = buildParameter.TryAssignValue(stringComponents);
                            if (found)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Multi line
                        bool found = false;

                        foreach (MailboxLineSingleSubObject singleObject in SingleSubObjects)
                        {
                            if (singleObject.ValueName == stringComponents.ValueName)
                            {
                                line++;
                                List<string> SubJSONList = new List<string>();
                                int currentBracketCounter = 0;

                                int counter = 0;
                                int linebefore = line;

                                while (true)
                                {
                                    string currentLine = value[line];

                                    if (currentLine == "{")
                                    {
                                        currentBracketCounter++;
                                        if (currentBracketCounter == 1)
                                        {
                                            line++;
                                            continue;
                                        }
                                    }
                                    else if (currentLine == "}," || currentLine == "}")
                                    {
                                        currentBracketCounter--;
                                        if (currentBracketCounter == 0)
                                        {
                                            //Create sub object
                                            //singleObject.JSONStrings = SubJSONList;

                                            //line++;
                                            break;
                                        }
                                    }
                                    SubJSONList.Add(currentLine);
                                    line++;
                                    counter++;
                                }

                                singleObject.JSONStrings = SubJSONList;

                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            foreach (MailboxLineMultipleSubObject multipleSubObject in MultipleSubObjects)
                            {
                                if (multipleSubObject.ValueName == stringComponents.ValueName)
                                {
                                    line += 1; //Don't skip "["
                                    List<string> SubJSONList = new List<string>();
                                    int currentBracketCounter = 0;

                                    while (true)
                                    {
                                        string currentLine = value[line];

                                        if (currentLine == "[")
                                        {
                                            currentBracketCounter++;
                                            if (currentBracketCounter == 1)
                                            {
                                                line++;
                                                continue;
                                            }
                                        }
                                        else if (currentLine == "]," || currentLine == "]")
                                        {
                                            currentBracketCounter--;
                                            if (currentBracketCounter == 0)
                                            {
                                                //Create sub object
                                                //multipleSubObject.JSONStrings = SubJSONList;

                                                //line+=2; //Skipt "]"
                                                break;
                                            }
                                        }
                                        SubJSONList.Add(currentLine);
                                        line++;
                                    }

                                    List<string> tempList = SubJSONList;

                                    multipleSubObject.JSONStrings = tempList;

                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            Debug.LogWarning("Error: Mailbox line not found, line = " + parameterLine + " in object " + ObjectHolder.IdentifierString);
                            ObjectHolder.Failed = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}

