using RestSharp;
using System.Text;

namespace DynamicDLL.Services
{
    public class Encoder
    {
        private RestClient _client;
        private string schema;
        private string data;
        private Encoding encoding;

        public Encoder(string schema, string data) 
        {
            this.schema = schema;
            this.data = data;
            _client = new RestClient();
            encoding = Encoding.GetEncoding(1252);
        }

        public byte[]? Encode(string name)
        {
            var request = new RestRequest("http://localhost:5000/encode");
            request.AddBody(data);
            request.AddParameter("schema", schema);
            request.AddParameter("name", "Rocket");
            request.AddHeader("Content-Type", "text/plain");
            request.Method = Method.Get;
            var responseEncode = _client.Execute(request);
            string? encodedData = responseEncode.Content;
            if (encodedData == null) { return null; }
            return encoding.GetBytes(encodedData);
        }

        public string? Decode(byte[] bytes)
        {
            string? reverseDecodedData = encoding.GetString(bytes);
            var requestDecode = new RestRequest("http://localhost:5000/decode");
            requestDecode.AddBody(reverseDecodedData);
            requestDecode.AddParameter("schema", schema);
            requestDecode.AddParameter("name", "Rocket");
            requestDecode.AddHeader("Content-Type", "text/plain");
            requestDecode.Method = Method.Get;
            var responseDecode = _client.Execute(requestDecode);
            return responseDecode.Content;
        }
    }
}
