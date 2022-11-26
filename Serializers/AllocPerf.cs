using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// Test Allocation performance by reading UTF-8 Data into strings. It is kind of a NOP serializer because it does not really serialized data in a format which needs any parsing. 
    /// Serialize time is NOT something to consider because it does create the flat buffer along with indices's. 
    /// /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("", SerializerTypes.Binary)]
    [IgnoreSerializeTimeAttribute("The data is prepared in the Serialize method for the deserialize call. Ignore it to not confuse users.")]
    class AllocPerf<TSerialize> : TestBase<TSerialize, TSerialize, Program> where TSerialize : class
    {
        int myCount = 0;
        List<uint> myStartIdxAndLength = new List<uint>();
        byte[] myUtf8Data = null;

        public AllocPerf(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        /// <summary>
        /// Create flat buffer with index array which simulates during deserialize a flat buffer where the data is copied to objects without any parsing overhead.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            if( obj is BookShelf shelf)
            {
                MemoryStream utf8Data = new MemoryStream();
                myCount = shelf.Books.Count;
                for(int i=0;i<shelf.Books.Count;i++)
                {
                    myStartIdxAndLength.Add((uint) utf8Data.Position);
                    byte[] bytes = Encoding.UTF8.GetBytes(shelf.Books[i].Title);
                    utf8Data.Write(bytes, 0, bytes.Length);
                    myStartIdxAndLength.Add((uint) bytes.Length);
                }

                myUtf8Data = utf8Data.ToArray();
                stream.Write(myUtf8Data, 0, myUtf8Data.Length);
            }
            else
            {
                throw new NotSupportedException("This class is only meant to be used with Bookshelf objects");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            TSerialize lret = default;
            if( typeof(BookShelf) == typeof(TSerialize) )
            {
                var tmp = new BookShelf("This is secret")
                {
                    Books = new List<Book>()
                };

                var books = tmp.Books;

                // This will test basically the allocation performance of .NET which will when the allocation rate becomes too high put 
                // Thread.Sleeps into the allocation calls. 
                for(int i=0;i<myCount;i++)
                {
                    books.Add(new Book()
                    {
                        Id = i+1,
                        Title = Encoding.UTF8.GetString(myUtf8Data, (int) myStartIdxAndLength[i*2], (int) myStartIdxAndLength[i*2+1] )
                    });
                }

                lret = (TSerialize) (object) tmp;
            }
            else
            {
                throw new NotSupportedException("This class is only meant to be used with Bookshelf objects");
            }
            return lret;
        }
    }
}
