namespace DynamicDLL
{
    public class ASNTypeProperty
    {
        public string name;
        public Type type;
        public List<object>? possibleValues;

        public ASNTypeProperty(string name, Type type, List<object>? possibleValues = null)
        {
            this.name = name;
            this.type = type;
            this.possibleValues = possibleValues;
        }
    }
}
