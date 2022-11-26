using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/kevin-montrose/Jil",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class Jil<TSerialize> : TestBase<TSerialize, TSerialize, Jil.JSON> where TSerialize : class
    {
        public Jil(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            Jil.JSON.Serialize<TSerialize>(obj, text);
            text.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            TextReader text = new StreamReader(stream);
            return Jil.JSON.Deserialize<TSerialize>(text);
        }
    }
}
