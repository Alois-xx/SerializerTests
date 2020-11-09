using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salar.Bois;
using System.Runtime.CompilerServices;
using Salar.Bois.LZ4;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/salarcode/Bois",
                    SerializerTypes.Binary)]
    class Bois<T> : TestBase<T, BoisSerializer>
    {
        new BoisSerializer Formatter = new BoisSerializer();
        public Bois(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize<T>(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return Formatter.Deserialize<T>(stream);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/salarcode/Bois",
	    SerializerTypes.Binary)]
    class Bois_LZ4<T> : TestBase<T, BoisLz4Serializer>
    {
	    new BoisLz4Serializer Formatter = new BoisLz4Serializer();
	    public Bois_LZ4(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
	    {
	    }

	    [MethodImpl(MethodImplOptions.NoInlining)]
	    protected override void Serialize(T obj, Stream stream)
	    {
		    Formatter.Pickle(obj, stream);
	    }

	    [MethodImpl(MethodImplOptions.NoInlining)]
	    protected override T Deserialize(Stream stream)
	    {
		    return Formatter.Unpickle<T>(stream);
	    }
    }

}
