using AutoFixture.Xunit3;
using Nivaes.DataModel.Packer;

namespace Nivaes.DataModel.Packer1.UnitTest;


public class CircularReferencePackUnitTest
{
    public class RootModel : IPackable<RootModel>
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public required ReferenceModel Reference { get; set; }

        void IPackable<RootModel>.Serialize(ref PackerWriter writer)
        {
            writer.Write(String1);
            writer.Write(String2);
            writer.Write(Reference);
        }

        static RootModel? IPackable<RootModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();

            var value = new RootModel
            {
                String1 = string1,
                String2 = string2,
                Reference = reader.Reader<ReferenceModel>()!
            };
            return value;
        }
    }

    public class ReferenceModel : IPackable<ReferenceModel>
    {
        public required string String1 { get; set; }
        public required string String2 { get; set; }

        public RootModel? RootReference { get; set; }

        void IPackable<ReferenceModel>.Serialize(ref PackerWriter writer)
        {
            writer.Write(String1);
            writer.Write(String2);
            writer.Write(RootReference);
        }

        static ReferenceModel? IPackable<ReferenceModel>.Deserialize(ref PackerReader reader)
        {
            var string1 = reader.ReadString();
            var string2 = reader.ReadString();

            var value = new ReferenceModel
            {
                String1 = string1,
                String2 = string2,
                RootReference = reader.Reader<RootModel>()
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
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<RootModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
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
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<RootModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
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
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<RootModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
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
        model.Reference.RootReference = model;

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<RootModel>(cache);

        copyModel.ShouldNotBeNull();
        copyModel.String1.ShouldBe(model.String1);
        copyModel.String2.ShouldBe(model.String2);
        copyModel.Reference.ShouldNotBeNull();
        copyModel.Reference.String1.ShouldBe(model.Reference.String1);
        copyModel.Reference.String2.ShouldBe(model.Reference.String2);

        copyModel.Reference.RootReference.ShouldNotBeNull();
        copyModel.Reference.RootReference.ShouldBe(copyModel);
    }
}
