using GsmComm.GsmCommunication;
using GsmComm.Interfaces;
using GsmComm.PduConverter;
using System;
using System.Threading;

/// <summary>
/// Implements a remotable object to send SMS messages.
/// </summary>
namespace GsmComm.Server
{
	public class SmsSender : MarshalByRefObject, ISmsSender
	{
		private GsmCommMain comm;

		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.Server.SmsSender" /> class.
		/// </summary>
		/// <param name="portName">The COM port to connect to.</param>
		/// <param name="baudRate">The baud rate to use.</param>
		/// <param name="timeout">The communictaion timeout.</param>
		public SmsSender(string portName, int baudRate, int timeout)
		{
			this.disposed = false;
			this.comm = new GsmCommMain(portName, baudRate, timeout);
			this.ConnectEvents();
			try
			{
				this.comm.Open();
			}
			catch (Exception exception)
			{
				this.DisconnectEvents();
				this.comm = null;
				throw;
			}
		}

		private void comm_MessageSendComplete(object sender, MessageEventArgs e)
		{
			if (this.MessageSendComplete != null)
			{
				string userDataText = e.Pdu.UserDataText;
				string empty = string.Empty;
				if (e.Pdu is SmsSubmitPdu)
				{
					empty = (e.Pdu as SmsSubmitPdu).DestinationAddress;
				}
				MessageSendEventArgs messageSendEventArg = new MessageSendEventArgs(userDataText, empty, this.GetIdentityName());
				this.MessageSendComplete(this, messageSendEventArg);
			}
		}

		private void comm_MessageSendFailed(object sender, MessageErrorEventArgs e)
		{
			if (this.MessageSendFailed != null)
			{
				string userDataText = e.Pdu.UserDataText;
				string empty = string.Empty;
				if (e.Pdu is SmsSubmitPdu)
				{
					empty = (e.Pdu as SmsSubmitPdu).DestinationAddress;
				}
				MessageSendErrorEventArgs messageSendErrorEventArg = new MessageSendErrorEventArgs(userDataText, empty, e.Exception, this.GetIdentityName());
				this.MessageSendFailed(this, messageSendErrorEventArg);
			}
		}

		private void comm_MessageSendStarting(object sender, MessageEventArgs e)
		{
			if (this.MessageSendStarting != null)
			{
				string userDataText = e.Pdu.UserDataText;
				string empty = string.Empty;
				if (e.Pdu is SmsSubmitPdu)
				{
					empty = (e.Pdu as SmsSubmitPdu).DestinationAddress;
				}
				MessageSendEventArgs messageSendEventArg = new MessageSendEventArgs(userDataText, empty, this.GetIdentityName());
				this.MessageSendStarting(this, messageSendEventArg);
			}
		}

		private void ConnectEvents()
		{
			this.comm.MessageSendStarting += new GsmCommMain.MessageEventHandler(this.comm_MessageSendStarting);
			this.comm.MessageSendComplete += new GsmCommMain.MessageEventHandler(this.comm_MessageSendComplete);
			this.comm.MessageSendFailed += new GsmCommMain.MessageErrorEventHandler(this.comm_MessageSendFailed);
		}

		private void DisconnectEvents()
		{
			this.comm.MessageSendStarting -= new GsmCommMain.MessageEventHandler(this.comm_MessageSendStarting);
			this.comm.MessageSendComplete -= new GsmCommMain.MessageEventHandler(this.comm_MessageSendComplete);
			this.comm.MessageSendFailed -= new GsmCommMain.MessageErrorEventHandler(this.comm_MessageSendFailed);
		}

		private string GetIdentityName()
		{
			return Thread.CurrentPrincipal.Identity.Name;
		}

		/// <summary>
		/// Determines how long the remoting object lives.
		/// </summary>
		/// <returns>Always null so that the object lives forever.</returns>
		public override object InitializeLifetimeService()
		{
			return null;
		}

		/// <summary>
		/// Sends an SMS message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="destination">The destination (phone number) to which the message should be sent.</param>
		public void SendMessage(string message, string destination)
		{
			lock (this)
			{
				if (!this.disposed)
				{
					SmsSubmitPdu smsSubmitPdu = new SmsSubmitPdu(message, destination);
					this.comm.SendMessage(smsSubmitPdu);
				}
				else
				{
					throw new ObjectDisposedException("SmsSender");
				}
			}
		}

		/// <summary>
		/// Sends an SMS message.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="destination">The destination (phone number) to which the message should be sent.</param>
		/// <param name="unicode">Specifies if the message should be sent as Unicode.</param>
		public void SendMessage(string message, string destination, bool unicode)
		{
			SmsSubmitPdu smsSubmitPdu;
			lock (this)
			{
				if (!this.disposed)
				{
					if (!unicode)
					{
						smsSubmitPdu = new SmsSubmitPdu(message, destination);
					}
					else
					{
						smsSubmitPdu = new SmsSubmitPdu(message, destination, 8);
					}
					this.comm.SendMessage(smsSubmitPdu);
				}
				else
				{
					throw new ObjectDisposedException("SmsSender");
				}
			}
		}

		/// <summary>
		/// Stops the SMS sender and releases its resources.
		/// </summary>
		public void Shutdown()
		{
			if (!this.disposed)
			{
				this.comm.Close();
				this.DisconnectEvents();
				this.disposed = true;
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