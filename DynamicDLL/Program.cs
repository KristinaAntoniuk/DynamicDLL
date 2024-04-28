using DynamicDLL;
using DynamicDLL.Models;
using DynamicDLL.Services;
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

        ASNParser parser = new ASNParser(inputFile);
        List<string> blocks = parser.GetBlocks();

        string outputPath = @"..\\..\\..\\Output";
        CCUGenerator ccu = new CCUGenerator("TestDynamicDLL", outputPath);

        foreach (string block in blocks)
        {
            ASNType type = parser.ParseBlock(block);
            var newClass = ccu.CreateClass(type.name);

            foreach(ASNTypeProperty property in type.properties)
            {
                ccu.AddField(type.name, property.name, new CodeTypeReference(property.type));
            }
        }

        ccu.ExportCs();
        string[] filePaths = Directory.GetFiles(outputPath);
        ccu.CombineDLL(filePaths);
        ccu.AddDLLToProject();
        ccu.ExportJson();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        #endregion
    }
}
