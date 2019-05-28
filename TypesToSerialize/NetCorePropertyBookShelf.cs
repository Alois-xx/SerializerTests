using System;
using System.Collections.Generic;
using System.Text;

namespace SerializerTests.TypesToSerialize
{

    public class NetCorePropertyBookShelf
    {
        public List<NetCoreBook> Books
        {
            get;
            set;
        }

        private string Secret;

        public NetCorePropertyBookShelf(string secret)
        {
            Secret = secret;
        }

        public NetCorePropertyBookShelf() // Parameterless ctor is needed for every protocol buffer class during deserialization
        { }
    }

    /// <summary>
    /// JsonSerializer of .NET Core can only serializer public fields so far
    /// </summary>
    public class NetCoreBook
    {
        public string Title { get; set; }

        public int Id { get; set; }
    }
}
