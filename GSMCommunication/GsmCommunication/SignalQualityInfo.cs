using System;

/// <summary>
/// Contains the signal strength as calculcated by the ME.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class SignalQualityInfo
	{
		private int signalStrength;

		private int bitErrorRate;

		/// <summary>
		/// Gets the bit error rate.
		/// </summary>
		/// <remarks>Usually 99 is used if the bit error rate is not known.</remarks>
		public int BitErrorRate
		{
			get
			{
				return this.bitErrorRate;
			}
		}

		/// <summary>
		/// Gets the signal strength.
		/// </summary>
		/// <remarks>Usual value is an RSSI value in the range of 0 (no signal) to 31 (best signal),
		/// 99 if not known.</remarks>
		public int SignalStrength
		{
			get
			{
				return this.signalStrength;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="signalStrength">The signal strength, usual as an RSSI value in the range of 0 (no signal)
		/// to 31 (best signal), 99 if not known.</param>
		/// <param name="bitErrorRate">The bit error rate, 99 if not known.</param>
		public SignalQualityInfo(int signalStrength, int bitErrorRate)
		{
			this.signalStrength = signalStrength;
			this.bitErrorRate = bitErrorRate;
		}
	}
}