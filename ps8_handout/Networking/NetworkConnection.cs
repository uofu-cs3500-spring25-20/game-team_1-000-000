// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

/*
 * NetworkConnection
 * 
 * The purpose of this file is to create instances of the TcpClient,
 * establish it's reader and writer, and hold various methods to read
 * and write lines between clients.
 * 
 * Authors: Sydney Burt, Levi Hammond
 * Date: 3-22-2025
 */

using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace CS3500.Networking;

/// <summary>
///   Wraps the StreamReader/Writer/TcpClient together so we
///   don't have to keep creating all three for network actions.
/// </summary>
public sealed class NetworkConnection : IDisposable
{
    /// <summary>
    ///   The connection/socket abstraction
    /// </summary>
    private TcpClient _tcpClient = new();

    /// <summary>
    ///   Reading end of the connection
    /// </summary>
    private StreamReader? _reader = null;

    /// <summary>
    ///   Writing end of the connection
    /// </summary>
    private StreamWriter? _writer = null;

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.
    ///   </para>
    /// </summary>
    /// <param name="tcpClient">
    ///   An already existing TcpClient
    /// </param>
    public NetworkConnection( TcpClient tcpClient )
    {
        if ( IsConnected )
        {
            // Only establish the reader/writer if the provided TcpClient is already connected.
            _reader = new StreamReader( _tcpClient.GetStream(), Encoding.UTF8 );
            _writer = new StreamWriter( _tcpClient.GetStream(), Encoding.UTF8 ) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.  The tcpClient will be unconnected at the start.
    ///   </para>
    /// </summary>
    public NetworkConnection( )
        : this( new TcpClient( ) )
    {
    }

    /// <summary>
    /// Gets a value indicating whether the socket is connected.
    /// </summary>
    public bool IsConnected
    {
        get { return _tcpClient.Connected; }
    }

    /// <summary>
    ///   Try to connect to the given host:port. 
    /// </summary>
    /// <param name="host"> The URL or IP address, e.g., www.cs.utah.edu, or  127.0.0.1. </param>
    /// <param name="port"> The port, e.g., 11000. </param>
    public void Connect( string host, int port )
    {
        //try
        //{
        //    _tcpClient = new TcpClient();
        //    _tcpClient.Connect(host, port);
        //    _reader = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8);
        //    _writer = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        //}
        //catch (Exception e)
        //{
        //    throw new SocketException();
        //}

        if (IsConnected) 
        {
            TcpClient tcpClient = new TcpClient( host, port );
        }
    }

    /// <summary>
    ///   Send a message to the remote server.  If the <paramref name="message"/> contains
    ///   new lines, these will be treated on the receiving side as multiple messages.
    ///   This method should attach a newline to the end of the <paramref name="message"/>
    ///   (by using WriteLine).
    ///   If this operation can not be completed (e.g. because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <param name="message"> The string of characters to send. </param>
    public void Send( string message )
    {
        if (IsConnected == false) // Throws an exception if the socket is not connected yet.
        {
            throw new InvalidOperationException();
        }
        else if (_writer == null)
        {
            throw new InvalidOperationException();
        }
        _writer!.WriteLineAsync(message); // Sends a message with a new line at the end through the writer.
    }


    /// <summary>
    ///   Read a message from the remote side of the connection.  The message will contain
    ///   all characters up to the first new line. See <see cref="Send"/>.
    ///   If this operation can not be completed (e.g. because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <returns> The contents of the message. </returns>
    public string ReadLine( )
    {
        // TODO: implement this
        if (IsConnected == false) // Throws an exception if the socket is not connected yet.
        {
            throw new InvalidOperationException();
        }
        else if (_reader == null)
        {
            throw new InvalidOperationException();
        }
        return _reader!.ReadLine()!; // Reads the message from the other side of the connection.
    }

    /// <summary>
    ///   If connected, disconnect the connection and clean 
    ///   up (dispose) any streams.
    /// </summary>
    public void Disconnect()
    {
        if (IsConnected == true) // If the socket is not connected, it will not call Close and throw an error.
        {
            _tcpClient.Close(); // Disposes of this instance of _tcpClient and disposes of its contents.
        }
    }

    /// <summary>
    ///   Automatically called with a using statement (see IDisposable)
    /// </summary>
    public void Dispose( )
    {
        Disconnect();
    }
}
