using EchoServer;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

var port = 5000;

var server = new TcpListener(IPAddress.Loopback, port);
server.Start();

Console.WriteLine("Server started");

while (true)
{
    string[] methods = { "read", "create", "update", "delete", "echo"};

    //List<Category> categories = new()
    //{
    //    new Category() { Id = 1, Name = "Beverages" },
    //    new Category() { Id = 2, Name = "Condiments" },
    //    new Category() { Id = 3, Name = "Confections" }
    //};

    var categories = new List<object>
    {
        new {cid = 1, name = "Beverages"},
        new {cid = 2, name = "Condiments"},
        new {cid = 3, name = "Confections"}
    };

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
    }

    if (requestObj?.Method == "read" && requestObj?.Path?.Length > 17)
    {
        try
        {
            var res = new Response
            {
                Status = "1 Ok",
                Body = JsonSerializer.Serialize(categories[Int32.Parse(requestObj.Path.Substring(16)) - 1])
            };

            client.MyWrite(JsonSerializer.Serialize(res));
            Console.WriteLine("1 Ok");
            return;
        }
        catch (Exception)
        {
            var res = new Response
            {
                Status = "5 not found",
            };

            client.MyWrite(JsonSerializer.Serialize(res));
            Console.WriteLine("5 not found");
            return;
        }
    }

    if (requestObj.Method == "read" && requestObj.Path == "/api/categories")
    {
        Console.WriteLine("1 Ok");
        var res = new Response
        {
            Status = "1 Ok",
            Body = JsonSerializer.Serialize(categories)
        };

        client.MyWrite(JsonSerializer.Serialize(res));
        return;
    }

    if (requestObj.Path != $"/api/categories/1" ||
      requestObj.Path != $"/api/categories/2" ||
      requestObj.Path != $"/api/categories/3"
      )
    {
        Console.WriteLine("4 Bad Request");
        var res = new Response
        {
            Status = "4 Bad Request"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    var requestBody = JsonConvert.DeserializeAnonymousType(requestObj.Body, new { Name = "" });
    if (string.IsNullOrWhiteSpace(requestBody.Name))
    {
        Console.WriteLine("4 Bad Request");
        var res = new Response
        {
            Status = "4 Bad Request"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (!requestObj.Path.Contains("/api/categories/"))
    {
        Console.WriteLine("4 Bad Request");
        var res = new Response
        {
            Status = "4 Bad Request"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (!string.IsNullOrWhiteSpace(requestObj.Body))
    {
        Console.WriteLine("body exists");
        var res = new Response
        {
            Body = requestObj.Body
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (string.IsNullOrWhiteSpace(requestObj?.Body))
    {
        Console.WriteLine("missing body");
        var res = new Response
        {
            Status = "missing body"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (!HelperMethods.IsValidJson(requestObj.Body))
    {
        Console.WriteLine("illegal body");
        var res = new Response
        {
            Status = "illegal body"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (string.IsNullOrWhiteSpace(requestObj?.Date))
    {
        Console.WriteLine("missing date");
        var res = new Response
        {
            Status = "missing date"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (string.IsNullOrWhiteSpace(requestObj?.Method))
    {
        Console.WriteLine("missing method");
        var res = new Response
        {
            Status = "4 missing method"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (!methods.Contains(requestObj.Method))
    {
        Console.WriteLine("illegal method");
        var res = new Response
        {
            Status = "illegal method"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
    }

    if (string.IsNullOrWhiteSpace(requestObj?.Method) ||
        string.IsNullOrWhiteSpace(requestObj?.Path) ||
        string.IsNullOrWhiteSpace(requestObj?.Date) ||
        string.IsNullOrWhiteSpace(requestObj?.Body)
        )
    {
        Console.WriteLine("missing resource");
        var res = new Response
        {
            Status = "missing resource"
        };

        client.MyWrite(JsonSerializer.Serialize(res));
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
    }

    //var result = categories.Find(x => x.cid == Int32.Parse(requestObj.Path.Substring(16)));

    //if (requestObj.Method == "read" && requestObj.Path.Length > 17)
    //{
    //    Console.WriteLine("5 not found");
    //    var res = new Response
    //    {
    //        Status = "5 not found",
    //        Body = JsonSerializer.Serialize(categories[Int32.Parse(requestObj.Path.Substring(16)) - 1])
    //    };

    //    client.MyWrite(JsonSerializer.Serialize(res));
    //    return;
    //}

    //string id = requestObj.Path.Substring(16);

    //if (requestObj.Path != $"/api/categories/{id}" && !int.TryParse("123", out _))
    //{
    //    Console.WriteLine("4 Bad Request");
    //    var res = new Response
    //    {
    //        Status = "4 Bad Request"
    //    };

    //    client.MyWrite(JsonSerializer.Serialize(res));
    //}
   
    Console.WriteLine($"Request: {request}");

    var response = request;

    client.MyWrite(response);
}