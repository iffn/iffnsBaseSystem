using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace iffnsStuff.iffnsBaseSystemForUnity
{
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

        public static BaseLoadFileInfo GetBaseFileInfoFromFileLocation(string completeFileLocation, string fileEnding)
        {
            //List<string> allLines = File.ReadAllLines(completeFileLocation).OfType<string>().ToList();
            List<string> lines = File.ReadLines(completeFileLocation).Take(BaseLoadFileInfo.numberOfLinesNeededForIdentification).ToList();

            int slashLocation = completeFileLocation.LastIndexOfAny(new char[] { MyStringComponents.slash, MyStringComponents.backslash });

            string fileName = completeFileLocation.Substring(slashLocation + 1);
            fileName = fileName.Substring(0, fileName.Length - fileEnding.Length);

            //BaseLoadFileInfo fileInfo = GetBaseFileInfoFromJson(fileNameWithoutEnding: fileName, jsonString: lines.GetRange(0, BaseLoadFileInfo.numberOfLinesNeededForIdentification));
            BaseLoadFileInfo fileInfo = GetBaseFileInfoFromJson(fileNameWithoutEnding: fileName, jsonString: lines);

            return fileInfo;
        }

        public static BaseLoadFileInfo GetBaseFileInfoFromJson(string fileNameWithoutEnding, List<string> jsonString)
        {
            jsonString = CleanupJSONStringForLoading(jsonString);

            BaseLoadFileInfo fileInfo = new(fileNameWithoutEnding: fileNameWithoutEnding, jsonStringCleanedUp: jsonString);

            return fileInfo;
        }

        public static FullLoadFileInfo GetFileInfoFromFileLocation(string completeFileLocation, string fileEnding)
        {
            List<string> allLines = File.ReadAllLines(completeFileLocation).OfType<string>().ToList();

            int slashLocation = completeFileLocation.LastIndexOfAny(new char[] { MyStringComponents.slash, MyStringComponents.backslash });

            string fileName = completeFileLocation.Substring(slashLocation + 1);
            fileName = fileName.Substring(0, fileName.Length - fileEnding.Length);

            FullLoadFileInfo fileInfo = GetFileInfoFromJson(fileNameWithoutEnding: fileName, jsonString: allLines);

            return fileInfo;
        }

        public static FullLoadFileInfo GetFileInfoFromJson(string fileNameWithoutEnding, List<string> jsonString)
        {
            jsonString = CleanupJSONStringForLoading(jsonString);

            FullLoadFileInfo fileInfo = new(fileNameWithoutEnding: fileNameWithoutEnding, jsonStringCleanedUp: jsonString);

            return fileInfo;
        }

        public static List<string> GetFileNamesFromFolderLocation(string completeFolderLocation, string fileEnding)
        {
            List<string> returnList = new();
            List<string> fileLocations = Directory.GetFiles(completeFolderLocation).ToList();

            foreach (string fileLocation in fileLocations)
            {
                if (fileLocation.Substring(fileLocation.Length - fileEnding.Length).Equals(fileEnding) == false) continue; //ignore

                int slashLocation = fileLocation.LastIndexOfAny(new char[] { MyStringComponents.slash, MyStringComponents.backslash });

                string fileName = fileLocation.Substring(slashLocation + 1);
                fileName = fileName.Substring(0, fileName.Length - fileEnding.Length);

                returnList.Add(fileName);
            }

            return returnList;
        }

        public static List<BaseLoadFileInfo> GetBaseFileInfosFromFolderLocation(string completeFolderLocation, string fileEnding)
        {
            List<BaseLoadFileInfo> returnList = new();
            List<string> fileList = Directory.GetFiles(completeFolderLocation).ToList();

            foreach (string file in fileList)
            {
                if (file.Substring(file.Length - fileEnding.Length).Equals(fileEnding) == false) continue; //ignore

                BaseLoadFileInfo fileInfo = GetBaseFileInfoFromFileLocation(completeFileLocation: file, fileEnding: fileEnding);

                if (fileInfo != null && fileInfo.IsValid) returnList.Add(fileInfo);
            }

            return returnList;
        }

        public static List<FullLoadFileInfo> GetFileInfosFromFolderLocation(string completeFolderLocation, string fileEnding)
        {
            List<FullLoadFileInfo> returnList = new();
            List<string> fileList = Directory.GetFiles(completeFolderLocation).ToList();

            foreach (string file in fileList)
            {
                if (file.Substring(file.Length - fileEnding.Length).Equals(fileEnding) == false) continue; //ignore

                FullLoadFileInfo fileInfo = GetFileInfoFromFileLocation(completeFileLocation: file, fileEnding: fileEnding);

                if(fileInfo != null && fileInfo.IsValid) returnList.Add(fileInfo);
            }

            return returnList;
        }

        //Find file
        public static List<string> GetFileListFromLocation(string Type, string completeFileLocation, string fileEnding)
        {
            List<string> returnList = new();
            List<string> fileList = Directory.GetFiles(completeFileLocation).ToList();

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

                returnList[^1] = returnList[^1] + ",";
            }

            returnList[^1] = returnList[^1].Remove(-1);

            return returnList;
        }

        //Save file
        public static void SaveFileToFileLocation(SaveFileInfo fileInfo, string completeFileLocation)
        {
            List<string> fileContent = fileInfo.JsonString;

            SaveLinesTextToFile(fileContent: fileContent, completeFileLocation: completeFileLocation);
        }

        public abstract class FileInfo
        {
            public string identifier;
            public string version;
        }

        public abstract class LoadFileInfo : FileInfo
        {
            public bool IsValid { get; protected set; } = true;
            public string FileNameWithoutEnding { get; private set; }

            public LoadFileInfo(string fileNameWithoutEnding)
            {
                FileNameWithoutEnding = fileNameWithoutEnding;
            }

            public void SetBaseInfo(List<string> jsonString)
            {
                if (jsonString[0] != "{")
                {
                    IsValid = false;
                    return;
                }

                if (JsonLineHelper.JsonValue.FromJsonString(jsonString: jsonString[1]) is not JsonLineHelper.JsonStringValue typeFormat)
                {
                    IsValid = false;
                    return;
                }

                if (typeFormat.IsValid == false)
                {
                    IsValid = false;
                    return;
                }

                identifier = typeFormat.value;

                JsonLineHelper.JsonStringValue versionFormat = JsonLineHelper.JsonValue.FromJsonString(jsonString: jsonString[2]) as JsonLineHelper.JsonStringValue;

                if (typeFormat == null)
                {
                    IsValid = false;
                    return;
                }

                if (typeFormat.IsValid == false)
                {
                    IsValid = false;
                    return;
                }

                version = versionFormat.value;
            }
        }

        public class BaseLoadFileInfo : LoadFileInfo
        {
            readonly public static int numberOfLinesNeededForIdentification = 3;

            public BaseLoadFileInfo(string fileNameWithoutEnding, List<string> jsonStringCleanedUp) : base(fileNameWithoutEnding: fileNameWithoutEnding)
            {
                SetBaseInfo(jsonStringCleanedUp);
            }
        }

        public class FullLoadFileInfo : LoadFileInfo
        {
            public List<string> LoadObjectString { get; private set; }

            public FullLoadFileInfo(string fileNameWithoutEnding, List<string> jsonStringCleanedUp) : base(fileNameWithoutEnding: fileNameWithoutEnding)
            {
                SetBaseInfo(jsonStringCleanedUp);

                if (!IsValid) return;

                jsonStringCleanedUp.RemoveRange(0, 4);

                jsonStringCleanedUp.RemoveAt(jsonStringCleanedUp.Count - 1);

                LoadObjectString = jsonStringCleanedUp;
            }
        }

        public class SaveFileInfo : FileInfo
        {
            public List<SaveObjectInfo> saveObjects;

            public SaveFileInfo(string type, string version, SaveObjectInfo saveObject)
            {
                this.identifier = type;
                this.version = version;
                this.saveObjects = new List<SaveObjectInfo> { saveObject };
            }

            public SaveFileInfo(string type, string version, List<SaveObjectInfo> saveObjects)
            {
                this.identifier = type;
                this.version = version;
                this.saveObjects = saveObjects;
            }

            public class SaveObjectInfo
            {
                public string name;
                public IBaseObject saveObject;

                public SaveObjectInfo(string name, IBaseObject saveObject)
                {
                    this.name = name;
                    this.saveObject = saveObject;
                }
            }

            public List<string> JsonString
            {
                get
                {
                    List<string> returnList = new();

                    returnList.Add("{");

                    JsonLineHelper.JsonStringValue typeFormat = new(name: "Type", value: identifier);
                    JsonLineHelper.JsonStringValue versionFormat = new(name: "Version", value: version);

                    returnList.Add(MyStringComponents.tab + typeFormat.JsonString + ",");
                    returnList.Add(MyStringComponents.tab + versionFormat.JsonString + ",");

                    foreach(SaveObjectInfo saveObjectInfo in saveObjects)
                    {
                        returnList.Add(MyStringComponents.tab + saveObjectInfo.name + ":");

                        returnList.Add(MyStringComponents.tab + "{");
                        returnList.AddRange(GetSaveJSONStringFromObject(saveObject: saveObjectInfo.saveObject, tabs: 1));
                        returnList.Add(MyStringComponents.tab + "},");
                    }

                    returnList[^1] = returnList[^1].Remove(returnList[^1].Length - 1);

                    returnList.Add("}");

                    return returnList;
                }
            }
        }

        public static void SaveLinesTextToFile(List<string> fileContent, string completeFileLocation)
        {
            string outputText = string.Join(separator: MyStringComponents.newLine, values: fileContent); //Very fast, takes about 2ms for 13k lines

            File.WriteAllText(completeFileLocation, outputText);
        }
        

        static List<string> GetSaveJSONStringFromObject(IBaseObject saveObject, int tabs)
        {
            string tabString0 = "";

            for(int i = 0; i<tabs; i++)
            {
                tabString0 += MyStringComponents.tab;
            }

            string tabString1 = tabString0 += MyStringComponents.tab;

            List<string> returnList = saveObject.JSONBuildParameters;

            returnList.Insert(0, MyStringComponents.quote + "Type" + MyStringComponents.quote + ": " + MyStringComponents.quote + saveObject.IdentifierString + MyStringComponents.quote + ",");

            for (int i = 0; i < returnList.Count; i++)
            {
                returnList[i] = returnList[i].Insert(0, tabString1);
            }

            return returnList;
        }

        //Load file
        public static List<string> GetJSONStringFromFileLocation(string completeFileLocation)
        {
            List<string> allLines = File.ReadAllLines(completeFileLocation).OfType<string>().ToList();

            return allLines;
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
            for (int i = 0; i < JSONString.Count; i++)
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
            SingleStringComponents firstLine = new();
            firstLine.JSONString = JSONString[0];
            if (!firstLine.IsValid || firstLine.ValueName != "Type")
            {
                Debug.LogWarning("Error, first line is not valid");
            }

            IBaseObject returnObject = ResourceLibrary.GetObjectFromStringIdentifier(identifier: firstLine.ValueString, superObject: superObject);

            if (returnObject == null) return null;

            JSONString.RemoveAt(0);

            returnObject.JSONBuildParameters = JSONString;

            return returnObject;
        }
    }
}