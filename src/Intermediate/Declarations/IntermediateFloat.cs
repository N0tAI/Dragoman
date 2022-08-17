namespace Dragoman.Intermediate.Declarations
{
    internal struct IntermediateFloat : IIntermediateValue<float>
    {
        public string Name { get; init; }
        public float Value { get; init; }
        internal IntermediateFloat(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }
}
