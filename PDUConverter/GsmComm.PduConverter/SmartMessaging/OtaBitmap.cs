using System;
using System.Collections;
using System.Drawing;

/// <summary>
/// Represents an OTA (over-the-air) bitmap.
/// </summary>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class OtaBitmap
	{
		private byte[] bitmap;

		private byte infoField;

		private byte width;

		private byte height;

		private byte grayscales;

		private int dataStart;

		private int dataLen;

		/// <summary>
		/// Gets the actual bitmap data.
		/// </summary>
		public byte[] Data
		{
			get
			{
				byte[] numArray = new byte[this.dataLen];
				Array.Copy(this.bitmap, this.dataStart, numArray, 0, this.dataLen);
				return numArray;
			}
		}

		/// <summary>
		/// Gets the bitmap's height.
		/// </summary>
		public byte Height
		{
			get
			{
				return this.height;
			}
		}

		/// <summary>
		/// Gets the bitmap's InfoField.
		/// </summary>
		public byte InfoField
		{
			get
			{
				return this.infoField;
			}
		}

		/// <summary>
		/// Gets the bitmap's number of grayscales.
		/// </summary>
		public byte NumGrayscales
		{
			get
			{
				return this.grayscales;
			}
		}

		/// <summary>
		/// Gets the bitmap's width.
		/// </summary>
		public byte Width
		{
			get
			{
				return this.width;
			}
		}

		/// <summary>
		/// Creates a new OTA bitmap from an existing <see cref="T:System.Drawing.Bitmap" /> object.
		/// </summary>
		/// <param name="bitmap">The <see cref="T:System.Drawing.Bitmap" /> to create the OTA bitmap from.</param>
		public OtaBitmap(Bitmap bitmap) : this(OtaBitmap.BitmapToOtaBitmap(bitmap))
		{
		}

		/// <summary>
		/// Creates a new OTA bitmap from an existing byte array.
		/// </summary>
		/// <param name="otaBitmap">The byte array containing the OTA bitmap.</param>
		/// <exception cref="T:System.ArgumentNullException">otaBitmap is null.</exception>
		public OtaBitmap(byte[] otaBitmap)
		{
			if (otaBitmap != null)
			{
				int num = 0;
				int num1 = num;
				num = num1 + 1;
				this.infoField = otaBitmap[num1];
				int num2 = num;
				num = num2 + 1;
				this.width = otaBitmap[num2];
				int num3 = num;
				num = num3 + 1;
				this.height = otaBitmap[num3];
				int num4 = num;
				num = num4 + 1;
				this.grayscales = otaBitmap[num4];
				this.dataStart = num;
				this.dataLen = (int)otaBitmap.Length - num;
				this.bitmap = new byte[(int)otaBitmap.Length];
				otaBitmap.CopyTo(this.bitmap, 0);
				return;
			}
			else
			{
				throw new ArgumentException("otaBitmap");
			}
		}

		/// <summary>
		/// Converts a <see cref="T:System.Drawing.Bitmap" /> into an OTA (over-the-air) bitmap.
		/// </summary>
		/// <param name="bitmap">The <see cref="T:System.Drawing.Bitmap" /> to convert. The maximum allowed
		/// size is 255x255 pixels, minimum is 1x1. The bitmap can be any
		/// pixel format, but only the black pixels are converted.
		/// Can be null to get an empty header.</param>
		/// <returns>The converted image. If bitmap is null, an empty OTA bitmap
		/// header and no data is returned.</returns>
		/// <exception cref="T:System.ArgumentException">bitmap is greater than 255x255 pixels.</exception>
		private static byte[] BitmapToOtaBitmap(Bitmap bitmap)
		{
			byte[] numArray = null;
			byte[] numArray1 = null;
			if (bitmap == null)
			{
				numArray1 = new byte[0];
				byte[] numArray2 = new byte[4];
				numArray2[3] = 1;
				numArray = numArray2;
			}
			else
			{
				if (bitmap.Height < 1 || bitmap.Width < 1 || bitmap.Height > 255 || bitmap.Width > 255)
				{
					throw new ArgumentException("Invalid bitmap dimensions. Maximum size is 255x255, minimum size is 1x1 pixels.");
				}
				else
				{
					int num = 7;
					byte num1 = 0;
					ArrayList arrayLists = new ArrayList();
					for (int i = 0; i < bitmap.Height; i++)
					{
						for (int j = 0; j < bitmap.Width; j++)
						{
							byte num2 = (byte)Math.Pow(2, (double)num);
							Color pixel = bitmap.GetPixel(j, i);
							Color black = Color.Black;
							if (pixel.ToArgb() == black.ToArgb())
							{
								num1 = (byte)(num1 | num2);
							}
							if (num != 0)
							{
								num--;
							}
							else
							{
								arrayLists.Add(num1);
								num1 = 0;
								num = 7;
							}
						}
					}
					if (num < 7)
					{
						arrayLists.Add(num1);
					}
					numArray1 = new byte[arrayLists.Count];
					arrayLists.CopyTo(numArray1);
					byte[] width = new byte[4];
					width[1] = (byte)bitmap.Width;
					width[2] = (byte)bitmap.Height;
					width[3] = 1;
					numArray = width;
				}
			}
			byte[] numArray3 = new byte[(int)numArray.Length + (int)numArray1.Length];
			numArray.CopyTo(numArray3, 0);
			numArray1.CopyTo(numArray3, (int)numArray.Length);
			return numArray3;
		}

		public static explicit operator Bitmap(OtaBitmap b)
		{
			return b.ToBitmap();
		}

		public static explicit operator OtaBitmap(Bitmap b)
		{
			return new OtaBitmap(b);
		}

		public static explicit operator OtaBitmap(byte[] b)
		{
			return new OtaBitmap(b);
		}

		public static implicit operator Byte[](OtaBitmap b)
		{
			return b.ToByteArray();
		}

		/// <summary>
		/// Converts an OTA bitmap into a <see cref="T:System.Drawing.Bitmap" />.
		/// </summary>
		/// <param name="otaBitmap">The OTA bitmap to convert. Can be null.</param>
		/// <returns>The converted image. If otaBitmap is null, null is returned.
		/// null is also returned, if the height or width of the OTA bitmap is 0.</returns>
		/// <remarks>
		/// <para>The grayscales attribute of the bitmap is ignored, always a monochrome bitmap is created.</para>
		/// </remarks>
		private static Bitmap OtaBitmapToBitmap(byte[] otaBitmap)
		{
			Color black;
			if (otaBitmap != null)
			{
				int num = 0;
				int num1 = num;
				num = num1 + 1;
				int num2 = num;
				num = num2 + 1;
				byte num3 = otaBitmap[num2];
				int num4 = num;
				num = num4 + 1;
				byte num5 = otaBitmap[num4];
				int num6 = num;
				num = num6 + 1;
				if (num3 == 0 || num5 == 0)
				{
					return null;
				}
				else
				{
					Bitmap bitmap = new Bitmap(num3, num5);
					int num7 = 0;
					byte num8 = 0;
					for (int i = 0; i < num5; i++)
					{
						for (int j = 0; j < num3; j++)
						{
							if (num7 != 0)
							{
								num7--;
							}
							else
							{
								int num9 = num;
								num = num9 + 1;
								num8 = otaBitmap[num9];
								num7 = 7;
							}
							byte num10 = (byte)Math.Pow(2, (double)num7);
							Bitmap bitmap1 = bitmap;
							int num11 = j;
							int num12 = i;
							if ((num8 & num10) > 0)
							{
								black = Color.Black;
							}
							else
							{
								black = Color.White;
							}
							bitmap1.SetPixel(num11, num12, black);
						}
					}
					return bitmap;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the <see cref="T:System.Drawing.Bitmap" /> equivalent of this instance.
		/// </summary>
		/// <returns>The <see cref="T:System.Drawing.Bitmap" />.</returns>
		public Bitmap ToBitmap()
		{
			return OtaBitmap.OtaBitmapToBitmap(this.bitmap);
		}

		/// <summary>
		/// Returns the byte array equivalent of this instance.
		/// </summary>
		/// <returns>The byte array.</returns>
		public byte[] ToByteArray()
		{
			byte[] numArray = new byte[(int)this.bitmap.Length];
			this.bitmap.CopyTo(numArray, 0);
			return numArray;
		}
	}
}