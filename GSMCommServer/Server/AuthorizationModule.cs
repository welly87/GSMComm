using System;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;

/// <summary>
/// Implements the authorization module for the server.
/// </summary>
namespace GsmComm.Server
{
	public class AuthorizationModule : IAuthorizeRemotingConnection
	{
		private bool allowAnonymous;

		/// <summary>
		/// Initializes a new instance of the module.
		/// </summary>
		/// <param name="allowAnonymous">Specifies if users authenticated anonymously can
		/// connect to the current channel.</param>
		public AuthorizationModule(bool allowAnonymous)
		{
			this.allowAnonymous = allowAnonymous;
		}

		/// <summary>
		/// Gets a Boolean value that indicates whether the network address of the client is
		/// authorized to connect on the current channel. 
		/// </summary>
		/// <param name="endPoint">The <see cref="T:System.Net.EndPoint" /> that identifies the network address of the client.</param>
		/// <returns>true if the network address of the client is authorized; otherwise, false.</returns>
		public bool IsConnectingEndPointAuthorized(EndPoint endPoint)
		{
			return true;
		}

		/// <summary>
		/// Gets a Boolean value that indicates whether the user identity of the client is
		/// authorized to connect on the current channel. 
		/// </summary>
		/// <param name="identity">The <see cref="T:System.Security.Principal.IIdentity" /> that represents the user identity of the client.</param>
		/// <returns>true if the user identity of the client is authorized; otherwise, false.</returns>
		public bool IsConnectingIdentityAuthorized(IIdentity identity)
		{
			bool flag = true;
			WindowsIdentity windowsIdentity = identity as WindowsIdentity;
			if (windowsIdentity != null && windowsIdentity.IsAnonymous && !this.allowAnonymous)
			{
				flag = false;
			}
			return flag;
		}
	}
}