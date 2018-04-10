using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    // https://github.com/kevin-montrose/Jil
    class Jil<T> : TestBase<T, Jil.JSON> where T : class
    {
        public Jil(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            Jil.JSON.Serialize<T>(obj, text);
            text.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            TextReader text = new StreamReader(stream);
            return Jil.JSON.Deserialize<T>(text);
        }
    }
}
