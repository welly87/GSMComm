using System;

/// <summary>
/// The method that handles the <see cref="E:GsmComm.Server.SmsSender.MessageSendStarting" /> and <see cref="E:GsmComm.Server.SmsSender.MessageSendComplete" />
/// events.
/// </summary>
/// <param name="sender">The origin of the event.</param>
/// <param name="e">The <see cref="T:GsmComm.Server.MessageSendEventArgs" /> associated with the event.</param>
namespace GsmComm.Server
{
	public delegate void MessageSendEventHandler(object sender, MessageSendEventArgs e);
}