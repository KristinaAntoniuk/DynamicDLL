namespace DynamicDLL
{
    public class ASNType
    {
        public string name;
        public List<ASNTypeProperty> properties;

        public ASNType(string name)
        {
            this.name = name;
            properties = new List<ASNTypeProperty>();
        }

        public void AddProperty(string name, Type type, List<object>? possibleValues = null)
        {
            if (String.IsNullOrEmpty(name) || type == null) return;
            ASNTypeProperty prop = new ASNTypeProperty(name, type, possibleValues);
            properties.Add(prop);
        }

        public void AddProperty(ASNTypeProperty property)
        {
            properties.Add(property);
        }

        public void AddProperties(List<ASNTypeProperty> properties)
        {
            properties.AddRange(properties);
        }
    }
}
