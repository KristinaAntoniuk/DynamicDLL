using DynamicDLL;
using System.CodeDom;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main(string[] args)
    {
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
