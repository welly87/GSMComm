using System;

/// <summary>
/// Provides data for the <see cref="E:GsmComm.GsmCommunication.GsmPhone.ReceiveProgress" /> and <see cref="E:GsmComm.GsmCommunication.GsmPhone.ReceiveComplete" /> events.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class ProgressEventArgs : EventArgs
	{
		private int progress;

		/// <summary>
		/// Get the current progress value.
		/// </summary>
		public int Progress
		{
			get
			{
				return this.progress;
			}
		}

		/// <summary>
		/// Initializes a new instance of the event args.
		/// </summary>
		/// <param name="progress">The current progress value.</param>
		public ProgressEventArgs(int progress)
		{
			this.progress = progress;
		}
	}
}