// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using System.Net.Sockets;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    static List<NetworkConnection> connectionList = new List<NetworkConnection>();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main( string[] args )
    {
        Server.StartServer( HandleConnect, 11_000 );
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect( NetworkConnection connection )
    {
        connectionList.Add(connection);
        StreamReader r = new StreamReader(connection.GetTcpClient().GetStream(), Encoding.UTF8);

        // handle all messages until disconnect.
        try
        {
            while ( true )
            {
               string? message = r.ReadLine( );
                lock (connection)
                {
                    foreach (NetworkConnection socket in connectionList)
                    {
                        socket.Send(message!);
                    }
                }
            }
        }
        catch ( Exception )
        {
            // do anything necessary to handle a disconnected client in here
        }
    }
}