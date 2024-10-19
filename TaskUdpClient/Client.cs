using System.Net;
using System.Net.Sockets;
using System.Text;

//Istenilen file sechilir. servere gonderir.
//server client uchun folder (IP Unvanina uygun folder), onun ichirisine download edir.
//using UDP

Console.WriteLine("Client...");

var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 58758);
var rep = new IPEndPoint(IPAddress.Any, 0);
var client = new UdpClient();

var path = @"C:\Users\ASUS\Desktop\Black.png"; // change Path this Path have not your pc

if (Path.Exists(path))
{
    FileInfo file = new FileInfo(path);

    var fileNameBytes = Encoding.UTF8.GetBytes(file.Name);
    client.Send(fileNameBytes, fileNameBytes.Length, endPoint);
    
    var fileLengthBytes = Encoding.UTF8.GetBytes(file.Length.ToString());
    client.Send(fileLengthBytes, fileLengthBytes.Length, endPoint);

    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
    {
        var bytes = new byte[1024];
        var len = 0;
        while ((len = fs.Read(bytes, 0, bytes.Length)) > 0)
        {
            client.Send(bytes, len, endPoint);
            client.Receive(ref rep);
        }
    }
    Console.WriteLine("File sent");
    Console.ReadKey();
}
else
    Console.WriteLine("Have Not File With Your Path");