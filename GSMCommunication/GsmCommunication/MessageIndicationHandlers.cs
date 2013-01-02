using GsmComm.PduConverter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// This class contains the handlers for unsolicited messages sent by the phone. It is for use by the GsmPhone
/// class only and must not be made public.
/// </summary>
namespace GsmComm.GsmCommunication
{
	internal class MessageIndicationHandlers
	{
		private const string deliverMemoryIndication = "\\+CMTI: \"(\\w+)\",(\\d+)";

		private const string deliverMemoryIndicationStart = "\\+CMTI: ";

		private const string deliverPduModeIndication = "\\+CMT: (\\w*),(\\d+)\\r\\n(\\w+)";

		private const string deliverPduModeIndicationStart = "\\+CMT: ";

		private const string statusReportMemoryIndication = "\\+CDSI: \"(\\w+)\",(\\d+)";

		private const string statusReportMemoryIndicationStart = "\\+CDSI: ";

		private const string statusReportPduModeIndication = "\\+CDS: (\\d+)\\r\\n(\\w+)";

		private const string statusReportPduModeIndicationStart = "\\+CDS: ";

		private List<MessageIndicationHandlers.UnsoMessage> messages;

		public MessageIndicationHandlers()
		{
			this.messages = new List<MessageIndicationHandlers.UnsoMessage>();
			MessageIndicationHandlers.UnsoMessage unsoMessage = new MessageIndicationHandlers.UnsoMessage("\\+CMTI: \"(\\w+)\",(\\d+)", new MessageIndicationHandlers.UnsoHandler(this.HandleDeliverMemoryIndication));
			unsoMessage.StartPattern = "\\+CMTI: ";
			unsoMessage.Description = "New SMS-DELIVER received (indicated by memory location)";
			this.messages.Add(unsoMessage);
			MessageIndicationHandlers.UnsoMessage unsoCompleteChecker = new MessageIndicationHandlers.UnsoMessage("\\+CMT: (\\w*),(\\d+)\\r\\n(\\w+)", new MessageIndicationHandlers.UnsoHandler(this.HandleDeliverPduModeIndication));
			unsoCompleteChecker.StartPattern = "\\+CMT: ";
			unsoCompleteChecker.Description = "New SMS-DELIVER received (indicated by PDU mode version)";
			unsoCompleteChecker.CompleteChecker = new MessageIndicationHandlers.UnsoCompleteChecker(this.IsCompleteDeliverPduModeIndication);
			this.messages.Add(unsoCompleteChecker);
			MessageIndicationHandlers.UnsoMessage unsoMessage1 = new MessageIndicationHandlers.UnsoMessage("\\+CDSI: \"(\\w+)\",(\\d+)", new MessageIndicationHandlers.UnsoHandler(this.HandleStatusReportMemoryIndication));
			unsoMessage1.StartPattern = "\\+CDSI: ";
			unsoMessage1.Description = "New SMS-STATUS-REPORT received (indicated by memory location)";
			this.messages.Add(unsoMessage1);
			MessageIndicationHandlers.UnsoMessage unsoCompleteChecker1 = new MessageIndicationHandlers.UnsoMessage("\\+CDS: (\\d+)\\r\\n(\\w+)", new MessageIndicationHandlers.UnsoHandler(this.HandleStatusReportPduModeIndication));
			unsoCompleteChecker1.StartPattern = "\\+CDS: ";
			unsoCompleteChecker1.Description = "New SMS-STATUS-REPORT received (indicated by PDU mode version)";
			unsoCompleteChecker1.CompleteChecker = new MessageIndicationHandlers.UnsoCompleteChecker(this.IsCompleteStatusReportPduModeIndication);
			this.messages.Add(unsoCompleteChecker1);
		}

		private IMessageIndicationObject HandleDeliverMemoryIndication(ref string input)
		{
			Regex regex = new Regex("\\+CMTI: \"(\\w+)\",(\\d+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				MemoryLocation memoryLocation = new MemoryLocation(value, num);
				input = input.Remove(match.Index, match.Length);
				return memoryLocation;
			}
			else
			{
				throw new ArgumentException("Input string does not contain an SMS-DELIVER memory location indication.");
			}
		}

		private IMessageIndicationObject HandleDeliverPduModeIndication(ref string input)
		{
			Regex regex = new Regex("\\+CMT: (\\w*),(\\d+)\\r\\n(\\w+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				string str = match.Groups[3].Value;
				ShortMessage shortMessage = new ShortMessage(value, num, str);
				input = input.Remove(match.Index, match.Length);
				return shortMessage;
			}
			else
			{
				throw new ArgumentException("Input string does not contain an SMS-DELIVER PDU mode indication.");
			}
		}

		private IMessageIndicationObject HandleStatusReportMemoryIndication(ref string input)
		{
			Regex regex = new Regex("\\+CDSI: \"(\\w+)\",(\\d+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				MemoryLocation memoryLocation = new MemoryLocation(value, num);
				input = input.Remove(match.Index, match.Length);
				return memoryLocation;
			}
			else
			{
				throw new ArgumentException("Input string does not contain an SMS-STATUS-REPORT memory location indication.");
			}
		}

