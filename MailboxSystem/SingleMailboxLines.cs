using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
    public class MailboxLineDistinctUnnamed : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineDistinctUnnamed";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return StringHelper.ConvertIntToString(value: Val, globalFormat: true);
            }
            set
            {
                int newValue = StringHelper.ConvertStringToInt(value, globalFormat: true, out bool worked);

                if (worked) Val = newValue;
            }
        }

        int max;
        int min = 0;

        int val; //val other word than value since value is a C# keyword
        public int Val //Sepparation between val and Val for correct constructor opperation
        {
            get
            {
                return val;
            }

            set
            {
                //keep val between min and max
                if (value > max) val = max;
                else if (value < min) val = min;
                else val = value;
            }
        }


        public int Max //Sepparation between max and Max for correct constructor opperation
        {
            get
            {
                return max;
            }

            set
            {
                //make sure max is not smaller than min
                if (value < min) max = min;
                else max = value;

                //Adjust val if it is now larger than max
                if (val > max) val = max;
            }
        }


        public int Min //Sepparation between min and Min for correct constructor opperation
        {
            get
            {
                return min;
            }

            set
            {
                //make sure min is not smaller than max
                if (value > max) min = max;
                else min = value;

                //Adjust val if it is now smaller than min
                if (val < min) val = min;
            }
        }

        public MailboxLineDistinctUnnamed(string name, Mailbox objectHolder, Mailbox.ValueType valueType, int Max, int Min = 0, int DefaultValue = 0) : base(valueName: name, objectHolder: objectHolder, valueType: valueType) //name, objectHolder, valueType
        {
            max = Max;
            this.Min = Min;
            Val = DefaultValue;
        }
    }

    /*
    public class MailboxLineDistinctEnum : MailboxLine
    {
        enum valEnum;

        public MailboxLineDistinctEnum(string name, Mailbox objectHolder, Mailbox.ValueType valueType, enum valEnum, int defaultValue) : base(name, objectHolder, valueType)
        {

        }

        public override string GetOutputString()
        {
            string returnString = "";

            return returnString;
        }

        public override void SetValueFromString(string value)
        {
        
        }
    }
    */

    public class MailboxLineDistinctNamed : MailboxLineSingle
    {
        public static bool outputIntValueOnly = false;

        protected override string typeName
        {
            get
            {
                return "MailboxLineDistinctNamed";
            }
        }

        public int LongestStringLength
        {
            get
            {
                int returnValue = 0;

                foreach(string entry in Entries)
                {
                    if(entry.Length > returnValue) returnValue = entry.Length;
                }

                return returnValue;
            }
        }

        protected override string JSONValueString
        {
            get
            {
                if (outputIntValueOnly)
                {
                    return StringHelper.ConvertIntToString(value: Val, globalFormat: true);
                }
                else
                {
                    return MyStringComponents.quote + ValString + MyStringComponents.quote;
                }
            }
            set
            {
                if (value[0].Equals(MyStringComponents.quote))
                {
                    string searchString = value.Replace(MyStringComponents.quote.ToString(), "");

                    for (int i = 0; i < Entries.Count; i++)
                    {
                        if (Entries[i] == searchString)
                        {
                            Val = i;
                            return;
                        }
                    }
                }
                else
                {
                    int newValue = StringHelper.ConvertStringToInt(value, globalFormat: true, out bool worked);

                    if (worked) Val = newValue;
                }

            }
        }

        int val;

        public int Val
        {
            get
            {
                return val;
            }

            set
            {
                if (value < 0) val = 0;
                if (value > Max) val = Max;
                else val = value;
            }
        }

        public string ValString
        {
            get
            {
                return Entries[val];
            }
        }

        public List<string> Entries { get; private set; }

        public int Max
        {
            get
            {
                return Entries.Count - 1; // -1 because the maximum value is one less than the count. -> First index = 0
            }
        }

        public MailboxLineDistinctNamed(string name, Mailbox objectHolder, Mailbox.ValueType valueType, List<string> entries, int DefaultValue = 0) : base(name, objectHolder, valueType)
        {
            Entries = entries;
            Val = DefaultValue;
        }
    }

    public class MailboxLineBool : MailboxLineSingle
    {

        protected override string typeName
        {
            get
            {
                return "MailboxLineBool";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                if (Val) return "true";
                else return "false";
            }
            set
            {
                if (value.Equals("1")
                || value.Equals("true")
                || value.Equals("True"))
                {
                    Val = true;
                }
                else if (value.Equals("0")
                || value.Equals("false")
                || value.Equals("False"))
                {
                    Val = false;
                }
                else
                {
                    Debug.LogWarning($"Error: Boolean input value of {Name} not detected. String value = {value}");
                }
            }
        }

        public bool Val = false;

        public MailboxLineBool(string name, Mailbox objectHolder, Mailbox.ValueType valueType, bool DefaultValue = false) : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }
    }

    public class MailboxLineRanged : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineRanged";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return StringHelper.ConvertFloatToString(value: Val, globalFormat: true);
            }
            set
            {
                float newValue = StringHelper.ConvertStringToFloat(value, globalFormat: true, out bool worked);

                if (worked) Val = newValue;
            }
        }

        float val; //val other word than value since value is a C# keyword
        float max;
        float min = 0;

        public float Val //Sepparation between val and Val for correct constructor opperation
        {
            get
            {
                return val;
            }

            set
            {
                //keep val between min and max

                if (value > max) val = max;
                else if (value < min) val = min;
                else val = value;
            }
        }


        public float Max //Sepparation between max and Max for correct constructor opperation
        {
            get
            {
                return max;
            }

            set
            {

                //make sure max is not smaller than min
                max = value;
                if (max < min) min = max;


                //Adjust val if it is now larger than max
                if (val > max) val = max;
            }
        }


        public float Min //Sepparation between min and Min for correct constructor opperation
        {
            get
            {
                return min;
            }

            set
            {

                //make sure min is not smaller than max
                min = value;
                if (min > max) max = min;

                //Adjust val if it is now smaller than min
                if (val < min) val = min;
            }
        }

        public MailboxLineRanged(string name, Mailbox objectHolder, Mailbox.ValueType valueType, float Max, float Min = 0, float DefaultValue = 0) : base(name, objectHolder, valueType)
        {
            if (Max >= Min)
            {
                max = Max;
                this.Min = Min;
            }
            else //Catch accidental minimum and maximum order problem
            {
                max = Min;
                min = Max;

                Debug.LogWarning("Error in Ranged line creation: Maximum is smaller than Minimum value when creating line: " + name + " -> Min and Max have been automatically switched");
            }


            Val = DefaultValue;
        }

    }

    public class MailboxLineString : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineString";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return MyStringComponents.quote + Val + MyStringComponents.quote;
            }
            set
            {
                Val = value.Replace("\"", "");
            }
        }

        public string Val;


        public MailboxLineString(string name, Mailbox objectHolder, Mailbox.ValueType valueType, string DefaultValue = "") : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }
    }

    public class MailboxLineVector3 : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineVector3";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return JsonLineHelper.ConvertVector3ToJSONString(Val);
            }
            set
            {
                Vector3 newValue = JsonLineHelper.ConvertJSONStringToVector3(value, out bool worked);

                if(worked) Val = newValue;
            }
        }

        public Vector3 Val;


        public MailboxLineVector3(string name, Mailbox objectHolder, Mailbox.ValueType valueType, Vector3 DefaultValue) : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }

        public MailboxLineVector3(string name, Mailbox objectHolder, Mailbox.ValueType valueType) : base(name, objectHolder, valueType)
        {
            Val = new Vector3(0, 0, 0);
        }
    }

    public class MailboxLineVector2Int : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineVector2Int";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return JsonLineHelper.ConvertVector2IntToJSONString(Val);
            }
            set
            {
                Vector2Int newValue = JsonLineHelper.ConvertJSONStringToVector2Int(value, out bool worked);

                if (worked) Val = newValue;
            }
        }

        public Vector2Int Val;
        public MailboxLineVector2Int(string name, Mailbox objectHolder, Mailbox.ValueType valueType, Vector2Int DefaultValue) : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }

        public MailboxLineVector2Int(string name, Mailbox objectHolder, Mailbox.ValueType valueType) : base(name, objectHolder, valueType)
        {
            Val = new Vector2Int(0, 0);
        }
    }

    public class MailboxLineVector2 : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineVector2";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return JsonLineHelper.ConvertVector2ToJSONString(Val);
            }
            set
            {
                Vector2 newValue = JsonLineHelper.ConvertJSONStringToVector2(value, out bool worked);

                if (worked) Val = newValue;
            }
        }

        public Vector2 Val;

        public MailboxLineVector2(string name, Mailbox objectHolder, Mailbox.ValueType valueType, Vector2 DefaultValue) : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }

        public MailboxLineVector2(string name, Mailbox objectHolder, Mailbox.ValueType valueType) : base(name, objectHolder, valueType)
        {
            Val = new Vector2(0, 0);
        }
    }

    public class MailboxLineQuaternion : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineQuaternion";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return JsonLineHelper.ConvertQuaternionToJSONString(Val);
            }
            set
            {
                Quaternion newValue = JsonLineHelper.ConvertJSONStringToQuaternion(value, out bool worked);

                if (worked) Val = newValue;
            }
        }

        Quaternion Val;

        public MailboxLineQuaternion(string name, Mailbox objectHolder, Mailbox.ValueType valueType, Quaternion DefaultValue) : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }

        public MailboxLineQuaternion(string name, Mailbox objectHolder, Mailbox.ValueType valueType) : base(name, objectHolder, valueType)
        {
            Val = new Quaternion(0, 0, 0, 1);
        }
    }

    public class MailboxLineColor : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineColor";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return JsonLineHelper.ConvertColorToJSONString(Val);
            }
            set
            {
                Color newValue = JsonLineHelper.ConvertJSONStringToColor(value, out bool worked);

                if (worked) Val = newValue;
            }
        }

        float minAlpha = 0;
        float maxAlpha = 1;

        public float MinAlpha
        {
            get
            {
                return minAlpha;
            }
            set
            {
                minAlpha = value;
                if (minAlpha < 0) minAlpha = 0;
            }
        }

        public float MaxAlpha
        {
            get
            {
                return maxAlpha;
            }
            set
            {
                maxAlpha = value;
                if (maxAlpha > 1) maxAlpha = 1;
            }
        }

        Color val;

        public Color Val
        {
            get
            {
                return val;
            }

            set
            {
                val = value;

                val.a = Mathf.Clamp(value.a, min: MinAlpha, max: MaxAlpha);
            }
        }

        public float ValRed
        {
            get
            {
                return val.r;
            }
            set
            {
                val.r = value;
            }
        }

        public byte ValRedByte
        {
            get
            {
                return (byte)(255 * val.r);
            }
            set
            {
                val.r = 1f * value / 255f;
            }
        }

        public float ValBlue
        {
            get
            {
                return val.b;
            }
            set
            {
                val.b = value;
            }
        }

        public byte ValBlueByte
        {
            get
            {
                return (byte)(255 * val.b);
            }
            set
            {
                val.b = 1f * value / 255f;
            }
        }

        public float ValGreen
        {
            get
            {
                return val.g;
            }
            set
            {
                val.g = value;
            }
        }

        public byte ValGreenByte
        {
            get
            {
                return (byte)(255 * val.g);
            }
            set
            {
                val.g = 1f * value / 255f;
            }
        }

        public MailboxLineColor(string name, Mailbox objectHolder, Mailbox.ValueType valueType, Color defaultValue) : base(name, objectHolder, valueType)
        {
            MaxAlpha = maxAlpha;
            MinAlpha = minAlpha;
            Val = defaultValue;
        }
    }

    public class MailboxLineMaterial : MailboxLineSingle
    {
        protected override string typeName
        {
            get
            {
                return "MailboxLineMaterial";
            }
        }

        protected override string JSONValueString
        {
            get
            {
                return MyStringComponents.quote + Val.Identifier + MyStringComponents.quote;
            }
            set
            {
                string seachString = value.Replace("\"", "");
                Val = MaterialLibrary.GetMaterialFromIdentifier(identifier: seachString);
            }
        }

        public MaterialManager Val;

        public MailboxLineMaterial(string name, Mailbox objectHolder, Mailbox.ValueType valueType, MaterialManager DefaultValue) : base(name, objectHolder, valueType)
        {
            Val = DefaultValue;
        }
    }
}