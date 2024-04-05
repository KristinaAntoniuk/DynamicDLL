using DynamicDLL;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

public class Program
{
    public const string DynamicAssemblyName = "DynamicAssembly";
    public const string DynamicModuleName = "DynamicModule";
    public const string DynamicNamespace = "DynamicNamespace";

    public static List<string> DynamicClasses = new List<string>()
    {
        "DynamicClass1",
        "DynamicClass2",
        "DynamicClass3",
        "DynamicClass4"
    };

    public static List<string> DynamicProperties = new List<string>()
    {
        "DynamicProperty1",
        "DynamicProperty2",
        "DynamicProperty3",
        "DynamicProperty4"
    };


    public static void Main(string[] args)
    {
        // Create an assembly and module
        AssemblyGenerator assemblyGenerator = new AssemblyGenerator(DynamicAssemblyName);
        ModuleBuilder moduleBuilder = assemblyGenerator.GetModuleBuilder(DynamicModuleName);

        //Create classes

        foreach(string type in DynamicClasses)
        {
            // Define a new type dynamically
            TypeGenerator typeGenerator = new TypeGenerator(DynamicNamespace, type, moduleBuilder);
            
            foreach(string prop in DynamicProperties)
            {
                typeGenerator.AddProperty(typeof(string), prop);
            }

            // Create the type
            Type dynamicType = typeGenerator.IstantiateType();

            // Generate C# code for the dynamic type
            string generatedCode = GenerateCSharpCodeForType(dynamicType);

            // Save the generated code to a .cs file
            File.WriteAllText($"..\\..\\..\\Output\\{type}.cs", generatedCode);
        }
    }

    private static string GenerateCSharpCodeForType(Type type)
    {
        StringBuilder sb = new StringBuilder();

        // Generate namespace and class definition
        sb.AppendLine($"namespace {type.Namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    public class {type.Name}");
        sb.AppendLine("    {");

        // Generate property definition
        foreach(PropertyInfo prop in type.GetProperties())
        {
            sb.AppendLine($"        public {prop.PropertyType.FullName} {prop.Name} {{get; set;}}");
        }

        // Close class and namespace
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}