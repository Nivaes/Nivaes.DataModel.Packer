using static Nivaes.Serializer.UnitTest.SimplePackUnitTest;

namespace Nivaes.Serializer.UnitTest;

public class SimplePackGeneratorUnitTest
{
    private readonly ITestOutputHelper _output;

    public SimplePackGeneratorUnitTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [SerializePacker]
    public class SimpleModel
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }
    }

    [Fact]   
    public void SerializerDataTest()
    {
        var model = new SimpleModel()
        {
            String1 = "String1",
            String2 = "String2",
        };

        var cache = SerializePacker.Serialize(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = SerializePacker.Deserialize<SimpleModel>(cache);

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

        var cache = SerializePacker.Serialize(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = SerializePacker.Deserialize<SimpleModel>(cache);

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

        var cache = SerializePacker.Serialize(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = SerializePacker.Deserialize<SimpleModel>(cache);

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

        var cache = SerializePacker.Serialize(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        var copyModel = SerializePacker.Deserialize<SimpleModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }

    [Theory, AutoData]
    public void SerializerAutoDataTest2(SimpleModel model)
    {
        var cache = SerializePacker.Serialize(model);
        cache.ToArray().ShouldNotBeNull().ShouldNotBeEmpty();

        _output.WriteLine(
            string.Join(" ", cache.ToArray().Select((b, i) => $"{i}:{b}"))
        );

        var copyModel = SerializePacker.Deserialize<SimpleModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
    }
}
