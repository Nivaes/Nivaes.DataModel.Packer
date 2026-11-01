namespace Nivaes.DataModel.Packer;

public interface IDeserializer<T>
{
    //int IdObject { get; set; }

    T? Deserialize(ref PackerReader reader);
}
