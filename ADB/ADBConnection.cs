/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/15/2016
 * Time: 18:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using LogFramework;
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Utilities.Endianness;

namespace WPF_S39_Commander.ADB
{
	/// <summary>
	/// Description of ADBConnection.
	/// </summary>
	public class ADBConnection
	{
		private readonly TcpClient Client;

		public bool IsConnected
		{
			get
			{
				return this.Client.Connected;
			}
		}

		public EndianBinaryReader StreamReader
		{
			get;
			private set;
		}

		public ADBConnection()
		{
			this.Client = new TcpClient
			{
				NoDelay = true
			};
		}

		public bool Connect()
		{
			bool result;
			try
			{
				this.Client.Connect("127.0.0.1", 5037);
				this.StreamReader = new EndianBinaryReader(EndianBitConverter.Little, this.Client.GetStream());
				Logger.LogDebug("Connection to ADB server established", "", "d:\\Android\\Ten Studio\\AndroidDebugBridge\\ADBConnection.cs", "Connect");
				result = true;
			}
			catch (Exception ex)
			{
				Logger.LogError("Error while trying to connect to ADB server! Exception: " + ex.Message, "", "d:\\Android\\Ten Studio\\AndroidDebugBridge\\ADBConnection.cs", "Connect");
				result = false;
			}
			return result;
		}

		public void Disconnect()
		{
			throw new NotImplementedException();
		}

		public ADBResponse ReadResponse()
		{
			if (!this.IsConnected)
			{
				Logger.LogError("Can't read response from client, not connected!", "", "d:\\Android\\Ten Studio\\AndroidDebugBridge\\ADBConnection.cs", "ReadResponse");
				return null;
			}
			return ADBResponse.GetResponse(this.StreamReader);
		}

		public byte[] ReadBytes(int count)
		{
			if (!this.IsConnected)
			{
				Logger.LogError("Can't read data from client, not connected!", "", "d:\\Android\\Ten Studio\\AndroidDebugBridge\\ADBConnection.cs", "ReadBytes");
				return null;
			}
			return this.StreamReader.ReadBytes(count);
		}

		public string ReadString(int length)
		{
			byte[] bytes = this.ReadBytes(length);
			return Encoding.ASCII.GetString(bytes);
		}

		public int ReadDataLength()
		{
			int result;
			try
			{
				string s = this.ReadString(4);
				int num = int.Parse(s, NumberStyles.HexNumber);
				result = num;
			}
			catch (Exception ex)
			{
				Logger.LogError("Error while trying to read data length, exception: " + ex.Message, "", "d:\\Android\\Ten Studio\\AndroidDebugBridge\\ADBConnection.cs", "ReadDataLength");
				result = -1;
			}
			return result;
		}

		public void SendBytes(byte[] data)
		{
			if (!this.IsConnected)
			{
				Logger.LogError("Can't write data to client, not connected!", "", "d:\\Android\\Ten Studio\\AndroidDebugBridge\\ADBConnection.cs", "SendBytes");
				return;
			}
			this.Client.GetStream().Write(data, 0, data.Length);
		}

		public void SendStringRequest(string request)
		{
			byte[] data = ADBConnection.FormADBRequest(request);
			this.SendBytes(data);
		}

		public static byte[] FormADBRequest(string request)
		{
			string s = string.Format("{0:X4}{1}", request.Length, request);
			return Encoding.ASCII.GetBytes(s);
		}
	
	}
}
