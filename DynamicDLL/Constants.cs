namespace DynamicDLL
{
    public static class Constants
    {
        public static Dictionary<string, Type> asnTypeMapping = new Dictionary<string, Type>()
        {
            {"INTEGER", typeof(int) },
            {"UTF8String", typeof(string) },
            {"ENUMERATED", typeof(string) },
            {"CHOICE", typeof(string) },
            {"SEQUENCE OF UTF8String", typeof(string[]) },
            {"SEQUENCE", typeof(object) }
        };
    }
}
