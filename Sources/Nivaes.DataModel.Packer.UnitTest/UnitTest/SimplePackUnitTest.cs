using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer1.UnitTest;


public class SimplePackUnitTest
{
    public class SimpleModel : IPackable<SimpleModel>
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        void IPackable<SimpleModel>.Serialize(ref PackerWriter writer)
        {
            writer.Write(String1);
            writer.Write(String2);
        }

        static SimpleModel? IPackable<SimpleModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();

            var value = new SimpleModel
            {
                String1 = string1,
                String2 = string2
            };

            return value;
        }
    }

    [Fact]
    public void SerializerVoidDataTest()
    {
        var model = new SimpleModel()
        {
            String1 = null!,
            String2 = null!,
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<SimpleModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }

    [Fact]
    public void SerializerEmptyDataTest()
    {
        var model = new SimpleModel()
        {
            String1 = string.Empty,
            String2 = string.Empty,
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<SimpleModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }

    [Fact]
    public void SerializerTest()
    {
        var model = new SimpleModel
        {
            String1 = "test1",
            String2 = "test2",
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<SimpleModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }
}
