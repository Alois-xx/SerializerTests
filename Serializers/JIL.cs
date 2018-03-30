using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests.Serializers
{
    // https://github.com/kevin-montrose/Jil
    class Jil<T> : TestBase<T, Jil.JSON> where T : class
    {
        public Jil(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        protected override void Serialize(T obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            Jil.JSON.Serialize<T>(obj, text);
            text.Flush();
        }

        protected override T Deserialize(Stream stream)
        {
            TextReader text = new StreamReader(stream);
            return Jil.JSON.Deserialize<T>(text);
        }
    }
}
