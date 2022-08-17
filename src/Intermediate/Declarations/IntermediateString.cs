namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateString : IIntermediateValue<string>
    {
        public string Name { get; init; }
        public string Value { get; init; }
        internal IntermediateString(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
