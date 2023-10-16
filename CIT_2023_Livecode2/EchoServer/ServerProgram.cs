using EchoServer;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

var port = 5000;

var server = new TcpListener(IPAddress.Loopback, port);
server.Start();

Console.WriteLine("Server started");

while (true)
{
    string[] methods = { "read", "create", "update", "delete", "echo"}; 
   
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");

    string request = client.MyRead();

    Request? requestObj = JsonSerializer.Deserialize<Request>(request, new JsonSerializerOptions { 
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    });

    if (!client.Connected)
    {
        Console.WriteLine("no client");
        var res = new Response
        {
            Status = "no client"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }

    if (string.IsNullOrWhiteSpace(requestObj?.Method))
    {
        Console.WriteLine("missing method");
        var res = new Response
        {
            Status = "missing method"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }

    if (!methods.Contains(requestObj.Method))
    {
        Console.WriteLine("illegal method");
        var res = new Response
        {
            Status = "illegal method"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }
    
    if (string.IsNullOrWhiteSpace(requestObj?.Path) || string.IsNullOrWhiteSpace(requestObj?.Body))
    {
        Console.WriteLine("missing resource");
        var res = new Response
        {
            Status = "missing resource"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }

    if (string.IsNullOrWhiteSpace(requestObj?.Date))
    {
        Console.WriteLine("missing date");
        var res = new Response
        {
            Status = "missing date"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }

    if (requestObj.Date.Length >= DateTimeOffset.Now.ToUnixTimeSeconds().ToString().Length &&
        !int.TryParse(requestObj.Date, out _) &&
        char.IsDigit(requestObj.Date[0]) &&
        char.GetNumericValue(requestObj.Date[0]) > 0)
    {
        Console.WriteLine("illegal date");
        var res = new Response
        {
            Status = "illegal date"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }

    Console.WriteLine($"Request: {requestObj}");

    var response = request;

    client.MyWrite(response);
}