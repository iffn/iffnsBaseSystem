using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class FileUpgraderTest : MonoBehaviour
{
    string fileEnding = ".json";

    List<ReplacementPair> replacementPairs;

    // Start is called before the first frame update
    void Start()
    {
        if (replacementPairs == null) SetupReplacementList();

        Run();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Run()
    {
        string inputFolder = Path.Combine(Application.streamingAssetsPath, "Buildings");

        List<string> fileList = Directory.GetFiles(inputFolder).ToList();

        StreamReader reader;

        foreach (string file in fileList)
        {
            if (file.Substring(file.Length - fileEnding.Length).Equals(fileEnding) == false) continue; //ignore

            reader = new StreamReader(file);

            List<string> originalLines = File.ReadAllLines(file).OfType<string>().ToList();

            List<string> outputLines = new List<string>();

            foreach(string line in originalLines)
            {
                outputLines.Add(RunReplaceFunction(line));
            }

            string outputText = string.Join(separator: MyStringComponents.newLine, values: outputLines); //Very fast, takes about 2ms for 13k lines

            reader.Close();

            File.WriteAllText(file, outputText);
        }
    }

    void SetupReplacementList()
    {
        replacementPairs = new List<ReplacementPair>();

        replacementPairs.Add(new ReplacementType(original: "Table Float", replacement: "TableFloat"));
        replacementPairs.Add(new ReplacementType(original: "Rectangular roof", replacement: "RectangularRoof"));
        replacementPairs.Add(new ReplacementType(original: "Floor", replacement: "FloorController"));
        replacementPairs.Add(new ReplacementType(original: "Railing arc", replacement: "RailingArc"));
        replacementPairs.Add(new ReplacementType(original: "Roof wall", replacement: "RoofWall"));
        replacementPairs.Add(new ReplacementType(original: "Circular roof", replacement: "CircularRoof"));
        replacementPairs.Add(new ReplacementType(original: "Railing linear", replacement: "RailingLinear"));
        replacementPairs.Add(new ReplacementType(original: "Chair Grid", replacement: "ChairGrid"));
        replacementPairs.Add(new ReplacementType(original: "Building", replacement: "HumanBuildingController"));
        replacementPairs.Add(new ReplacementType(original: "Non-cardinal wall", replacement: "NonCardinalWall"));
        replacementPairs.Add(new ReplacementType(original: "Bed Grid", replacement: "BedGrid"));
        replacementPairs.Add(new ReplacementType(original: "Bed Float", replacement: "BedFloat"));
        replacementPairs.Add(new ReplacementType(original: "Triangular roof", replacement: "TriangularRoof"));
        replacementPairs.Add(new ReplacementType(original: "Vertical arc", replacement: "VerticalArc"));
        replacementPairs.Add(new ReplacementType(original: "Rectangular stairs", replacement: "RectangularStairs"));
        replacementPairs.Add(new ReplacementType(original: "Node Wall System", replacement: "NodeWallSystem"));
        replacementPairs.Add(new ReplacementType(original: "Circular stairs", replacement: "CircularStair"));
        replacementPairs.Add(new ReplacementType(original: "Table Grid", replacement: "TableGrid"));
        replacementPairs.Add(new ReplacementType(original: "Horizontal arc", replacement: "HorizontalArc"));
        replacementPairs.Add(new ReplacementType(original: "Virtual block", replacement: "VirtualBlock"));
        replacementPairs.Add(new ReplacementType(original: "Node wall", replacement: "NodeWall"));
    }

    string RunReplaceFunction(string inputString)
    {
        string returnString = inputString;

        foreach (ReplacementPair pair in replacementPairs)
        {
            returnString = returnString.Replace(oldValue: pair.original, newValue: pair.replacement);
        }

        return returnString;
    }

    class ReplacementPair
    {
        public string original;
        public string replacement;

        public ReplacementPair(string original, string replacement)
        {
            this.original = '"' + original + '"';
            this.replacement = '"' + replacement + '"';
        }

        protected ReplacementPair()
        {

        }
    }

    class ReplacementType : ReplacementPair
    {
        static string startingIdentifier = "\"Type\": ";

        public ReplacementType(string original, string replacement)
        {
            this.original = startingIdentifier + '"' + original + '"';
            this.replacement = startingIdentifier + '"' + replacement + '"';
        }
    }
}
