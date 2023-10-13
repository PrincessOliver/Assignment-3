using EchoServer;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var port = 5000;

var server = new TcpListener(IPAddress.Loopback, port);
server.Start();

Console.WriteLine("Server started");

while (true)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");
    try
    {
        var stream = client.GetStream();

        var buffer = new byte[1024];
        var readCnt = stream.Read(buffer);

        //var request = Encoding.UTF8.GetString(buffer, 0, readCnt);

        //Console.WriteLine($"Request: {request}");

        //var response = request.ToUpper();

        var requestText = Encoding.UTF8.GetString(buffer, 0, readCnt);
        Console.WriteLine(requestText);

        var request = JsonSerializer.Deserialize<Request>(requestText);

        if (string.IsNullOrEmpty(request?.Method))
        {
            var response = new Response
            {
                Status = "4 missing method"
            };
            var responseText = JsonSerializer.Serialize<Response>(response);
            var responseBuffer = Encoding.UTF8.GetBytes(responseText);
            stream.Write(responseBuffer);
        }

        //stream.Write(Encoding.UTF8.GetBytes(response));



        stream.Close();
    }
    catch (Exception)
    {
        Console.WriteLine("Unable to commicate with client...");
    }
}
