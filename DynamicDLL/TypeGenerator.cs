using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection.PortableExecutable;

namespace DynamicDLL
{
    public class TypeGenerator
    {
        private string namespaceName {  get; set; }
        private string typeName { get; set; }
        private ModuleBuilder moduleBuilder;
        private TypeBuilder typeBuilder;

        public TypeGenerator(string namespaceName, string typeName, ModuleBuilder moduleBuilder)
        {
            this.namespaceName = namespaceName;
            this.typeName = typeName;
            this.moduleBuilder = moduleBuilder;
            this.typeBuilder = moduleBuilder.DefineType(namespaceName + "." + typeName, TypeAttributes.Public);
        }

        public Type IstantiateType()
        {
            return typeBuilder.CreateType();
        }

        public void AddProperty(Type propertyType, string propertyName)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField(propertyName, propertyType, FieldAttributes.Public);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the "get" accessor method.
            MethodBuilder custNameGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, getSetAttr, propertyType, Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();
            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
            custNameGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder custNameSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, getSetAttr, null, new Type[] { typeof(string) });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();
            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, fieldBuilder);
            custNameSetIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to
            // their corresponding behaviors, "get" and "set" respectively.
            propertyBuilder.SetGetMethod(custNameGetPropMthdBldr);
            propertyBuilder.SetSetMethod(custNameSetPropMthdBldr);
        }
    }
}
