using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

/// <summary>
/// Interacts with a mobile phone at a low level to execute various functions.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class GsmPhone : IProtocol
	{
		private const int receiveTimeout = 5000;

		private const string commOK = "\r\nOK\r\n";

		private const string commError = "\r\nERROR\r\n";

		private const string messageServiceErrorPattern = "\\r\\n\\+CMS ERROR: (\\d+)\\r\\n";

		private const string mobileEquipmentErrorPattern = "\\r\\n\\+CME ERROR: (\\d+)\\r\\n";

		private SerialPort port;

		private string portName;

		private int baudRate;

		private int timeout;

		private bool logEnabled;

		private LogLevel logLevel;

		private int commThreadCheckDelay;

		private Queue rawQueue;

		private Queue inputQueue;

		private ManualResetEvent terminateCommThread;

		private ManualResetEvent dataReceived;

		private ManualResetEvent noData;

		private Thread commThread;

		private string dataToSend;

		private ManualResetEvent sendNow;

		private ManualResetEvent receiveNow;

		private ManualResetEvent sendDone;

		private bool connectionState;

		private AutoResetEvent checkConnection;

		private ManualResetEvent logThreadInitialized;

		private ManualResetEvent terminateLogThread;

		private Queue logQueue;

		private Thread logThread;

		/// <summary>
		/// Gets the baud rate.
		/// </summary>
		public int BaudRate
		{
			get
			{
				return this.baudRate;
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
				return this.commThreadCheckDelay;
			}
			set
			{
				int num;
				if (value < 1000)
				{
					num = 1000;
				}
				else
				{
					num = value;
				}
				int num1 = num;
				this.commThreadCheckDelay = num1;
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
			}
		}

		/// <summary>
		/// Gets the COM port.
		/// </summary>
		public string PortName
		{
			get
			{
				return this.portName;
			}
		}

		/// <summary>
		/// Gets the communication timeout.
		/// </summary>
		public int Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		/// <summary>
		/// Initializing a new instance of the class.
		/// </summary>
		/// <param name="portName">The communication (COM) port to use.</param>
		/// <param name="baudRate">The baud rate (speed) to use.</param>
		/// <param name="timeout">The communication timeout in milliseconds.</param>
		public GsmPhone(string portName, int baudRate, int timeout)
		{
			this.commThreadCheckDelay = 10000;
			if (portName != null)
			{
				if (portName == null || portName.Length != 0)
				{
					if (baudRate >= 0)
					{
						if (timeout >= 0)
						{
							this.port = null;
							this.portName = portName;
							this.baudRate = baudRate;
							this.timeout = timeout;
							this.logEnabled = true;
							this.logLevel = LogLevel.Verbose;
							this.rawQueue = new Queue();
							this.inputQueue = Queue.Synchronized(this.rawQueue);
							this.terminateCommThread = new ManualResetEvent(false);
							this.dataReceived = new ManualResetEvent(false);
							this.noData = new ManualResetEvent(false);
							this.commThread = null;
							this.dataToSend = string.Empty;
							this.sendNow = new ManualResetEvent(false);
							this.receiveNow = new ManualResetEvent(false);
							this.sendDone = new ManualResetEvent(false);
							this.connectionState = false;
							this.checkConnection = new AutoResetEvent(false);
							this.logThreadInitialized = new ManualResetEvent(false);
							this.terminateLogThread = new ManualResetEvent(false);
							this.logQueue = new Queue();
							this.logThread = null;
							return;
						}
						else
						{
							throw new ArgumentException("timeout must not be negative.", "timeout");
						}
					}
					else
					{
						throw new ArgumentException("baudRate must not be negative.", "baudRate");
					}
				}
				else
				{
					throw new ArgumentException("portName must not be an empty string.");
				}
			}
			else
			{
				throw new ArgumentNullException("portName");
			}
		}

		/// <summary>
		/// Initializing a new instance of the class.
		/// </summary>
		/// <param name="portNumber">The communication (COM) port to use.</param>
		/// <param name="baudRate">The baud rate (speed) to use.</param>
		/// <param name="timeout">The communication timeout in milliseconds.</param>
		public GsmPhone(int portNumber, int baudRate, int timeout) : this(string.Concat("COM", portNumber.ToString()), baudRate, timeout)
		{
		}

		/// <summary>
		/// AT+CNMA. Confirms reception of a new message (SMS-DELIVER or SMS-STATUS-REPORT) which is routed
		/// directly to the TE. This acknowledgement command shall be used when "service" parameter
		/// of the <see cref="M:GsmComm.GsmCommunication.GsmPhone.GetCurrentMessageService(System.Int32@,System.Int32@,System.Int32@,System.Int32@)" /> function equals 1.
		/// </summary>
		/// <remarks>This sends a positive acknowledgement to the network.</remarks>
		public void AcknowledgeNewMessage()
		{
			lock (this)
			{
				this.AcknowledgeNewMessage(true);
			}
		}

		/// <summary>
		/// AT+CNMA. Confirms reception of a new message (SMS-DELIVER or SMS-STATUS-REPORT) which is routed
		/// directly to the TE.  This acknowledgement command shall be used when "service" parameter
		/// of the <see cref="M:GsmComm.GsmCommunication.GsmPhone.GetCurrentMessageService(System.Int32@,System.Int32@,System.Int32@,System.Int32@)" /> function equals 1.
		/// </summary>
		/// <param name="ok">Specifies whether the message was received correctly.
		/// Setting this parameter to true, will send a positive (RP-ACK) acknowledgement to the network.
		/// Setting this parameter to false, will send a negative (RP-ERROR) acknowledgement to the network.
		/// </param>
		/// <remarks>
		/// <para>If ME does not get acknowledgement within required time (network timeout), ME should send RP-ERROR to
		/// the network. ME/TA shall automatically disable routing to TE.</para>
		/// <para>If command is executed, but no acknowledgement is expected, or some other ME related error occurs,
		/// a <see cref="T:GsmComm.GsmCommunication.MessageServiceErrorException" /> is raised.</para>
		/// </remarks>
		public void AcknowledgeNewMessage(bool ok)
		{
			byte num;
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				string str = "AT+CNMA=";
				if (ok)
				{
					num = 0;
				}
				else
				{
					num = 2;
				}
				string str1 = string.Concat(str, num);
				this.ExecAndReceiveMultiple(str1);
			}
		}

		/// <summary>
		/// AT+CMGF. Activates the PDU mode. Device must support it or the call will fail.
		/// </summary>
		private void ActivatePduMode()
		{
			this.LogIt(LogLevel.Info, "Activating PDU mode...");
			this.ExecAndReceiveMultiple("AT+CMGF=0");
		}

		/// <summary>
		/// AT+CMGF. Activates the text mode. Device must support it or the call will fail.
		/// </summary>
		private void ActivateTextMode()
		{
			this.LogIt(LogLevel.Info, "Activating text mode...");
			this.ExecAndReceiveMultiple("AT+CMGF=1");
		}

		private void AsyncCallback(IAsyncResult ar)
		{
			AsyncResult asyncResult = (AsyncResult)ar;
			if (asyncResult.AsyncDelegate as MessageReceivedEventHandler == null)
			{
				if (asyncResult.AsyncDelegate as EventHandler == null)
				{
					if (asyncResult.AsyncDelegate as ProgressEventHandler == null)
					{
						this.LogIt(LogLevel.Warning, string.Concat("AsyncCallback got unknown delegate: ", asyncResult.AsyncDelegate.GetType().ToString()));
						return;
					}
					else
					{
						ProgressEventHandler asyncDelegate = (ProgressEventHandler)asyncResult.AsyncDelegate;
						asyncDelegate.EndInvoke(ar);
						return;
					}
				}
				else
				{
					this.LogIt(LogLevel.Info, "Ending async EventHandler call");
					EventHandler eventHandler = (EventHandler)asyncResult.AsyncDelegate;
					eventHandler.EndInvoke(ar);
					return;
				}
			}
			else
			{
				this.LogIt(LogLevel.Info, "Ending async MessageReceivedEventHandler call");
				MessageReceivedEventHandler messageReceivedEventHandler = (MessageReceivedEventHandler)asyncResult.AsyncDelegate;
				messageReceivedEventHandler.EndInvoke(ar);
				return;
			}
		}

		private void CheckConnection()
		{
			if (Monitor.TryEnter(this))
			{
				try
				{
					bool flag = this.IsConnectedInternal();
					this.SetNewConnectionState(flag);
				}
				finally
				{
					Monitor.Exit(this);
				}
				return;
			}
			else
			{
				this.LogIt(LogLevel.Verbose, "Object locked - connection check not performed.");
				return;
			}
		}

		/// <summary>
		/// Closes the connection to the device.
		/// </summary>
		/// <remarks>You can check the current connection state with the <see cref="M:GsmComm.GsmCommunication.GsmPhone.IsOpen" /> method.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.IsOpen" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.Open" />
		/// </remarks>
		/// <exception cref="T:System.InvalidOperationException">Port not open.</exception>
		public void Close()
		{
			lock (this)
			{
				if (this.IsOpen())
				{
					this.TerminateCommThread();
					try
					{
						this.ClosePort();
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.LogIt(LogLevel.Warning, "Closing the port failed. The exeption will not be rethrown to allow proper shutdown.");
						this.LogIt(LogLevel.Warning, "Error details:");
						this.LogItShow(LogLevel.Warning, exception.ToString(), "");
					}
					this.TerminateLogThread();
				}
				else
				{
					throw new InvalidOperationException("Port not open.");
				}
			}
		}

		private void ClosePort()
		{
			if (!this.port.IsOpen)
			{
				this.LogIt(LogLevel.Warning, "Attempted to close a closed serial connection. Ignored.");
			}
			else
			{
				this.LogIt(LogLevel.Info, "Closing serial connection.");
				this.port.Close();
				this.port.DataReceived -= new SerialDataReceivedEventHandler(this.port_DataReceived);
				if (this.connectionState)
				{
					this.connectionState = false;
					this.OnPhoneDisconnected();
					return;
				}
			}
		}

		private void CommThread()
		{
			this.LogIt(LogLevel.Info, "Communication thread started.");
            System.Timers.Timer timer = new System.Timers.Timer((double)this.commThreadCheckDelay);
			timer.Elapsed += new ElapsedEventHandler(this.connectionTimer_Elapsed);
			timer.AutoReset = false;
			timer.Start();
			object[] objArray = new object[1];
			objArray[0] = this.commThreadCheckDelay;
			this.LogIt(LogLevel.Info, "Connection to phone will be checked every {0} ms.", objArray);
			WaitHandle[] waitHandleArray = new WaitHandle[4];
			waitHandleArray[0] = this.terminateCommThread;
			waitHandleArray[1] = this.sendNow;
			waitHandleArray[2] = this.receiveNow;
			waitHandleArray[3] = this.checkConnection;
			WaitHandle[] waitHandleArray1 = waitHandleArray;
			while (true)
			{
				int num = WaitHandle.WaitAny(waitHandleArray1);
				if (num == 0)
				{
					break;
				}
				if (num != 1)
				{
					if (num != 2)
					{
						if (num == 3)
						{
							timer.Stop();
							this.CheckConnection();
							timer.Start();
						}
					}
					else
					{
						if (this.CommThreadReceive())
						{
							timer.Stop();
							this.checkConnection.Reset();
							timer.Start();
						}
					}
				}
				else
				{
					string str = this.dataToSend;
					this.sendNow.Reset();
					this.inputQueue.Clear();
					this.dataReceived.Reset();
					try
					{
						this.SendInternal(str, true);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.LogIt(LogLevel.Error, "Error while sending data to the phone.");
						this.LogIt(LogLevel.Error, "Error details:");
						this.LogIt(LogLevel.Error, exception.ToString());
					}
					this.sendDone.Set();
				}
			}
			this.LogIt(LogLevel.Info, "Communication thread is terminating.");
			if (!this.terminateCommThread.WaitOne(0, false))
			{
				this.LogIt(LogLevel.Warning, "Communication thread terminates without a stop signal!");
			}
			timer.Stop();
			timer.Dispose();
			this.checkConnection.Reset();
			this.receiveNow.Reset();
			this.dataReceived.Reset();
			this.noData.Reset();
			this.inputQueue.Clear();
			this.sendNow.Reset();
			this.sendDone.Reset();
			this.dataToSend = string.Empty;
		}

		private bool CommThreadReceive()
		{
			bool flag;
			string str = null;
			this.receiveNow.Reset();
			this.noData.Reset();
			bool flag1 = false;
			MessageIndicationHandlers messageIndicationHandler = new MessageIndicationHandlers();
			StringBuilder stringBuilder = new StringBuilder();
			do
			{
				flag = false;
				try
				{
					if (this.ReceiveInternal(out str))
					{
						stringBuilder.Append(str);
						flag = true;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.LogIt(LogLevel.Error, "Error while receiving data from the phone.");
					this.LogIt(LogLevel.Error, "Error details:");
					this.LogIt(LogLevel.Error, exception.ToString());
					stringBuilder = new StringBuilder();
					flag = false;
					break;
				}
				try
				{
					if (!flag && messageIndicationHandler.IsIncompleteUnsolicitedMessage(stringBuilder.ToString()))
					{
						this.LogIt(LogLevel.Info, "Incomplete unsolicited message found, reading on after sleep.");
						Thread.Sleep(this.timeout);
						flag = true;
					}
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					this.LogIt(LogLevel.Error, "Error while checking received data for unsolicited messages.");
					this.LogIt(LogLevel.Error, "Received data was:");
					this.LogItShow(LogLevel.Error, stringBuilder.ToString(), ">> ");
					this.LogIt(LogLevel.Error, "This data will not be processed further.");
					this.LogIt(LogLevel.Error, "Error details:");
					this.LogIt(LogLevel.Error, exception2.ToString());
					stringBuilder = new StringBuilder();
					flag = false;
				}
			}
			while (flag);
			if (stringBuilder.Length <= 0)
			{
				this.noData.Set();
				flag1 = false;
			}
			else
			{
				this.inputQueue.Enqueue(stringBuilder.ToString());
				string str1 = this.MakeQueueString(this.inputQueue);
				if (this.HandleUnsolicitedMessages(ref str1))
				{
					this.inputQueue.Clear();
					this.inputQueue.Enqueue(str1);
				}
				this.dataReceived.Set();
				flag1 = true;
			}
			return flag1;
		}

		private void connectionTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.checkConnection.Set();
		}

		private void CreateCommThread()
		{
			if (!this.IsCommThreadRunning())
			{
				this.terminateCommThread.Reset();
				this.commThread = new Thread(new ThreadStart(this.CommThread));
				this.commThread.Name = "GsmPhone comm thread";
				this.commThread.Start();
				return;
			}
			else
			{
				this.LogIt(LogLevel.Warning, "Comm thread already created, ignoring call to CreateCommThread.");
				return;
			}
		}

		private void CreateLogThread()
		{
			if (this.logThread == null || !this.logThread.IsAlive)
			{
				this.logThreadInitialized.Reset();
				this.terminateLogThread.Reset();
				this.logThread = new Thread(new ThreadStart(this.LogThread));
				this.logThread.Name = "GsmPhone log thread";
				this.logThread.Start();
				this.logThreadInitialized.WaitOne();
				this.LogIt(LogLevel.Info, "Log thread started.");
				return;
			}
			else
			{
				this.LogIt(LogLevel.Warning, "Log thread already created, ignoring call to CreateLogThread.");
				return;
			}
		}

		/// <summary>
		/// Decodes a data stream with phonebook entries into <see cref="T:GsmComm.GsmCommunication.PhonebookEntry" /> objects.
		/// </summary>
		/// <param name="input">The entries to decode</param>
		/// <param name="prefix">The string the lines start with</param>
		private PhonebookEntry[] DecodePhonebookStream(string input, string prefix)
		{
			this.LogIt(LogLevel.Info, "Decoding phonebook entries...");
			ArrayList arrayLists = new ArrayList();
			Regex regex = new Regex(string.Concat(Regex.Escape(prefix), "(\\d+),\"(.+)\",(\\d+),\"(.+)\".*\\r\\n"));
			for (Match i = regex.Match(input); i.Success; i = i.NextMatch())
			{
				int num = int.Parse(i.Groups[1].Value);
				string value = i.Groups[2].Value;
				int num1 = int.Parse(i.Groups[3].Value);
				string str = i.Groups[4].Value;
				arrayLists.Add(new PhonebookEntry(num, value, num1, str));
				string[] strArrays = new string[9];
				strArrays[0] = "Entry: index=";
				strArrays[1] = num.ToString();
				strArrays[2] = ", number=\"";
				strArrays[3] = value;
				strArrays[4] = "\", type=";
				strArrays[5] = num1.ToString();
				strArrays[6] = ", text=\"";
				strArrays[7] = str;
				strArrays[8] = "\"";
				this.LogIt(LogLevel.Info, string.Concat(strArrays));
			}
			if (arrayLists.Count == 1)
			{
				this.LogIt(LogLevel.Info, "1 entry decoded.");
			}
			else
			{
				int count = arrayLists.Count;
				this.LogIt(LogLevel.Info, string.Concat(count.ToString(), " entries decoded."));
			}
			PhonebookEntry[] phonebookEntryArray = new PhonebookEntry[arrayLists.Count];
			arrayLists.CopyTo(phonebookEntryArray, 0);
			return phonebookEntryArray;
		}

		/// <summary>
		/// AT+CMGD. Deletes the specified SMS message from the current read/delete storage.
		/// </summary>
		/// <param name="index">The index of the message to delete.</param>
		public void DeleteMessage(int index)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Deleting message with index ", index.ToString(), "..."));
				this.ExecAndReceiveMultiple(string.Concat("AT+CMGD=", index.ToString()));
			}
		}

		/// <summary>
		/// AT+CMGD. Deletes the specified SMS message from the current read/delete storage.
		/// </summary>
		/// <param name="index">The index of the message to delete.</param>
		/// <param name="delflag">The delete flag, this controls the behaviour of the delete command.</param>
		public void DeleteMessage(int index, DeleteFlag delflag)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				string[] str = new string[5];
				str[0] = "Deleting message with index ";
				str[1] = index.ToString();
				str[2] = ", using delflag ";
				str[3] = delflag.ToString();
				str[4] = "...";
				this.LogIt(LogLevel.Info, string.Concat(str));
				int num = delflag;
				string str1 = string.Concat("AT+CMGD=", index.ToString(), ",", num.ToString());
				this.ExecAndReceiveMultiple(str1);
			}
		}

		/// <summary>
		/// AT+CPBW. Deletes a phonebook entry.
		/// </summary>
		/// <param name="index">The index of the entry to delete.</param>
		/// <remarks>In this case it does not matter whether the specified index is valid.
		/// If the entry does not exist, no error is returned.</remarks>
		public void DeletePhonebookEntry(int index)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Deleting phonebook entry ", index.ToString(), "..."));
				this.ExecAndReceiveMultiple(string.Concat("AT+CPBW=", index.ToString()));
			}
		}

		private bool DispatchLog()
		{
			LoglineAddedEventArgs loglineAddedEventArg = null;
			lock (this.logQueue)
			{
				if (this.logQueue.Count > 0)
				{
					loglineAddedEventArg = (LoglineAddedEventArgs)this.logQueue.Dequeue();
				}
			}
			if (loglineAddedEventArg == null)
			{
				return false;
			}
			else
			{
				try
				{
					if (this.LoglineAdded != null)
					{
						this.LoglineAdded(this, loglineAddedEventArg);
					}
				}
				catch (Exception exception)
				{
				}
				return true;
			}
		}

		/// <summary>
		/// AT+CPIN. Enters a password at the phone which is necessary before it can operated.
		/// </summary>
		/// <param name="pin">The SIM PIN, SIM PUK or other password required.</param>
		/// <remarks>Get the current PIN status with <see cref="M:GsmComm.GsmCommunication.GsmPhone.GetPinStatus" /> to check
		/// whether a password must be entered.</remarks>
		public void EnterPin(string pin)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				object[] objArray = new object[1];
				objArray[0] = pin;
				this.LogIt(LogLevel.Info, "Entering PIN...", objArray);
				this.logEnabled = false;
				try
				{
					string str = string.Format("AT+CPIN=\"{0}\"", pin);
					this.ExecAndReceiveMultiple(str);
				}
				finally
				{
					this.logEnabled = true;
				}
			}
		}

		private string ExecCommandInternal(string command, string receiveErrorMessage)
		{
			string str = null;
			string str1 = string.Concat(command, "\r");
			this.receiveNow.Reset();
			this.SendInternal(str1, false);
			StringBuilder stringBuilder = new StringBuilder();
			while (this.receiveNow.WaitOne(this.timeout, false))
			{
				this.receiveNow.Reset();
				this.ReceiveInternal(out str);
				stringBuilder.Append(str);
			}
			string str2 = stringBuilder.ToString();
			if (str2.Length != 0)
			{
				if (this.IsSuccess(str2))
				{
					if (str2.EndsWith("\r\nOK\r\n"))
					{
						str2 = str2.Remove(str2.LastIndexOf("\r\nOK\r\n"), "\r\nOK\r\n".Length);
					}
					if (str2.StartsWith(str1))
					{
						str2 = str2.Substring(str1.Length);
					}
					return str2;
				}
				else
				{
					this.HandleCommError(str2);
					return null;
				}
			}
			else
			{
				this.HandleRecvError(str2, receiveErrorMessage);
				return null;
			}
		}

		/// <summary>
		/// AT+CPBF. Searches for the specified text in the phonebook.
		/// </summary>
		/// <param name="findText">The text to find.</param>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.PhonebookEntry" /> objects containing
		/// the specified text</returns>
		public PhonebookEntry[] FindPhonebookEntries(string findText)
		{
			PhonebookEntry[] phonebookEntryArray;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Searching phonebook entry \"", findText, "\"..."));
				string str = string.Concat("AT+CPBF=\"", findText, "\"");
				string str1 = this.ExecAndReceiveMultiple(str);
				phonebookEntryArray = this.DecodePhonebookStream(str1, "+CPBF: ");
			}
			return phonebookEntryArray;
		}

		/// <summary>
		/// AT+CBC. Gets the ME battery charging status and charge level.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.BatteryChargeInfo" /> object containing the battery details.</returns>
		public BatteryChargeInfo GetBatteryCharge()
		{
			BatteryChargeInfo batteryChargeInfo;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting battery charge...");
				string str = this.ExecAndReceiveMultiple("AT+CBC");
				Regex regex = new Regex("\\+CBC: (\\d+),(\\d+)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					batteryChargeInfo = null;
				}
				else
				{
					int num = int.Parse(match.Groups[1].Value);
					int num1 = int.Parse(match.Groups[2].Value);
					this.LogIt(LogLevel.Info, string.Concat("Battery charging status = ", num.ToString()));
					this.LogIt(LogLevel.Info, string.Concat("Battery charge level = ", num1.ToString()));
					BatteryChargeInfo batteryChargeInfo1 = new BatteryChargeInfo(num, num1);
					batteryChargeInfo = batteryChargeInfo1;
				}
			}
			return batteryChargeInfo;
		}

		/// <summary>
		/// AT+CSCS. Retrives the currently selected character set.
		/// </summary>
		/// <returns>The current character set.</returns>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.SelectCharacterSet(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.GetSupportedCharacterSets" />
		/// </remarks>
		public string GetCurrentCharacterSet()
		{
			string empty;
			string str;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Retrieving current character set...");
				string str1 = this.ExecAndReceiveMultiple("AT+CSCS?");
				Regex regex = new Regex("\\+CSCS: \"([^\"]+)\"");
				Match match = regex.Match(str1);
				if (!match.Success)
				{
					empty = string.Empty;
					this.HandleCommError(str1);
				}
				else
				{
					empty = match.Groups[1].Value;
				}
				this.LogIt(LogLevel.Info, string.Concat("Current character set is \"", empty, "\"."));
				str = empty;
			}
			return str;
		}

		/// <summary>
		/// AT+CSMS. Gets the supported message types along with the current service setting.
		/// </summary>
		/// <param name="service">Specifies the compatibility level of the SMS AT commands.
		/// The requirement of service setting 1 depends on specific commands.
		/// </param>
		/// <param name="mt">ME supports mobile terminated messages</param>
		/// <param name="mo">ME supports mobile originated messages</param>
		/// <param name="bm">ME supports broadcast type messages</param>
		public void GetCurrentMessageService(out int service, out int mt, out int mo, out int bm)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				string str = this.ExecAndReceiveMultiple("AT+CSMS?");
				Regex regex = new Regex("\\+CSMS: (\\d+),(\\d+),(\\d+),(\\d+)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					service = int.Parse(match.Groups[1].Value);
					mt = int.Parse(match.Groups[2].Value);
					mo = int.Parse(match.Groups[3].Value);
					bm = int.Parse(match.Groups[4].Value);
				}
			}
		}

		/// <summary>
		/// AT+COPS. Gets the currently selected network operator.
		/// </summary>
		/// <returns>An <see cref="T:GsmComm.GsmCommunication.OperatorInfo" /> object containing the data or null if there is no current operator.</returns>
		public OperatorInfo GetCurrentOperator()
		{
			OperatorInfo operatorInfo;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting current operator...");
				string str = this.ExecAndReceiveMultiple("AT+COPS?");
				Regex regex = new Regex("\\+COPS: (\\d+)(?:,(\\d+),\"(.+)\")?(?:,(.+))?");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					int.Parse(match.Groups[1].Value);
					if (match.Groups.Count <= 1)
					{
						this.LogIt(LogLevel.Info, "There is no operator currently selected!");
						operatorInfo = null;
					}
					else
					{
						int num = int.Parse(match.Groups[2].Value);
						string value = match.Groups[3].Value;
						string empty = string.Empty;
						if (match.Groups.Count > 3)
						{
							empty = match.Groups[4].Value;
						}
						object[] objArray = new object[3];
						objArray[0] = num;
						objArray[1] = value;
						objArray[2] = empty;
						this.LogIt(LogLevel.Info, "format={0}, oper=\"{1}\", act=\"{2}\"", objArray);
						if (Enum.IsDefined(typeof(OperatorFormat), num))
						{
							OperatorFormat operatorFormat = (OperatorFormat)Enum.Parse(typeof(OperatorFormat), num.ToString());
							OperatorInfo operatorInfo1 = new OperatorInfo(operatorFormat, value, empty);
							operatorInfo = operatorInfo1;
						}
						else
						{
							throw new CommException(string.Concat("Unknown operator format ", num.ToString()), str);
						}
					}
				}
			}
			return operatorInfo;
		}

		/// <summary>
		/// AT+CNMI. Gets the current message notification settings.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MessageIndicationSettings" /> structure containing the detailed settings.</returns>
		public MessageIndicationSettings GetMessageIndications()
		{
			MessageIndicationSettings messageIndicationSetting;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting current message indications...");
				string str = this.ExecAndReceiveMultiple("AT+CNMI?");
				Regex regex = new Regex("\\+CNMI: (\\d+),(\\d+),(\\d+),(\\d+),(\\d+)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					MessageIndicationSettings messageIndicationSetting1 = new MessageIndicationSettings();
					messageIndicationSetting1.Mode = int.Parse(match.Groups[1].Value);
					messageIndicationSetting1.DeliverStyle = int.Parse(match.Groups[2].Value);
					messageIndicationSetting1.CellBroadcastStyle = int.Parse(match.Groups[3].Value);
					messageIndicationSetting1.StatusReportStyle = int.Parse(match.Groups[4].Value);
					messageIndicationSetting1.BufferSetting = int.Parse(match.Groups[5].Value);
					object[] mode = new object[5];
					mode[0] = messageIndicationSetting1.Mode;
					mode[1] = messageIndicationSetting1.DeliverStyle;
					mode[2] = messageIndicationSetting1.CellBroadcastStyle;
					mode[3] = messageIndicationSetting1.StatusReportStyle;
					mode[4] = messageIndicationSetting1.BufferSetting;
					this.LogIt(LogLevel.Info, string.Format("mode={0:g}, mt={1:g}, bm={2:g}, ds={3:g}, bfr={4:g}", mode));
					messageIndicationSetting = messageIndicationSetting1;
				}
			}
			return messageIndicationSetting;
		}

		/// <summary>
		/// Returns the message service error code in the input string.
		/// </summary>
		/// <param name="input">The data received</param>
		/// <returns>The error code</returns>
		/// <remarks>Use the <see cref="M:GsmComm.GsmCommunication.GsmPhone.IsMessageServiceError(System.String)" /> method to check if the string
		/// contains a message service error message.</remarks>
		/// <exception cref="T:System.ArgumentException">Input string does not contain a message service error code</exception>
		private int GetMessageServiceErrorCode(string input)
		{
			Regex regex = new Regex("\\r\\n\\+CMS ERROR: (\\d+)\\r\\n");
			Match match = regex.Match(input);
			if (!match.Success || match.Groups[1].Captures.Count <= 0)
			{
				throw new ArgumentException("The input string does not contain a message service error code.");
			}
			else
			{
				return int.Parse(match.Groups[1].Captures[0].ToString());
			}
		}

		/// <summary>
		/// AT+CPMS. Gets the supported message storages.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MessageStorageInfo" /> object that contains details about the supported storages.</returns>
		public MessageStorageInfo GetMessageStorages()
		{
			MessageStorageInfo messageStorageInfo;
			lock (this)
			{
				this.VerifyValidConnection();
				ArrayList arrayLists = new ArrayList();
				this.LogIt(LogLevel.Info, "Enumerating supported message storages...");
				string str = this.ExecAndReceiveMultiple("AT+CPMS=?");
				int num = str.IndexOf("+CPMS: ");
				if (num < 0)
				{
					this.HandleCommError(str);
				}
				else
				{
					str = str.Substring(num + "+CPMS: ".Length);
				}
				Regex regex = new Regex("\\((?:\"(\\w+)\"(?(?!\\)),))+\\)");
				for (Match i = regex.Match(str); i.Success; i = i.NextMatch())
				{
					int count = i.Groups[1].Captures.Count;
					string[] value = new string[count];
					for (int j = 0; j < count; j++)
					{
						value[j] = i.Groups[1].Captures[j].Value;
					}
					arrayLists.Add(value);
				}
			}
			return messageStorageInfo;
		}

		/// <summary>
		/// Returns the mobile equipment error code in the input string.
		/// </summary>
		/// <param name="input">The data received</param>
		/// <returns>The mobile equipment error code</returns>
		/// <remarks>Use the <see cref="M:GsmComm.GsmCommunication.GsmPhone.IsMobileEquipmentError(System.String)" /> method to check if the string
		/// contains mobile equipment error message.</remarks>
		/// <exception cref="T:System.ArgumentException">Input string does not contain a mobile equipment error code</exception>
		private int GetMobileEquipmentErrorCode(string input)
		{
			Regex regex = new Regex("\\r\\n\\+CME ERROR: (\\d+)\\r\\n");
			Match match = regex.Match(input);
			if (!match.Success || match.Groups[1].Captures.Count <= 0)
			{
				throw new ArgumentException("The input string does not contain a mobile equipment error code.");
			}
			else
			{
				return int.Parse(match.Groups[1].Captures[0].ToString());
			}
		}

		/// <summary>
		/// AT+CMMS. Gets the current SMS batch mode setting.
		/// </summary>
		/// <returns>The current mode.</returns>
		public MoreMessagesMode GetMoreMessagesToSend()
		{
			MoreMessagesMode moreMessagesMode;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting more messages mode...");
				string str = string.Format("AT+CMMS?", new object[0]);
				string str1 = this.ExecAndReceiveMultiple(str);
				Regex regex = new Regex("\\+CMMS: (\\d+)");
				Match match = regex.Match(str1);
				if (!match.Success)
				{
					this.HandleCommError(str1);
					throw new CommException("Unexpected response.", str1);
				}
				else
				{
					int num = int.Parse(match.Groups[1].Value);
					object[] objArray = new object[1];
					objArray[0] = num;
					this.LogIt(LogLevel.Info, "mode=\"{0}\"", objArray);
					if (Enum.IsDefined(typeof(MoreMessagesMode), num))
					{
						MoreMessagesMode moreMessagesMode1 = (MoreMessagesMode)Enum.Parse(typeof(MoreMessagesMode), num.ToString());
						moreMessagesMode = moreMessagesMode1;
					}
					else
					{
						throw new CommException(string.Concat("Unknown more messages mode ", num.ToString(), "."), str1);
					}
				}
			}
			return moreMessagesMode;
		}

		/// <summary>
		/// AT+COPS. Determines the current mode to select a network operator.
		/// </summary>
		/// <returns>The current mode, see <see cref="T:GsmComm.GsmCommunication.OperatorSelectionMode" /> for possible values.</returns>
		public int GetOperatorSelectionMode()
		{
			int num;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting current operator selection mode...");
				string str = this.ExecAndReceiveMultiple("AT+COPS?");
				Regex regex = new Regex("\\+COPS: (\\d+)(?:,(\\d+),\"(.+)\")?(?:,(.+))?");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					int num1 = int.Parse(match.Groups[1].Value);
					object[] objArray = new object[1];
					objArray[0] = num1.ToString();
					this.LogIt(LogLevel.Info, "Current mode is {0}.", objArray);
					num = num1;
				}
			}
			return num;
		}

		/// <summary>
		/// AT+CPBS. Gets the memory status of the currently selected phonebook storage.
		/// </summary>
		/// <returns>The memory status of the currently selected storage.</returns>
		public MemoryStatusWithStorage GetPhonebookMemoryStatus()
		{
			MemoryStatusWithStorage memoryStatusWithStorage;
			int used;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting phonebook memory status...");
				string str = this.ExecAndReceiveMultiple("AT+CPBS?");
				MemoryStatusWithStorage memoryStatusWithStorage1 = this.ParsePhonebookMemoryStatus(str);
				if (memoryStatusWithStorage1.Total > 0)
				{
					used = (int)((double)memoryStatusWithStorage1.Used / (double)memoryStatusWithStorage1.Total * 100);
				}
				else
				{
					used = 0;
				}
				int num = used;
				object[] storage = new object[4];
				storage[0] = memoryStatusWithStorage1.Storage;
				storage[1] = memoryStatusWithStorage1.Used;
				storage[2] = memoryStatusWithStorage1.Total;
				storage[3] = num;
				this.LogIt(LogLevel.Info, string.Format("Memory status: storage=\"{0}\" {1}/{2} ({3}% used)", storage));
				memoryStatusWithStorage = memoryStatusWithStorage1;
			}
			return memoryStatusWithStorage;
		}

		/// <summary>
		/// AT+CPBR. Queries the size of the currently selected phonebook.
		/// </summary>
		/// <param name="lowerBound">Receives the lower bound of the phonebook</param>
		/// <param name="upperBound">Receives the upper bound of the phonebook</param>
		/// <param name="nLength">Receives the maximum number length, 0 if unknown</param>
		/// <param name="tLength">Receives the maximum text length, 0 if unknown</param>
		public void GetPhonebookSize(out int lowerBound, out int upperBound, out int nLength, out int tLength)
		{
			int num;
			int num1;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting phonebook size...");
				string str = this.ExecAndReceiveMultiple("AT+CPBR=?");
				Regex regex = new Regex("\\+CPBR: \\((\\d+)-(\\d+)\\)\\,(\\d*),(\\d*)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
				}
				lowerBound = int.Parse(match.Groups[1].Value);
				upperBound = int.Parse(match.Groups[2].Value);
				int& numPointer = nLength;
				if (match.Groups[3].Value != "")
				{
					num = int.Parse(match.Groups[3].Value);
				}
				else
				{
					num = 0;
				}
				*(numPointer) = num;
				int& numPointer1 = tLength;
				if (match.Groups[4].Value != "")
				{
					num1 = int.Parse(match.Groups[4].Value);
				}
				else
				{
					num1 = 0;
				}
				*(numPointer1) = num1;
				string[] strArrays = new string[8];
				strArrays[0] = "lowerBound=";
				strArrays[1] = lowerBound.ToString();
				strArrays[2] = ", upperBound=";
				strArrays[3] = upperBound.ToString();
				strArrays[4] = ", nLength=";
				strArrays[5] = nLength.ToString();
				strArrays[6] = ", tLength=";
				strArrays[7] = tLength.ToString();
				this.LogIt(LogLevel.Info, string.Concat(strArrays));
			}
		}

		/// <summary>
		/// AT+CPBS. Gets the supported phonebook storages.
		/// </summary>
		/// <returns>An array of the supported storages</returns>
		public string[] GetPhonebookStorages()
		{
			string[] strArrays;
			string str;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Enumerating supported storages...");
				string str1 = this.ExecAndReceiveMultiple("AT+CPBS=?");
				string[] value = null;
				Regex regex = new Regex("\\+CPBS: \\((?:\"(\\w+)\"(?(?!\\)),))+\\)");
				Match match = regex.Match(str1);
				if (!match.Success)
				{
					this.HandleCommError(str1);
				}
				else
				{
					int count = match.Groups[1].Captures.Count;
					value = new string[count];
					for (int i = 0; i < count; i++)
					{
						value[i] = match.Groups[1].Captures[i].Value;
					}
				}
				string empty = string.Empty;
				int num = 0;
				while (num < (int)value.Length)
				{
					string str2 = empty;
					if (empty == string.Empty)
					{
						str = "";
					}
					else
					{
						str = ", ";
					}
					empty = string.Concat(str2, str, value[num]);
					num++;
				}
				this.LogIt(LogLevel.Info, string.Concat("Supported storages: ", empty));
				strArrays = value;
			}
			return strArrays;
		}

		/// <summary>
		/// AT+CPIN. Returns a value indicating whether some password must be entered at the phone or not.
		/// </summary>
		/// <returns>The current PIN status as one of the <see cref="T:GsmComm.GsmCommunication.PinStatus" /> values.</returns>
		public PinStatus GetPinStatus()
		{
			PinStatus status;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting PIN status...");
				string str = string.Format("AT+CPIN?", new object[0]);
				string str1 = this.ExecAndReceiveMultiple(str);
				Regex regex = new Regex("\\+CPIN: (.+)\\r\\n");
				Match match = regex.Match(str1);
				if (!match.Success)
				{
					this.HandleCommError(str1);
					throw new CommException("Unexpected response.", str1);
				}
				else
				{
					string value = match.Groups[1].Value;
					object[] objArray = new object[1];
					objArray[0] = value;
					this.LogIt(LogLevel.Info, "status=\"{0}\"", objArray);
					status = this.MapPinStatusStringToStatus(value);
				}
			}
			return status;
		}

		/// <summary>
		/// Enables access to the protocol level of the current connection.
		/// </summary>
		/// <returns>An <see cref="T:GsmComm.GsmCommunication.IProtocol" /> object that sends and receives data at the protocol level.</returns>
		/// <remarks>This method enables execution of custom commands that are not directly supported. It also disables execution of background
		/// operations that would usually take place, such as checking whether the phone is still connected.
		/// <para>The <see cref="M:GsmComm.GsmCommunication.GsmPhone.ReleaseProtocol" /> method must be called as soon as execution of the custom commands is completed,
		/// and allows for normal operations to continue. Execution of other commands besides from <see cref="T:GsmComm.GsmCommunication.IProtocol" /> is not allowed
		/// until <see cref="M:GsmComm.GsmCommunication.GsmPhone.ReleaseProtocol" /> is called.</para>
		/// </remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.ReleaseProtocol" />
		public IProtocol GetProtocol()
		{
			Monitor.Enter(this);
			return this;
		}

		/// <summary>
		/// AT+CSQ. Gets the signal quality as calculated by the ME.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.SignalQualityInfo" /> object containing the signal details.</returns>
		public SignalQualityInfo GetSignalQuality()
		{
			SignalQualityInfo signalQualityInfo;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting signal quality...");
				string str = this.ExecAndReceiveMultiple("AT+CSQ");
				Regex regex = new Regex("\\+CSQ: (\\d+),(\\d+)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					signalQualityInfo = null;
				}
				else
				{
					int num = int.Parse(match.Groups[1].Value);
					int num1 = int.Parse(match.Groups[2].Value);
					this.LogIt(LogLevel.Info, string.Concat("Signal strength = ", num.ToString()));
					this.LogIt(LogLevel.Info, string.Concat("Bit error rate = ", num1.ToString()));
					SignalQualityInfo signalQualityInfo1 = new SignalQualityInfo(num, num1);
					signalQualityInfo = signalQualityInfo1;
				}
			}
			return signalQualityInfo;
		}

		/// <summary>
		/// AT+CSCA. Gets the SMS Service Center Address.
		/// </summary>
		/// <returns>The current SMSC address</returns>
		/// <remarks>This command returns the SMSC address, through which SMS messages are transmitted.
		/// In text mode, this setting is used by SMS sending and SMS writing commands. In PDU mode, this setting is
		/// used by the same commands, but only when the length of the SMSC address coded into the PDU data equals
		/// zero.</remarks>
		public AddressData GetSmscAddress()
		{
			AddressData addressDatum;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting SMSC address...");
				string str = "AT+CSCA?";
				string str1 = this.ExecAndReceiveMultiple(str);
				Regex regex = new Regex("\\+CSCA: \"(.*)\",(\\d+)");
				Match match = regex.Match(str1);
				if (!match.Success)
				{
					this.HandleCommError(str1);
					throw new CommException("Unexpected response.", str1);
				}
				else
				{
					string value = match.Groups[1].Value;
					int num = int.Parse(match.Groups[2].Value);
					object[] objArray = new object[2];
					objArray[0] = value;
					objArray[1] = num.ToString();
					this.LogIt(LogLevel.Info, "address=\"{0}\", typeOfAddress={1}", objArray);
					AddressData addressDatum1 = new AddressData(value, num);
					addressDatum = addressDatum1;
				}
			}
			return addressDatum;
		}

		/// <summary>
		/// AT+CNUM. Returns the MSISDNs related to the subscriber.
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
			SubscriberInfo[] subscriberInfoArray;
			string empty;
			int num;
			int num1;
			int num2;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting subscriber numbers...");
				string str = this.ExecAndReceiveMultiple("AT+CNUM");
				Regex regex = new Regex("\\+CNUM: (.*),\"(.*)\",(\\d+)(?:,(\\d+),(\\d+)(?:,(\\d+))?)?(?:\\r\\n)?");
				Match match = regex.Match(str);
				List<SubscriberInfo> subscriberInfos = new List<SubscriberInfo>();
				while (match.Success)
				{
					if (match.Groups[1].Captures.Count <= 0 || match.Groups[1].Length <= 0)
					{
						empty = string.Empty;
					}
					else
					{
						empty = match.Groups[1].Value;
					}
					string str1 = empty;
					string value = match.Groups[2].Value;
					int num3 = int.Parse(match.Groups[3].Value);
					if (match.Groups[4].Captures.Count <= 0 || match.Groups[4].Length <= 0)
					{
						num = -1;
					}
					else
					{
						num = int.Parse(match.Groups[4].Value);
					}
					int num4 = num;
					if (match.Groups[5].Captures.Count <= 0 || match.Groups[5].Length <= 0)
					{
						num1 = -1;
					}
					else
					{
						num1 = int.Parse(match.Groups[5].Value);
					}
					int num5 = num1;
					if (match.Groups[6].Captures.Count <= 0 || match.Groups[6].Length <= 0)
					{
						num2 = -1;
					}
					else
					{
						num2 = int.Parse(match.Groups[6].Value);
					}
					int num6 = num2;
					object[] objArray = new object[6];
					objArray[0] = str1;
					objArray[1] = value;
					objArray[2] = num3;
					objArray[3] = num4;
					objArray[4] = num5;
					objArray[5] = num6;
					this.LogIt(LogLevel.Info, "alpha=\"{0}\",number=\"{1}\",type={2},speed={3},service={4},itc={5}", objArray);
					SubscriberInfo subscriberInfo = new SubscriberInfo(str1, value, num3, num4, num5, num6);
					subscriberInfos.Add(subscriberInfo);
					match = match.NextMatch();
				}
				SubscriberInfo[] subscriberInfoArray1 = new SubscriberInfo[subscriberInfos.Count];
				subscriberInfos.CopyTo(subscriberInfoArray1);
				subscriberInfoArray = subscriberInfoArray1;
			}
			return subscriberInfoArray;
		}

		/// <summary>
		/// AT+CSCS. Retrieves the phone's supported character sets.
		/// </summary>
		/// <returns>A string array containing the supported character sets.</returns>
		/// <remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.SelectCharacterSet(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.GetCurrentCharacterSet" />
		/// </remarks>
		public string[] GetSupportedCharacterSets()
		{
			string[] strArrays;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Enumerating supported character sets...");
				string str = this.ExecAndReceiveMultiple("AT+CSCS=?");
				string[] value = null;
				Regex regex = new Regex("\\+CSCS: \\((?:\"([^\"]+)\"(?(?!\\)),))+\\)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
				}
				else
				{
					int count = match.Groups[1].Captures.Count;
					value = new string[count];
					for (int i = 0; i < count; i++)
					{
						value[i] = match.Groups[1].Captures[i].Value;
					}
				}
				string str1 = this.MakeArrayString(value);
				this.LogIt(LogLevel.Info, string.Concat("Supported character sets: ", str1));
				strArrays = value;
			}
			return strArrays;
		}

		/// <summary>
		/// AT+CNMI. Gets the supported new message indications from the phone.
		/// </summary>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MessageIndicationSupport" /> object containing information about the supported
		/// indications.</returns>
		public MessageIndicationSupport GetSupportedIndications()
		{
			MessageIndicationSupport messageIndicationSupport;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Getting supported message indications...");
				string str = this.ExecAndReceiveMultiple("AT+CNMI=?");
				Regex regex = new Regex("\\+CNMI: \\(([\\d,-])+\\),\\(([\\d,-]+)\\),\\(([\\d,-]+)\\),\\(([\\d,-]+)\\),\\(([\\d,-]+)\\)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					string empty = string.Empty;
					string empty1 = string.Empty;
					string str1 = string.Empty;
					string empty2 = string.Empty;
					string str2 = string.Empty;
					foreach (Capture capture in match.Groups[1].Captures)
					{
						empty = string.Concat(empty, capture.Value);
					}
					foreach (Capture capture1 in match.Groups[2].Captures)
					{
						empty1 = string.Concat(empty1, capture1.Value);
					}
					foreach (Capture capture2 in match.Groups[3].Captures)
					{
						str1 = string.Concat(str1, capture2.Value);
					}
					foreach (Capture capture3 in match.Groups[4].Captures)
					{
						empty2 = string.Concat(empty2, capture3.Value);
					}
					foreach (Capture capture4 in match.Groups[5].Captures)
					{
						str2 = string.Concat(str2, capture4.Value);
					}
					object[] objArray = new object[5];
					objArray[0] = empty;
					objArray[1] = empty1;
					objArray[2] = str1;
					objArray[3] = empty2;
					objArray[4] = str2;
					this.LogIt(LogLevel.Info, "mode=\"{0}\", deliver=\"{1}\", cellBroadcast=\"{2}\", statusReport=\"{3}\", buffer=\"{4}\"", objArray);
					MessageIndicationSupport messageIndicationSupport1 = new MessageIndicationSupport(empty, empty1, str1, empty2, str2);
					messageIndicationSupport = messageIndicationSupport1;
				}
			}
			return messageIndicationSupport;
		}

		/// <summary>
		/// Executes the specified command and reads multiple times from the phone
		/// until a specific pattern is detected in the response.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="pattern">The regular expression pattern that the received data must match to stop
		/// reading.</param>
		/// <returns>The response received.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveAnything(System.String)" />
		public string ExecAndReceiveAnything(string command, string pattern)
		{
			string str = string.Concat(command, "\r");
			this.Send(str);
			string str1 = this.ReceiveAnything(pattern);
			if (str1.StartsWith(str))
			{
				str1 = str1.Substring(str.Length);
			}
			return str1;
		}

		/// <summary>Executes the specified command and reads multiple times from the phone
		/// until one of the defined message termination patterns is detected in the response.</summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>The response received.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveAnything(System.String,System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveMultiple" />
		public string ExecAndReceiveMultiple(string command)
		{
            
			string str = string.Concat(command, "\r");
			this.Send(str);
			string str1 = this.ReceiveMultiple();
			if (str1.EndsWith("\r\nOK\r\n"))
			{
				str1 = str1.Remove(str1.LastIndexOf("\r\nOK\r\n"), "\r\nOK\r\n".Length);
			}
			if (str1.StartsWith(str))
			{
				str1 = str1.Substring(str.Length);
			}
			return str1;
		}

		/// <summary>Executes the specified command and reads a single response.</summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>The response received.</returns>
		/// <remarks>
		/// <para>This method returns whatever response comes in from the phone during a single read operation.
		/// The response received may not be complete.</para>
		/// <para>If you want to ensure that always complete responses are read, use <see cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" /> instead.</para>
		/// </remarks>
		/// <see cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		/// <see cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveAnything(System.String,System.String)" />
		public string ExecCommand(string command)
		{
			return this.ExecCommand(command, "No answer from phone.");
		}

		/// <summary>
		/// Executes the specified command and reads a single response.</summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="receiveErrorMessage">The message text for the exception if no data is received.</param>
		/// <returns>The response received.</returns>
		/// <remarks>
		/// <para>This method returns whatever response comes in from the phone during a single read operation.
		/// The response received may not be complete.</para>
		/// <para>If you want to ensure that always complete responses are read, use <see cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" /> instead.</para>
		/// </remarks>
		/// <see cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		/// <see cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveAnything(System.String,System.String)" />
		public string ExecCommand(string command, string receiveErrorMessage)
		{
			string str = null;
			string str1 = string.Concat(command, "\r");
			this.Send(str1);
			if (this.Receive(out str))
			{
				this.LogItShow(LogLevel.Verbose, str, ">> ");
				if (this.IsSuccess(str))
				{
					if (str.EndsWith("\r\nOK\r\n"))
					{
						str = str.Remove(str.LastIndexOf("\r\nOK\r\n"), "\r\nOK\r\n".Length);
					}
					if (str.StartsWith(str1))
					{
						str = str.Substring(str1.Length);
					}
					return str;
				}
				else
				{
					this.HandleCommError(str);
					return null;
				}
			}
			else
			{
				this.HandleRecvError(str, receiveErrorMessage);
				return null;
			}
		}

		/// <summary>Receives raw string data.</summary>
		/// <param name="input">The data received.</param>
		/// <returns>true if reception was successful, otherwise false.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		public bool GsmComm.GsmCommunication.IProtocol.Receive(out string input)
		{
			input = string.Empty;
			if (!this.IsCommThreadRunning())
			{
				throw new InvalidOperationException("Communication thread is not running.");
			}
			else
			{
				WaitHandle[] waitHandleArray = new WaitHandle[2];
				waitHandleArray[0] = this.dataReceived;
				waitHandleArray[1] = this.noData;
				WaitHandle[] waitHandleArray1 = waitHandleArray;
				int num = WaitHandle.WaitAny(waitHandleArray1, 5000, false);
				if (num == 0)
				{
					this.dataReceived.Reset();
					if (this.inputQueue.Count <= 0)
					{
						this.LogIt(LogLevel.Warning, "Nothing in input queue");
					}
					else
					{
						while (this.inputQueue.Count > 0)
						{
							input = string.Concat(input, (string)this.inputQueue.Dequeue());
						}
					}
				}
				this.noData.Reset();
				return input.Length > 0;
			}
		}

		/// <summary>Reads multiple times from the phone until a specific pattern is detected in the response.</summary>
		/// <returns>The response received.</returns>
		/// <param name="pattern">The regular expression pattern that the received data must match to
		/// stop reading. Can be an empty string if the pattern should not be checked.</param>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveAnything(System.String,System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveMultiple" />
		public string GsmComm.GsmCommunication.IProtocol.ReceiveAnything(string pattern)
		{
			string str = null;
			string empty = string.Empty;
			int tickCount = Environment.TickCount;
			this.OnReceiveProgress(empty.Length);
			int num = 0;
			int num1 = 0;
			while (!this.IsSuccess(empty) && !this.IsCommError(empty) && !this.IsMessageServiceError(empty) && !this.IsMobileEquipmentError(empty) && num < 6)
			{
				if (!this.Receive(out str))
				{
					num++;
					if (num <= 1)
					{
						continue;
					}
					this.LogIt(LogLevel.Info, string.Concat("Waiting for response from phone (", num.ToString(), " empty read(s))."));
				}
				else
				{
					empty = string.Concat(empty, str);
					num = 0;
					num1++;
					this.OnReceiveProgress(empty.Length);
					if (pattern.Length <= 0 || !Regex.IsMatch(empty, pattern))
					{
						continue;
					}
					break;
				}
			}
			int tickCount1 = Environment.TickCount - tickCount;
			this.LogItShow(LogLevel.Verbose, empty, ">> ");
			this.OnReceiveComplete(empty.Length);
			if (num < 6)
			{
				if (tickCount1 > this.timeout)
				{
					object[] objArray = new object[2];
					int length = empty.Length;
					objArray[0] = length.ToString();
					objArray[1] = tickCount1.ToString();
					this.LogIt(LogLevel.Info, "{0} characters received after {1} ms.", objArray);
				}
				if (pattern.Length != 0 || this.IsSuccess(empty))
				{
					this.HandleUnsolicitedMessages(ref empty);
					return empty;
				}
				else
				{
					this.HandleCommError(empty);
					return string.Empty;
				}
			}
			else
			{
				string[] strArrays = new string[5];
				strArrays[0] = "Timeout after ";
				strArrays[1] = num.ToString();
				strArrays[2] = " empty reading attempts within ";
				strArrays[3] = tickCount1.ToString();
				strArrays[4] = " ms.";
				this.LogIt(LogLevel.Error, string.Concat(strArrays));
				throw new CommException(string.Concat("No data received from phone after waiting for ", tickCount1.ToString(), " ms."));
			}
		}

		/// <summary>Reads multiple times from the phone until one of the defined
		/// message termination patterns is detected in the response.</summary>
		/// <returns>The response received.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveAnything(System.String)" />
		public string GsmComm.GsmCommunication.IProtocol.ReceiveMultiple()
		{
			return this.ReceiveAnything(string.Empty);
		}

		/// <summary>Sends raw string data.</summary>
		/// <param name="output">The data to send.</param>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		public void GsmComm.GsmCommunication.IProtocol.Send(string output)
		{
			if (!this.IsCommThreadRunning())
			{
				throw new InvalidOperationException("Communication thread is not running.");
			}
			else
			{
				this.dataToSend = output;
				this.sendDone.Reset();
				this.sendNow.Set();
				this.sendDone.WaitOne();
				return;
			}
		}

		/// <summary>
		/// Handles a communication error.
		/// </summary>
		/// <param name="input">The data received.</param>
		/// <remarks>Call this function when communicating and the response is not a success response. This
		/// function checks if the response contains an error message. In any case a <see cref="T:GsmComm.GsmCommunication.CommException" />
		/// or an exception derived from it is thrown based on the type of error.
		/// </remarks>
		/// <exception cref="T:GsmComm.GsmCommunication.CommException">Always thrown based on type of error.</exception>
		private void HandleCommError(string input)
		{
			if (!this.IsCommError(input))
			{
				if (!this.IsMessageServiceError(input))
				{
					if (!this.IsMobileEquipmentError(input))
					{
						if (input.Length != 0)
						{
							this.LogIt(LogLevel.Error, "Failed. Unexpected response.");
							this.LogIt(LogLevel.Error, "Received input:");
							this.LogItShow(LogLevel.Error, input, "");
							throw new CommException(string.Concat("Unexpected response received from phone:", Environment.NewLine, Environment.NewLine, input));
						}
						else
						{
							this.LogIt(LogLevel.Error, "Failed. No answer from phone.");
							throw new CommException("No answer from phone.");
						}
					}
					else
					{
						int mobileEquipmentErrorCode = this.GetMobileEquipmentErrorCode(input);
						this.LogIt(LogLevel.Error, string.Concat("Failed. Phone reports mobile equipment (ME) error ", mobileEquipmentErrorCode.ToString(), "."));
						this.LogItShow(LogLevel.Error, input, "");
						throw new MobileEquipmentErrorException(string.Concat("Mobile equipment error ", mobileEquipmentErrorCode.ToString(), " occurred."), mobileEquipmentErrorCode, input);
					}
				}
				else
				{
					int messageServiceErrorCode = this.GetMessageServiceErrorCode(input);
					this.LogIt(LogLevel.Error, string.Concat("Failed. Phone reports message service (MS) error ", messageServiceErrorCode.ToString(), "."));
					this.LogItShow(LogLevel.Error, input, "");
					throw new MessageServiceErrorException(string.Concat("Message service error ", messageServiceErrorCode.ToString(), " occurred."), messageServiceErrorCode, input);
				}
			}
			else
			{
				string str = "The phone reports an unspecified error. This typically happens when a command is not supported by the device, a command is not valid for the current state or if a parameter is incorrect.";
				this.LogIt(LogLevel.Error, string.Concat("Failed. ", str, " The response received was: "));
				this.LogItShow(LogLevel.Error, input, "");
				throw new CommException(str, input);
			}
		}

		private void HandleRecvError(string input, string userMessage)
		{
			if (input.Length <= 0)
			{
				this.LogIt(LogLevel.Error, "Failed. Receiving error.");
			}
			else
			{
				this.LogIt(LogLevel.Error, "Failed. Receiving error. Data until error:");
				this.LogItShow(LogLevel.Error, input, "");
			}
			throw new CommException(userMessage);
		}

		private bool HandleUnsolicitedMessages(ref string bigInput)
		{
			string str = null;
			MessageIndicationHandlers messageIndicationHandler = new MessageIndicationHandlers();
			if (!messageIndicationHandler.IsUnsolicitedMessage(bigInput))
			{
				return false;
			}
			else
			{
				this.LogItShow(LogLevel.Verbose, bigInput, ">> ");
				IMessageIndicationObject messageIndicationObject = messageIndicationHandler.HandleUnsolicitedMessage(ref bigInput, out str);
				this.LogIt(LogLevel.Info, string.Concat("Unsolicited message: ", str));
				this.OnMessageReceived(messageIndicationObject);
				return true;
			}
		}

		private bool IsCommError(string input)
		{
			return input.IndexOf("\r\nERROR\r\n") >= 0;
		}

		private bool IsCommThreadRunning()
		{
			if (this.commThread == null)
			{
				return false;
			}
			else
			{
				return this.commThread.IsAlive;
			}
		}

		/// <summary>
		/// Checks if there is a device connected and responsive.
		/// </summary>
		/// <returns>true if there is really a device connected and it responds to commands, false otherwise.</returns>
		/// <remarks>
		/// You can use this function after opening the port with <see cref="M:GsmComm.GsmCommunication.GsmPhone.Open" /> to verify that there is really a device connected
		/// before processding.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.Open" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.IsOpen" />
		/// </remarks>
		public bool IsConnected()
		{
			bool flag;
			lock (this)
			{
				flag = this.connectionState;
			}
			return flag;
		}

		/// <summary>
		/// AT. Checks if there is a device connected and responsive.
		/// </summary>
		/// <returns>true if there is really a device connected and it responds to commands, false otherwise.</returns>
		/// <remarks>
		/// Bypasses the communication thread and does a direct send/receive.
		/// </remarks>
		private bool IsConnectedInternal()
		{
			bool flag;
			lock (this)
			{
				try
				{
					this.ExecCommandInternal("AT", "No phone connected.");
				}
				catch (Exception exception)
				{
					flag = false;
					return flag;
				}
				flag = true;
			}
			return flag;
		}

		private bool IsMessageServiceError(string input)
		{
			return Regex.IsMatch(input, "\\r\\n\\+CMS ERROR: (\\d+)\\r\\n");
		}

		/// <summary>
		/// Checks if there is a mobile equipment error message in the input string.
		/// </summary>
		/// <param name="input">The data received</param>
		/// <returns>true if there is a mobile equipment error message in the string, otherwiese false.</returns>
		private bool IsMobileEquipmentError(string input)
		{
			return Regex.IsMatch(input, "\\r\\n\\+CME ERROR: (\\d+)\\r\\n");
		}

		/// <summary>
		/// Checks if the communication to the device is open.
		/// </summary>
		/// <returns>true if the port is open, false otherwise.</returns>
		/// <remarks><para>The port is open after a auccessful call to <see cref="M:GsmComm.GsmCommunication.GsmPhone.Open" /> and must be closed with
		/// <see cref="M:GsmComm.GsmCommunication.GsmPhone.Close" />.</para>
		/// <para>This function does not check if there is actually a device connected, use the <see cref="M:GsmComm.GsmCommunication.GsmPhone.IsConnected" />
		/// function for that.</para>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.Open" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.Close" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.IsConnected" />
		/// </remarks>
		public bool IsOpen()
		{
			bool flag;
			bool isOpen;
			lock (this)
			{
				if (this.port == null)
				{
					isOpen = false;
				}
				else
				{
					isOpen = this.port.IsOpen;
				}
				flag = isOpen;
			}
			return flag;
		}

		private bool IsSuccess(string input)
		{
			return input.IndexOf("\r\nOK\r\n") >= 0;
		}

		/// <summary>
		/// AT+CMGL. Reads SMS messages from the current read/delete storage using the PDU mode.
		/// </summary>
		/// <param name="status">The message status</param>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.ShortMessage" /> objects representing the messages read.</returns>
		/// <remarks>Always switches to PDU mode at the beginning.</remarks>
		public ShortMessageFromPhone[] ListMessages(PhoneMessageStatus status)
		{
			ShortMessageFromPhone[] shortMessageFromPhoneArray;
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				this.LogIt(LogLevel.Info, string.Concat("Reading messages, requesting status \"", status.ToString(), "\"..."));
				string str = string.Concat("AT+CMGL=", (int)status);
				string str1 = this.ExecAndReceiveMultiple(str);
				ArrayList arrayLists = new ArrayList();
				Regex regex = new Regex("\\+CMGL: (\\d+),(\\d+),(?:\"(\\w*)\")?,(\\d+)\\r\\n(\\w+)");
				for (Match i = regex.Match(str1); i.Success; i = i.NextMatch())
				{
					int num = int.Parse(i.Groups[1].Value);
					int num1 = int.Parse(i.Groups[2].Value);
					string value = i.Groups[3].Value;
					int num2 = int.Parse(i.Groups[4].Value);
					string value1 = i.Groups[5].Value;
					string[] strArrays = new string[8];
					strArrays[0] = "index=";
					strArrays[1] = num.ToString();
					strArrays[2] = ", stat=";
					strArrays[3] = num1.ToString();
					strArrays[4] = ", alpha=\"";
					strArrays[5] = value;
					strArrays[6] = "\", length=";
					strArrays[7] = num2.ToString();
					this.LogIt(LogLevel.Info, string.Concat(strArrays));
					ShortMessageFromPhone shortMessageFromPhone = new ShortMessageFromPhone(num, num1, value, num2, value1);
					arrayLists.Add(shortMessageFromPhone);
				}
				int count = arrayLists.Count;
				this.LogIt(LogLevel.Info, string.Concat(count.ToString(), " message(s) read."));
				ShortMessageFromPhone[] shortMessageFromPhoneArray1 = new ShortMessageFromPhone[arrayLists.Count];
				arrayLists.CopyTo(shortMessageFromPhoneArray1, 0);
				shortMessageFromPhoneArray = shortMessageFromPhoneArray1;
			}
			return shortMessageFromPhoneArray;
		}

		/// <summary>
		/// Lists the network operators detected by the phone.
		/// </summary>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.OperatorInfo2" /> objects containing the data of each operator.</returns>
		/// <remarks>If you want to determine the current operator, use the <see cref="M:GsmComm.GsmCommunication.GsmPhone.GetCurrentOperator" /> method.</remarks>
		public OperatorInfo2[] ListOperators()
		{
			OperatorInfo2[] operatorInfo2Array;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Listing operators...");
				string str = this.ExecAndReceiveMultiple("AT+COPS=?");
				ArrayList arrayLists = new ArrayList();
				if (Regex.IsMatch(str, "\\+COPS: .*"))
				{
					Regex regex = new Regex("\\((\\d+),(?:\"([^\\(\\)\\,]+)\")?,(?:\"([^\\(\\)\\,]+)\")?,(?:\"(\\d+)\")?(?:,([^\\(\\)\\,]+))?\\)");
					Match match = regex.Match(str);
					while (match.Success)
					{
						int num = int.Parse(match.Groups[1].Value);
						string value = match.Groups[2].Value;
						string value1 = match.Groups[3].Value;
						string str1 = match.Groups[4].Value;
						string value2 = match.Groups[5].Value;
						object[] objArray = new object[5];
						objArray[0] = num;
						objArray[1] = value;
						objArray[2] = value1;
						objArray[3] = str1;
						objArray[4] = value2;
						this.LogIt(LogLevel.Info, "stat={0}, longAlpha=\"{1}\", shortAlpha=\"{2}\", numeric=\"{3}\", act=\"{4}\"", objArray);
						if (Enum.IsDefined(typeof(OperatorStatus), num))
						{
							OperatorStatus operatorStatu = (OperatorStatus)Enum.Parse(typeof(OperatorStatus), num.ToString());
							OperatorInfo2 operatorInfo2 = new OperatorInfo2(operatorStatu, value, value1, str1, value2);
							arrayLists.Add(operatorInfo2);
							match = match.NextMatch();
						}
						else
						{
							throw new CommException(string.Concat("Unknown operator status ", num.ToString(), "."), str);
						}
					}
					int count = arrayLists.Count;
					this.LogIt(LogLevel.Info, string.Concat(count.ToString(), " operator(s) enumerated."));
					OperatorInfo2[] operatorInfo2Array1 = new OperatorInfo2[arrayLists.Count];
					arrayLists.CopyTo(operatorInfo2Array1, 0);
					operatorInfo2Array = operatorInfo2Array1;
				}
				else
				{
					this.HandleCommError(str);
					operatorInfo2Array = null;
				}
			}
			return operatorInfo2Array;
		}

		private void LogIt(LogLevel level, string text)
		{
			if (this.LoglineAdded != null && this.logEnabled && level <= this.logLevel)
			{
				DateTime now = DateTime.Now;
				string str = string.Format("{0} [gsmphone] {1}", now.ToString("HH:mm:ss.fff"), text);
				LoglineAddedEventArgs loglineAddedEventArg = new LoglineAddedEventArgs(level, str);
				lock (this.logQueue)
				{
					this.logQueue.Enqueue(loglineAddedEventArg);
				}
			}
		}

		private void LogIt(LogLevel level, string text, params object[] args)
		{
			if (this.LoglineAdded != null && this.logEnabled && level <= this.logLevel)
			{
				string str = string.Format(text, args);
				DateTime now = DateTime.Now;
				string str1 = string.Format("{0} [gsmphone] {1}", now.ToString("HH:mm:ss.fff"), str);
				LoglineAddedEventArgs loglineAddedEventArg = new LoglineAddedEventArgs(level, str1);
				lock (this.logQueue)
				{
					this.logQueue.Enqueue(loglineAddedEventArg);
				}
			}
		}

		private void LogItShow(LogLevel level, string data, string prefix, bool hideEmptyLines)
		{
			string[] strArrays = this.SplitLines(data);
			if ((int)strArrays.Length != 0)
			{
				bool flag = false;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					if (!hideEmptyLines || hideEmptyLines && strArrays[i].Length > 0)
					{
						if (flag)
						{
							this.LogIt(level, strArrays[i].PadLeft(strArrays[i].Length + prefix.Length, ' '));
						}
						else
						{
							this.LogIt(level, string.Concat(prefix, strArrays[i]));
							flag = true;
						}
					}
				}
				return;
			}
			else
			{
				return;
			}
		}

		private void LogItShow(LogLevel level, string data, string prefix)
		{
			this.LogItShow(level, data, prefix, false);
		}

		private void LogMemoryStatus(MemoryStatus status)
		{
			int used;
			if (status.Total > 0)
			{
				used = (int)((double)status.Used / (double)status.Total * 100);
			}
			else
			{
				used = 0;
			}
			int num = used;
			this.LogIt(LogLevel.Info, string.Format("Memory status: {0}/{1} ({2}% used)", status.Used, status.Total, num));
		}

		private void LogThread()
		{
			this.logQueue.Clear();
			this.logThreadInitialized.Set();
			while (!this.terminateLogThread.WaitOne(100, false))
			{
				while (this.DispatchLog())
				{
				}
			}
			while (this.DispatchLog())
			{
			}
			this.logQueue.Clear();
		}

		private string MakeArrayString(string[] array)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] strArrays = array;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(str);
			}
			return stringBuilder.ToString();
		}

		private string MakeQueueString(Queue queue)
		{
			StringBuilder stringBuilder = new StringBuilder();
			lock (queue.SyncRoot)
			{
				foreach (string str in queue)
				{
					stringBuilder.Append(str);
				}
			}
			return stringBuilder.ToString();
		}

		private PinStatus MapPinStatusStringToStatus(string s)
		{
			PinStatus pinStatu = PinStatus.Ready;
			Dictionary<string, PinStatus> strs = new Dictionary<string, PinStatus>();
			strs["READY"] = PinStatus.Ready;
			strs["SIM PIN"] = PinStatus.SimPin;
			strs["SIM PUK"] = PinStatus.SimPuk;
			strs["PH-SIM PIN"] = PinStatus.PhoneToSimPin;
			strs["PH-FSIM PIN"] = PinStatus.PhoneToFirstSimPin;
			strs["PH-FSIM PUK"] = PinStatus.PhoneToFirstSimPuk;
			strs["SIM PIN2"] = PinStatus.SimPin2;
			strs["SIM PUK2"] = PinStatus.SimPuk2;
			strs["PH-NET PIN"] = PinStatus.PhoneToNetworkPin;
			strs["PH-NET PUK"] = PinStatus.PhoneToNetworkPuk;
			strs["PH-NETSUB PIN"] = PinStatus.PhoneToNetworkSubsetPin;
			strs["PH-NETSUB PUK"] = PinStatus.PhoneToNetworkSubsetPuk;
			strs["PH-SP PIN"] = PinStatus.PhoneToServiceProviderPin;
			strs["PH-SP PUK"] = PinStatus.PhoneToServiceProviderPuk;
			strs["PH-CORP PIN"] = PinStatus.PhoneToCorporatePin;
			strs["PH-CORP PUK"] = PinStatus.PhoneToCorporatePuk;
			if (!strs.TryGetValue(s, out pinStatu))
			{
				throw new CommException(string.Concat("Unknown PIN status \"", s, "\"."));
			}
			else
			{
				return pinStatu;
			}
		}

		private void OnMessageReceived(IMessageIndicationObject obj)
		{
			if (this.MessageReceived == null)
			{
				this.LogIt(LogLevel.Info, "No event handlers for MessageReceived event, message is ignored.");
				return;
			}
			else
			{
				this.LogIt(LogLevel.Info, "Firing async MessageReceived event.");
				MessageReceivedEventArgs messageReceivedEventArg = new MessageReceivedEventArgs(obj);
				this.MessageReceived.BeginInvoke(this, messageReceivedEventArg, new AsyncCallback(this.AsyncCallback), null);
				return;
			}
		}

		private void OnPhoneConnected()
		{
			if (this.PhoneConnected != null)
			{
				this.LogIt(LogLevel.Info, "Firing async PhoneConnected event.");
				this.PhoneConnected.BeginInvoke(this, EventArgs.Empty, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		private void OnPhoneDisconnected()
		{
			if (this.PhoneDisconnected != null)
			{
				this.LogIt(LogLevel.Info, "Firing async PhoneDisconnected event.");
				this.PhoneDisconnected.BeginInvoke(this, EventArgs.Empty, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		private void OnReceiveComplete(int bytesReceived)
		{
			if (this.ReceiveComplete != null)
			{
				ProgressEventArgs progressEventArg = new ProgressEventArgs(bytesReceived);
				this.ReceiveComplete.BeginInvoke(this, progressEventArg, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		private void OnReceiveProgress(int bytesReceived)
		{
			if (this.ReceiveProgress != null)
			{
				ProgressEventArgs progressEventArg = new ProgressEventArgs(bytesReceived);
				this.ReceiveProgress.BeginInvoke(this, progressEventArg, new AsyncCallback(this.AsyncCallback), null);
			}
		}

		/// <summary>
		/// Opens the connection to the device.
		/// </summary>
		/// <remarks>You can check the current connection state with the <see cref="M:GsmComm.GsmCommunication.GsmPhone.IsOpen" /> method.
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.IsOpen" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.Close" />
		/// </remarks>
		/// <exception cref="T:System.InvalidOperationException">Connection to device already open.</exception>
		/// <exception cref="T:GsmComm.GsmCommunication.CommException">Unable to open the port.</exception>
		public void Open()
		{
			lock (this)
			{
				if (!this.IsOpen())
				{
					this.CreateLogThread();
					try
					{
						this.OpenPort(this.portName, this.baudRate, this.timeout);
						this.LogIt(LogLevel.Info, string.Concat("Port ", this.portName.ToString(), " now open."));
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string str = string.Concat("Unable to open port ", this.portName, ": ", exception.Message);
						this.LogIt(LogLevel.Error, str);
						this.TerminateLogThread();
						throw new CommException(str, exception);
					}
					this.CheckConnection();
					try
					{
						this.CreateCommThread();
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						this.ClosePort();
						this.TerminateLogThread();
						throw new CommException("Unable to create communication thread.", exception2);
					}
				}
				else
				{
					throw new InvalidOperationException("Port already open.");
				}
			}
		}

		private void OpenPort(string portName, int baudRate, int timeout)
		{
			this.LogIt(LogLevel.Info, "Initializing serial connection...");
			this.LogIt(LogLevel.Info, string.Concat("  Port = ", portName));
			this.LogIt(LogLevel.Info, string.Concat("  Baud rate = ", baudRate.ToString()));
			this.LogIt(LogLevel.Info, string.Concat("  Timeout = ", timeout.ToString()));
			if (this.port != null)
			{
				if (this.port.IsOpen)
				{
					throw new CommException("Port already open.");
				}
			}
			else
			{
				SerialPortFixer.Execute(portName);
				this.port = new SerialPort();
			}
			try
			{
				this.port.PortName = portName;
				this.port.BaudRate = baudRate;
				this.port.DataBits = 8;
				this.port.StopBits = StopBits.One;
				this.port.Parity = Parity.None;
				this.port.ReadTimeout = timeout;
				this.port.WriteTimeout = timeout;
				this.port.Encoding = Encoding.GetEncoding(1252);
				this.port.DataReceived += new SerialDataReceivedEventHandler(this.port_DataReceived);
				this.port.Open();
				this.port.DtrEnable = true;
				this.port.RtsEnable = true;
			}
			catch (Exception exception)
			{
				if (this.port.IsOpen)
				{
					this.port.Close();
				}
				throw;
			}
		}

		/// <summary>
		/// Parses the memory status response of the CPMS command.
		/// </summary>
		/// <param name="input">A response to the +CPMS set command</param>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MessageMemoryStatus" /> object containing the status information of the storages.</returns>
		/// <remarks>
		/// <para>Data for the ReceiveStorage (mem3) or for the WriteStorage (mem2) is null if there is no information available about it.</para>
		/// </remarks>
		private MessageMemoryStatus ParseMessageMemoryStatus(string input)
		{
			MessageMemoryStatus messageMemoryStatu;
			Regex regex = new Regex("\\+CPMS: (\\d+),(\\d+)(?:,(\\d+),(\\d+))?(?:,(\\d+),(\\d+))?");
			Match match = regex.Match(input);
			if (!match.Success)
			{
				messageMemoryStatu = this.TryParseMessageMemoryStatus2(input);
				if (messageMemoryStatu == null)
				{
					this.HandleCommError(input);
					return null;
				}
			}
			else
			{
				messageMemoryStatu = new MessageMemoryStatus();
				int num = int.Parse(match.Groups[1].Value);
				int num1 = int.Parse(match.Groups[2].Value);
				messageMemoryStatu.ReadStorage = new MemoryStatus(num, num1);
				if (match.Groups[3].Captures.Count > 0 && match.Groups[4].Captures.Count > 0)
				{
					int num2 = int.Parse(match.Groups[3].Value);
					int num3 = int.Parse(match.Groups[4].Value);
					messageMemoryStatu.WriteStorage = new MemoryStatus(num2, num3);
				}
				if (match.Groups[5].Captures.Count > 0 && match.Groups[6].Captures.Count > 0)
				{
					int num4 = int.Parse(match.Groups[5].Value);
					int num5 = int.Parse(match.Groups[6].Value);
					messageMemoryStatu.ReceiveStorage = new MemoryStatus(num4, num5);
				}
			}
			return messageMemoryStatu;
		}

		/// <summary>
		/// Parses the memory status response of the CPBS command.
		/// </summary>
		/// <param name="input">A response to the +CPBS set command</param>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MemoryStatus" /> object containing the memory details.</returns>
		private MemoryStatusWithStorage ParsePhonebookMemoryStatus(string input)
		{
			Regex regex = new Regex("\\+CPBS: \"(\\w+)\",(\\d+),(\\d+)");
			Match match = regex.Match(input);
			if (!match.Success)
			{
				this.HandleCommError(input);
				return null;
			}
			else
			{
				string value = match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				int num1 = int.Parse(match.Groups[3].Value);
				MemoryStatusWithStorage memoryStatusWithStorage = new MemoryStatusWithStorage(value, num, num1);
				return memoryStatusWithStorage;
			}
		}

		private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			if (e.EventType == SerialData.Chars)
			{
				this.receiveNow.Set();
			}
		}

		/// <summary>
		/// AT+CMGR. Reads a single SMS message from the current read/delete storage using the PDU mode.
		/// </summary>
		/// <param name="index">The index of the message to read.</param>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.ShortMessage" /> object containing the message at the index specified.</returns>
		/// <remarks>Always switches to PDU mode at the beginning.</remarks>
		public ShortMessageFromPhone ReadMessage(int index)
		{
			ShortMessageFromPhone shortMessageFromPhone;
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				this.LogIt(LogLevel.Info, string.Concat("Reading message from index ", index.ToString(), "..."));
				string str = this.ExecAndReceiveMultiple(string.Concat("AT+CMGR=", index.ToString()));
				Regex regex = new Regex("\\+CMGR: (\\d+),(?:\"(\\w*)\")?,(\\d+)\\r\\n(\\w+)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					int num = int.Parse(match.Groups[1].Value);
					string value = match.Groups[2].Value;
					int num1 = int.Parse(match.Groups[3].Value);
					string value1 = match.Groups[4].Value;
					string[] strArrays = new string[6];
					strArrays[0] = "stat=";
					strArrays[1] = num.ToString();
					strArrays[2] = ", alpha=\"";
					strArrays[3] = value;
					strArrays[4] = "\", length=";
					strArrays[5] = num1.ToString();
					this.LogIt(LogLevel.Info, string.Concat(strArrays));
					ShortMessageFromPhone shortMessageFromPhone1 = new ShortMessageFromPhone(index, num, value, num1, value1);
					shortMessageFromPhone = shortMessageFromPhone1;
				}
			}
			return shortMessageFromPhone;
		}

		/// <summary>
		/// Gets the entire phonebook of the currently selected phonebook storage.
		/// </summary>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.PhonebookEntry" /> objects.</returns>
		public PhonebookEntry[] ReadPhonebookEntries()
		{
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			int num3 = 0;
			PhonebookEntry[] phonebookEntryArray;
			lock (this)
			{
				this.GetPhonebookSize(out num, out num1, out num2, out num3);
				phonebookEntryArray = this.ReadPhonebookEntries(num, num1);
			}
			return phonebookEntryArray;
		}

		/// <summary>
		/// AT+CPBR. Gets the specified range of phonebook entries.
		/// </summary>
		/// <param name="lowerBound">The first entry to get</param>
		/// <param name="upperBound">The last entry to get</param>
		/// <returns>An array of <see cref="T:GsmComm.GsmCommunication.PhonebookEntry" /> objects</returns>
		public PhonebookEntry[] ReadPhonebookEntries(int lowerBound, int upperBound)
		{
			PhonebookEntry[] phonebookEntryArray;
			lock (this)
			{
				this.VerifyValidConnection();
				object[] objArray = new object[5];
				objArray[0] = "Getting phonebook from index ";
				objArray[1] = lowerBound;
				objArray[2] = " to ";
				objArray[3] = upperBound;
				objArray[4] = "...";
				this.LogIt(LogLevel.Info, string.Concat(objArray));
				string str = string.Concat("AT+CPBR=", lowerBound.ToString(), ",", upperBound.ToString());
				string str1 = this.ExecAndReceiveMultiple(str);
				phonebookEntryArray = this.DecodePhonebookStream(str1, "+CPBR: ");
			}
			return phonebookEntryArray;
		}

		/// <summary>
		/// Receives raw data as a string.
		/// </summary>
		/// <param name="input">Receives the data received.</param>
		/// <returns>true if data was received, otherwise false.</returns>
		private bool ReceiveInternal(out string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (string i = this.port.ReadExisting(); i.Length > 0; i = this.port.ReadExisting())
			{
				stringBuilder.Append(i);
			}
			input = stringBuilder.ToString();
			return input.Length > 0;
		}

		/// <summary>
		/// Disables access to the protocol level of the current connection.
		/// </summary>
		/// <remarks>This method must be called as soon as the execution of the custom commands initiated
		/// by <see cref="M:GsmComm.GsmCommunication.GsmPhone.GetProtocol" /> is completed and allows for normal operations to continue.</remarks>
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.GetProtocol" />
		public void ReleaseProtocol()
		{
			Monitor.Exit(this);
		}

		/// <summary>
		/// AT+CGMI. Requests manufacturer identification.
		/// </summary>
		/// <returns>The product manufacturer identification.</returns>
		public string RequestManufacturer()
		{
			string str;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Requesting manufacturer...");
				string str1 = this.ExecAndReceiveMultiple("AT+CGMI");
				string str2 = this.TrimLineBreaks(str1);
				this.LogIt(LogLevel.Info, string.Concat("Manufacturer = \"", str2, "\""));
				str = str2;
			}
			return str;
		}

		/// <summary>
		/// AT+CGMM. Requests model identification.
		/// </summary>
		/// <returns>The product model identification.</returns>
		public string RequestModel()
		{
			string str;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Requesting model...");
				string str1 = this.ExecAndReceiveMultiple("AT+CGMM");
				string str2 = this.TrimLineBreaks(str1);
				this.LogIt(LogLevel.Info, string.Concat("Model = \"", str2, "\""));
				str = str2;
			}
			return str;
		}

		/// <summary>
		/// AT+CGMR. Requests revision identification.
		/// </summary>
		/// <returns>The product revision identification.</returns>
		public string RequestRevision()
		{
			string str;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Requesting revision...");
				string str1 = this.ExecAndReceiveMultiple("AT+CGMR");
				string str2 = this.TrimLineBreaks(str1);
				this.LogIt(LogLevel.Info, string.Concat("Revision = \"", str2, "\""));
				str = str2;
			}
			return str;
		}

		/// <summary>
		/// AT+CGSN. Requests serial number identification.
		/// </summary>
		/// <returns>The product serial number.</returns>
		public string RequestSerialNumber()
		{
			string str;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Requesting serial number...");
				string str1 = this.ExecAndReceiveMultiple("AT+CGSN");
				string str2 = this.TrimLineBreaks(str1);
				this.LogIt(LogLevel.Info, string.Concat("Serial Number = \"", str2, "\""));
				str = str2;
			}
			return str;
		}

		/// <summary>
		/// ATZ. Settings that are not stored in a profile will be reset to their factory defaults.
		/// </summary>
		public void ResetToDefaultConfig()
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Resetting to default configuration...");
				this.ExecAndReceiveMultiple("ATZ");
			}
		}

		/// <summary>
		/// AT+CSCS. Selects the character set.
		/// </summary>
		/// <param name="charset">The character set to use.</param>
		/// <remarks>This command informs the data card of which character set is used by the TE. The data card is
		/// then able to convert character strings correctly between TE and ME character sets. When the data card-TE
		/// interface is set to 8-bit operation and the TE uses a 7-bit alphabet, the highest bit shall be set to
		/// zero. This setting affects text mode SMS data and alpha fields in the phone book memory. If the ME is
		/// using the GSM default alphabet, its characters shall be padded with the 8th bit (zero) before
		/// converting them to hexadecimal numbers (that is, a 7-bit alphabet is not packed in the SMS-style
		/// packing).
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.GetCurrentCharacterSet" />
		/// <seealso cref="M:GsmComm.GsmCommunication.GsmPhone.GetSupportedCharacterSets" />
		/// </remarks>
		public void SelectCharacterSet(string charset)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Selecting character set \"", charset, "\"..."));
				this.ExecAndReceiveMultiple(string.Concat("AT+CSCS=\"", charset, "\""));
			}
		}

		/// <summary>
		/// AT+CSMS. Selects the specified messaging service.
		/// </summary>
		/// <param name="service">The service to select. Specifies the compatibility level of the SMS AT commands.
		/// The requirement of service setting 1 depends on specific commands.
		/// </param>
		/// <param name="mt">ME supports mobile terminated messages</param>
		/// <param name="mo">ME supports mobile originated messages</param>
		/// <param name="bm">ME supports broadcast type messages</param>
		public void SelectMessageService(int service, out int mt, out int mo, out int bm)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				string str = this.ExecAndReceiveMultiple(string.Concat("AT+CSMS=", service.ToString()));
				Regex regex = new Regex("\\+CSMS: (\\d+),(\\d+),(\\d+)");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.HandleCommError(str);
					throw new CommException("Unexpected response.", str);
				}
				else
				{
					mt = int.Parse(match.Groups[1].Value);
					mo = int.Parse(match.Groups[2].Value);
					bm = int.Parse(match.Groups[3].Value);
				}
			}
		}

		/// <summary>
		/// AT+CPBS. Selects the storage to use for phonebook operations.
		/// </summary>
		/// <param name="storage">The storage to use.</param>
		/// <returns>The memory status of the selected storage.</returns>
		public void SelectPhonebookStorage(string storage)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Selecting \"", storage, "\" phonebook memory..."));
				this.ExecAndReceiveMultiple(string.Concat("AT+CPBS=\"", storage, "\""));
			}
		}

		/// <summary>
		/// AT+CPMS. Selects the storage to use for read and delete operations.
		/// </summary>
		/// <param name="storage">The storage to use</param>
		/// <returns>The memory status of the selected storage.</returns>
		/// <remarks>This selects the preferred message storage "mem1" on the device.</remarks>
		public MemoryStatus SelectReadStorage(string storage)
		{
			MemoryStatus memoryStatu;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Selecting \"", storage, "\" as read storage..."));
				string str = this.ExecAndReceiveMultiple(string.Concat("AT+CPMS=\"", storage, "\""));
				MessageMemoryStatus messageMemoryStatu = this.ParseMessageMemoryStatus(str);
				MemoryStatus readStorage = messageMemoryStatu.ReadStorage;
				this.LogMemoryStatus(readStorage);
				memoryStatu = readStorage;
			}
			return memoryStatu;
		}

		/// <summary>
		/// AT+CPMS. Selects the storage to use for write and send operations.
		/// </summary>
		/// <param name="storage">The storage to use</param>
		/// <returns>The memory status of the selected storage</returns>
		/// <remarks>This selects the preferred message storage "mem2" on the device. Additionaly, the "mem1" storage
		/// is also set to the same storage since the read storage must also be set when selecting the 
		/// write storage.</remarks>
		public MemoryStatus SelectWriteStorage(string storage)
		{
			MemoryStatus memoryStatu;
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, string.Concat("Selecting \"", storage, "\" as write storage..."));
				string str = storage;
				string str1 = storage;
				string[] strArrays = new string[5];
				strArrays[0] = "AT+CPMS=\"";
				strArrays[1] = str;
				strArrays[2] = "\",\"";
				strArrays[3] = str1;
				strArrays[4] = "\"";
				string str2 = string.Concat(strArrays);
				string str3 = this.ExecAndReceiveMultiple(str2);
				MessageMemoryStatus messageMemoryStatu = this.ParseMessageMemoryStatus(str3);
				MemoryStatus writeStorage = messageMemoryStatu.WriteStorage;
				this.LogMemoryStatus(writeStorage);
				memoryStatu = writeStorage;
			}
			return memoryStatu;
		}

		/// <summary>
		/// Sends raw data as a string.
		/// </summary>
		/// <param name="output">The data to send.</param>
		/// <param name="logIt">Specifies if the data sent should be logged.</param>
		/// <remarks>Send and receive buffers are cleared before sending.</remarks>
		private void SendInternal(string output, bool logIt)
		{
			if (logIt)
			{
				this.LogItShow(LogLevel.Verbose, output, "<< ");
			}
			this.port.DiscardOutBuffer();
			this.port.DiscardInBuffer();
			this.port.Write(output);
		}

		/// <summary>
		/// AT+CMGS. Sends an SMS message using PDU mode.
		/// </summary>
		/// <param name="pdu">The PDU stream to send</param>
		/// <param name="actualLength">The actual length of the PDU (not counting the SMSC data)</param>
		/// <returns>The message reference</returns>
		/// <remarks>Always switches to PDU mode at the beginning.</remarks>
		public byte SendMessage(string pdu, int actualLength)
		{
			byte num;
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				this.LogIt(LogLevel.Info, "Sending message...");
				this.ExecAndReceiveAnything(string.Concat("AT+CMGS=", actualLength.ToString()), "\\r\\n> ");
				this.Send(string.Concat(pdu, (char)26));
				string str = this.ReceiveMultiple();
				byte num1 = 0;
				Regex regex = new Regex("\\+CMGS: (\\d+)(,(\\w+))*");
				Match match = regex.Match(str);
				if (!match.Success)
				{
					this.LogIt(LogLevel.Warning, string.Concat("Could not get message reference. Answer received was: \"", str, "\""));
				}
				else
				{
					num1 = (byte)int.Parse(match.Groups[1].Value);
					this.LogIt(LogLevel.Info, string.Concat("Message reference = ", num1.ToString()));
				}
				num = num1;
			}
			return num;
		}

		/// <summary>
		/// AT+CNMI. Selects the procedure for indicating new messages received from the network.
		/// </summary>
		/// <param name="settings">A <see cref="T:GsmComm.GsmCommunication.MessageIndicationSettings" /> structure containing the
		/// detailed settings.</param>
		/// <remarks>The function switches to the PDU mode before setting the notifications. This
		/// causes all short messages, that are directly routed, to be presented in PDU mode. If the mode
		/// is changed (such as a switch to the text mode), all indications (containing a message) following the
		/// change are sent in the new mode.
		/// </remarks>
		public void SetMessageIndications(MessageIndicationSettings settings)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				this.LogIt(LogLevel.Info, "Setting message notifications...");
				object[] mode = new object[5];
				mode[0] = settings.Mode;
				mode[1] = settings.DeliverStyle;
				mode[2] = settings.CellBroadcastStyle;
				mode[3] = settings.StatusReportStyle;
				mode[4] = settings.BufferSetting;
				string str = string.Format("AT+CNMI={0},{1},{2},{3},{4}", mode);
				this.ExecAndReceiveMultiple(str);
			}
		}

		/// <summary>
		/// AT+CMMS. Sets the SMS batch mode.
		/// </summary>
		/// <param name="mode">The new mode to set.</param>
		public void SetMoreMessagesToSend(MoreMessagesMode mode)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				object[] objArray = new object[1];
				objArray[0] = mode;
				this.LogIt(LogLevel.Info, "Setting more messages mode {0}...", objArray);
				string str = string.Format("AT+CMMS={0}", (int)mode);
				this.ExecAndReceiveMultiple(str);
			}
		}

		private void SetNewConnectionState(bool newState)
		{
			if (newState != this.connectionState)
			{
				this.connectionState = newState;
				if (!this.connectionState)
				{
					this.LogIt(LogLevel.Info, "Phone disconnected.");
					this.OnPhoneDisconnected();
				}
				else
				{
					this.LogIt(LogLevel.Info, "Phone connected.");
					this.OnPhoneConnected();
					return;
				}
			}
		}

		/// <summary>
		/// AT+CSCA. Sets the SMS Service Center Address.
		/// </summary>
		/// <param name="data">An <see cref="T:GsmComm.GsmCommunication.AddressData" /> object containing the new address</param>
		/// <remarks>This command changes the SMSC address, through which SMS messages are transmitted.
		/// In text mode, this setting is used by SMS sending and SMS writing commands. In PDU mode, this setting is
		/// used by the same commands, but only when the length of the SMSC address coded into the PDU data equals
		/// zero.</remarks>
		public void SetSmscAddress(AddressData data)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				object[] address = new object[2];
				address[0] = data.Address;
				int typeOfAddress = data.TypeOfAddress;
				address[1] = typeOfAddress.ToString();
				this.LogIt(LogLevel.Info, "Setting SMSC address to \"{0}\", type {1}...", address);
				int num = data.TypeOfAddress;
				string str = string.Format("AT+CSCA=\"{0}\",{1}", data.Address, num.ToString());
				this.ExecAndReceiveMultiple(str);
			}
		}

		/// <summary>
		/// AT+CSCA. Sets the SMS Service Center Address.
		/// </summary>
		/// <param name="address">The new SMSC address</param>
		/// <remarks>This command changes the SMSC address, through which SMS messages are transmitted.
		/// In text mode, setting is used by send and write commands. In PDU mode, setting is used by the same
		/// commands, but only when the length of the SMSC address coded into the PDU data equals zero.</remarks>
		public void SetSmscAddress(string address)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				object[] objArray = new object[1];
				objArray[0] = address;
				this.LogIt(LogLevel.Info, "Setting SMSC address to \"{0}\"...", objArray);
				string str = string.Format("AT+CSCA=\"{0}\"", address);
				this.ExecAndReceiveMultiple(str);
			}
		}

		private string[] SplitLines(string data)
		{
			char chr = '\r';
			char chr1 = '\n';
			char chr2 = '\r';
			data = data.Replace(string.Concat(chr.ToString(), chr1.ToString()), chr2.ToString());
			char[] chrArray = new char[1];
			chrArray[0] = '\r';
			return data.Split(chrArray);
		}

		private void TerminateCommThread()
		{
			if (this.IsCommThreadRunning())
			{
				this.terminateCommThread.Set();
				try
				{
					if (!this.commThread.Join(10000))
					{
						this.LogIt(LogLevel.Warning, "Communication thread did not exit within the timeout, aborting thread.");
						this.commThread.Abort();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.LogIt(LogLevel.Error, string.Concat("Error while terminating comm thread: ", exception.Message));
				}
				this.commThread = null;
			}
		}

		private void TerminateLogThread()
		{
			if (this.logThread != null && this.logThread.IsAlive)
			{
				this.LogIt(LogLevel.Info, "Log thread is terminating.");
				this.terminateLogThread.Set();
				try
				{
					if (!this.logThread.Join(10000))
					{
						this.logThread.Abort();
					}
				}
				catch (Exception exception)
				{
				}
				this.logThread = null;
			}
		}

		/// <summary>
		/// Removes all leading and trailing line termination characters from a string.
		/// </summary>
		/// <param name="input">The string to trim.</param>
		/// <returns>The modified string.</returns>
		private string TrimLineBreaks(string input)
		{
			char[] chrArray = new char[2];
			chrArray[0] = '\r';
			chrArray[1] = '\n';
			return input.Trim(chrArray);
		}

		/// <summary>
		/// Tries to parse an alternative memory status response of the CPMS command.
		/// </summary>
		/// <param name="input">A response to the +CPMS set command</param>
		/// <returns>A <see cref="T:GsmComm.GsmCommunication.MessageMemoryStatus" /> object containing the status information of the storages
		/// if successful, otherwise null.</returns>
		private MessageMemoryStatus TryParseMessageMemoryStatus2(string input)
		{
			MessageMemoryStatus messageMemoryStatu = null;
			Regex regex = new Regex("\\+CPMS: \"(\\w+)\",(\\d+),(\\d+),\"(\\w+)\",(\\d+),(\\d+)(?:,\"(\\w+)\",(\\d+),(\\d+))?");
			Match match = regex.Match(input);
			if (match.Success)
			{
				messageMemoryStatu = new MessageMemoryStatus();
				string value = match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				int num1 = int.Parse(match.Groups[3].Value);
				string str = match.Groups[4].Value;
				int num2 = int.Parse(match.Groups[5].Value);
				int num3 = int.Parse(match.Groups[6].Value);
				messageMemoryStatu.ReadStorage = new MemoryStatusWithStorage(value, num, num1);
				messageMemoryStatu.WriteStorage = new MemoryStatusWithStorage(str, num2, num3);
				if (match.Groups[7].Captures.Count > 0)
				{
					string value1 = match.Groups[7].Value;
					int num4 = int.Parse(match.Groups[8].Value);
					int num5 = int.Parse(match.Groups[9].Value);
					messageMemoryStatu.ReceiveStorage = new MemoryStatusWithStorage(value1, num4, num5);
				}
			}
			return messageMemoryStatu;
		}

		private void VerifyValidConnection()
		{
			if (this.IsOpen())
			{
				if (this.connectionState)
				{
					return;
				}
				else
				{
					this.LogIt(LogLevel.Error, "No phone connected.");
					throw new CommException("No phone connected.");
				}
			}
			else
			{
				this.LogIt(LogLevel.Error, "Port not open.");
				throw new InvalidOperationException("Port not open.");
			}
		}

		/// <summary>
		/// AT+CMGW. Stores an SMS message in the current write/send storage using PDU mode.
		/// </summary>
		/// <param name="pdu">The message in PDU format</param>
		/// <param name="actualLength">The actual length of the PDU (not counting the SMSC data)</param>
		/// <param name="status">The status that the message should get when saved.</param>
		/// <returns>The index of the message. If the index could not be retrieved, zero is returned.</returns>
		/// <remarks><para>The message is saved with a status predefined by the phone</para>
		/// <para>This function always switches to the PDU mode at the beginning.</para></remarks>
		public int WriteMessageToMemory(string pdu, int actualLength, int status)
		{
			int memoryPart2;
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				this.LogIt(LogLevel.Info, "Writing message to memory...");
				this.ExecAndReceiveAnything(string.Concat("AT+CMGW=", actualLength.ToString(), ",", status.ToString()), "\\r\\n> ");
				memoryPart2 = this.WriteMessageToMemoryPart2(pdu);
			}
			return memoryPart2;
		}

		/// <summary>
		/// AT+CMGW. Stores an SMS message in the current write/send storage using PDU mode.
		/// </summary>
		/// <param name="pdu">The message in PDU format</param>
		/// <param name="actualLength">The actual length of the PDU (not counting the SMSC data)</param>
		/// <returns>The index of the message. If the index could not be retrieved, zero is returned.</returns>
		/// <remarks><para>The message is saved with a status predefined by the phone</para>
		/// <para>This function always switches to the PDU mode at the beginning.</para></remarks>
		public int WriteMessageToMemory(string pdu, int actualLength)
		{
			int memoryPart2;
			lock (this)
			{
				this.VerifyValidConnection();
				this.ActivatePduMode();
				this.LogIt(LogLevel.Info, "Writing message to memory...");
				this.ExecAndReceiveAnything(string.Concat("AT+CMGW=", actualLength.ToString()), "\\r\\n> ");
				memoryPart2 = this.WriteMessageToMemoryPart2(pdu);
			}
			return memoryPart2;
		}

		private int WriteMessageToMemoryPart2(string pdu)
		{
			this.Send(string.Concat(pdu, (char)26));
			string str = this.ReceiveMultiple();
			int num = 0;
			Regex regex = new Regex("\\+CMGW: (\\d+)");
			Match match = regex.Match(str);
			if (!match.Success)
			{
				this.LogIt(LogLevel.Warning, string.Concat("Could not get message index. Answer received was: \"", str, "\""));
			}
			else
			{
				num = int.Parse(match.Groups[1].Value);
				this.LogIt(LogLevel.Info, string.Concat("Message index = ", num.ToString()));
			}
			return num;
		}

		/// <summary>
		/// AT+CPBW. Creates a new phonebook entry.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <remarks>The <see cref="P:GsmComm.GsmCommunication.PhonebookEntry.Index" /> property of the entry is ignored, 
		/// the entry is always saved in the first free location. All other properties must be set
		/// correctly.</remarks>
		public void WritePhonebookEntry(PhonebookEntry entry)
		{
			lock (this)
			{
				this.VerifyValidConnection();
				this.LogIt(LogLevel.Info, "Writing phonebook entry...");
				object[] number = new object[7];
				number[0] = "AT+CPBW=,\"";
				number[1] = entry.Number;
				number[2] = "\",";
				number[3] = entry.Type;
				number[4] = ",\"";
				number[5] = entry.Text;
				number[6] = "\"";
				string str = string.Concat(number);
				this.ExecAndReceiveMultiple(str);
			}
		}

		/// <summary>
		/// The event that occurs when a new line was added to the log.
		/// </summary>
		public event LoglineAddedEventHandler LoglineAdded;

		/// <summary>
		/// The event that occurs when a new SMS message was received.
		/// </summary>
		public event MessageReceivedEventHandler MessageReceived;

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
	}
}