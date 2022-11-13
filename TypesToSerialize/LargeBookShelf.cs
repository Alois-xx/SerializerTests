#if NETCOREAPP3_0_OR_GREATER
using MemoryPack;
#endif
using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#pragma warning disable 0169

namespace SerializerTests.TypesToSerialize
{
    [Serializable, DataContract, ProtoContract, MessagePackObject
#if NETCOREAPP3_0_OR_GREATER
        , MemoryPackable
#endif
    ]
    public partial class LargeBookShelf
    {
        [DataMember, ProtoMember(1), Key(0)]
        public List<LargeBook> Books
        {
            get;
            set;
        }

        [DataMember, ProtoMember(2), Key(1)]
        public string Secret;

        [DataMember, ProtoMember(3), Key(2)]
        public int Id;

        [DataMember, ProtoMember(4), Key(3)]
        public int Id4;

        [DataMember, ProtoMember(5), Key(4)]
        public double Id5;

        [DataMember, ProtoMember(6), Key(5)]
        public List<float> Id6;

        [DataMember, ProtoMember(7), Key(6)]
        public List<float> Id7;

        [DataMember, ProtoMember(8), Key(7)]
        public List<float> Id8;

        [DataMember, ProtoMember(9), Key(8)]
        public List<float> Id9;

        [DataMember, ProtoMember(10), Key(9)]
        public List<KeyValuePair<string,double>> Id10;

        [DataMember, ProtoMember(11), Key(10)]
        public List<KeyValuePair<string,double>> Id11;

        [DataMember, ProtoMember(12), Key(11)]
        public List<KeyValuePair<string,double>> Id12;

        [DataMember, ProtoMember(13), Key(12)]
        public List<KeyValuePair<string,double>> Id13;

        [DataMember, ProtoMember(14), Key(13)]
        public List<KeyValuePair<string,double>> Id14 { get; set; }

        [DataMember, ProtoMember(15), Key(14)]
        public List<KeyValuePair<string,double>> Id15 { get; set; }

        [DataMember, ProtoMember(16), Key(15)]
        public List<KeyValuePair<string,double>> Id16 { get; set; }

        [DataMember, ProtoMember(17), Key(16)]
        public List<KeyValuePair<string,double>> Id17 { get; set; }

        [DataMember, ProtoMember(18), Key(17)]
        public List<KeyValuePair<string,double>> Id18 { get; set; }

        [DataMember, ProtoMember(19), Key(18)]
        public List<KeyValuePair<string,double>> Id19 { get; set; }

        [DataMember, ProtoMember(20), Key(19)]
        public List<KeyValuePair<string,double>> Id20 { get; set; }

        public LargeBookShelf(string secret)
        {
            Secret = secret;
        }
#if NETCOREAPP3_0_OR_GREATER
        [MemoryPackConstructor]
#endif
        public LargeBookShelf() // Parameterless ctor is needed for every protocol buffer class during deserialization
        { }


    }

    [Serializable, DataContract, ProtoContract, MessagePackObject
#if NETCOREAPP3_0_OR_GREATER
        , MemoryPackable
#endif
    ]
    public partial class LargeBook
    {
        [DataMember, ProtoMember(1), Key(0)]
        public string Title;

        [DataMember, ProtoMember(2), Key(1)]
        public int Id;

        [DataMember, ProtoMember(3), Key(2)]
        public int Id3;

        [DataMember, ProtoMember(4), Key(3)]
        public int Id4;

        [DataMember, ProtoMember(5), Key(4)]
        public double Id5;

        [DataMember, ProtoMember(6), Key(5)]
        public List<float> Id6;

        [DataMember, ProtoMember(7), Key(6)]
        public List<float> Id7;

        [DataMember, ProtoMember(8), Key(7)]
        public List<float> Id8;

        [DataMember, ProtoMember(9), Key(8)]
        public List<float> Id9;

        [DataMember, ProtoMember(10), Key(9)]
        public List<KeyValuePair<string,double>> Id10;

        [DataMember, ProtoMember(11), Key(10)]
        public List<KeyValuePair<string,double>> Id11;

        [DataMember, ProtoMember(12), Key(11)]
        public List<KeyValuePair<string,double>> Id12;

        [DataMember, ProtoMember(13), Key(12)]
        public List<KeyValuePair<string,double>> Id13;

        [DataMember, ProtoMember(14), Key(13)]
        public List<KeyValuePair<string,double>> Id14 { get; set; }

        [DataMember, ProtoMember(15), Key(14)]
        public List<KeyValuePair<string,double>> Id15 { get; set; }

        [DataMember, ProtoMember(16), Key(15)]
        public List<KeyValuePair<string,double>> Id16 { get; set; }

        [DataMember, ProtoMember(17), Key(16)]
        public List<KeyValuePair<string,double>> Id17 { get; set; }

        [DataMember, ProtoMember(18), Key(17)]
        public List<KeyValuePair<string,double>> Id18 { get; set; }

        [DataMember, ProtoMember(19), Key(18)]
        public List<KeyValuePair<string,double>> Id19 { get; set; }

        [DataMember, ProtoMember(20), Key(19)]
        public List<KeyValuePair<string,double>> Id20 { get; set; }

        public LargeBook()
        {

        }


    }
}
