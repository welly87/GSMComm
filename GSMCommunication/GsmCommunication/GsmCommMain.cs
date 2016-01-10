using GsmComm.PduConverter;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

/// <summary>
/// Interacts with a mobile phone to execute various functions.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class GsmCommMain
	{
		/// <summary>
		/// The default port to connect to.
		/// </summary>
		public const string DefaultPortName = "COM1";

		/// <summary>
		/// The default baud rate to use.
		/// </summary>
		public const int DefaultBaudRate = 19200;

		/// <summary>
		/// The default communication timeout.
		/// </summary>
		public const int DefaultTimeout = 300;

		private GsmPhone theDevice;

		private static bool versionInfoLogged;

		private LogLevel logLevel;

		/// <summary>
		/// Gets the baud rate in use.
		/// </summary>
		public int BaudRate
		{
			get
			{
				return this.theDevice.BaudRate;
			}
		}

		/// <summary>
		/// Gets or sets the delay in milliseconds between the checks to verify that the connection
		/// to the phone is still alive.
		/// </summary>
		public int ConnectionCheckDelay
		{
			get
			{
				return this.theDevice.ConnectionCheckDelay;
			}
			set
			{
				this.theDevice.ConnectionCheckDelay = value;
			}
		}

		/// <summary>
		/// Get or sets the current log level for this instance.
		/// </summary>
		public LogLevel LogLevel
		{
			get
			{
				return this.logLevel;
			}
			set
			{
				this.logLevel = value;
				this.theDevice.LogLevel = value;
			}
		}

		/// <summary>
		/// Gets COM port currently connected to.
		/// </summary>
		public string PortName
		{
			get
			{
				return this.theDevice.PortName;
			}
		}

		/// <summary>
		/// Gets the current communication timeout.
		/// </summary>
		public int Timeout
		{
			get
			{
				return this.theDevice.Timeout;
			}
		}

		static GsmCommMain()
		{
			GsmCommMain.versionInfoLogged = false;
		}

		/// <summary>
		/// Initializes a new instance of the class using default parameters.
		/// </summary>
		/// <remarks>Uses the default values: port=COM1, baud rate=19200, timeout=300ms.</remarks>
		public GsmCommMain()
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone("COM1", 19200, 300);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified parameters.
		/// </summary>
		/// <param name="portName">The communication (COM) port to use.</param>
		/// <remarks>Uses the default values: baud rate=19200, timeout=300ms.</remarks>
		public GsmCommMain(string portName)
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone(portName, 19200, 300);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified parameters.
		/// </summary>
		/// <param name="portNumber">The communication (COM) port to use.</param>
		/// <remarks>Uses the default values: baud rate=19200, timeout=300ms.</remarks>
		public GsmCommMain(int portNumber)
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone(portNumber, 19200, 300);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified parameters.
		/// </summary>
		/// <param name="portName">The communication (COM) port to use.</param>
		/// <param name="baudRate">The baud rate (speed) to use.</param>
		/// <remarks>Uses the default values: timeout=300ms.</remarks>
		public GsmCommMain(string portName, int baudRate)
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone(portName, baudRate, 300);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified parameters.
		/// </summary>
		/// <param name="portNumber">The communication (COM) port to use.</param>
		/// <param name="baudRate">The baud rate (speed) to use.</param>
		/// <remarks>Uses the default values: timeout=300ms.</remarks>
		public GsmCommMain(int portNumber, int baudRate)
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone(portNumber, baudRate, 300);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified parameters.
		/// </summary>
		/// <param name="portName">The communication (COM) port to use.</param>
		/// <param name="baudRate">The baud rate (speed) to use.</param>
		/// <param name="timeout">The communication timeout in milliseconds.</param>
		public GsmCommMain(string portName, int baudRate, int timeout)
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone(portName, baudRate, timeout);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Initializes a new instance of the class using the specified parameters.
		/// </summary>
		/// <param name="portNumber">The communication (COM) port to use.</param>
		/// <param name="baudRate">The baud rate (speed) to use.</param>
		/// <param name="timeout">The communication timeout in milliseconds.</param>
		public GsmCommMain(int portNumber, int baudRate, int timeout)
		{
			this.logLevel = LogLevel.Verbose;
			this.theDevice = new GsmPhone(portNumber, baudRate, timeout);
			this.theDevice.LogLevel = this.logLevel;
		}

		/// <summary>
		/// Acknowledges a new received short message that was directly routed to the application.
		/// </summary>
		/// <remarks>Acknowledges are required for most received messages if <see cref="M:GsmComm.GsmCommunication.GsmCommMain.IsAcknowledgeRequired" />
		/// returns true. The acknowledge requirement can be changed with the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.RequireAcknowledge(System.Boolean)" />
		/// method.
		/// </remarks>
		public void AcknowledgeNewMessage()
		{
			this.theDevice.AcknowledgeNewMessage();
		}

		private void AsyncCallback(IAsyncResult ar)
		{
			AsyncResult asyncResult = (AsyncResult)ar;
			if (asyncResult.AsyncDelegate as GsmCommMain.MessageEventHandler == null)
			{
				if (asyncResult.AsyncDelegate as GsmCommMain.MessageErrorEventHandler == null)
				{
					this.LogIt(LogLevel.Warning, string.Concat("AsyncCallback got unknown delegate: ", asyncResult.AsyncDelegate.GetType().ToString()));
					return;
				}
				else
				{
					this.LogIt(LogLevel.Info, "Ending async MessageErrorEventHandler call");
					GsmCommMain.MessageErrorEventHandler asyncDelegate = (GsmCommMain.MessageErrorEventHandler)asyncResult.AsyncDelegate;
					asyncDelegate.EndInvoke(ar);
					return;
				}
			}
			else
			{
				this.LogIt(LogLevel.Info, "Ending async MessageEventHandler call");
				GsmCommMain.MessageEventHandler messageEventHandler = (GsmCommMain.MessageEventHandler)asyncResult.AsyncDelegate;
				messageEventHandler.EndInvoke(ar);
				return;
			}
		}

		/// <summary>
		/// Closes the connection to the device.
		/// </summary>
		/// <remarks>You can check the current connection state with the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.IsOpen" /> method.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.IsOpen" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.Open" />
		/// </remarks>
		public void Close()
		{
			this.theDevice.Close();
			this.DisconnectEvents();
		}

		private void ConnectEvents()
		{
			this.theDevice.LoglineAdded += new LoglineAddedEventHandler(this.theDevice_LoglineAdded);
			this.theDevice.ReceiveProgress += new ProgressEventHandler(this.theDevice_ReceiveProgress);
			this.theDevice.ReceiveComplete += new ProgressEventHandler(this.theDevice_ReceiveComplete);
			this.theDevice.MessageReceived += new MessageReceivedEventHandler(this.theDevice_MessageReceived);
			this.theDevice.PhoneConnected += new EventHandler(this.theDevice_PhoneConnected);
			this.theDevice.PhoneDisconnected += new EventHandler(this.theDevice_PhoneDisconnected);
		}

		/// <summary>
		/// Creates a new phonebook entry.
		/// </summary>
		/// <param name="entry">The entry to create.</param>
		/// <param name="storage">The storage to save the entry.</param>
		/// <remarks>The <see cref="P:GsmComm.GsmCommunication.PhonebookEntry.Index" /> property of the entry is ignored, 
		/// the entry is always saved in the first free location. All other properties must be set
		/// correctly.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DeletePhonebookEntry(System.Int32,System.String)" /></remarks>
		public void CreatePhonebookEntry(PhonebookEntry entry, string storage)
		{
			this.theDevice.SelectPhonebookStorage(storage);
			this.theDevice.WritePhonebookEntry(entry);
		}

		/// <summary>
		/// Decodes a received short message.
		/// </summary>
		/// <param name="message">The message to decode</param>
		/// <returns>The decoded message as <see cref="T:GsmComm.PduConverter.SmsPdu" /> object.</returns>
		public SmsPdu DecodeReceivedMessage(ShortMessage message)
		{
			IncomingSmsPdu incomingSmsPdu = null;
			try
			{
				incomingSmsPdu = IncomingSmsPdu.Decode(message.Data, true, message.Length);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.LogIt(LogLevel.Error, string.Concat("IncomingSmsPdu decoder can't decode message: ", exception.Message));
				this.LogIt(LogLevel.Error, string.Concat("  Message data = \"", message.Data, "\""));
				int length = message.Length;
				this.LogIt(LogLevel.Error, string.Concat("  Message length = ", length.ToString()));
				throw;
			}
			return incomingSmsPdu;
		}

		/// <summary>
		/// Decodes a short message read from the phone.
		/// </summary>
		/// <param name="message">The message to decode.</param>
		/// <returns>The decoded short message.</returns>
		/// <remarks>
		/// Use this function to decode messages that were read with the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadRawMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" /> function.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadRawMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" />
		/// </remarks>
		/// <exception cref="T:GsmComm.GsmCommunication.CommException">There is no decoder available that can handle the message's status.</exception>
		public SmsPdu DecodeShortMessage(ShortMessageFromPhone message)
		{
			PhoneMessageStatus status = (PhoneMessageStatus)message.Status;
			SmsPdu smsPdu = null;
			PhoneMessageStatus phoneMessageStatu = status;
			if (phoneMessageStatu == PhoneMessageStatus.ReceivedUnread || phoneMessageStatu == PhoneMessageStatus.ReceivedRead)
			{
				try
				{
					smsPdu = this.DecodeReceivedMessage(message);
				}
				catch (Exception exception)
				{
					this.LogIt(LogLevel.Warning, "Unable to decode message with specified status - trying other variants.");
					smsPdu = this.DecodeStoredMessage(message);
				}
			}
			else if (phoneMessageStatu == PhoneMessageStatus.StoredUnsent || phoneMessageStatu == PhoneMessageStatus.StoredSent)
			{
				try
				{
					smsPdu = this.DecodeStoredMessage(message);
				}
				catch (Exception exception1)
				{
					this.LogIt(LogLevel.Warning, "Unable to decode message with specified status - trying other variants.");
					smsPdu = this.DecodeReceivedMessage(message);
				}
			}
			else
			{
				string[] str = new string[5];
				str[0] = "No decoder available for message of status \"";
				str[1] = status.ToString();
				str[2] = "\" (index ";
				int index = message.Index;
				str[3] = index.ToString();
				str[4] = ").";
				string str1 = string.Concat(str);
				this.LogIt(LogLevel.Error, str1);
				throw new CommException(str1);
			}
			return smsPdu;
		}

		private SmsPdu DecodeStoredMessage(ShortMessage message)
		{
			OutgoingSmsPdu outgoingSmsPdu = null;
			try
			{
				outgoingSmsPdu = OutgoingSmsPdu.Decode(message.Data, true, message.Length);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.LogIt(LogLevel.Error, string.Concat("OutgoingSmsPdu decoder can't decode message: ", exception.Message));
				this.LogIt(LogLevel.Error, string.Concat("  Message data = \"", message.Data, "\""));
				int length = message.Length;
				this.LogIt(LogLevel.Error, string.Concat("  Message length = ", length.ToString()));
				throw;
			}
			return outgoingSmsPdu;
		}

		/// <summary>
		/// Deletes all phonebook entries.
		/// </summary>
		/// <param name="storage">The storage to use.</param>
		/// <remarks>To delete single entries, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DeletePhonebookEntry(System.Int32,System.String)" /> function.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DeletePhonebookEntry(System.Int32,System.String)" />
		/// </remarks>
		public void DeleteAllPhonebookEntries(string storage)
		{
			this.theDevice.SelectPhonebookStorage(storage);
			PhonebookEntry[] phonebookEntryArray = this.theDevice.ReadPhonebookEntries();
			PhonebookEntry[] phonebookEntryArray1 = phonebookEntryArray;
			for (int i = 0; i < (int)phonebookEntryArray1.Length; i++)
			{
				PhonebookEntry phonebookEntry = phonebookEntryArray1[i];
				this.theDevice.DeletePhonebookEntry(phonebookEntry.Index);
			}
		}

		/// <summary>
		/// Deletes the specified short message.
		/// </summary>
		/// <param name="index">The index of the message to delete.</param>
		/// <param name="storage">The storage to use.</param>
		/// <remarks>
		/// To delete a group of messages, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DeleteMessages(GsmComm.GsmCommunication.DeleteScope,System.String)" /> function.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DeleteMessages(GsmComm.GsmCommunication.DeleteScope,System.String)" />
		/// </remarks>
		public void DeleteMessage(int index, string storage)
		{
			this.LogIt(LogLevel.Info, "Deleting message...");
			this.theDevice.SelectReadStorage(storage);
			this.theDevice.DeleteMessage(index);
		}

		/// <summary>
		/// Deletes the specified group of messages.
		/// </summary>
		/// <param name="scope">Specifies the messages that are affected by this command.</param>
		/// <param name="storage">The storage to use.</param>
		/// <remarks>
		/// To delete a single message, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DeleteMessage(System.Int32,System.String)" /> function.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DeleteMessage(System.Int32,System.String)" />
		/// </remarks>
		public void DeleteMessages(DeleteScope scope, string storage)
		{
			int num = (int)scope;
			DeleteFlag deleteFlag = (DeleteFlag)Enum.Parse(typeof(DeleteFlag), num.ToString());
			string[] str = new string[5];
			str[0] = "Deleting \"";
			str[1] = scope.ToString();
			str[2] = "\" messages in storage \"";
			str[3] = storage;
			str[4] = "\"...";
			this.LogIt(LogLevel.Info, string.Concat(str));
			this.theDevice.SelectReadStorage(storage);
			this.theDevice.DeleteMessage(1, deleteFlag);
		}

		/// <summary>
		/// Deletes a phonebook entry.
		/// </summary>
		/// <param name="index">The index of the entry to delete.</param>
		/// <param name="storage">The storage to use.</param>
		/// <remarks>To delete all phonebook entries at once, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DeleteAllPhonebookEntries(System.String)" />
		/// function.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DeleteAllPhonebookEntries(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.CreatePhonebookEntry(GsmComm.GsmCommunication.PhonebookEntry,System.String)" />
		/// </remarks>
		public void DeletePhonebookEntry(int index, string storage)
		{
			this.theDevice.SelectPhonebookStorage(storage);
			this.theDevice.DeletePhonebookEntry(index);
		}

		/// <summary>
		/// Disables all message notifications.
		/// </summary>
		/// <remarks>
		/// <para>Call this function after a call to <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageNotifications" /> to disable this functionality
		/// again.</para>
		/// <para>It's highly recommended to disable notifications again before closing the connection to
		/// the phone. If it doesn't get disabled, the phone will still try to send notifications, which will not be
		/// successful. The phone <b>may</b> buffer unsuccessful notifications, but you should generally not rely
		/// on this.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageNotifications" />
		/// </remarks>
		public void DisableMessageNotifications()
		{
			this.theDevice.SetMessageIndications(new MessageIndicationSettings(0, 0, 0, 0, 0));
		}

		/// <summary>
		/// Disables all message routings.
		/// </summary>
		/// <remarks>
		/// <para>Call this function after a call to <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageRouting" /> to disable this functionality
		/// again.</para>
		/// <para><b>CAUTION:</b> It's highly recommended to disable routing again before closing the connection to
		/// the phone. If it doesn't get disabled, the phone will still try to route messages on, which will not be
		/// successful. You <b>may</b> lose messages in such a case if the phone doesn't buffer unsuccessful routings.
		/// </para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageRouting" />
		/// </remarks>
		public void DisableMessageRouting()
		{
			this.theDevice.SetMessageIndications(new MessageIndicationSettings(0, 0, 0, 0, 0));
		}

		/// <summary>
		/// Disables the SMS batch mode.
		/// </summary>
		/// <remarks>Disables the SMS batch mode previously enabled with
		/// <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnablePermanentSmsBatchMode" /> or <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableTemporarySmsBatchMode" />.
		/// </remarks>
		public void DisableSmsBatchMode()
		{
			this.theDevice.SetMoreMessagesToSend(MoreMessagesMode.Disabled);
		}

		private void DisconnectEvents()
		{
			this.theDevice.LoglineAdded -= new LoglineAddedEventHandler(this.theDevice_LoglineAdded);
			this.theDevice.ReceiveProgress -= new ProgressEventHandler(this.theDevice_ReceiveProgress);
			this.theDevice.ReceiveComplete -= new ProgressEventHandler(this.theDevice_ReceiveComplete);
			this.theDevice.MessageReceived -= new MessageReceivedEventHandler(this.theDevice_MessageReceived);
			this.theDevice.PhoneConnected -= new EventHandler(this.theDevice_PhoneConnected);
			this.theDevice.PhoneDisconnected -= new EventHandler(this.theDevice_PhoneDisconnected);
		}

		/// <summary>
		/// Enables notifications of new received short messages.
		/// </summary>
		/// <remarks>
		/// <para>When a new message is received that is either a standard SMS message or a status report,
		/// the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageReceived" /> event is fired. The <see cref="T:GsmComm.GsmCommunication.IMessageIndicationObject" />
		/// in this event must be cast to a <see cref="T:GsmComm.GsmCommunication.MemoryLocation" /> object, which contains the memory
		/// location of the message that was saved in the phone. You can then use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadMessage(System.Int32,System.String)" />
		/// method (for example) to read the new message.</para>
		/// <para>The supported notification settings vary between different phone models. Therefore the phone is
		/// queried first and then the supported settings of the phone are compared to the settings needed for
		/// the notification functionality to work. If a specific setting is not supported and there is no
		/// alternative setting possible, an exception will be raised.</para>
		/// <para>To disable message notifications, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DisableMessageNotifications" /> function.
		/// </para>
		/// <para>EnableMessageNotifications can't be used with <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageRouting" /> at the same time.
		/// Disable one functionality before using the other.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DisableMessageNotifications" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageRouting" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadMessage(System.Int32,System.String)" />
		/// </remarks>
		public void EnableMessageNotifications()
		{
            
			MessageIndicationSupport supportedIndications = this.theDevice.GetSupportedIndications();
            
            MessageIndicationMode messageIndicationMode = MessageIndicationMode.BufferAndFlush;
            
            if (!supportedIndications.SupportsMode(messageIndicationMode))
			{
                
				if (supportedIndications.SupportsMode(MessageIndicationMode.SkipWhenReserved))
				{
                    
					messageIndicationMode = MessageIndicationMode.SkipWhenReserved;
				}
				else
				{
					if (supportedIndications.SupportsMode(MessageIndicationMode.ForwardAlways))
					{
                        
						messageIndicationMode = MessageIndicationMode.ForwardAlways;
					}
					else
					{
						throw new CommException("The phone does not support any of the required message indication modes.");
					}
				}
			}
            
			SmsDeliverIndicationStyle smsDeliverIndicationStyle = SmsDeliverIndicationStyle.RouteMemoryLocation;
            
            if (supportedIndications.SupportsDeliverStyle(smsDeliverIndicationStyle))
			{
				CbmIndicationStyle cbmIndicationStyle = CbmIndicationStyle.Disabled;
				SmsStatusReportIndicationStyle smsStatusReportIndicationStyle = SmsStatusReportIndicationStyle.RouteMemoryLocation;
				if (!supportedIndications.SupportsStatusReportStyle(smsStatusReportIndicationStyle))
				{
					this.LogIt(LogLevel.Warning, "Attention: The phone does not support notification about new status reports. As a fallback it will be disabled.");
					smsStatusReportIndicationStyle = SmsStatusReportIndicationStyle.Disabled;
				}
				IndicationBufferSetting indicationBufferSetting = IndicationBufferSetting.Flush;
                
				if (!supportedIndications.SupportsBufferSetting(indicationBufferSetting))
				{
					if (supportedIndications.SupportsBufferSetting(IndicationBufferSetting.Clear))
					{
						indicationBufferSetting = IndicationBufferSetting.Clear;
					}
					else
					{
						throw new CommException("The phone does not support any of the required buffer settings.");
					}
				}
                
				MessageIndicationSettings messageIndicationSetting = new MessageIndicationSettings(messageIndicationMode, smsDeliverIndicationStyle, cbmIndicationStyle, smsStatusReportIndicationStyle, indicationBufferSetting);
				this.theDevice.SetMessageIndications(messageIndicationSetting);
				return;
			}
			else
			{
				throw new CommException("The phone does not support notification for standard SMS (SMS-DELIVER) messages. ");
			}
		}

		/// <summary>
		/// Enables direct routing of new received short messages to the application.
		/// </summary>
		/// <remarks>
		/// <para>When a new message is received that is either a standard SMS message or a status report,
		/// the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageReceived" /> event is fired. The <see cref="T:GsmComm.GsmCommunication.IMessageIndicationObject" />
		/// in this event must be cast to a <see cref="T:GsmComm.GsmCommunication.ShortMessage" /> object which can then be decoded using
		/// <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DecodeReceivedMessage(GsmComm.GsmCommunication.ShortMessage)" />.</para>
		/// <para><b>CAUTION:</b> Because the messages are forwared directly, they are <b>not</b> saved in the phone.
		/// If for some reason the message must be saved it must explicitly be done afterwards. Either by using
		/// the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessage(GsmComm.GsmCommunication.ShortMessage,System.String,System.Int32)" /> or <see cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessageWithoutStatus(GsmComm.GsmCommunication.ShortMessage,System.String)" /> functions
		/// to write the message back to the phone or by storing the message somewhere else for later use.</para>
		/// <para>It may be necessary to acknlowledge new routed messages to the phone, either because this
		/// is desired for reliable message transfer or because it is preconfigured in the phone. Use 
		/// <see cref="M:GsmComm.GsmCommunication.GsmCommMain.IsAcknowledgeRequired" /> to find out if acknowledgements must be done. To do the actual
		/// acknowledge, use <see cref="M:GsmComm.GsmCommunication.GsmCommMain.AcknowledgeNewMessage" />.</para>
		/// <para>The supported routing settings vary between different phone models. Therefore the phone is
		/// queried first and then the supported settings of the phone are compared to the settings needed for
		/// the routing functionality to work. If a specific setting is not supported and there is no
		/// alternative setting possible, an exception will be raised.</para>
		/// <para>To disable message routing, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DisableMessageRouting" /> function.</para>
		/// <para>EnableMessageRouting can't be used with <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageNotifications" /> at the same time.
		/// Disable one functionality before using the other.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DisableMessageRouting" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableMessageNotifications" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DecodeReceivedMessage(GsmComm.GsmCommunication.ShortMessage)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessage(GsmComm.GsmCommunication.ShortMessage,System.String,System.Int32)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessageWithoutStatus(GsmComm.GsmCommunication.ShortMessage,System.String)" />
		/// </remarks>
		public void EnableMessageRouting()
		{
			MessageIndicationSupport supportedIndications = this.theDevice.GetSupportedIndications();
			MessageIndicationMode messageIndicationMode = MessageIndicationMode.BufferAndFlush;
			if (!supportedIndications.SupportsMode(messageIndicationMode))
			{
				if (supportedIndications.SupportsMode(MessageIndicationMode.SkipWhenReserved))
				{
					messageIndicationMode = MessageIndicationMode.SkipWhenReserved;
				}
				else
				{
					if (supportedIndications.SupportsMode(MessageIndicationMode.ForwardAlways))
					{
						messageIndicationMode = MessageIndicationMode.ForwardAlways;
					}
					else
					{
						throw new CommException("The phone does not support any of the required message indication modes.");
					}
				}
			}
			SmsDeliverIndicationStyle smsDeliverIndicationStyle = SmsDeliverIndicationStyle.RouteMessage;
			if (supportedIndications.SupportsDeliverStyle(smsDeliverIndicationStyle))
			{
				CbmIndicationStyle cbmIndicationStyle = CbmIndicationStyle.Disabled;
				SmsStatusReportIndicationStyle smsStatusReportIndicationStyle = SmsStatusReportIndicationStyle.RouteMessage;
				if (!supportedIndications.SupportsStatusReportStyle(smsStatusReportIndicationStyle))
				{
					this.LogIt(LogLevel.Warning, "Attention: The phone does not support routing of new status reports. As a fallback it will be disabled.");
					smsStatusReportIndicationStyle = SmsStatusReportIndicationStyle.Disabled;
				}
				IndicationBufferSetting indicationBufferSetting = IndicationBufferSetting.Flush;
				if (!supportedIndications.SupportsBufferSetting(indicationBufferSetting))
				{
					if (supportedIndications.SupportsBufferSetting(IndicationBufferSetting.Clear))
					{
						indicationBufferSetting = IndicationBufferSetting.Clear;
					}
					else
					{
						throw new CommException("The phone does not support any of the required buffer settings.");
					}
				}
				MessageIndicationSettings messageIndicationSetting = new MessageIndicationSettings(messageIndicationMode, smsDeliverIndicationStyle, cbmIndicationStyle, smsStatusReportIndicationStyle, indicationBufferSetting);
				this.theDevice.SetMessageIndications(messageIndicationSetting);
				return;
			}
			else
			{
				throw new CommException("The phone does not support routing of standard SMS (SMS-DELIVER) messages.");
			}
		}

		/// <summary>
		/// Enables the SMS batch mode permanently.
		/// </summary>
		/// <remarks>
		/// <para>When this feature is enabled (and supported by the network), multiple messages can be sent much
		/// faster, as the SMS link is kept open between the messages.</para>
		/// <para>If there is no message sent for 1-5 seconds (the exact value is up to the phones's implementation)
		/// after the last sent SMS message, the SMS link is closed but the batch mode is kept enabled. You have
		/// to explicitely disable it with <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DisableSmsBatchMode" />.</para>
		/// <para>If you don't want to care about turning the batch mode off when done with sending,
		/// consider using <see cref="M:GsmComm.GsmCommunication.GsmCommMain.EnableTemporarySmsBatchMode" /> instead.</para>
		/// </remarks>
		public void EnablePermanentSmsBatchMode()
		{
			this.theDevice.SetMoreMessagesToSend(MoreMessagesMode.Permanent);
		}

		/// <summary>
		/// Enables the SMS batch mode temporarily.
		/// </summary>
		/// <remarks>
		/// <para>When this feature is enabled (and supported by the network), multiple messages can be sent much
		/// faster, as the SMS link is kept open between the messages.</para>
		/// <para>If there is no message sent for 1-5 seconds (the exact value is up to the phones's implementation)
		/// after the last sent SMS message, the SMS link is closed and the batch mode is disabled
		/// automatically. You have to re-enable it before you send the next batch of messages, but you
		/// also don't have to care about turning it off when finished with sending.</para>
		/// </remarks>
		public void EnableTemporarySmsBatchMode()
		{
			this.theDevice.SetMoreMessagesToSend(MoreMessagesMode.Temporary);
		}

		/// <summary>
		/// Enters a password at the phone which is necessary before it can operated.
		/// </summary>
		/// <param name="pin">The SIM PIN, SIM PUK or other password required.</param>
		/// <remarks>Get the current PIN status with <see cref="M:GsmComm.GsmCommunication.GsmCommMain.GetPinStatus" /> to check
		/// whether a password must be entered.</remarks>
		public void EnterPin(string pin)
		{
			this.theDevice.EnterPin(pin);
		}

		/// <summary>
		/// Finds phonebook entries.
		/// </summary>
		/// <param name="findtext">The text in the entry to find.</param>
		/// <param name="storage">The storage to search.</param>
		/// <returns>An array of phonebook entries matching the specified criteria.</returns>
		/// <remarks>The device executes the actual search. If you need the storage information with the results,
		/// use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.FindPhonebookEntriesWithStorage(System.String,System.String)" /> function instead.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.FindPhonebookEntriesWithStorage(System.String,System.String)" />
		/// </remarks>
		public PhonebookEntry[] FindPhonebookEntries(string findtext, string storage)
		{
			new ArrayList();
			this.theDevice.SelectPhonebookStorage(storage);
			return this.theDevice.FindPhonebookEntries(findtext);
		}

		/// <summary>
		/// Finds phonebook entries and saves the storage where they came from.
		/// </summary>
		/// <param name="findtext">The text in the entry to find.</param>
		/// <param name="storage">The storage to search.</param>
		/// <returns>An array of phonebook entries matching the specified criteria.</returns>
		/// <remarks>The device executes the actual search. If you don't need the storage information with the
		/// results, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.FindPhonebookEntries(System.String,System.String)" /> function instead.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.FindPhonebookEntries(System.String,System.String)" />
		/// </remarks>
		public PhonebookEntryWithStorage[] FindPhonebookEntriesWithStorage(string findtext, string storage)
		{
			ArrayList arrayLists = new ArrayList();
			this.theDevice.SelectPhonebookStorage(storage);
			PhonebookEntry[] phonebookEntryArray = this.theDevice.FindPhonebookEntries(findtext);
			for (int i = 0; i < (int)phonebookEntryArray.Length; i++)
			{
				PhonebookEntry phonebookEntry = phonebookEntryArray[i];
				arrayLists.Add(new PhonebookEntryWithStorage(phonebookEntry, storage));
			}
			PhonebookEntryWithStorage[] phonebookEntryWithStorageArray = new PhonebookEntryWithStorage[arrayLists.Count];
			arrayLists.CopyTo(phonebookEntryWithStorageArray, 0);
			return phonebookEntryWithStorageArray;
		}

		/// <summary>
		/// Gets the phone's battery charging status.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.BatteryChargeInfo" /> object containing the battery details.</returns>
		public BatteryChargeInfo GetBatteryCharge()
		{
			return this.theDevice.GetBatteryCharge();
		}

		/// <summary>
		/// Gets the currenty selected text mode character set.
		/// </summary>
		/// <returns>A string containing the name of the currently selected character set.</returns>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.SelectCharacterSet(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetSupportedCharacterSets" />
		/// </remarks>
		public string GetCurrentCharacterSet()
		{
			return this.theDevice.GetCurrentCharacterSet();
		}

		/// <summary>
		/// AT+COPS. Gets the currently selected network operator.
		/// </summary>
		/// <returns>An <see cref="T:GsmComm.GsmCommunication.OperatorInfo" /> object containing the data or null if there is no current operator.</returns>
		public OperatorInfo GetCurrentOperator()
		{
			return this.theDevice.GetCurrentOperator();
		}

		/// <summary>
		/// Gets the memory status of the specified message storage.
		/// </summary>
		/// <param name="storage">The storage to return the status for</param>
		/// <returns>An object containing the memory status of the specified storage.</returns>
		public MemoryStatus GetMessageMemoryStatus(string storage)
		{
			return this.theDevice.SelectReadStorage(storage);
		}

		/// <summary>
		/// Gets the device's supported message storages.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MessageStorageInfo" /> object that contains details about the supported storages.</returns>
		public MessageStorageInfo GetMessageStorages()
		{
			return this.theDevice.GetMessageStorages();
		}

		/// <summary>
		/// Determines the current mode to select a network operator.
		/// </summary>
		/// <returns>The current mode.</returns>
		public OperatorSelectionMode GetOperatorSelectionMode()
		{
			int operatorSelectionMode = this.theDevice.GetOperatorSelectionMode();
			if (Enum.IsDefined(typeof(OperatorSelectionMode), operatorSelectionMode))
			{
				OperatorSelectionMode operatorSelectionMode1 = (OperatorSelectionMode)Enum.Parse(typeof(OperatorSelectionMode), operatorSelectionMode.ToString());
				return operatorSelectionMode1;
			}
			else
			{
				throw new CommException(string.Concat("Unknown operator selection mode ", operatorSelectionMode));
			}
		}

		/// <summary>
		/// Gets the entire phonebook of the selected storage.
		/// </summary>
		/// <param name="storage">The storage to read the data from.</param>
		/// <returns>An array of phonebook entries. If you need the storage information with the
		/// results, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.GetPhonebookWithStorage(System.String)" /> function instead.</returns>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetPhonebookWithStorage(System.String)" />
		/// </remarks>
		public PhonebookEntry[] GetPhonebook(string storage)
		{
			new ArrayList();
			this.theDevice.SelectPhonebookStorage(storage);
			return this.theDevice.ReadPhonebookEntries();
		}

		/// <summary>
		/// Gets the memory status of the specified phonebook storage.
		/// </summary>
		/// <param name="storage">The storage to return the status for</param>
		/// <returns>An object containing the memory status of the specified storage.</returns>
		public MemoryStatusWithStorage GetPhonebookMemoryStatus(string storage)
		{
			this.theDevice.SelectPhonebookStorage(storage);
			return this.theDevice.GetPhonebookMemoryStatus();
		}

		/// <summary>
		/// Gets the device's supported phonebook storages.
		/// </summary>
		/// <returns>An array of supported storages in coded form, usually "SM" for SIM, "ME" for
		/// phone, etc.</returns>
		public string[] GetPhonebookStorages()
		{
			return this.theDevice.GetPhonebookStorages();
		}

		/// <summary>
		/// Gets the entire phonebook of the selected storage and saves the storage where the entries came from.
		/// </summary>
		/// <param name="storage">The storage to read the data from.</param>
		/// <returns>An array of phonebook entries. If you don't need the storage information with the
		/// results, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.GetPhonebook(System.String)" /> function instead.</returns>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetPhonebook(System.String)" />
		/// </remarks>
		public PhonebookEntryWithStorage[] GetPhonebookWithStorage(string storage)
		{
			ArrayList arrayLists = new ArrayList();
			this.theDevice.SelectPhonebookStorage(storage);
			PhonebookEntry[] phonebookEntryArray = this.theDevice.ReadPhonebookEntries();
			for (int i = 0; i < (int)phonebookEntryArray.Length; i++)
			{
				PhonebookEntry phonebookEntry = phonebookEntryArray[i];
				arrayLists.Add(new PhonebookEntryWithStorage(phonebookEntry, storage));
			}
			PhonebookEntryWithStorage[] phonebookEntryWithStorageArray = new PhonebookEntryWithStorage[arrayLists.Count];
			arrayLists.CopyTo(phonebookEntryWithStorageArray, 0);
			return phonebookEntryWithStorageArray;
		}

		/// <summary>
		/// Returns a value indicating whether some password must be entered at the phone or not.
		/// </summary>
		/// <returns>The current PIN status as one of the <see cref="T:GsmComm.GsmCommunication.PinStatus" /> values.</returns>
		public PinStatus GetPinStatus()
		{
			return this.theDevice.GetPinStatus();
		}

		/// <summary>
		/// Enables access to the protocol level of the current connection.
		/// </summary>
		/// <returns>An <see cref="T:GsmComm.GsmCommunication.IProtocol" /> object that sends and receives data at the protocol level.</returns>
		/// <remarks>This method enables execution of custom commands that are not directly supported. It also disables execution of background
		/// operations that would usually take place, such as checking whether the phone is still connected.
		/// <para>The <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReleaseProtocol" /> method must be called as soon as execution of the custom commands is completed,
		/// and allows for normal operations to continue. Execution of other commands besides from <see cref="T:GsmComm.GsmCommunication.IProtocol" /> is not allowed
		/// until <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReleaseProtocol" /> is called.</para>
		/// </remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.ReleaseProtocol" />
		public IProtocol GetProtocol()
		{
			return this.theDevice.GetProtocol();
		}

		/// <summary>
		/// Gets the signal quality as calculated by the phone.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.SignalQualityInfo" /> object containing the signal details.</returns>
		public SignalQualityInfo GetSignalQuality()
		{
			return this.theDevice.GetSignalQuality();
		}

		/// <summary>
		/// Gets the current SMS batch mode setting.
		/// </summary>
		/// <returns>The current mode.</returns>
		public MoreMessagesMode GetSmsBatchModeSetting()
		{
			return this.theDevice.GetMoreMessagesToSend();
		}

		/// <summary>
		/// Gets the SMS Service Center Address.
		/// </summary>
		/// <returns>The current SMSC address</returns>
		/// <remarks>This command returns the SMSC address, through which SMS messages are transmitted.
		/// In text mode, this setting is used by SMS sending and SMS writing commands. In PDU mode, this setting is
		/// used by the same commands, but only when the length of the SMSC address coded into the PDU data equals
		/// zero.</remarks>
		public AddressData GetSmscAddress()
		{
			return this.theDevice.GetSmscAddress();
		}

		/// <summary>
		/// Returns the MSISDNs related to the subscriber.
		/// </summary>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.SubscriberInfo" /> objects with one for each MSISDN
		/// (Mobile Subscriber ISDN Number), depending on the services subscribed.</returns>
		/// <remarks>
		/// <para>This information can be stored in the SIM/UICC or in the MT.</para>
		/// <para>If the command is supported by the phone but no number can be retrieved,
		/// an empty array is returned.</para>
		/// </remarks>
		public SubscriberInfo[] GetSubscriberNumbers()
		{
			return this.theDevice.GetSubscriberNumbers();
		}

		/// <summary>
		/// Gets the phone's supported text mode character sets.
		/// </summary>
		/// <returns>A string array containing the names of the phone's supportet text mode character sets.</returns>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.SelectCharacterSet(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetCurrentCharacterSet" />
		/// </remarks>
		public string[] GetSupportedCharacterSets()
		{
			return this.theDevice.GetSupportedCharacterSets();
		}

		/// <summary>
		/// Gathers information that identifiy the connected device.
		/// </summary>
		/// <returns>An <see cref="T:GsmComm.GsmCommunication.IdentificationInfo" /> object containing data about the device.</returns>
		public IdentificationInfo IdentifyDevice()
		{
			this.LogIt(LogLevel.Info, "Identifying device...");
			IdentificationInfo identificationInfo = new IdentificationInfo();
			identificationInfo.Manufacturer = this.theDevice.RequestManufacturer();
			identificationInfo.Model = this.theDevice.RequestModel();
			identificationInfo.Revision = this.theDevice.RequestRevision();
			identificationInfo.SerialNumber = this.theDevice.RequestSerialNumber();
			this.LogIt(LogLevel.Info, string.Concat("Manufacturer: ", identificationInfo.Manufacturer));
			this.LogIt(LogLevel.Info, string.Concat("Model: ", identificationInfo.Model));
			this.LogIt(LogLevel.Info, string.Concat("Revision: ", identificationInfo.Revision));
			this.LogIt(LogLevel.Info, string.Concat("Serial number: ", identificationInfo.SerialNumber));
			return identificationInfo;
		}

		/// <summary>
		/// Checks if it is required to acknowledge new directly routed incoming messages.
		/// </summary>
		/// <returns>true if directly routed incoming messages need to be acknowledged, false if not.</returns>
		public bool IsAcknowledgeRequired()
		{
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			int num3 = 0;
			this.theDevice.GetCurrentMessageService(out num, out num1, out num2, out num3);
			return num == 1;
		}

		/// <summary>
		/// Determines if there is actually a device connected that responds to commands.
		/// </summary>
		/// <returns>true if there is a device connected and responsive, otherwise false.</returns>
		/// <remarks>
		/// You can use this function after opening the port with <see cref="M:GsmComm.GsmCommunication.GsmCommMain.Open" /> to verify that there is really a device connected
		/// before processding.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.Open" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.IsOpen" />
		/// </remarks>
		public bool IsConnected()
		{
			return this.theDevice.IsConnected();
		}

		/// <summary>
		/// Determines if the port is currently open.
		/// </summary>
		/// <returns>true if the port is open, otherwise false.</returns>
		/// <remarks><para>The port is open after a auccessful call to <see cref="M:GsmComm.GsmCommunication.GsmCommMain.Open" /> and must be closed with
		/// <see cref="M:GsmComm.GsmCommunication.GsmCommMain.Close" />.</para>
		/// <para>This function does not check if there is actually a device connected, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.IsConnected" />
		/// function for that.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.Open" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.Close" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.IsConnected" />
		/// </remarks>
		public bool IsOpen()
		{
			return this.theDevice.IsOpen();
		}

		/// <summary>
		/// Lists the network operators detected by the phone.
		/// </summary>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.OperatorInfo2" /> objects containing the data of each operator.</returns>
		/// <remarks>If you want to determine the current operator, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.GetCurrentOperator" /> method.</remarks>
		public OperatorInfo2[] ListOperators()
		{
			return this.theDevice.ListOperators();
		}

		private void LogIt(LogLevel level, string text)
		{
			if (this.LoglineAdded != null && level <= this.logLevel)
			{
				DateTime now = DateTime.Now;
				text = string.Concat(now.ToString("HH:mm:ss.fff"), " ", text);
				this.LoglineAdded(this, new LoglineAddedEventArgs(level, text));
			}
		}

		private void LogVersionInfo()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Version version = executingAssembly.GetName().Version;
			string str = version.ToString(2);
			string str1 = version.ToString();
			string imageRuntimeVersion = executingAssembly.ImageRuntimeVersion;
			string str2 = string.Format("GSMComm {0} (Build {1} for .NET {2})", str, str1, imageRuntimeVersion);
			this.LogIt(LogLevel.Info, str2);
		}

		private void OnMessageSendComplete(OutgoingSmsPdu pdu)
		{
			if (this.MessageSendComplete != null)
			{
				this.LogIt(LogLevel.Info, "Firing async MessageSendComplete event.");
				MessageEventArgs messageEventArg = new MessageEventArgs(pdu);
				this.MessageSendComplete.BeginInvoke(this, messageEventArg, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		private void OnMessageSendFailed(OutgoingSmsPdu pdu, Exception exception)
		{
			if (this.MessageSendFailed != null)
			{
				this.LogIt(LogLevel.Info, "Firing async MessageSendFailed event.");
				MessageErrorEventArgs messageErrorEventArg = new MessageErrorEventArgs(pdu, exception);
				this.MessageSendFailed.BeginInvoke(this, messageErrorEventArg, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		private void OnMessageSendStarting(OutgoingSmsPdu pdu)
		{
			if (this.MessageSendStarting != null)
			{
				this.LogIt(LogLevel.Info, "Firing async MessageSendStarting event.");
				MessageEventArgs messageEventArg = new MessageEventArgs(pdu);
				this.MessageSendStarting.BeginInvoke(this, messageEventArg, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		/// <summary>
		/// Opens the connection to the device.
		/// </summary>
		/// <remarks>You can check the current connection state with the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.IsOpen" /> method.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.IsOpen" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.Close" />
		/// </remarks>
		public void Open()
		{
			if (!GsmCommMain.versionInfoLogged)
			{
				this.LogVersionInfo();
				GsmCommMain.versionInfoLogged = true;
			}
			this.ConnectEvents();
			this.theDevice.Open();
		}

		/// <summary>
		/// Reads a single short message.
		/// </summary>
		/// <param name="index">The index of the message to read.</param>
		/// <param name="storage">The storage to look in for the message.</param>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.DecodedShortMessage" /> object containing the message at the index specified.</returns>
		public DecodedShortMessage ReadMessage(int index, string storage)
		{
			this.LogIt(LogLevel.Info, "Reading message...");
			this.theDevice.SelectReadStorage(storage);
			ShortMessageFromPhone shortMessageFromPhone = this.theDevice.ReadMessage(index);
			if (!Enum.IsDefined(typeof(PhoneMessageStatus), shortMessageFromPhone.Status))
			{
				int status = shortMessageFromPhone.Status;
				throw new CommException(string.Concat("Unknown message status \"", status.ToString(), "\"!"));
			}
			else
			{
				int num = shortMessageFromPhone.Status;
				PhoneMessageStatus phoneMessageStatu = (PhoneMessageStatus)Enum.Parse(typeof(PhoneMessageStatus), num.ToString());
				DecodedShortMessage decodedShortMessage = new DecodedShortMessage(shortMessageFromPhone.Index, this.DecodeShortMessage(shortMessageFromPhone), phoneMessageStatu, storage);
				return decodedShortMessage;
			}
		}

		/// <summary>
		/// Reads and decodes short messages from phone.
		/// </summary>
		/// <param name="status">The status of the messages to read.</param>
		/// <param name="storage">The storage to look in for the messages.</param>
		/// <returns>An array of decoded messages.</returns>
		/// <remarks>As the decoded version of the message is not always guaranteed to
		/// be exactly the same when encoded back, do not use this function if you want
		/// to save the message in their original form. Use <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadRawMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" /> instead
		/// for that case.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadRawMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" />
		/// </remarks>
		public DecodedShortMessage[] ReadMessages(PhoneMessageStatus status, string storage)
		{
			this.LogIt(LogLevel.Info, "Reading messages...");
			ArrayList arrayLists = new ArrayList();
			this.theDevice.SelectReadStorage(storage);
			ShortMessageFromPhone[] shortMessageFromPhoneArray = this.theDevice.ListMessages(status);
			for (int i = 0; i < (int)shortMessageFromPhoneArray.Length; i++)
			{
				ShortMessageFromPhone shortMessageFromPhone = shortMessageFromPhoneArray[i];
				try
				{
					if (!Enum.IsDefined(typeof(PhoneMessageStatus), shortMessageFromPhone.Status))
					{
						int num = shortMessageFromPhone.Status;
						throw new CommException(string.Concat("Unknown message status \"", num.ToString(), "\"!"));
					}
					else
					{
						int num1 = shortMessageFromPhone.Status;
						PhoneMessageStatus phoneMessageStatu = (PhoneMessageStatus)Enum.Parse(typeof(PhoneMessageStatus), num1.ToString());
						arrayLists.Add(new DecodedShortMessage(shortMessageFromPhone.Index, this.DecodeShortMessage(shortMessageFromPhone), phoneMessageStatu, storage));
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.LogIt(LogLevel.Error, string.Concat("Error while decoding a message: ", exception.Message, " The message was ignored."));
				}
			}
			DecodedShortMessage[] decodedShortMessageArray = new DecodedShortMessage[arrayLists.Count];
			arrayLists.CopyTo(decodedShortMessageArray);
			return decodedShortMessageArray;
		}

		/// <summary>
		/// Reads short messages in their original form from phone.
		/// </summary>
		/// <param name="status">The status of the messages to read.</param>
		/// <param name="storage">The storage to look in for the messages.</param>
		/// <returns>An array of undecoded short messages, as read from the phone.</returns>
		/// <remarks><para>This function is intended to download the messages exactly as they are
		/// returned from the phone, e.g. for a backup. If you want to use the messages
		/// directly, e.g. displaying them to the user without saving them, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" />
		/// function instead.</para>
		/// <para>If you want to decode the saved messages later, you can use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.DecodeShortMessage(GsmComm.GsmCommunication.ShortMessageFromPhone)" />
		/// function.</para>
		/// <para>You can import the saved message back to the phone using the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessage(GsmComm.GsmCommunication.ShortMessageFromPhone,System.String)" />
		/// function.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.DecodeShortMessage(GsmComm.GsmCommunication.ShortMessageFromPhone)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessageWithoutStatus(GsmComm.GsmCommunication.ShortMessage,System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessage(GsmComm.GsmCommunication.ShortMessageFromPhone,System.String)" />
		/// </remarks>
		public ShortMessageFromPhone[] ReadRawMessages(PhoneMessageStatus status, string storage)
		{
			this.LogIt(LogLevel.Info, "Reading messages...");
			new ArrayList();
			this.theDevice.SelectReadStorage(storage);
			return this.theDevice.ListMessages(status);
		}

		/// <summary>
		/// Disables access to the protocol level of the current connection.
		/// </summary>
		/// <remarks>This method must be called as soon as the execution of the custom commands initiated
		/// by <see cref="M:GsmComm.GsmCommunication.GsmCommMain.GetProtocol" /> is completed and allows for normal operations to continue.</remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetProtocol" />
		public void ReleaseProtocol()
		{
			this.theDevice.ReleaseProtocol();
		}

		/// <summary>
		/// Enables or disables the requirement to acknowledge new received short messages
		/// that are directly routed to the application.
		/// </summary>
		/// <param name="require">Set to true to require acknowledgements, set to false
		/// to turn off the requirement.</param>
		/// <remarks><para>It depends on the phone when this setting can actually be changed.
		/// Because of this, it is recommended to execute <see cref="M:GsmComm.GsmCommunication.GsmCommMain.IsAcknowledgeRequired" />
		/// after a call to RequireAcknowledge to verify the new setting.</para>
		/// </remarks>
		public void RequireAcknowledge(bool require)
		{
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			int num3;
			if (require)
			{
				num3 = 1;
			}
			else
			{
				num3 = 0;
			}
			int num4 = num3;
			this.theDevice.SelectMessageService(num4, out num, out num1, out num2);
		}

		/// <summary>
		/// Resets all settings that are not stored in a profile to their factory defaults.
		/// </summary>
		/// <remarks>This function is useful if you don't know the state your phone is and
		/// want to set it up from scratch.</remarks>
		public void ResetToDefaultConfig()
		{
			this.theDevice.ResetToDefaultConfig();
		}

		/// <summary>Selects the text mode character set.</summary>
		/// <param name="charset">The character set to use.</param>
		/// <remarks>
		/// <para>The <see cref="T:GsmComm.GsmCommunication.Charset" /> class contains some common character sets.</para>
		/// <para>To get a list of the character sets that the phone supports, use <see cref="M:GsmComm.GsmCommunication.GsmCommMain.GetSupportedCharacterSets" />.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetCurrentCharacterSet" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.GetSupportedCharacterSets" />
		/// </remarks>
		public void SelectCharacterSet(string charset)
		{
			this.theDevice.SelectCharacterSet(charset);
		}

		/// <summary>
		/// Sends a short message.
		/// </summary>
		/// <param name="pdu">The object containing the message to send.</param>
		/// <param name="throwExceptions">Indicates whether an exception should be
		/// thrown upon an error.</param>
		/// <remarks>
		/// <para>This method sends the short message contained in the PDU object.
		/// The message reference returned by the phone is stored in the MessageReference
		/// property of the PDU object. If there is an error, an exception will be thrown.</para>
		/// <para>Additionally, this function also fires the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendFailed" /> event
		/// upon an error and the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendComplete" /> event upon success.
		/// Set the <b>throwExceptions</b> parameter to false if your message handling
		/// uses only the events fired by this method.</para>
		/// <para>To send multiple messages in succession, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.SendMessages(GsmComm.PduConverter.OutgoingSmsPdu[])" /> function.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.SendMessages(GsmComm.PduConverter.OutgoingSmsPdu[])" />
		/// </remarks>
		public void SendMessage(OutgoingSmsPdu pdu, bool throwExceptions)
		{
			this.LogIt(LogLevel.Info, "Sending message...");
			this.OnMessageSendStarting(pdu);
			byte num = 0;
			try
			{
				num = this.theDevice.SendMessage(pdu.ToString(), pdu.ActualLength);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.LogIt(LogLevel.Error, string.Concat("Error while sending the message: ", exception.Message));
				this.OnMessageSendFailed(pdu, exception);
				if (!throwExceptions)
				{
					return;
				}
				else
				{
					throw;
				}
			}
			if (pdu.MessageReference == 0)
			{
				pdu.MessageReference = num;
			}
			this.LogIt(LogLevel.Info, "Message sent successfully.");
			this.OnMessageSendComplete(pdu);
		}

		/// <summary>
		/// Sends a short message.
		/// </summary>
		/// <param name="pdu">The object containing the message to send.</param>
		/// <remarks>
		/// <para>This method sends the short message contained in the PDU object.
		/// The message reference returned by the phone is stored in the MessageReference
		/// property of the PDU object. If there is an error, an exception will be thrown.</para>
		/// <para>Additionally, this function also fires the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendFailed" /> event
		/// upon an error and the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendComplete" /> event upon success.</para>
		/// <para>To send multiple messages in succession, use the <see cref="M:GsmComm.GsmCommunication.GsmCommMain.SendMessages(GsmComm.PduConverter.OutgoingSmsPdu[])" /> function.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.SendMessages(GsmComm.PduConverter.OutgoingSmsPdu[])" />
		/// </remarks>
		public void SendMessage(OutgoingSmsPdu pdu)
		{
			this.SendMessage(pdu, true);
		}

		/// <summary>
		/// Sends multiple messages in succession. Sending stops at the first error.
		/// </summary>
		/// <param name="pdus">The messages to send.</param>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.SendMessage(GsmComm.PduConverter.OutgoingSmsPdu,System.Boolean)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.SendMessage(GsmComm.PduConverter.OutgoingSmsPdu)" />
		/// </remarks>
		public void SendMessages(OutgoingSmsPdu[] pdus)
		{
			if (pdus != null)
			{
				if ((int)pdus.Length != 0)
				{
					int length = (int)pdus.Length;
					this.LogIt(LogLevel.Info, string.Concat(length.ToString(), " message(s) to send."));
					for (int i = 0; i < (int)pdus.Length; i++)
					{
						OutgoingSmsPdu outgoingSmsPdu = pdus[i];
						string[] str = new string[5];
						str[0] = "Sending message ";
						int num = i + 1;
						str[1] = num.ToString();
						str[2] = " of ";
						int length1 = (int)pdus.Length;
						str[3] = length1.ToString();
						str[4] = "...";
						this.LogIt(LogLevel.Info, string.Concat(str));
						this.SendMessage(outgoingSmsPdu);
					}
					return;
				}
				else
				{
					this.LogIt(LogLevel.Warning, "Nothing to do!");
					return;
				}
			}
			else
			{
				this.LogIt(LogLevel.Error, "Failed. Message array is null.");
				throw new ArgumentNullException("pdus");
			}
		}

		/// <summary>
		/// Sets the new SMS service center address.
		/// </summary>
		/// <param name="data">An <see cref="T:GsmComm.GsmCommunication.AddressData" /> object containing the new address</param>
		/// <remarks>This command changes the SMSC address, through which SMS messages are transmitted.
		/// In text mode, this setting is used by SMS sending and SMS writing commands. In PDU mode, this setting is
		/// used by the same commands, but only when the length of the SMSC address coded into the PDU data equals
		/// zero.</remarks>
		public void SetSmscAddress(AddressData data)
		{
			this.theDevice.SetSmscAddress(data);
		}

		/// <summary>
		/// Sets the new SMS service center address.
		/// </summary>
		/// <param name="address">The new SMSC address</param>
		/// <remarks>This command changes the SMSC address, through which SMS messages are transmitted.
		/// In text mode, this setting is used by SMS sending and SMS writing commands. In PDU mode, this setting is
		/// used by the same commands, but only when the length of the SMSC address coded into the PDU data equals
		/// zero.</remarks>
		public void SetSmscAddress(string address)
		{
			this.theDevice.SetSmscAddress(address);
		}

		private void theDevice_LoglineAdded(object sender, LoglineAddedEventArgs e)
		{
			if (this.LoglineAdded != null)
			{
				this.LoglineAdded(this, e);
			}
		}

		private void theDevice_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			if (this.MessageReceived != null)
			{
				this.MessageReceived(this, e);
			}
		}

		private void theDevice_PhoneConnected(object sender, EventArgs e)
		{
			if (this.PhoneConnected != null)
			{
				this.PhoneConnected(this, e);
			}
		}

		private void theDevice_PhoneDisconnected(object sender, EventArgs e)
		{
			if (this.PhoneDisconnected != null)
			{
				this.PhoneDisconnected(this, e);
			}
		}

		private void theDevice_ReceiveComplete(object sender, ProgressEventArgs e)
		{
			if (this.ReceiveComplete != null)
			{
				this.ReceiveComplete(this, e);
			}
		}

		private void theDevice_ReceiveProgress(object sender, ProgressEventArgs e)
		{
			if (this.ReceiveProgress != null)
			{
				this.ReceiveProgress(this, e);
			}
		}

		/// <summary>
		/// Stores a raw short message in the specified storage.
		/// </summary>
		/// <param name="message">The message to store.</param>
		/// <param name="storage">The storage to store the message in.</param>
		/// <returns>The index of the message. If the index could not be retrieved, zero is returned.</returns>
		/// <remarks>
		/// This function is useful for importing messages that were previously exported with <see cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadRawMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" />.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmCommMain.ReadRawMessages(GsmComm.GsmCommunication.PhoneMessageStatus,System.String)" />
		/// </remarks>
		public int WriteRawMessage(ShortMessageFromPhone message, string storage)
		{
			this.theDevice.SelectWriteStorage(storage);
			return this.theDevice.WriteMessageToMemory(message.Data, message.Length, message.Status);
		}

		/// <summary>
		/// Stores a raw short message in the specified storage.
		/// </summary>
		/// <param name="message">The message to store.</param>
		/// <param name="storage">The storage to store the message in.</param>
		/// <param name="status">The status to set for the message.</param>
		/// <returns>The index of the message. If the index could not be retrieved, zero is returned.</returns>
		public int WriteRawMessage(ShortMessage message, string storage, int status)
		{
			this.theDevice.SelectWriteStorage(storage);
			return this.theDevice.WriteMessageToMemory(message.Data, message.Length, status);
		}

		/// <summary>
		/// Stores a raw short message in the specified storage without setting a specific message status.
		/// </summary>
		/// <param name="message">The message to store.</param>
		/// <param name="storage">The storage to store the message in.</param>
		/// <returns>The index of the message. If the index could not be retrieved, zero is returned.</returns>
		/// <remarks>
		/// <para>The message is stored with a predefined status set in the phone.</para>
		/// <see cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessage(GsmComm.GsmCommunication.ShortMessageFromPhone,System.String)" />
		/// <see cref="M:GsmComm.GsmCommunication.GsmCommMain.WriteRawMessage(GsmComm.GsmCommunication.ShortMessage,System.String,System.Int32)" />
		/// </remarks>
		public int WriteRawMessageWithoutStatus(ShortMessage message, string storage)
		{
			this.theDevice.SelectWriteStorage(storage);
			return this.theDevice.WriteMessageToMemory(message.Data, message.Length);
		}

		/// <summary>
		/// The event that occurs when a new line was added to the log.
		/// </summary>
		public event LoglineAddedEventHandler LoglineAdded;

		/// <summary>
		/// The event that occurs when a new message was received.
		/// </summary>
		public event MessageReceivedEventHandler MessageReceived;

		/// <summary>
		/// The event that occurs after a successful message transfer.
		/// </summary>
		public event GsmCommMain.MessageEventHandler MessageSendComplete;

		/// <summary>
		/// The event that occurs after a failed message transfer.
		/// </summary>
		public event GsmCommMain.MessageErrorEventHandler MessageSendFailed;

		/// <summary>
		/// The event that occurs immediately before transferring a new message.
		/// </summary>
		public event GsmCommMain.MessageEventHandler MessageSendStarting;

		/// <summary>The event that occurs when the phone is connected.</summary>
		public event EventHandler PhoneConnected;

		/// <summary>The event that occurs when the phone is disconnected.</summary>
		public event EventHandler PhoneDisconnected;

		/// <summary>The event that occurs when receiving from the phone is completed.</summary>
		/// <remarks>This event is only fired by reading operations that may take longer to complete.</remarks>
		public event ProgressEventHandler ReceiveComplete;

		/// <summary>The event that occurs when new data was received from the phone.</summary>
		/// <remarks>This event is only fired by reading operations that may take longer to complete.</remarks>
		public event ProgressEventHandler ReceiveProgress;

		/// <summary>
		/// The method that handles the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendFailed" /> event.
		/// </summary>
		/// <param name="sender">The origin of the event.</param>
		/// <param name="e">The <see cref="T:GsmComm.GsmCommunication.MessageErrorEventArgs" /> associated with the event.</param>
		public delegate void MessageErrorEventHandler(object sender, MessageErrorEventArgs e);

		/// <summary>
		/// The method that handles the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendStarting" /> and <see cref="E:GsmComm.GsmCommunication.GsmCommMain.MessageSendComplete" />
		/// events.
		/// </summary>
		/// <param name="sender">The origin of the event.</param>
		/// <param name="e">The <see cref="T:GsmComm.GsmCommunication.MessageEventArgs" /> associated with the event.</param>
		public delegate void MessageEventHandler(object sender, MessageEventArgs e);
	}
}