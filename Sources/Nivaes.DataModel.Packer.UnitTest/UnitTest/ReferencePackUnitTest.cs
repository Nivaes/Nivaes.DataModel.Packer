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

    public class RootModelPacker : IPacker<RootModel>
    {
        int IPacker<RootModel>.IdClassType => 5;

        void IPacker<RootModel>.Serialize(scoped in RootModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
            writer.Write<ReferenceModel, ReferenceModelPacker>(value.Reference);
        }
  
        RootModel? IPacker<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();
            var reference = reader.Read<ReferenceModel, ReferenceModelPacker>()!;

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference = reference
            };
            return value;
        }

        void IPacker<RootModel>.DeserializeCircular(ref PackerReader reader)
        {
        }
    }

    public class ReferenceModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
    }

    public class ReferenceModelPacker : IPacker<ReferenceModel>
    {
        int IPacker<ReferenceModel>.IdClassType => 6;

        void IPacker<ReferenceModel>.Serialize(scoped in ReferenceModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
        }
    
        //void IPacker<ReferenceModel>.DeserializeRegister()
        //{

        //}

        ReferenceModel? IPacker<ReferenceModel>.Deserialize(ref PackerReader reader)
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

        void IPacker<ReferenceModel>.DeserializeCircular(ref PackerReader reader)
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

        var cache = DataModelPacker.Serialize<RootModel, RootModelPacker>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelPacker>(cache);

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

        var cache = DataModelPacker.Serialize<RootModel, RootModelPacker>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelPacker>(cache);

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

        var cache = DataModelPacker.Serialize<RootModel, RootModelPacker>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelPacker>(cache);

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

        var cache = DataModelPacker.Serialize<RootModel, RootModelPacker>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelPacker>(cache);

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
        var cache = DataModelPacker.Serialize<RootModel, RootModelPacker>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<RootModel, RootModelPacker>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);
    }
}
