using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests.TypesToSerialize
{
    [Serializable, DataContract, ProtoContract]
    public class LargeBookShelf
    {
        [DataMember, ProtoMember(1)]
        public List<LargeBook> Books
        {
            get;
            set;
        }

        [DataMember, ProtoMember(2)]
        private string Secret;

        [DataMember, ProtoMember(3)]
        private int Id;

        [DataMember, ProtoMember(4)]
        private int Id4;

        [DataMember, ProtoMember(5)]
        private double Id5;

        [DataMember, ProtoMember(6)]
        private List<float> Id6;

        [DataMember, ProtoMember(7)]
        private List<float> Id7;

        [DataMember, ProtoMember(8)]
        private List<float> Id8;

        [DataMember, ProtoMember(9)]
        private List<float> Id9;

        [DataMember, ProtoMember(10)]
        private Dictionary<double, string> Id10;

        [DataMember, ProtoMember(11)]
        private Dictionary<double, string> Id11;

        [DataMember, ProtoMember(12)]
        private Dictionary<double, string> Id12;

        [DataMember, ProtoMember(13)]
        private Dictionary<double, string> Id13;

        [DataMember, ProtoMember(14)]
        private Dictionary<double, string> Id14 { get; set; }

        [DataMember, ProtoMember(15)]
        private Dictionary<double, string> Id15 { get; set; }

        [DataMember, ProtoMember(16)]
        private Dictionary<double, string> Id16 { get; set; }

        [DataMember, ProtoMember(17)]
        private Dictionary<double, string> Id17 { get; set; }

        [DataMember, ProtoMember(18)]
        private Dictionary<double, string> Id18 { get; set; }

        [DataMember, ProtoMember(19)]
        private Dictionary<double, string> Id19 { get; set; }

        [DataMember, ProtoMember(20)]
        private Dictionary<double, string> Id20 { get; set; }

        public LargeBookShelf(string secret)
        {
            Secret = secret;
        }

        public LargeBookShelf() // Parameterless ctor is needed for every protocol buffer class during deserialization
        { }
    }

    [Serializable, DataContract, ProtoContract]
    public class LargeBook
    {
        [DataMember, ProtoMember(1)]
        public string Title;

        [DataMember, ProtoMember(2)]
        public int Id;

        [DataMember, ProtoMember(3)]
        private int Id3;

        [DataMember, ProtoMember(4)]
        private int Id4;

        [DataMember, ProtoMember(5)]
        private double Id5;

        [DataMember, ProtoMember(6)]
        private List<float> Id6;

        [DataMember, ProtoMember(7)]
        private List<float> Id7;

        [DataMember, ProtoMember(8)]
        private List<float> Id8;

        [DataMember, ProtoMember(9)]
        private List<float> Id9;

        [DataMember, ProtoMember(10)]
        private Dictionary<double, string> Id10;

        [DataMember, ProtoMember(11)]
        private Dictionary<double, string> Id11;

        [DataMember, ProtoMember(12)]
        private Dictionary<double, string> Id12;

        [DataMember, ProtoMember(13)]
        private Dictionary<double, string> Id13;

        [DataMember, ProtoMember(14)]
        private Dictionary<double, string> Id14 { get; set; }

        [DataMember, ProtoMember(15)]
        private Dictionary<double, string> Id15 { get; set; }

        [DataMember, ProtoMember(16)]
        private Dictionary<double, string> Id16 { get; set; }

        [DataMember, ProtoMember(17)]
        private Dictionary<double, string> Id17 { get; set; }

        [DataMember, ProtoMember(18)]
        private Dictionary<double, string> Id18 { get; set; }

        [DataMember, ProtoMember(19)]
        private Dictionary<double, string> Id19 { get; set; }

        [DataMember, ProtoMember(20)]
        private Dictionary<double, string> Id20 { get; set; }
    }
}
