// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Server
//
// The purpose of this file is to set up a server task that 
// waits for connections. The server then calls the given delegate
// when a connection is made so the message can be sent to all
// necessary connections.
// 
// Authors: Sydney Burt, Levi Hammond
// Date: 3-28-2025

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
    //A list to keep track of all the connected clients.
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
        //Listens for a new connection 
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        while (true)
        {
            //Listener accepts a new client
            TcpClient client = listener.AcceptTcpClient();
            StreamWriter w = new StreamWriter(client.GetStream(), Encoding.UTF8)
            {
                AutoFlush = true
            };
            lock (clients)
            {
                clients.Add(w);
            }
            //Creates a new network connection for the client so the provided delegate can be called.
            NetworkConnection nc = new NetworkConnection(client);
            //Calls the provided delegate when a connection is made.
            new Thread(() => handleConnect(nc)).Start();
        }
    }
}
