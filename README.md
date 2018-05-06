# SerializerTests
.NET Serializer testing framework

This test framework compares the most popular and fastest serializers for .NET which was the input for https://aloiskraus.wordpress.com/2017/04/23/the-definitive-serialization-performance-guide/

The project compiles to .NET 4.7 and .NET Core 2.0 where you can check out the serialization performance in your favorite .NET Framework. 
The currently tested serializers are
- BinaryFormatter
- Bois
- DataContract
- FastJson
- FlatBuffer
- GroBuf
- JIL
- Json.NET
- MessagePackSharp
- MsgPack.Cli
- Protobuf.NET
- SerivceStack
- SlimSerializer
- Wire
- Hyperion (Wire Fork)
- XmlSerializer
- ZeroFormatter

The test suite tries its best to be fair and vendor neutral. More than one serializer claims to be the fastest. 
Now you can test at your own if that is really the case. If I have forgot a great serializer (should be as fast or faster than Protobuf) 
please drop me a note and I will include it. 
