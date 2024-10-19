using System.Net;
using System.Net.Sockets;
using System.Text;

var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 58758);
var server = new UdpClient(endPoint);

Console.WriteLine("Server is running");

Task myTask = Task.Run(() =>
{
    var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

    while (true)
    {
        var fileNameBytes = server.Receive(ref remoteEndPoint);
        var fileLengthBytes = server.Receive(ref remoteEndPoint);

        var fileName = Encoding.UTF8.GetString(fileNameBytes);
        int fileLen = int.Parse(Encoding.UTF8.GetString(fileLengthBytes));

        var destination = Environment.CurrentDirectory;//vs2022 myproj directory
        destination = Path.Combine(destination, $"{remoteEndPoint.Address}");

        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }

        var path = $"{destination}\\{DateTime.Now:HH.mm.ss}{Path.GetExtension(fileName)}";

        using (var fStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            var totalRecievedByte = 0;
            while (true)
            {
                var fileByte = server.Receive(ref remoteEndPoint);
                fStream.Write(fileByte);

                totalRecievedByte += fileByte.Length;
                server.Send(null, remoteEndPoint);

                if (fileLen == totalRecievedByte)
                    break;
            }
        }
        Console.WriteLine("File Downloaded");
    }
});
myTask.Wait();


