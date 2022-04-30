using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class StaticSaveAndLoadSystem
{
    //Data
    public static string UserFileLocation = Application.streamingAssetsPath;

    //Create folder
    public static string CreateFolder(string folderName, string folderPath)
    {
        string completeFolderLocation = Path.Combine(folderPath, folderName);

        if (!Directory.Exists(completeFolderLocation))
        {
            Directory.CreateDirectory(completeFolderLocation);
        }

        return completeFolderLocation;
    }

    //Find file
    public static List<string> GetFileListFromLocation(string Type, string completeFileLocation, string fileEnding)
    {
        List<string> returnList = new List<string>();
        List<string> fileList = System.IO.Directory.GetFiles(completeFileLocation).ToList();

        StreamReader reader;

        SingleStringComponents typeLine;

        foreach (string file in fileList)
        {
            if (file.Substring(file.Length - fileEnding.Length).Equals(fileEnding) == false) continue; //ignore

            reader = new StreamReader(file);

            reader.ReadLine(); //Skip first line

            typeLine = new SingleStringComponents
            {
                JSONString = reader.ReadLine().Replace(MyStringComponents.tab, "")
            };

            if (typeLine.ValueName.Equals("Type") == false) continue; //ignore second line does not specify the type
            if (typeLine.ValueString.Equals(MyStringComponents.quote + Type + MyStringComponents.quote) == false) continue; //ignore if type does not match

            int slashLocation = file.LastIndexOfAny(new char[] { MyStringComponents.slash, MyStringComponents.backslash });

            string fileName = file.Substring(slashLocation + 1);

            returnList.Add(fileName);
        }

        return returnList;
    }

    //Save file
    public static void SaveObjectToFileLocation(IBaseObject saveObject, string completeFileLocation)
    {
        SaveLinesTextToFile(fileContent: GetSaveJSONStringFromObject(saveObject: saveObject), completeFileLocation: completeFileLocation);

        /*
        string outputText = "";

        outputText = string.Join(separator: MyStringComponents.newLine, values: GetSaveJSONStringFromObject(saveObject: saveObject)); //Very fast, takes about 2ms for 13k lines

        File.WriteAllText(completeFileLocation, outputText);
        */
    }

    public static void SaveLinesTextToFile(List<string> fileContent, string completeFileLocation)
    {
        string outputText = "";

        outputText = string.Join(separator: MyStringComponents.newLine, values: fileContent); //Very fast, takes about 2ms for 13k lines

        File.WriteAllText(completeFileLocation, outputText);
    }


    static List<string> GetSaveJSONStringFromObject(IBaseObject saveObject)
    {
        List<string> returnList = saveObject.JSONBuildParameters;
        returnList.Insert(0, MyStringComponents.quote + "Type" + MyStringComponents.quote + ": " + MyStringComponents.quote + saveObject.IdentifierString + MyStringComponents.quote + ",");

        for (int i = 0; i < returnList.Count; i++)
        {
            returnList[i] = returnList[i].Insert(0, MyStringComponents.tab);
        }

        returnList.Insert(0, "{");
        returnList.Add("}");

        return returnList;
    }

    //Load file
    public static List<string> GetJSONStringFromFileLocation(string completeFileLocation)
    {
        List<string> allLines = System.IO.File.ReadAllLines(completeFileLocation).OfType<string>().ToList();

        return allLines;
    }

    public static IBaseObject LoadBaseObjectIntoSuperObject(string completeFileLocation, IBaseObject superObject)
    {
        List<string> allLines = GetJSONStringFromFileLocation(completeFileLocation: completeFileLocation);

        IBaseObject returnObject = LoadBaseObjectIntoSuperObject(RawJSONString: allLines, superObject: superObject);

        return returnObject;
    }

    public static IBaseObject LoadBaseObjectIntoSuperObject(List<string> RawJSONString, IBaseObject superObject)
    {
        RawJSONString = CleanupJSONStringForLoading(RawJSONString);

        //remove first and last line because they are {}
        RawJSONString.RemoveAt(0);
        RawJSONString.RemoveAt(RawJSONString.Count - 1);

        IBaseObject returnObject = GetFullObjectFromJSONString(JSONString: RawJSONString, superObject: superObject);

        return returnObject;
    }

    public static void LoadBaseObjectParametersToExistingObject(List<string> RawJSONString, IBaseObject baseObject)
    {
        RawJSONString = CleanupJSONStringForLoading(RawJSONString);

        //remove first and last line because they are {}
        RawJSONString.RemoveAt(0);
        RawJSONString.RemoveAt(RawJSONString.Count - 1);

        baseObject.ResetObject();

        baseObject.JSONBuildParameters = RawJSONString;
    }

    public static void LoadBaseObjectParametersToExistingObject(string completeFileLocation, IBaseObject baseObject)
    {
        List<string> JSONString = GetJSONStringFromFileLocation(completeFileLocation: completeFileLocation);

        LoadBaseObjectParametersToExistingObject(RawJSONString: JSONString, baseObject: baseObject);
    }

    public static List<string> CleanupJSONStringForLoading(List<string> JSONString)
    {
        //Remove all tabs
        for (int i = 0; i<JSONString.Count; i++)
        {
            string line = JSONString[i];
            line = line.Replace(MyStringComponents.tab, ""); //ToDo: Only remove tabs at the beginning of the line

            JSONString[i] = line;
        }

        //ToDo: Make sure all brackets are on 1 line

        return JSONString;
    }

    public static IBaseObject GetFullObjectFromJSONString(List<string> JSONString, IBaseObject superObject)
    {
        //Assumption: String list between {} including Type without \t, any {}[] on different lines
        SingleStringComponents firstLine = new SingleStringComponents();
        firstLine.JSONString = JSONString[0];
        if (!firstLine.IsValid || firstLine.ValueName != "Type")
        {
            Debug.LogWarning("Error, first line is not valid");
        }

        IBaseObject returnObject = ResourceLibrary.GetObjectFromStringIdentifier(identifier: firstLine.ValueString, superObject: superObject);

        JSONString.RemoveAt(0);

        returnObject.JSONBuildParameters = JSONString;

        return returnObject;
    }
}
