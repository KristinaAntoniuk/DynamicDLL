using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace DynamicDLL
{
    public class CCUGenerator
    {
        private CodeCompileUnit ccu { get; set; }
        private CodeNamespace ns { get; set; }
        private List<CodeTypeDeclaration> classes { get; set; }

        public CCUGenerator(string namespaceName)
        {
            ccu = new CodeCompileUnit();
            ns = new CodeNamespace(namespaceName);
            classes = new List<CodeTypeDeclaration>();
        }

        //TODO: Potentially make this method flexible and add imports according required types in the new class
        private void SetNamespace()
        {
            ns.Imports.Add(new CodeNamespaceImport("System"));
        }

        public void CreateClass(string className)
        {
            SetNamespace();
            CodeTypeDeclaration newClass = new CodeTypeDeclaration(className);
            newClass.IsClass = true;
            newClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
            ns.Types.Add(newClass);
            ccu.Namespaces.Add(ns);
            classes.Add(newClass);
        }

        public void AddField(string className, string fieldName, Type fieldType)
        {
            CodeTypeDeclaration? customClass = classes.FirstOrDefault(x => x.Name == className);
            if (customClass == null) { throw new Exception($"Class {className} does not exist"); }

            CodeMemberField newField = new CodeMemberField();
            newField.Attributes = MemberAttributes.Public;
            newField.Name = fieldName;
            newField.Type = new CodeTypeReference(fieldType);
            
            customClass.Members.Add(newField);
        }

        public void Export()
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            foreach(CodeTypeDeclaration customClass in  classes)
            {
                using (StreamWriter sourceWriter = new StreamWriter($"..\\..\\..\\Output\\{customClass.Name}.cs"))
                {
                    provider.GenerateCodeFromCompileUnit(
                        ccu, sourceWriter, options);
                }
            }
            
        }
    }
}
