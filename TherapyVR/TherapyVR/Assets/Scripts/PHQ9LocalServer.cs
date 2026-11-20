using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// Add this component to the PHQ9Manager GameObject.
// Call Serve(html) before opening the browser URL.
public class PHQ9LocalServer : MonoBehaviour
{
    private const int Port = 52879;

    private TcpListener _listener;
    private Thread _thread;
    private string _html;
    private volatile bool _running;

    public void Serve(string html)
    {
        _html = html;
        if (_listener == null) StartServer();
    }

    private void StartServer()
    {
        try
        {
            _running = true;
            _listener = new TcpListener(IPAddress.Loopback, Port);
            _listener.Start();
            _thread = new Thread(Loop) { IsBackground = true };
            _thread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PHQ9LocalServer] Failed to start: {e.Message}");
        }
    }

    private void Loop()
    {
        while (_running)
        {
            try
            {
                TcpClient client = _listener.AcceptTcpClient();
                HandleClient(client);
            }
            catch (SocketException) { break; }
            catch { }
        }
    }

    private void HandleClient(TcpClient client)
    {
        using (client)
        {
            NetworkStream stream = client.GetStream();

            // Drain the HTTP request — we serve the same HTML regardless of path.
            byte[] buf = new byte[4096];
            if (stream.DataAvailable) stream.Read(buf, 0, buf.Length);

            byte[] body   = Encoding.UTF8.GetBytes(_html ?? "<h1>No report available</h1>");
            string header = $"HTTP/1.1 200 OK\r\n" +
                            $"Content-Type: text/html; charset=utf-8\r\n" +
                            $"Content-Length: {body.Length}\r\n" +
                            $"Connection: close\r\n\r\n";
            byte[] headerBytes = Encoding.ASCII.GetBytes(header);

            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(body, 0, body.Length);
        }
    }

    void OnDestroy()
    {
        _running = false;
        try { _listener?.Stop(); } catch { }
        _listener = null;
    }
}
