using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GsmComm.GsmCommunication
{
	internal class SerialPortFixer : IDisposable
	{
		private const int DcbFlagAbortOnError = 14;

		private const int CommStateRetries = 10;

		private SafeFileHandle m_Handle;

		private SerialPortFixer(string portName)
		{
			if (portName == null || !portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Invalid Serial Port", "portName");
			}
			else
			{
				SafeFileHandle safeFileHandle = SerialPortFixer.CreateFile(string.Concat("\\\\.\\", portName), -1073741824, 0, IntPtr.Zero, 3, 1073741824, IntPtr.Zero);
				if (safeFileHandle.IsInvalid)
				{
					SerialPortFixer.WinIoError();
				}
				try
				{
					int fileType = SerialPortFixer.GetFileType(safeFileHandle);
					if (fileType == 2 || fileType == 0)
					{
						this.m_Handle = safeFileHandle;
						this.InitializeDcb();
					}
					else
					{
						throw new ArgumentException("Invalid Serial Port", "portName");
					}
				}
				catch
				{
					safeFileHandle.Close();
					this.m_Handle = null;
					throw;
				}
				return;
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern bool ClearCommError(SafeFileHandle hFile, ref int lpErrors, ref SerialPortFixer.Comstat lpStat);

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		public void Dispose()
		{
			if (this.m_Handle != null)
			{
				this.m_Handle.Close();
				this.m_Handle = null;
			}
		}

		public static void Execute(string portName)
		{
			using (SerialPortFixer serialPortFixer = new SerialPortFixer(portName))
			{
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr arguments);

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern bool GetCommState(SafeFileHandle hFile, ref SerialPortFixer.Dcb lpDcb);

		private void GetCommStateNative(ref SerialPortFixer.Dcb lpDcb)
		{
			int num = 0;
			SerialPortFixer.Comstat comstat = new SerialPortFixer.Comstat();
			int num1 = 0;
			while (num1 < 10)
			{
				if (!SerialPortFixer.ClearCommError(this.m_Handle, ref num, ref comstat))
				{
					SerialPortFixer.WinIoError();
				}
				if (!SerialPortFixer.GetCommState(this.m_Handle, ref lpDcb))
				{
					if (num1 == 9)
					{
						SerialPortFixer.WinIoError();
					}
					num1++;
				}
				else
				{
					return;
				}
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None)]
		private static extern int GetFileType(SafeFileHandle hFile);

		private static string GetMessage(int errorCode)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (SerialPortFixer.FormatMessage(12800, new HandleRef(null, IntPtr.Zero), errorCode, 0, stringBuilder, stringBuilder.Capacity, IntPtr.Zero) == 0)
			{
				return "Unknown Error";
			}
			else
			{
				return stringBuilder.ToString();
			}
		}

		private void InitializeDcb()
		{
			SerialPortFixer.Dcb flags = new SerialPortFixer.Dcb();
			this.GetCommStateNative(ref flags);
			flags.Flags = flags.Flags & -16385;
			this.SetCommStateNative(ref flags);
		}

		private static int MakeHrFromErrorCode(int errorCode)
		{
			return -2147024896 | errorCode;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern bool SetCommState(SafeFileHandle hFile, ref SerialPortFixer.Dcb lpDcb);

		private void SetCommStateNative(ref SerialPortFixer.Dcb lpDcb)
		{
			int num = 0;
			SerialPortFixer.Comstat comstat = new SerialPortFixer.Comstat();
			int num1 = 0;
			while (num1 < 10)
			{
				if (!SerialPortFixer.ClearCommError(this.m_Handle, ref num, ref comstat))
				{
					SerialPortFixer.WinIoError();
				}
				if (!SerialPortFixer.SetCommState(this.m_Handle, ref lpDcb))
				{
					if (num1 == 9)
					{
						SerialPortFixer.WinIoError();
					}
					num1++;
				}
				else
				{
					return;
				}
			}
		}

		private static void WinIoError()
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			throw new IOException(SerialPortFixer.GetMessage(lastWin32Error), SerialPortFixer.MakeHrFromErrorCode(lastWin32Error));
		}

		private struct Comstat
		{
			public readonly uint Flags;

			public readonly uint cbInQue;

			public readonly uint cbOutQue;
		}

		private struct Dcb
		{
			public readonly uint DCBlength;

			public readonly uint BaudRate;

			public uint Flags;

			public readonly ushort wReserved;

			public readonly ushort XonLim;

			public readonly ushort XoffLim;

			public readonly byte ByteSize;

			public readonly byte Parity;

			public readonly byte StopBits;

			public readonly byte XonChar;

			public readonly byte XoffChar;

			public readonly byte ErrorChar;

			public readonly byte EofChar;

			public readonly byte EvtChar;

			public readonly ushort wReserved1;
		}
	}
}