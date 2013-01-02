using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

/// <summary>
/// Implements a server for sending SMS messages remotely.
/// </summary>
/// <remarks>
/// <para>The server uses .NET remoting with a TCP channel to publish an <see cref="T:GsmComm.Server.SmsSender" /> object.</para>
/// <para>After starting, the server can be accessed by default at <b>tcp://(servername):2000/SMSSender</b>.</para>
/// </remarks>
namespace GsmComm.Server
{
	public class SmsServer : IDisposable
	{
		private SmsSender smsSender;

		private IChannel channel;

		private ObjRef objRefSmsSender;

		private int networkPort;

		private string uri;

		private bool isSecured;

		private AuthorizationModule authModule;

		private bool allowAnonymous;

		private string portName;

		private int baudRate;

		private int timeout;

		/// <summary>
		/// Gets or sets whether anonymous users can connect when the server is secured.
		/// </summary>
		/// <remarks>If this property is changed, while the server is running, the server must be restarted.</remarks>
		public bool AllowAnonymous
		{
			get
			{
				return this.allowAnonymous;
			}
			set
			{
				this.allowAnonymous = value;
			}
		}

		/// <summary>
		/// Gets or sets the baud rate to use when communicating with the phone.
		/// </summary>
		/// <remarks>If this property is changed, while the server is running, the server must be restarted.</remarks>
		public int BaudRate
		{
			get
			{
				return this.baudRate;
			}
			set
			{
				this.baudRate = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether security is enabled for the server.
		/// </summary>
		/// <remarks>
		/// <para>When security is enabled, only user identities authenticated by Windows are allowed to
		/// connect to the server and the communication between server and client is encrypted.
		/// Clients must also have security enabled to be able to connect to the server.</para>
		/// <para>Additionally, access may be allowed or denied for specific users when in secure mode.
		/// This is currently determined by the <see cref="P:GsmComm.Server.SmsServer.AllowAnonymous" /> property that specifies whether
		/// anonymous users can connect or not.</para>
		/// <para>If this property is changed, while the server is running, the server must be restarted.</para>
		/// </remarks>
		public bool IsSecured
		{
			get
			{
				return this.isSecured;
			}
			set
			{
				this.isSecured = value;
			}
		}

		/// <summary>
		/// Gets or sets the network port for the SMS server to listen for requests.
		/// </summary>
		/// <remarks>If this property is changed, while the server is running, the server must be restarted.</remarks>
		public int NetworkPort
		{
			get
			{
				return this.networkPort;
			}
			set
			{
				this.networkPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the COM port where the phone is connected.
		/// </summary>
		/// <remarks>If this property is changed, while the server is running, the server must be restarted.</remarks>
		public string PortName
		{
			get
			{
				return this.portName;
			}
			set
			{
				this.portName = value;
			}
		}

		/// <summary>
		/// Gets or sets the timeout when communicating with the phone.
		/// </summary>
		/// <remarks>If this property is changed, while the server is running, the server must be restarted.</remarks>
		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		/// <summary>
		/// Gets or sets the URI under which the SMS sender is available.
		/// </summary>
		/// <remarks>If this property is changed, while the server is running, the server must be restarted.</remarks>
		public string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public SmsServer()
		{
			this.smsSender = null;
			this.channel = null;
			this.objRefSmsSender = null;
			this.networkPort = 2000;
			this.uri = "SMSSender";
			this.isSecured = false;
			this.authModule = null;
			this.allowAnonymous = false;
			this.portName = "COM1";
			this.baudRate = 19200;
			this.timeout = 300;
		}

		private void ConnectEvents()
		{
			this.smsSender.MessageSendStarting += new MessageSendEventHandler(this.smsSender_MessageSendStarting);
			this.smsSender.MessageSendComplete += new MessageSendEventHandler(this.smsSender_MessageSendComplete);
			this.smsSender.MessageSendFailed += new MessageSendErrorEventHandler(this.smsSender_MessageSendFailed);
		}

		private void DisconnectEvents()
		{
			this.smsSender.MessageSendStarting -= new MessageSendEventHandler(this.smsSender_MessageSendStarting);
			this.smsSender.MessageSendComplete -= new MessageSendEventHandler(this.smsSender_MessageSendComplete);
			this.smsSender.MessageSendFailed -= new MessageSendErrorEventHandler(this.smsSender_MessageSendFailed);
		}

		/// <summary>
		/// Disposes of the host.
		/// </summary>
		public void Dispose()
		{
			this.StopInternal();
		}

		/// <summary>
		/// Finalizes the class.
		/// </summary>
		protected override void Finalize()
		{
			try
			{
				this.Dispose();
			}
			finally
			{
				this.Finalize();
			}
		}

		/// <summary>
		/// Tells if the remoting server is currently running.
		/// </summary>
		/// <returns>true if the server is running, false otherwise.</returns>
		public bool IsRunning()
		{
			return this.smsSender != null;
		}

		private void smsSender_MessageSendComplete(object sender, MessageSendEventArgs e)
		{
			if (this.MessageSendComplete != null)
			{
				this.MessageSendComplete(this, e);
			}
		}

		private void smsSender_MessageSendFailed(object sender, MessageSendErrorEventArgs e)
		{
			if (this.MessageSendFailed != null)
			{
				this.MessageSendFailed(this, e);
			}
		}

		private void smsSender_MessageSendStarting(object sender, MessageSendEventArgs e)
		{
			if (this.MessageSendStarting != null)
			{
				this.MessageSendStarting(this, e);
			}
		}

		/// <summary>
		/// Starts the server.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Server is already running.</exception>
		public void Start()
		{
			if (this.smsSender == null)
			{
				IDictionary hashtables = new Hashtable();
				hashtables["port"] = this.networkPort;
				hashtables["name"] = string.Concat("SMSSenderTCPChannel", this.networkPort.ToString());
				if (this.isSecured)
				{
					hashtables["secure"] = "true";
					this.authModule = new AuthorizationModule(this.allowAnonymous);
				}
				TcpServerChannel tcpServerChannel = new TcpServerChannel(hashtables, null, this.authModule);
				SmsSender smsSender = null;
				ObjRef objRef = null;
				try
				{
					ChannelServices.RegisterChannel(tcpServerChannel, this.isSecured);
					try
					{
						smsSender = new SmsSender(this.portName, this.baudRate, this.timeout);
						objRef = RemotingServices.Marshal(smsSender, this.uri);
					}
					catch (Exception exception)
					{
						ChannelServices.UnregisterChannel(tcpServerChannel);
						throw;
					}
				}
				catch (Exception exception1)
				{
					tcpServerChannel.StopListening(null);
					throw;
				}
				this.channel = tcpServerChannel;
				this.smsSender = smsSender;
				this.objRefSmsSender = objRef;
				this.ConnectEvents();
				return;
			}
			else
			{
				throw new InvalidOperationException("Server is already running.");
			}
		}

		/// <summary>
		/// Stops the server.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Server is not running.</exception>
		public void Stop()
		{
			if (this.smsSender != null)
			{
				this.StopInternal();
				return;
			}
			else
			{
				throw new InvalidOperationException("Server is not running.");
			}
		}

		private void StopInternal()
		{
			if (this.smsSender != null)
			{
				try
				{
					RemotingServices.Disconnect(this.smsSender);
					this.smsSender.Shutdown();
					this.DisconnectEvents();
				}
				finally
				{
					this.objRefSmsSender = null;
					this.smsSender = null;
				}
			}
			if (this.channel != null)
			{
				try
				{
					ChannelServices.UnregisterChannel(this.channel);
				}
				finally
				{
					this.channel = null;
					this.authModule = null;
				}
			}
		}

		/// <summary>
		/// The event that occurs after a successful message transfer.
		/// </summary>
		public event MessageSendEventHandler MessageSendComplete;

		/// <summary>
		/// The event that occurs after a failed message transfer.
		/// </summary>
		public event MessageSendErrorEventHandler MessageSendFailed;

		/// <summary>
		/// The event that occurs immediately before transferring a new message.
		/// </summary>
		public event MessageSendEventHandler MessageSendStarting;
	}
}