using DynamicDLL;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        CCUGenerator ccu = new CCUGenerator("DynamicDLL");
        ccu.CreateClass("TestClass");
        ccu.AddField("TestClass", "TestField", typeof(int?));
        ccu.Export();
    }
}