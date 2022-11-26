using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SerializerTests.TypesToSerialize;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("http://www.newtonsoft.com/json",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    public class JsonNet<TSerialize> : TestBase<TSerialize, TSerialize, JsonSerializer> where TSerialize : class
    {
        public JsonNet(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            FormatterFactory = () => JsonSerializer.Create(new JsonSerializerSettings { PreserveReferencesHandling =
                refTracking ?  PreserveReferencesHandling.All : PreserveReferencesHandling.None });
        }

        class Person
        {
            public DateTime BirthDate { get; set; }
            public string Name { get; set; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            Formatter.Serialize(text, obj);
            text.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            TextReader text = new StreamReader(stream);
            return (TSerialize)Formatter.Deserialize(text, typeof(TSerialize));
        }
    }
}
