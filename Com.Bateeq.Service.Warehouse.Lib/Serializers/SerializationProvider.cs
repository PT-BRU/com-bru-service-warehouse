using MongoDB.Bson.Serialization;
using System;

namespace Com.Bateeq.Service.Warehouse.Lib.Serializers
{
    public class SerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(string))
            {
                return new CustomStringSerializer();
            }
            else if (type == typeof(double))
            {
                return new CustomDoubleSerializer();
            }
            else if (type == typeof(int))
            {
                return new CustomInt32Serializer();
            }
            else if (type == typeof(DateTimeOffset))
            {
                return new CustomDateTimeOffsetSerializer();
            }

            return null;
        }
    }
}
