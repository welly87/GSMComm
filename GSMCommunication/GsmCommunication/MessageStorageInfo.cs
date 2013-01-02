using System;

/// <summary>
/// Provides a structure that contains details about supported message storages.
/// </summary>
namespace GsmComm.GsmCommunication
{
	public struct MessageStorageInfo
	{
		/// <summary>Specifies the storages that can be used for reading and deleting.</summary>
		public string[] ReadStorages;

		/// <summary>Speicifies the storages that can be used for writing and sending.</summary>
		public string[] WriteStorages;

		/// <summary>Specifies the storages that can be used as the preferred receive storage.</summary>
		public string[] ReceiveStorages;
	}
}