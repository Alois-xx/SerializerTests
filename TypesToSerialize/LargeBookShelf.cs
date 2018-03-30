using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#pragma warning disable 0169

namespace SerializerTests.TypesToSerialize
{
    [Serializable, DataContract, ProtoContract, MessagePackObject]
    public class LargeBookShelf
    {
        [DataMember, ProtoMember(1), Key(0)]
        public List<LargeBook> Books
        {
            get;
            set;
        }

        [DataMember, ProtoMember(2), Key(1)]
        private string Secret;

        [DataMember, ProtoMember(3), Key(2)]
        private int Id;

        [DataMember, ProtoMember(4), Key(3)]
        private int Id4;

        [DataMember, ProtoMember(5), Key(4)]
        private double Id5;

        [DataMember, ProtoMember(6), Key(5)]
        private List<float> Id6;

        [DataMember, ProtoMember(7), Key(6)]
        private List<float> Id7;

        [DataMember, ProtoMember(8), Key(7)]
        private List<float> Id8;

        [DataMember, ProtoMember(9), Key(8)]
        private List<float> Id9;

        [DataMember, ProtoMember(10), Key(9)]
        private Dictionary<double, string> Id10;

        [DataMember, ProtoMember(11), Key(10)]
        private Dictionary<double, string> Id11;

        [DataMember, ProtoMember(12), Key(11)]
        private Dictionary<double, string> Id12;

        [DataMember, ProtoMember(13), Key(12)]
        private Dictionary<double, string> Id13;

        [DataMember, ProtoMember(14), Key(13)]
        private Dictionary<double, string> Id14 { get; set; }

        [DataMember, ProtoMember(15), Key(14)]
        private Dictionary<double, string> Id15 { get; set; }

        [DataMember, ProtoMember(16), Key(15)]
        private Dictionary<double, string> Id16 { get; set; }

        [DataMember, ProtoMember(17), Key(16)]
        private Dictionary<double, string> Id17 { get; set; }

        [DataMember, ProtoMember(18), Key(17)]
        private Dictionary<double, string> Id18 { get; set; }

        [DataMember, ProtoMember(19), Key(18)]
        private Dictionary<double, string> Id19 { get; set; }

        [DataMember, ProtoMember(20), Key(19)]
        private Dictionary<double, string> Id20 { get; set; }

        public LargeBookShelf(string secret)
        {
            Secret = secret;
        }

        public LargeBookShelf() // Parameterless ctor is needed for every protocol buffer class during deserialization
        { }

       
    }

    [Serializable, DataContract, ProtoContract, MessagePackObject]
    public class LargeBook
    {
        [DataMember, ProtoMember(1), Key(0)]
        public string Title;

        [DataMember, ProtoMember(2), Key(1)]
        public int Id;

        [DataMember, ProtoMember(3), Key(2)]
        private int Id3;

        [DataMember, ProtoMember(4), Key(3)]
        private int Id4;

        [DataMember, ProtoMember(5), Key(4)]
        private double Id5;

        [DataMember, ProtoMember(6), Key(5)]
        private List<float> Id6;

        [DataMember, ProtoMember(7), Key(6)]
        private List<float> Id7;

        [DataMember, ProtoMember(8), Key(7)]
        private List<float> Id8;

        [DataMember, ProtoMember(9), Key(8)]
        private List<float> Id9;

        [DataMember, ProtoMember(10), Key(9)]
        private Dictionary<double, string> Id10;

        [DataMember, ProtoMember(11), Key(10)]
        private Dictionary<double, string> Id11;

        [DataMember, ProtoMember(12), Key(11)]
        private Dictionary<double, string> Id12;

        [DataMember, ProtoMember(13), Key(12)]
        private Dictionary<double, string> Id13;

        [DataMember, ProtoMember(14), Key(13)]
        private Dictionary<double, string> Id14 { get; set; }

        [DataMember, ProtoMember(15), Key(14)]
        private Dictionary<double, string> Id15 { get; set; }

        [DataMember, ProtoMember(16), Key(15)]
        private Dictionary<double, string> Id16 { get; set; }

        [DataMember, ProtoMember(17), Key(16)]
        private Dictionary<double, string> Id17 { get; set; }

        [DataMember, ProtoMember(18), Key(17)]
        private Dictionary<double, string> Id18 { get; set; }

        [DataMember, ProtoMember(19), Key(18)]
        private Dictionary<double, string> Id19 { get; set; }

        [DataMember, ProtoMember(20), Key(19)]
        private Dictionary<double, string> Id20 { get; set; }

        public LargeBook()
        {

        }


    }
}
