using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NewTONConverter
{
    public static class NewtonWriter
    {
        public static void ConvertFromJObject(JObject jobject, string output)
        {
            using (FileStream fileStream = new FileStream(output, FileMode.Create))
            {
                ConvertFromJObject(jobject, fileStream);
            }
        }

        public static void ConvertFromJObject(JObject jobject, Stream stream)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            {
                ConvertFromJObject(jobject, binaryWriter);
            }
        }

        public static void ConvertFromJObject(JObject jobject, BinaryWriter binaryWriter)
        {
            uint slotCount = 0;
            JArray groups = new JArray();

            jobject.TryGetValue("slot_count", ref slotCount);
            if (jobject.TryGetValue("groups", out JToken? jtoken) == true && jtoken is JArray) groups = jtoken as JArray;

            binaryWriter.Write(slotCount);
            binaryWriter.Write((uint)groups.Count);

            foreach (JToken group in groups)
            {
                if (group is JObject) WriteGroup(group as JObject, binaryWriter);
            }
        }

        private static void WriteGroup(JObject jobject, BinaryWriter binaryWriter)
        {
            string type = "";
            string res = null;
            byte unknownValue = 1;
            string groupName = "";
            string parentName = null;

            JArray subgroups = new JArray();
            JArray resources = new JArray();

            if (jobject.TryGetValue("subgroups", out JToken? stoken) == true && stoken is JArray) subgroups = stoken as JArray;
            if (jobject.TryGetValue("resources", out JToken? rtoken) == true && rtoken is JArray) resources = rtoken as JArray;

            jobject.TryGetValue("type", ref type);
            jobject.TryGetValue("res", ref res);
            jobject.TryGetValue("id", ref groupName);
            jobject.TryGetValue("parent", ref parentName);

            switch(type)
            {
                case "composite": binaryWriter.Write((byte)1); break;
                case "simple": binaryWriter.Write((byte)2); break;
                default: throw new Exception($"Unknown group type {type}");
            }

            if (res != null) binaryWriter.Write(Convert.ToUInt32(res));
            else binaryWriter.Write(0);

            binaryWriter.Write((uint)subgroups.Count);
            binaryWriter.Write((uint)resources.Count);
            binaryWriter.Write(unknownValue);

            if (parentName != null) binaryWriter.Write(true);
            else binaryWriter.Write(false);

            WriteASCII(groupName, binaryWriter);
            if (parentName != null) WriteASCII(parentName, binaryWriter);

            foreach (JToken subgroup in subgroups)
            {
                if (subgroup is JObject) WriteSubgroup(subgroup as JObject, binaryWriter);
            }

            foreach (JToken resource in resources)
            {
                if (resource is JObject) WriteResource(resource as JObject, binaryWriter);
            }
        }

        private static void WriteSubgroup(JObject jobject, BinaryWriter binaryWriter)
        {
            string name = "";
            string res = null;

            jobject.TryGetValue("res", ref res);
            jobject.TryGetValue("id", ref name);

            if(res != null) binaryWriter.Write(Convert.ToUInt32(res));
            else binaryWriter.Write(0);

            WriteASCII(name, binaryWriter);
        }

        private static void WriteResource(JObject jobject, BinaryWriter binaryWriter)
        {
            string type = "";
            uint slotID = 0;
            uint width = 0;
            uint height = 0;
            int x = 0;
            int y = 0;
            uint ax = 0;
            uint ay = 0;
            uint aw = 0;
            uint ah = 0;
            uint cols = 1;

            uint unknownValueUInt32 = 1;

            bool isAtlas = false;

            byte unknownValueByte_F = 1;
            byte unknownValueByte_S = 1;

            string name = "";
            string path = "";
            string parent = null;

            jobject.TryGetValue("type", ref type);
            jobject.TryGetValue("slot", ref slotID);
            jobject.TryGetValue("width", ref width);
            jobject.TryGetValue("height", ref height);
            jobject.TryGetValue("x", ref x);
            jobject.TryGetValue("y", ref y);
            jobject.TryGetValue("ax", ref ax);
            jobject.TryGetValue("ay", ref ay);
            jobject.TryGetValue("aw", ref aw);
            jobject.TryGetValue("ah", ref ah);
            jobject.TryGetValue("cols", ref cols);
            jobject.TryGetValue("atlas", ref isAtlas);
            jobject.TryGetValue("id", ref name);
            jobject.TryGetValue("path", ref path);
            jobject.TryGetValue("parent", ref parent);

            byte typeID = type switch
            {
                "Image" => 1,
                "PopAnim" => 2,
                "SoundBank" => 3,
                "File" => 4,
                "PrimeFont" => 5,
                "RenderEffect" => 6,
                "DecodedSoundBank" => 7,
                _ => 0
            };

            binaryWriter.Write(typeID);
            binaryWriter.Write(slotID);
            binaryWriter.Write(width);
            binaryWriter.Write(height);
            binaryWriter.Write(x);
            binaryWriter.Write(y);
            binaryWriter.Write(ax);
            binaryWriter.Write(ay);
            binaryWriter.Write(aw);
            binaryWriter.Write(ah);
            binaryWriter.Write(cols);
            binaryWriter.Write(unknownValueUInt32);
            binaryWriter.Write(isAtlas);
            binaryWriter.Write(unknownValueByte_F);
            binaryWriter.Write(unknownValueByte_S);

            if (parent != null) binaryWriter.Write(true);
            else binaryWriter.Write(false);

            WriteASCII(name, binaryWriter);
            WriteASCII(path, binaryWriter);
            if (parent != null) WriteASCII(parent, binaryWriter);
        }

        private static void WriteASCII(string str, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)str.Length);
            byte[] buffer = Encoding.ASCII.GetBytes(str);
            binaryWriter.Write(buffer);
        }

        public static bool TryGetValue<T>(this JObject jobject, string key, ref T value)
        {
            if (jobject.ContainsKey(key) == true)
            {
                value = jobject.Value<JToken>(key).ToObject<T>();
                return true;
            }
            
            return false;
        }
    }
}
