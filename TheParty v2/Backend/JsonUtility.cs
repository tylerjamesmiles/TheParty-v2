using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TheParty_v2
{

    static class JsonUtility
    {
        public static T GetDeserialized<T>(string jsonFileName)
        {
            FileStream fs = new FileStream(jsonFileName, FileMode.Open);
            StreamReader streamReader = new StreamReader(fs);
            JsonTextReader jsonTextReader = new JsonTextReader(streamReader);
            JsonSerializer serializer = new JsonSerializer();

            return serializer.Deserialize<T>(jsonTextReader);
        }
    }
}
