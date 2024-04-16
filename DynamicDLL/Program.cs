using DynamicDLL;
using RestSharp;
using System.CodeDom;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public class Program
{
    public static void Main(string[] args)
    {
        #region Encode and decode example asn data using external Python encryptor
        //This line is necessary in order to use Windows-1252 coding
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string schema = File.ReadAllText(@"..\\..\\..\\Input\\SchemaEnc.asn");
        string data = File.ReadAllText(@"..\\..\\..\\Input\\DataEnc.asn");

        DynamicDLL.Services.Encoder encoder = new DynamicDLL.Services.Encoder(schema, data);
        byte[]? encoded = encoder.Encode("Rocket");
        string? decoded = encoder.Decode(encoded);
        #endregion

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

        #region Creting .css .json and .dll files programmatically
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
        #endregion
    }
}
