using System.Net.Sockets;

namespace CopyPath___Modular_MAUI_.Helpers
{
    public static class NetworkHelper
    {
        public static async Task SendFileAsync(string filePath, string ipAddress, int port)
        {
            using var client = new TcpClient(ipAddress, port);
            using var networkStream = client.GetStream();
            var fileBytes = File.ReadAllBytes(filePath);
            await networkStream.WriteAsync(fileBytes, 0, fileBytes.Length);
        }

        public static async Task ReceiveFileAsync(string savePath, int port)
        {
            var listener = new TcpListener(System.Net.IPAddress.Any, port);
            listener.Start();
            using var client = await listener.AcceptTcpClientAsync();
            using var networkStream = client.GetStream();
            using var fileStream = new FileStream(savePath, FileMode.Create);
            await networkStream.CopyToAsync(fileStream);
        }
    }
}
