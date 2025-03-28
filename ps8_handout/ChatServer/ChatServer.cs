// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using System.Data;
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
    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {

        // make a list of all the connected sockets.

        if (!connectionList.Contains(connection))
        {
            connectionList.Add(connection);
            sendCount = 0;
        }

        // LISTEN

        StreamReader r = new StreamReader(connection.GetClient().GetStream(), Encoding.UTF8);

        // Send a message to all of the connected sockets.

        // handle all messages until disconnect.

        try
        {
            while (true)
            {
                var message = r.ReadLine();


                foreach (NetworkConnection socket in connectionList)
                {
                    if (!userNames.ContainsKey(socket))
                    {
                        userNames.Add(socket, message);
                    }
                    else
                    {
                        string name;
                        userNames.TryGetValue(socket, out name);
                        socket.Send(name + ": " + message);
                    }
                }
            }
        }
        catch (Exception)
        {
            //connection.Send("This connection is not possible.");
        }
    }
}