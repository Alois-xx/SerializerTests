﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SerializerTests
{
    [Flags]
    enum SerializerTypes
    {
        Binary = 1 << 1,
        Json = 1 << 2,
        Xml = 1 << 3,

        SupportsVersioning =  1 << 8,

        /// <summary>
        /// https://msgpack.org/
        /// </summary>
        ProtocolMessagePack = 1 << 9,

        /// <summary>
        /// https://developers.google.com/protocol-buffers/
        /// </summary>
        ProtocolProtobuf = 1 << 10,
    }

    /// <summary>
    /// Describe serializer roughly which data format it writes and which data format it writes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class SerializerTypeAttribute : Attribute
    {
        public SerializerTypes SerializerTypeDescription
        {
            get;
        }

        public string ProjectHomeUrl
        {
            get;
        }

        public SerializerTypeAttribute(string projectHomeUrl, SerializerTypes serializerTypeDescription)
        {
            ProjectHomeUrl = projectHomeUrl;
            SerializerTypeDescription = serializerTypeDescription;
        }
    }

    /// <summary>
    /// When added to a serializer the serialize time is set to 0 because it is e.g. a serializer which only 
    /// can deserialize and the preparation is done in its serialize method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class IgnoreSerializeTimeAttribute : Attribute
    {
        public IgnoreSerializeTimeAttribute(string _)
        {
            
        }

    }
}
