using System.Reflection;
using System.Reflection.Emit;

namespace DynamicDLL
{
    public class AssemblyGenerator
    {
        private string asmName {  get; set; }

        public AssemblyGenerator(string asmName)
        {
            this.asmName = asmName;
        }

        public ModuleBuilder GetModuleBuilder(string moduleName)
        {
            AssemblyName assemblyName = new AssemblyName(asmName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder.DefineDynamicModule(moduleName);
        }
    }
}
