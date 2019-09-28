using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// Test how efficient deserialize can become by testing how fast one can create book objects from a plain UTF-8 buffer where all data is already parsed by indices's
    /// Serialize time is NOT something to consider because it does create the flat buffer along with indices's. 
    /// /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("", SerializerTypes.Binary)]
    class NopSerializer<T> : TestBase<T, Program> where T : class
    {
        int myCount = 0;
        List<uint> myStartIdxAndLength = new List<uint>();
        byte[] myUtf8Data = null;

        public NopSerializer(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        /// <summary>
        /// Create flat buffer with index array which simulates during deserialize a flat buffer where the data is copied to objects without any parsing overhead.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
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
        protected override T Deserialize(Stream stream)
        {
            T lret = default;
            if( typeof(BookShelf) == typeof(T) )
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

                lret = (T) (object) tmp;
            }
            else
            {
                throw new NotSupportedException("This class is only meant to be used with Bookshelf objects");
            }
            return lret;
        }
    }
}
