using System;

/// <summary>
/// The method that handles the <see cref="E:GsmComm.GsmCommunication.GsmCommMain.LoglineAdded" /> event.
/// </summary>
/// <param name="sender">The origin of the event.</param>
/// <param name="e">The data for the event.</param>
namespace GsmComm.GsmCommunication
{
	public delegate void LoglineAddedEventHandler(object sender, LoglineAddedEventArgs e);
}