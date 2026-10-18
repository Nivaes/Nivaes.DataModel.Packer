using AutoFixture.Xunit3;
using static Nivaes.DataModel.Packer.UnitTest.SimplePackUnitTest;

namespace Nivaes.DataModel.Packer.UnitTest;


public class SimplePackUnitTest
{
    private readonly ITestOutputHelper _output;

    public SimplePackUnitTest(ITestOutputHelper output)
    {
        _output = output;

        DataModelPacker.IdClassToType.TryAdd(1, typeof(SimpleModelSerializer));
        DataModelPacker.ClassToSerializerType.TryAdd(typeof(SimpleModel), typeof(SimpleModelSerializer));
    }

    public class SimpleModel 
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
    }

    public class SimpleModelSerializer : IPacker<SimpleModel>
    {
        int IPacker<SimpleModel>.IdClassType => 1;

        UnpackerReferences[] IPacker<SimpleModel>.UpackerReferences => new UnpackerReferences[1];

        void IPacker<SimpleModel>.Serialize(scoped in SimpleModel value, ref PackerWriter writer)
        {
            writer.Write(value.String1);
            writer.Write(value.String2);
        }

        SimpleModel? IPacker<SimpleModel>.Deserialize(ref PackerReader reader)
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

        //void IPacker<SimpleModel>.DeserializeCircular(SimpleModel value, ref PackerReader reader)
        //{
        //}
    }

    [Fact]   
    public void SerializerDataTest()
    {
        var model = new SimpleModel()
        {
            String1 = "String1",
            String2 = "String2",
        };
        
        var cache = DataModelPacker.Serialize<SimpleModel, SimpleModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<SimpleModel, SimpleModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }


    [Fact]
    public void SerializerVoidDataTest()
    {
        var model = new SimpleModel()
        {
            String1 = null!,
            String2 = null!,
        };

        var cache = DataModelPacker.Serialize<SimpleModel, SimpleModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<SimpleModel, SimpleModelSerializer>(cache);

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

        var cache = DataModelPacker.Serialize<SimpleModel, SimpleModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<SimpleModel, SimpleModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest1(string test1, string test2)
    {
        var model = new SimpleModel
        {
            String1 = test1,
            String2 = test2,
        };

        var cache = DataModelPacker.Serialize<SimpleModel, SimpleModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = DataModelPacker.Deserialize<SimpleModel, SimpleModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest2(SimpleModel model)
    {
        var cache = DataModelPacker.Serialize<SimpleModel, SimpleModelSerializer>(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = DataModelPacker.Deserialize<SimpleModel, SimpleModelSerializer>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }
}
