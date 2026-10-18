namespace Nivaes.DataModel.Packer;

public struct ReferenceSetter
{
    public int IdReference;
    public Action<object, object?> Setter;
    public object? Reference;
}

public interface IPacker<T>
{
    ReferenceSetter[]? ReferenceSetters { get => null; }

    int IdClassType { get; }

    void Serialize(scoped in T value, ref PackerWriter writer);

    T? Deserialize(ref PackerReader reader);

    void DeserializeCircular(T value)
    {
        if (ReferenceSetters is null)
            return;

        foreach(var setters in ReferenceSetters)
        {
            setters.Setter(value!, setters.Reference);
        }
    }
}
