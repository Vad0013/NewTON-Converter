using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewTONConverter;


/*Console.Write("Manifest.newton: ");
string? filepath = Console.ReadLine();

JObject jobject = NewtonReader.ConvertToJObject(filepath);

using (FileStream fileStream = File.Open(filepath + ".json", FileMode.Create))
{
    using (StreamWriter streamWriter = new StreamWriter(fileStream))
    {
        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
        {
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.IndentChar = '\t';
            jsonWriter.Indentation = 1;

            jobject.WriteTo(jsonWriter);
        }
    }
}*/

Console.Write("Manifest.json: ");
string? filepath = Console.ReadLine();

JObject jobject = JObject.Parse(File.ReadAllText(filepath));
NewtonWriter.ConvertFromJObject(jobject, filepath + ".newton");