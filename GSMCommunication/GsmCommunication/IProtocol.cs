using System;

/// <summary>
/// Provides an interface for low-level access to the device.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public interface IProtocol
	{
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
		string ExecAndReceiveAnything(string command, string pattern);

		/// <summary>Executes the specified command and reads multiple times from the phone
		/// until one of the defined message termination patterns is detected in the response.</summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>The response received.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveAnything(System.String,System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveMultiple" />
		string ExecAndReceiveMultiple(string command);

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
		string ExecCommand(string command);

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
		string ExecCommand(string command, string receiveErrorMessage);

		/// <summary>
		/// Receives raw string data.</summary>
		/// <param name="input">The data received.</param>
		/// <returns>true if reception was successful, otherwise false.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Send(System.String)" />
		bool Receive(out string input);

		/// <summary>Reads multiple times from the phone until a specific pattern is detected in the response.</summary>
		/// <returns>The response received.</returns>
		/// <param name="pattern">The regular expression pattern that the received data must match to
		/// stop reading. Can be an empty string if the pattern should not be checked.</param>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveAnything(System.String,System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveMultiple" />
		string ReceiveAnything(string pattern);

		/// <summary>Reads multiple times from the phone until one of the defined
		/// message termination patterns is detected in the response.</summary>
		/// <returns>The response received.</returns>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ExecAndReceiveMultiple(System.String)" />
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.ReceiveAnything(System.String)" />
		string ReceiveMultiple();

		/// <summary>Sends raw string data.</summary>
		/// <param name="output">The data to send.</param>
		/// <seealso cref="M:GsmComm.GsmCommunication.IProtocol.Receive(System.String@)" />
		void Send(string output);
	}
}