namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateBoolean : IIntermediateValue<bool>
    {
        public string Name { get; init; }
        public bool Value { get; init; }
        internal IntermediateBoolean(string name, bool value)
        {
            Name = name;
            Value = value;
        }
    }
}
