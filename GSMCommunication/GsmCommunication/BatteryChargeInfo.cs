using System;

/// <summary>
/// Contains the ME battery charging status and charge level.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public class BatteryChargeInfo
	{
		private int bcs;

		private int bcl;

		/// <summary>
		/// Gets the battery charge level.
		/// </summary>
		/// <remarks>Usual values are in the range from 0 (empty) to 100 (full).</remarks>
		public int BatteryChargeLevel
		{
			get
			{
				return this.bcl;
			}
		}

		/// <summary>
		/// Gets the battery charging status.
		/// </summary>
		/// <remarks>Usual values are 0 for "not charging" and 1 for "charging".</remarks>
		public int BatteryChargingStatus
		{
			get
			{
				return this.bcs;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="batteryChargingStatus">
		/// The battery charging status, usually 0 for "not charging" and 1 for "charging".
		/// </param>
		/// <param name="batteryChargeLevel">
		/// The battery charge level, usually in the range of 0 (empty) to 100 (full).
		/// </param>
		public BatteryChargeInfo(int batteryChargingStatus, int batteryChargeLevel)
		{
			this.bcs = batteryChargingStatus;
			this.bcl = batteryChargeLevel;
		}
	}
}