/// <summary>
/// Provides data for the <see cref="E:GsmComm.GsmCommunication.GsmPhone.MessageReceived" /> events.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class MessageReceivedEventArgs
	{
		private IMessageIndicationObject indicationObject;

		/// <summary>
		/// The object that indicates a new received message.
		/// </summary>
		public IMessageIndicationObject IndicationObject
		{
			get
			{
				return this.indicationObject;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="obj">The object that indicates a new received message.</param>
		public MessageReceivedEventArgs(IMessageIndicationObject obj)
		{
			this.indicationObject = obj;
		}
	}
}