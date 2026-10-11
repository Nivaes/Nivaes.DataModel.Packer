namespace Nivaes.DataModel.Packer;

public interface IPacker<T>
{
    int IdClassType { get; }

    void Serialize(scoped in T value, ref PackerWriter writer);

    T? Deserialize(ref PackerReader reader);

    void DeserializeCircular(T value,ref PackerReader reader); 
}
