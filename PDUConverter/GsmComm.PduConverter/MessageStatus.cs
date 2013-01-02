using System;

/// <summary>
/// TP-ST / TP-Status.
/// </summary>
namespace GsmComm.PduConverter
{
	public struct MessageStatus
	{
		private byte status;

		/// <summary>
		/// Gets the status category.
		/// </summary>
		/// <remarks>
		/// If the valus does not fall into one of the predefined categories, <see cref="F:GsmComm.PduConverter.StatusCategory.Reserved" />
		/// is returned.
		/// </remarks>
		public StatusCategory Category
		{
			get
			{
				if (this.status < 0 || this.status > 31)
				{
					if (this.status < 32 || this.status > 47)
					{
						if (this.status < 64 || this.status > 95)
						{
							if (this.status < 96 || this.status > 127)
							{
								return StatusCategory.Reserved;
							}
							else
							{
								return StatusCategory.TemporaryErrorNoRetry;
							}
						}
						else
						{
							return StatusCategory.PermanentError;
						}
					}
					else
					{
						return StatusCategory.TemporaryErrorWithRetry;
					}
				}
				else
				{
					return StatusCategory.Success;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.MessageStatus" />.
		/// </summary>
		/// <param name="status">The status code.</param>
		public MessageStatus(byte status)
		{
			this.status = status;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GsmComm.PduConverter.MessageStatus" />.
		/// </summary>
		/// <param name="status">One of the <see cref="T:GsmComm.PduConverter.KnownMessageStatus" /> values.</param>
		public MessageStatus(KnownMessageStatus status)
		{
			this.status = (byte)status;
		}

		/// <summary>
		/// Retrieves the known status of the current message status.
		/// </summary>
		/// <returns>Ae <see cref="T:GsmComm.PduConverter.KnownMessageStatus" /> representing the message status.</returns>
		/// <remarks>Check first with <see cref="M:GsmComm.PduConverter.MessageStatus.IsKnownStatus" /> before calling this method.</remarks>
		/// <exception cref="T:System.ArgumentException">Message status is not a known message status.</exception>
		public KnownMessageStatus GetKnownStatus()
		{
			if (!this.IsKnownStatus())
			{
				throw new ArgumentException(string.Concat(this.status.ToString(), " is not a known message status."));
			}
			else
			{
				return (KnownMessageStatus)Enum.Parse(typeof(KnownMessageStatus), this.status.ToString());
			}
		}

		/// <summary>
		/// Checks if the message status exists within the known status list.
		/// </summary>
		/// <returns>true if the status is a known status, otherwise false.</returns>
		public bool IsKnownStatus()
		{
			return Enum.IsDefined(typeof(KnownMessageStatus), this.status);
		}

		public static implicit operator Byte(MessageStatus s)
		{
			return s.ToByte();
		}

		public static implicit operator MessageStatus(byte b)
		{
			return new MessageStatus(b);
		}

		public static implicit operator MessageStatus(KnownMessageStatus s)
		{
			return new MessageStatus(s);
		}

		/// <summary>
		/// Returns the byte representation of the status.
		/// </summary>
		/// <returns>A <see cref="T:System.Byte" /> value representing the object's value.</returns>
		public byte ToByte()
		{
			return this.status;
		}

		/// <summary>
		/// Returns the string representation of the status.
		/// </summary>
		/// <returns>The string representation of the known status if it is a known status,
		/// the numerical status value otherwise.</returns>
		public override string ToString()
		{
			if (!this.IsKnownStatus())
			{
				return this.status.ToString();
			}
			else
			{
				return this.GetKnownStatus().ToString();
			}
		}
	}
}