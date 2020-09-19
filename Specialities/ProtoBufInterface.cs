using ProtoBuf;
using ProtoBuf.Meta;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SerializerTests.Specialities
{
    /// <summary>
    /// Protobuf-net can de/serialize interface types and class hierarchies. The main limitation is that 
    /// a concrete interface type cannot take part in a class inheritance hierarchy.
    /// Additionally a concrete type which implements potentially several interfaces cannot be used to register the same type for several interface types!
    /// </summary>
    class TestProtoBufInterface
    {
        public void Test()
        {
            Base b = new Base { Name = "Alois" };
            b.Instances.Add(new Next
            {
                Name = "Base",
                NextName = "Christian",
                Rex = new List<Regex> { new Regex("Alois.*"), new Regex("Christian.*", RegexOptions.Compiled) }
            });
            b.Instances.Add(new Base { Name = "SecondBase", Instances = b.Instances });
            var mem = new MemoryStream();
            ProtobufTypeModels.MainModel.Serialize(mem, b);
            mem.Position = 0;
            var deser = (Base)ProtobufTypeModels.MainModel.Deserialize(mem, null, typeof(Base));


            var model = RuntimeTypeModel.Create();
            var t1 = model.Add(typeof(IAbstraction), true);

            t1.AddSubType(100, typeof(ImplA));
            t1.AddSubType(101, typeof(ImplB));

            User user = new User()
            {
                Inst1 = new ImplA() { Name = "Alois" },
                Inst2 = new ImplB { NameB = "NameB", NameB2 = "AloisNameB2", Name = "Jetzt wirds interessant", NameBase = "NameBase" },
            };

            var clone = (User)model.DeepClone(user);
        }

    }


    [ProtoContract]
    public class Base : IAbstraction
    {
        static Base()
        {
            ProtobufTypeModels.MainModel.Add(typeof(IAbstraction), true).AddSubType(101, typeof(Base));
        }

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public List<IAbstraction> Instances = new List<IAbstraction>();
    }

    [ProtoContract]
    public class Next : Base
    {
        static Next()
        {
            ProtobufTypeModels.MainModel.Add(typeof(IAbstraction), true).AddSubType(100, typeof(Next));
            ProtobufTypeModels.MainModel.Add(typeof(Regex), false).SetSurrogate(typeof(RegexSurrogate));
        }

        [ProtoMember(1)]
        public string NextName { get; set; }

        [ProtoMember(2)]
        public List<Regex> Rex { get; set; }
    }


    /// <summary>
    ///  Simplistic runtime type model
    ///  If you do not know during deserialization which types were registered you need to record in this class
    ///  all calls to RuntimeTypeModel.Add and store them in your stream before the payload. That way you can first deserialize your type model and 
    ///  then deserialize the actual types following in the stream.
    ///  This is were ProtobufTypeModels.MainModel.De/SerializeWithLengthPrefix come in handy to put in your type infos beforehand.
    /// </summary>
    public static class ProtobufTypeModels
    {
        public static readonly RuntimeTypeModel MainModel = RuntimeTypeModel.Create();
        
    }

    /// <summary>
    /// Non serializable members can be made serializable by registering a surrogate type which has implicit conversion operators
    /// </summary>
    [ProtoContract]
    public class RegexSurrogate
    {
        [ProtoMember(1)]
        public string RegexString { get; set; }

        [ProtoMember(2)]
        public RegexOptions Options { get; set; }


        public RegexSurrogate(Regex input)
        {
            if (input != null)
            {
                Options = input.Options;
                RegexString = input.ToString();
            }
            else
            {
                Debug.Print("Null serialized");
            }
        }

        public static implicit operator RegexSurrogate(Regex value)
        {
            return new RegexSurrogate(value);
        }

        public static implicit operator Regex(RegexSurrogate value)
        {
            if (value == null || value.RegexString == null)
            {
                return null;
            }
            return new Regex(value.RegexString, value.Options);
        }
    }
}
