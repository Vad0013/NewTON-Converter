using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewTONConverter;

Console.WriteLine(" NewRTON Converter");
Console.WriteLine(" Author: VadDiCatliff");
Console.WriteLine(" YT: https://www.youtube.com/@Vad0013");
Console.WriteLine(" Version: 1.0.0");
Console.WriteLine(" <--------------------------------------------->");

while (true)
{
    try
    {
        Console.Write(" Mode(0 - Encode, 1 - Decode, 2 - Exit): ");
        int mode = Convert.ToInt32(Console.ReadLine());

        if (mode == 0)
        {
            Console.Write(" JSON (Manifest): ");
            string jsonPath = Console.ReadLine().Replace("\"", "");

            JObject jobject = JObject.Parse(File.ReadAllText(jsonPath));
            NewtonWriter.ConvertFromJObject(jobject, jsonPath + ".newton");

            Console.WriteLine(" COMPLETED!");
        }
        else if (mode == 1)
        {
            Console.Write(" NewTON: ");
            string newtonPath = Console.ReadLine().Replace("\"", "");
            JObject jobject = NewtonReader.ConvertToJObject(newtonPath);

            using (FileStream fileStream = File.Open(newtonPath + ".json", FileMode.Create))
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
            }

            Console.WriteLine(" COMPLETED!");
        }
        else
        {
            break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(" !ERROR: " + ex.Message);
    }

    Console.WriteLine(" <--------------------------------------------->");
}

Console.Write(" Press any key to continue...");
Console.ReadKey();

