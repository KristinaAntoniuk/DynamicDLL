using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Newtonsoft.Json;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;

namespace DynamicDLL.Services
{
    public class CCUGenerator
    {
        private CodeCompileUnit ccu { get; set; }
        private CodeNamespace? ns { get; set; }
        private List<CodeTypeDeclaration> classes { get; set; }
        private static string? outputFolder { get; set; }

        public CCUGenerator(string namespaceName, string outputFolderPath)
        {
            outputFolder = outputFolderPath;
            ccu = new CodeCompileUnit();
            classes = new List<CodeTypeDeclaration>();
            SetNamespace(namespaceName);
        }

        private void SetNamespace(string namespaceName)
        {
            ns = new CodeNamespace(namespaceName);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ccu.Namespaces.Add(ns);
        }

        public CodeTypeDeclaration CreateClass(string className)
        {
            CodeTypeDeclaration newClass = new CodeTypeDeclaration(className);
            newClass.IsClass = true;
            newClass.TypeAttributes = TypeAttributes.Public;
            ns.Types.Add(newClass);
            classes.Add(newClass);
            return newClass;
        }

        public void AddField(string className, string fieldName, CodeTypeReference fieldType)
        {
            CodeTypeDeclaration? customClass = classes.FirstOrDefault(x => x.Name == className);
            if (customClass == null) { throw new Exception($"Class {className} does not exist"); }

            CodeMemberField newField = new CodeMemberField();
            newField.Attributes = MemberAttributes.Public;
            newField.Name = fieldName;
            newField.Type = fieldType;

            customClass.Members.Add(newField);
        }

        public void ExportCs()
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            using (StreamWriter sourceWriter = new StreamWriter($"{outputFolder}\\{ns.Name}.cs"))
            {
                provider.GenerateCodeFromCompileUnit(
                    ccu, sourceWriter, options);
            }
        }
        public void ExportJson()
        {
            Assembly assembly = Assembly.LoadFrom($"{outputFolder}\\{ns.Name}.dll");

            foreach (Type t in assembly.GetTypes())
            {
                object? commandInstance = Activator.CreateInstance(t);

                string JSONresult = JsonConvert.SerializeObject(commandInstance);
                File.WriteAllText($"{outputFolder}\\{t.Name}.json", JSONresult);
            }
        }

        public void CombineDLL(string[] sourceFiles, bool overrideAssembly = false)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree> { };

            foreach (string file in sourceFiles)
            {
                string code = File.ReadAllText(file);
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(code));
            }

            var refPaths = new[] {
                typeof(object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
            };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create(
                ns.Name,
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            bool assemblyExists = File.Exists($"{outputFolder}\\{ns.Name}.dll");

            if (assemblyExists)
            {
                if (overrideAssembly)
                {
                    File.Delete($"{outputFolder}\\{ns.Name}.dll");
                }
                else return;
            }

            EmitResult result = compilation.Emit($"{outputFolder}\\{ns.Name}.dll");
        }

        public void AddDLLToProject()
        {
            Assembly.LoadFrom($"{outputFolder}\\{ns.Name}.dll");
        }
    }
}
