namespace DynamicDLL.Utils
{
    public struct ASNType(string name, Type type)
    {
        public string name = name;
        public Type type = type;
    }

    public class TypesMapping
    {
        public ASNType INTEGER = new ASNType("INTEGER", typeof(int));
        public ASNType UTF8String = new ASNType("UTF8String", typeof(string));
        public ASNType ENUMERATED = new ASNType("ENUMERATED", typeof(string));
        public ASNType CHOICE = new ASNType("CHOICE", typeof(string));
        public ASNType SEQUENCE_OF_UTF8String = new ASNType("SEQUENCE OF UTF8String", typeof(string[]));
    }
}
