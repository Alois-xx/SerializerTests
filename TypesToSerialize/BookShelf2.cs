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
    public class BookShelf2
    {
        [DataMember, ProtoMember(1)]
        public List<Book2> Books
        {
            get;
            set;
        }

        [DataMember, ProtoMember(2)]
        private string Secret;

        public BookShelf2(string secret)
        {
            Secret = secret;
        }

        public BookShelf2() // Parameterless ctor is needed for every protocol buffer class during deserialization
        { }
    }

    [Serializable, DataContract, ProtoContract]
    public class Book2
    {
        [DataMember, ProtoMember(1)]
        public string Title;

        [DataMember, ProtoMember(2)]
        public int Id;
    }
}
