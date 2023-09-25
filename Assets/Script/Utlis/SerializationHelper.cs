using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class SerializationHelper
{
    // Serialize a struct to a byte array
    public static byte[] SerializeToByteArray(object data)
    {
        try
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, data);
                return memoryStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Serialization error: " + ex.Message);
            return null;
        }
    }

    // Deserialize a byte array to a struct
    public static T DeserializeFromByteArray<T>(byte[] byteArray)
    {
        try
        {
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                T deserializedData = (T)formatter.Deserialize(memoryStream);
                return deserializedData;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Deserialization error: " + ex.Message);
            return default(T);
        }
    }
}
