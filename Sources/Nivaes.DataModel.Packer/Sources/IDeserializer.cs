namespace Nivaes.DataModel.Packer;

public struct ReferenceSetter
{
    //public int IdReference;
    public Action<object, object?> Setter;
    public object? Reference;
}

public interface IDeserializer
{
    int IdClassType { get; }

    ReferenceSetter[]? ReferenceSetters { get => null; }

    void DeserializeCircular(object value)
    {
        if (ReferenceSetters is null)
            return;

        foreach (var setters in ReferenceSetters)
        {
            setters.Setter(value!, setters.Reference);
        }
    }
}

public interface IDeserializer<T> : IDeserializer
{
    T? Deserialize(ref PackerReader reader);
}
