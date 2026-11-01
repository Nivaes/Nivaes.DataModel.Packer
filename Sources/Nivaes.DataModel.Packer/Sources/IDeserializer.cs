namespace Nivaes.DataModel.Packer;

public interface IDeserializer<T>
{
    T? Deserialize(ref PackerReader reader);
}
