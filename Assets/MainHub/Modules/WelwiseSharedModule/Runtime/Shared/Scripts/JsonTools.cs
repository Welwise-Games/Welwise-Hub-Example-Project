using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WelwiseSharedModule.Runtime.Shared.Scripts
{
    public static class JsonTools
    {
        public static string GetJsonSerializedObjectWithoutNulls(this object obj) =>
            JsonConvert.SerializeObject(obj,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto,
                        SerializationBinder = new KnownTypesBinder
                        {
                            KnownTypes = GetKnownTypes()
                        }
                    });


        public static T GetDeserializedWithoutNulls<T>(this string str) =>
            JsonConvert.DeserializeObject<T>(str,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto,
                    SerializationBinder = new KnownTypesBinder
                    {
                        KnownTypes = GetKnownTypes()
                    }
                }
            );

        public static object GetDeserializedWithoutNulls(this string str, Type type) =>
            JsonConvert.DeserializeObject(str, type,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto,
                    SerializationBinder = new KnownTypesBinder
                    {
                        KnownTypes = GetKnownTypes()
                    }
                }
            );

        private static List<Type> GetKnownTypes()
        {
            return new List<Type>();
        }
    }
}