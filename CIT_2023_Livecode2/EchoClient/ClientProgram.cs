﻿using Assignment3TestSuite;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var port = 5000;
var client = new TcpClient();
client.Connect(IPAddress.Loopback, port);
Console.WriteLine("Connected");

var stream = client.GetStream();

//Console.Write("Write message: ");
//var msg = Console.ReadLine();

var request = new
{
    Method = "read",
    Path = "/api/categories",
    Date = DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
};

var requestJsonStr = JsonSerializer.Serialize(request);
stream.Write(Encoding.UTF8.GetBytes(requestJsonStr));

var buffer = new byte[1024];
var rcnt = stream.Read(buffer);

Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, rcnt));

client.Close();