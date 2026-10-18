namespace Nivaes.DataModel.Packer.UnitTest;

public class ReferencePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public ReferencePackUnitTest(ITestOutputHelper output)
    {
        _output = output;

        ////DataModelPacker.IdClassToType.TryAdd(5, typeof(RootModel));
        ////DataModelPacker.IdClassToType.TryAdd(6, typeof(ReferenceModel));
        ////DataModelPacker.ClassToSerializerType.TryAdd(typeof(RootModel), typeof(RootModelPacker));
        ////DataModelPacker.ClassToSerializerType.TryAdd(typeof(ReferenceModel), typeof(ReferenceModelPacker));
    }

    public class RootModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
        public required ReferenceModel Reference { get; set; }
    }

    public class RootModelSerializer : ISerializer<RootModel>
    {
        int ISerializer<RootModel>.IdClassType => 5;

        void ISerializer<RootModel>.Serialize(scoped in RootModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
            writer.Write<ReferenceModel, ReferenceModelSerializer>(1, value.Reference);
        }
    }

    public class RootModelDeserializer : IDeserializer<RootModel>
    {
        int IDeserializer<RootModel>.IdClassType => 5;

        RootModel? IDeserializer<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference = reader.Read<RootModel, ReferenceModel, ReferenceModelDeserializer>()!;

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference = reference
            };
            return value;
        }
    }

    public class ReferenceModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
    }

    public class ReferenceModelSerializer : ISerializer<ReferenceModel>
    {
        int ISerializer<ReferenceModel>.IdClassType => 6;

        void ISerializer<ReferenceModel>.Serialize(scoped in ReferenceModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
        }
    }

    public class ReferenceModelDeserializer : IDeserializer<ReferenceModel>
    {
        int IDeserializer<ReferenceModel>.IdClassType => 6;

        ReferenceModel? IDeserializer<ReferenceModel>.Deserialize(ref PackerReader reader)
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

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

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
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

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

        var cache = DataModelPacker.Serialize<RootModel, RootModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

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
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

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
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelDeserializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }
}
