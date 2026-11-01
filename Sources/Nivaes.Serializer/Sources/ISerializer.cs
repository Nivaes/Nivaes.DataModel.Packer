namespace Nivaes.Serializer;

public interface ISerializer<T>
{
    void Serialize(scoped in T value, ref PackerWriter writer);
}
