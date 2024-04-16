using DynamicDLL;
using Python.Runtime;
using System.CodeDom;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main(string[] args)
    {
        Runtime.PythonDLL = @"C:\Users\kristina.antoniuk\AppData\Local\Programs\Python\Python312\python312.dll";
        string code = File.ReadAllText(@"C:\Users\kristina.antoniuk\Documents\Asfinag\DynamicDLL\DynamicDLL\TestScript.py");
        PythonEngine.Initialize();
        using (Py.GIL())
        {
            using var scope = Py.CreateScope();
            scope.Exec(code);
        }

        #region Parsing Region (unfinished)
        string inputFile = File.ReadAllText(@"..\\..\\..\\Input\\Schema.asn");
        Regex regexMainBlock = new Regex(@"(^|\s+)BEGIN((.*\n)+)(^|\s+)END", RegexOptions.Multiline);
        Regex regexComment = new Regex(@"(^|\s+)(\/|\*).*", RegexOptions.Multiline);
        Match mainBlock = regexMainBlock.Match(inputFile);
        string[] lines = mainBlock.Value.Split('\n');
        List<string> blocks = new List<string>();
        StringBuilder sb = new StringBuilder();
        bool read = false;

        foreach (string line in lines)
        {
            if (line.Contains("::="))
            {
                if (sb.Length > 0)
                {
                    blocks.Add(sb.ToString());
                }
                sb = new StringBuilder();
                sb.Append(line);
                read = true;
            }
            else if (regexComment.IsMatch(line) || line.Contains("END"))
            {
                read = false;
                continue;
            }
            else if (read) 
            {
                sb.Append(line); 
            }
        }

        if (sb.Length > 0)
        {
            blocks.Add(sb.ToString());
        }

        sb.Clear();

        foreach(string block in blocks)
        {
            string blockName;
            Type blockType;
            List<ASNTypeProperty> properties = new List<ASNTypeProperty>();

            if (block.Contains("::="))
            {
                string[] parts = block.Split("::=");
                blockName = parts[0].Trim();
                blockType = Constants.asnTypeMapping[parts[1].Trim().ToUpper()];
            }

            string[] lineParts = block.Split(' ');

            foreach(string linePart in lineParts)
            {
                string? propertyName = null;
                Type? propertyType = null;
                string[] endIds = { "--", "," };

                linePart.Trim();
                if (String.IsNullOrEmpty(linePart)) continue;
                else if (Constants.asnTypeMapping.ContainsKey(linePart))
                {
                    propertyType = Constants.asnTypeMapping[linePart];
                }
                else if (true/*endIds.Contains*/) 
                {
                    if (!String.IsNullOrEmpty(propertyName) && propertyType != null)
                    {
                        properties.Add(new ASNTypeProperty(propertyName, propertyType));
                        break;
                    }
                }
                else if (String.Equals(',', linePart))
                {
                    if (!String.IsNullOrEmpty(propertyName) && propertyType != null)
                    {
                        properties.Add(new ASNTypeProperty(propertyName, propertyType));
                        break;
                    }
                }
                else
                {
                    propertyName = linePart;
                }
            }

        }
        #endregion

        string outputPath = @"..\\..\\..\\Output";
        CCUGenerator ccu = new CCUGenerator("TestDynamicDLL", outputPath);
        var testClass1 = ccu.CreateClass("TestClass1");
        var testClass2 = ccu.CreateClass("TestClass2");
        var testClass3 = ccu.CreateClass("TestClass3");
        ccu.AddField("TestClass1", "TestFieldInt", new CodeTypeReference(typeof(int?)));
        ccu.AddField("TestClass2", "TestFieldString", new CodeTypeReference(typeof(string)));
        ccu.AddField("TestClass2", "TestFieldFloat", new CodeTypeReference(typeof(float)));
        ccu.AddField("TestClass2", "TestFieldDouble", new CodeTypeReference(typeof(double)));
        ccu.AddField("TestClass3", "TestFieldList", new CodeTypeReference(typeof(List<string>)));
        ccu.AddField("TestClass3", "TestFieldTestClass2", new CodeTypeReference(testClass2.Name));
        ccu.ExportCs();

        string[] filePaths = Directory.GetFiles(outputPath);
        ccu.CombineDLL(filePaths);
        ccu.AddDLLToProject();
        ccu.ExportJson();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }
}
