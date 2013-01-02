using System;

/// <summary>
/// The method that handles the <see cref="E:GsmComm.Server.SmsSender.MessageSendFailed" /> event.
/// </summary>
/// <param name="sender">The origin of the event.</param>
/// <param name="e">The <see cref="T:GsmComm.Server.MessageSendErrorEventArgs" /> associated with the event.</param>
namespace GsmComm.Server
{
	public delegate void MessageSendErrorEventHandler(object sender, MessageSendErrorEventArgs e);
}