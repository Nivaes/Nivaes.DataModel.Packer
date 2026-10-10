using AutoFixture.Xunit3;

namespace Nivaes.DataModel.Packer.UnitTest;

public class ReferencePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public ReferencePackUnitTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public class RootModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
        public required ReferenceModel Reference { get; set; }
    }

    public class RootModelSerializer : IPackable<RootModel>
    {
        int IPackable<RootModel>.IdClass => 5;

        void IPackable<RootModel>.Serialize(RootModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
            //writer.Write(value.Reference);
        }

        RootModel? IPackable<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference = (ReferenceModel?)null!; // reader.Read<ReferenceModel>()!;

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference = reference
            };
            return value;
        }

        void IPackable<RootModel>.DeserializeCircular(ref PackerReader reader)
        {
        }
    }

    public class ReferenceModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
    }

    public class ReferenceModelSerializer : IPackable<ReferenceModel>
    {
        int IPackable<ReferenceModel>.IdClass => 6;
        void IPackable<ReferenceModel>.Serialize(ReferenceModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
        }

        ReferenceModel? IPackable<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();

            var value = new ReferenceModel
            {
                String1 = string1,
                String2 = string2
            };

            return value;
        }

        void IPackable<ReferenceModel>.DeserializeCircular(ref PackerReader reader)
        {
        }
    }

    [Fact]
    public void SerializerVoidDataTest()
    {
        var model = new RootModel()
        {
            String1 = null!,
            String2 = null!,
            Reference = new ReferenceModel
            {
                String1 = null!,
                String2 = null!,
            }
        };

        var cache = DataModelPacker.Serialize<RootModel>(model);
        //var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel>(cache);
        //var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }

    [Fact]
    public void SerializerEmptyDataTest()
    {
        var model = new RootModel()
        {
            String1 = string.Empty,
            String2 = string.Empty,
            Reference = new ReferenceModel
            {
                String1 = string.Empty,
                String2 = string.Empty,
            }
        };

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }

    [Fact]
    public void SerializerTest()
    {
        var model = new RootModel
        {
            String1 = "test1",
            String2 = "test2",
            Reference = new ReferenceModel
            {
                String1 = "test3",
                String2 = "test4",
            }
        };

        var cache = DataModelPacker.Serialize(model);
        //var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest1(string test1, string test2, string test3, string test4)
    {
        var model = new RootModel
        {
            String1 = test1,
            String2 = test2,
            Reference = new ReferenceModel
            {
                String1 = test3,
                String2 = test4,
            }
        };

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest2(RootModel model)
    {
        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }
}
