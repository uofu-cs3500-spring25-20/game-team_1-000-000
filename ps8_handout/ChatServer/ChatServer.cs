// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// ChatServer
//
// The purpose of this file is to set up a chat server that handles each client separately.
// When one connected member sends a message, everyone in the chat gets the message.
// When a member disconnects, they can no longer receive or send messages. 
// 
// Authors: Sydney Burt, Levi Hammond
// Date: 3-28-2025

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
    //A list to keep track of all the connections.
    static List<NetworkConnection> connectionList = new List<NetworkConnection>();

    //A list to keep track of the names of each chat member/connection.
    static Dictionary<string, NetworkConnection> userNames = new Dictionary<string, NetworkConnection>();

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
        //Only adds the connection to the list if it is a new connection
        if (!connectionList.Contains(connection))
        {
            connectionList.Add(connection);
        }
        
        //Create a variable to hold the name of the chat member/connection.
        string? name = null;
        
        try
        {
            while (true)
            {
                //Read the message from the chat member.
                var message = connection.ReadLine();

                //If the chat member is new, the first thing they type and submit is their chat name.
                if (name == null)
                {
                    name = message;
                    userNames.Add(name, connection);
                    continue;
                }

                //Send the message (and the chat member's name) to every connected single chat member.
                foreach (NetworkConnection socket in connectionList)
                {
                    socket.Send(name + ": " + message);
                }
            }
        }
        catch (Exception)
        {
            //If the chat member has disconnected from the chat, remove them from the dictionary.
            userNames.Remove(name!);
        }
    }
}