using System;

/// <summary>
/// This class and its contained classes contain all possible values for the
/// Protocol Identifier.
/// </summary>
/// <remarks>The members represent the TP-PID octet in the PDU.</remarks>
namespace GsmComm.PduConverter
{
	public class ProtocolID
	{
		private const byte niwOffset = 0;

		private const byte iwOffset = 32;

		private const byte nuOffset = 64;

		private const byte resOffset = 128;

		private const byte scOffset = 192;

		private ProtocolID()
		{
		}

		/// <summary>
		/// Extracts the "Reserved" part of the PID.
		/// </summary>
		/// <param name="pid">The PID containing the value.</param>
		/// <returns>The value of the "Reserved" part.</returns>
		/// <exception cref="T:System.ArgumentException">Value is not from the "Reserved" range.</exception>
		public static byte GetReservedValue(byte pid)
		{
			if (ProtocolID.IsReserved(pid))
			{
				return (byte)(pid - 128);
			}
			else
			{
				throw new ArgumentException("Value is not from the \"Reserved\" range.", "pid");
			}
		}

		/// <summary>
		/// Gets the "SC Specific Use" part of the ProtocolID.
		/// </summary>
		/// <param name="pid">The PID byte to decode.</param>
		/// <returns>The "SC Specific Use" value.</returns>
		/// <exception cref="T:System.ArgumentException">Value in pid is not from the "SC specific" range.</exception>
		public static byte GetSCSpecificUseValue(byte pid)
		{
			if (ProtocolID.IsSCSpecificUse(pid))
			{
				return (byte)(pid - 192);
			}
			else
			{
				throw new ArgumentException("Value is not from the \"SC specific\" range.", "pid");
			}
		}

		/// <summary>
		/// Determines if the specified value is from the "Reserved" part.
		/// </summary>
		/// <param name="pid">The value to check.</param>
		/// <returns>true if the value is from the reserved part, false otherwise.</returns>
		public static bool IsReserved(byte pid)
		{
			if (pid < 128)
			{
				return false;
			}
			else
			{
				return pid - 128 <= 63;
			}
		}

		/// <summary>
		/// Determines if the specified PID is from the "SC Specific Use" part.
		/// </summary>
		/// <param name="pid">The value to check.</param>
		/// <returns>true if the value is for SC specific use, false otherwise.</returns>
		public static bool IsSCSpecificUse(byte pid)
		{
			if (pid < 192)
			{
				return false;
			}
			else
			{
				return pid - 192 <= 63;
			}
		}

		/// <summary>
		/// Allows the "Reserved" part of the ProtocolID to be used.
		/// </summary>
		/// <param name="value">The value for this part.</param>
		/// <exception cref="T:System.ArgumentException">Value is greater than 0x3F (63).</exception>
		/// <returns>The encoded protocol ID.</returns>
		public static byte Reserved(byte value)
		{
			if (value <= 63)
			{
				return (byte)(128 + value);
			}
			else
			{
				throw new ArgumentException("Value must not be greater than 0x3F (63).");
			}
		}

		/// <summary>
		/// Allows the "SC Specific Use" part of the ProtocolID to be used.
		/// </summary>
		/// <param name="value">The value for this part.</param>
		/// <exception cref="T:System.ArgumentException">Value is greater than 0x3F (63).</exception>
		/// <returns>The encoded Protocol ID.</returns>
		public static byte SCSpecificUse(byte value)
		{
			if (value <= 63)
			{
				return (byte)(192 + value);
			}
			else
			{
				throw new ArgumentException("Value must not be greater than 0x3F (63).");
			}
		}

		/// <summary>
		/// Telematic interworking.
		/// </summary>
		/// <remarks>
		/// <para>If an interworking protocol is specified in an SMS-SUBMIT PDU,
		/// it indicates that the SME is a telematic device of the specified type,
		/// and requests the SC to convert the SM into a form suited for that
		/// device type. If the destination network is ISDN, the SC must also
		/// select the proper service indicators for connecting to a device of
		/// that type.</para>
		/// <para>If an interworking protocol is specified in an SMS-DELIVER PDU,
		/// it indicates that the SME is a telematic device of the specified type.
		/// </para>
		/// </remarks>
		public enum Interworking : byte
		{
			Implicit,
			Telex,
			Group3Telefax,
			Group4Telefax,
			VoiceTelephone,
			Ermes,
			PagingSystem,
			VideoTex,
			Teletex,
			TeletexPSPDN,
			TeletexCSPDN,
			TeletexPSTN,
			TeletexISDN,
			Uci,
			Reserved0E,
			Reserved0F,
			MessageHandler,
			X400BasedHandler,
			InternetEMail,
			Reserved13,
			Reserved14,
			Reserved15,
			Reserved16,
			Reserved17,
			SCSpecific1,
			SCSpecific2,
			SCSpecific3,
			SCSpecific4,
			SCSpecific5,
			SCSpecific6,
			SCSpecific7,
			GsmMobileStation
		}

		/// <summary>
		/// For network use
		/// </summary>
		/// <remarks>
		/// Details are written in the remarks section of the types.
		/// </remarks>
		public enum NetworkUse : byte
		{
			ShortMessageType0,
			ReplaceShortMessageType1,
			ReplaceShortMessageType2,
			ReplaceShortMessageType3,
			ReplaceShortMessageType4,
			ReplaceShortMessageType5,
			ReplaceShortMessageType6,
			ReplaceShortMessageType7,
			ReturnCallMessage,
			MEDataDownload,
			MEDepersonalization,
			SIMDataDownload
		}

		/// <summary>
		/// For the straightforward case of simple MS-to-SC short message
		/// transfer. No interworking is performed.
		/// </summary>
		public enum NoInterworking : byte
		{
			SmeToSmeProtocol
		}
	}
}