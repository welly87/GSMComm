using System;

/// <summary>
/// The method that handles the <see cref="E:GsmComm.GsmCommunication.GsmPhone.ReceiveProgress" /> and the <see cref="E:GsmComm.GsmCommunication.GsmPhone.ReceiveComplete" />events.
/// </summary>
/// <param name="sender">The origin of the event.</param>
/// <param name="e">The arguments containing more information.</param>
namespace GsmComm.GsmCommunication
{
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
}