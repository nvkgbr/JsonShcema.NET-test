using Json.More;
using Json.Schema;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace json_everythingValidationTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Build Schema Registry
            string[] files = Directory.GetFiles("files/schemas/", "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var path = Path.GetFullPath(file);
                var schema = JsonSchema.FromFile(file);

                SchemaRegistry.Global.Register(new Uri(path), schema);
            }

            await ValidateFileAsync();

            Console.ReadKey();
        }

        private static async Task ValidateFileAsync()
        {
            var jsonText = await File.ReadAllTextAsync("files/input/test.json");
            var json = JsonNode.Parse(jsonText);

            var schema = JsonSchema.FromFile("files/schemas/main.schema.json");

            //Evaluation
            EvaluationResults? result = schema.Evaluate(json);

            //Printing out the result:
            var formattedResult = result.ToJsonDocument();
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                formattedResult.WriteTo(writer);
                writer.Flush(); 
                string output = Encoding.UTF8.GetString(stream.ToArray());
                Console.WriteLine(output);
            }
        }
    }
}