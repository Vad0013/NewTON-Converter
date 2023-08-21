using Newtonsoft.Json.Linq;
using System.Text;

namespace NewTONConverter
{
    public static class NewtonReader
    {
        public static JObject ConvertToJObject(string filepath)
        {
            using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                return ConvertToJObject(fileStream);
            }
        }

        public static JObject ConvertToJObject(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                return ConvertToJObject(binaryReader);
            }
        }

        public static JObject ConvertToJObject(BinaryReader binaryReader)
        {
            JObject manifest = new JObject();
            
            uint slotCount = binaryReader.ReadUInt32();
            uint groupCount = binaryReader.ReadUInt32();

            List<JObject> groups = new List<JObject>();

            for (int i = 0; i < groupCount; i++)
            {
                groups.Add(ReadGroup(binaryReader));
            }

            manifest.Add("version", 1);
            manifest.Add("content_version", 1);
            manifest.Add("slot_count", slotCount);
            manifest.Add("groups", JArray.FromObject(groups));

            return manifest;
        }

        private static JObject ReadGroup(BinaryReader binaryReader)
        {
            JObject group = new JObject();

            int groupTypeID = binaryReader.ReadByte();
            uint res = binaryReader.ReadUInt32();
            uint subgroupsCount = binaryReader.ReadUInt32();
            uint resourcesCount = binaryReader.ReadUInt32();
            int unknownValue = binaryReader.ReadByte(); //No resources with value other than 1
            bool hasParent = binaryReader.ReadBoolean();
            string groupName = ReadASCII(binaryReader);
            string? parentName = null;
            
            if (hasParent == true) parentName = ReadASCII(binaryReader);

            List<JObject> subgroups = new List<JObject>();
            List<JObject> resources = new List<JObject>();

            for (int i = 0; i < subgroupsCount; i++)
            {
                subgroups.Add(ReadSubgroup(binaryReader));
            }

            for (int i = 0; i < resourcesCount; i++)
            {
                resources.Add(ReadResource(binaryReader));
            }

            group.Add("type", groupTypeID == 1 ? "composite" : "simple");
            group.Add("id", groupName);
            if (res != 0) group.Add("res", res.ToString());
            if (parentName != null) group.Add("parent", parentName);
            if (subgroups.Count != 0) group.Add("subgroups", JArray.FromObject(subgroups));
            if (resources.Count != 0) group.Add("resources", JArray.FromObject(resources));

            return group;
        }

        private static JObject ReadSubgroup(BinaryReader binaryReader)
        {
            JObject subgroup = new JObject();

            uint res = binaryReader.ReadUInt32();
            string name = ReadASCII(binaryReader);

            subgroup.Add("id", name);
            if (res != 0) subgroup.Add("res", res.ToString());

            return subgroup;
        }

        private static JObject ReadResource(BinaryReader binaryReader)
        {
            JObject resource = new JObject();

            int type = binaryReader.ReadByte();
            uint slotID = binaryReader.ReadUInt32();
            uint width = binaryReader.ReadUInt32();
            uint height = binaryReader.ReadUInt32();
            int x = binaryReader.ReadInt32();
            int y = binaryReader.ReadInt32();
            uint ax = binaryReader.ReadUInt32();
            uint ay = binaryReader.ReadUInt32();
            uint aw = binaryReader.ReadUInt32();
            uint ah = binaryReader.ReadUInt32();
            uint cols = binaryReader.ReadUInt32();

            uint unknownValueUInt32 = binaryReader.ReadUInt32(); //No resources with value other than 1

            bool isAtlas = binaryReader.ReadBoolean(); 

            byte unknownValueByte_F = binaryReader.ReadByte(); //No resources with value other than 1. Maybe "runtime" ?
            byte unknownValueByte_S = binaryReader.ReadByte(); //No resources with value other than 1

            bool hasParent = binaryReader.ReadBoolean();
            string name = ReadASCII(binaryReader);
            string path = ReadASCII(binaryReader);
            string? parent = null;

            if (hasParent == true) parent = ReadASCII(binaryReader);

            string typeName = type switch
            {
                1 => "Image",
                2 => "PopAnim",
                3 => "SoundBank",
                4 => "File",
                5 => "PrimeFont",
                6 => "RenderEffect",
                7 => "DecodedSoundBank"
            };

            resource.Add("type", typeName);
            resource.Add("slot", slotID);
            resource.Add("id", name);
            resource.Add("path", path);
            if (cols != 1) resource.Add("cols", cols);
            if (parent != null) resource.Add("parent", parent);
            if (isAtlas == true) resource.Add("atlas", true);
            if (width != 0) resource.Add("width", width);
            if (height != 0) resource.Add("height", height);
            if (x != 0 && x != 2147483647) resource.Add("x", x);
            if (y != 0 && y != 2147483647) resource.Add("y", y);
            if (ax != 0) resource.Add("ax", ax);
            if (ay != 0) resource.Add("ay", ay);
            if (aw != 0) resource.Add("aw", aw);
            if (ah != 0) resource.Add("ah", ah);

            return resource;
        }

        private static string ReadASCII(BinaryReader binaryReader)
        {
            long count = binaryReader.ReadUInt32();
            byte[] output = new byte[count];

            binaryReader.Read(output, 0, (int)count);

            return Encoding.ASCII.GetString(output);
        }
    }
}
