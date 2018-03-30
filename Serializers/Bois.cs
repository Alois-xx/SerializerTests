using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salar.Bois;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/salarcode/Bois
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Bois<T> : TestBase<T, BoisSerializer>
    {
        new BoisSerializer Formatter = new BoisSerializer();
        public Bois(Func<int, T> testData, Action<T> toucher) : base(testData, toucher)
        {
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize<T>(obj, stream);
        }

        protected override T Deserialize(Stream stream)
        {
            return Formatter.Deserialize<T>(stream);
        }
    }
}
