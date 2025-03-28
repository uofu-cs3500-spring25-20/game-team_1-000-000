// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace CS3500.Networking;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{
    private static List<StreamWriter> clients = new();
    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer( Action<NetworkConnection> handleConnect, int port )
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        TcpClient c = listener.AcceptTcpClient();
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            StreamWriter w = new StreamWriter(client.GetStream(), Encoding.UTF8)
            {
                AutoFlush = true
            };
            lock (clients)
            {
                clients.Add(w);
            }
            Console.WriteLine("accepted a connection");
        }
    }
}
