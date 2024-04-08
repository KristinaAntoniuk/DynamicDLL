using DynamicDLL;
using System.CodeDom;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        string outputPath = @"..\\..\\..\\Output";
        CCUGenerator ccu = new CCUGenerator("DynamicDLL", outputPath);
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

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }
}
