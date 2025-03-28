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

    static Dictionary<NetworkConnection, string> userNames = new Dictionary<NetworkConnection, string>();

    static int sendCount;

    static string name = "";

    static NetworkConnection c;
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
        sendCount = 0;
        StreamReader r = new StreamReader(connection.GetTcpClient().GetStream(), Encoding.UTF8);

        // handle all messages until disconnect.
        try
        {
            while ( true )
            {
                string? message = r.ReadLine( );
                if (sendCount < 1)
                {
                    name = message!;
                    c = connection;
                    userNames.Add(c, message!);
                }
                lock (connection)
                {
                    foreach (NetworkConnection socket in connectionList)
                    {
                        socket.Send(userNames[c].ToString() + " " + message!);
                    }
                }
                sendCount++;
            }
        }
        catch ( Exception )
        {
            foreach (NetworkConnection socket in connectionList)
            {
                if (!socket.IsConnected)
                    connectionList.Remove(socket);
            }
        }
    }
}