		private IMessageIndicationObject HandleStatusReportPduModeIndication(ref string input)
		{
			Regex regex = new Regex("\\+CDS: (\\d+)\\r\\n(\\w+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				int num = int.Parse(match.Groups[1].Value);
				string value = match.Groups[2].Value;
				ShortMessage shortMessage = new ShortMessage(string.Empty, num, value);
				input = input.Remove(match.Index, match.Length);
				return shortMessage;
			}
			else
			{
				throw new ArgumentException("Input string does not contain an SMS-STATUS-REPORT PDU mode indication.");
			}
		}

		/// <summary>
		/// Handles an unsolicited message of the specified input string.
		/// </summary>
		/// <param name="input">The input string to handle, the unsolicited message will be removed</param>
		/// <param name="description">Receives a textual description of the message, may be empty</param>
		/// <returns>The message indication object generated from the message</returns>
		/// <exception cref="T:System.ArgumentException">Input string does not match any of the supported
		/// unsolicited messages</exception>
		public IMessageIndicationObject HandleUnsolicitedMessage(ref string input, out string description)
		{
			IMessageIndicationObject messageIndicationObject;
			List<MessageIndicationHandlers.UnsoMessage>.Enumerator enumerator = this.messages.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MessageIndicationHandlers.UnsoMessage current = enumerator.Current;
					if (!current.IsMatch(input))
					{
						continue;
					}
					IMessageIndicationObject messageIndicationObject1 = current.Handler(ref input);
					description = current.Description;
					messageIndicationObject = messageIndicationObject1;
					return messageIndicationObject;
				}
				throw new ArgumentException("Input string does not match any of the supported unsolicited messages.");
			}
			finally
			{
				enumerator.Dispose();
			}
			return messageIndicationObject;
		}

		private bool IsCompleteDeliverPduModeIndication(string input)
		{
			Regex regex = new Regex("\\+CMT: (\\w*),(\\d+)\\r\\n(\\w+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				string value = match.Groups[3].Value;
				if (BcdWorker.CountBytes(value) <= 0)
				{
					return false;
				}
				else
				{
					int num1 = BcdWorker.GetByte(value, 0);
					int num2 = num * 2 + num1 * 2 + 2;
					return value.Length >= num2;
				}
			}
			else
			{
				return false;
			}
		}

		private bool IsCompleteStatusReportPduModeIndication(string input)
		{
			Regex regex = new Regex("\\+CDS: (\\d+)\\r\\n(\\w+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				int num = int.Parse(match.Groups[1].Value);
				string value = match.Groups[2].Value;
				if (BcdWorker.CountBytes(value) <= 0)
				{
					return false;
				}
				else
				{
					int num1 = BcdWorker.GetByte(value, 0);
					int num2 = num * 2 + num1 * 2 + 2;
					return value.Length >= num2;
				}
			}
			else
			{
				return false;
			}
		}

		public bool IsIncompleteUnsolicitedMessage(string input)
		{
			bool flag = false;
			foreach (MessageIndicationHandlers.UnsoMessage message in this.messages)
			{
				if (!message.IsStartMatch(input) || message.IsMatch(input))
				{
					continue;
				}
				flag = true;
				break;
			}
			return flag;
		}

		public bool IsUnsolicitedMessage(string input)
		{
			bool flag = false;
			foreach (MessageIndicationHandlers.UnsoMessage message in this.messages)
			{
				if (!message.IsMatch(input))
				{
					continue;
				}
				flag = true;
				break;
			}
			return flag;
		}

		private delegate bool UnsoCompleteChecker(string input);

		private delegate IMessageIndicationObject UnsoHandler(ref string input);

		private class UnsoMessage
		{
			private string pattern;

			private string startPattern;

			private string description;

			private MessageIndicationHandlers.UnsoHandler handler;

			private MessageIndicationHandlers.UnsoCompleteChecker completeChecker;

			public MessageIndicationHandlers.UnsoCompleteChecker CompleteChecker
			{
				get
				{
					return this.completeChecker;
				}
				set
				{
					this.completeChecker = value;
				}
			}

			public string Description
			{
				get
				{
					return this.description;
				}
				set
				{
					this.description = value;
				}
			}

			public MessageIndicationHandlers.UnsoHandler Handler
			{
				get
				{
					return this.handler;
				}
				set
				{
					this.handler = value;
				}
			}

			public string Pattern
			{
				get
				{
					return this.pattern;
				}
				set
				{
					this.pattern = value;
				}
			}

			public string StartPattern
			{
				get
				{
					return this.startPattern;
				}
				set
				{
					this.startPattern = value;
				}
			}

			public UnsoMessage(string pattern, MessageIndicationHandlers.UnsoHandler handler)
			{
				this.pattern = pattern;
				this.startPattern = pattern;
				this.description = string.Empty;
				this.handler = handler;
				this.completeChecker = null;
			}

			public bool IsMatch(string input)
			{
				bool flag;
				if (this.completeChecker == null)
				{
					flag = Regex.IsMatch(input, this.pattern);
				}
				else
				{
					flag = this.completeChecker(input);
				}
				return flag;
			}

			public bool IsStartMatch(string input)
			{
				return Regex.IsMatch(input, this.startPattern);
			}
		}
	}
}