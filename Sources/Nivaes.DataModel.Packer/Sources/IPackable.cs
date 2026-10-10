namespace Nivaes.DataModel.Packer;

public interface IPackable<T>
{
    int IdClass { get; }

    void Serialize(T value, ref PackerWriter writer);

    T? Deserialize(ref PackerReader reader);

    void DeserializeCircular(ref PackerReader reader); 
}
