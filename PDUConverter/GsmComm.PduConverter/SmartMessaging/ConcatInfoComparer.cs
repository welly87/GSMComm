using System;
using System.Collections.Generic;

/// <summary>
/// Implements a method to compare <see cref="T:GsmComm.PduConverter.SmartMessaging.IConcatenationInfo" /> objects.
/// </summary>
/// <remarks>This comparer is provided for performing sort order comparisons. It does not perform exact equality comparisons.</remarks>
namespace GsmComm.PduConverter.SmartMessaging
{
	public class ConcatInfoComparer : IComparer<IConcatenationInfo>
	{
		public ConcatInfoComparer()
		{
		}

		/// <summary>
		/// Compares two <see cref="T:GsmComm.PduConverter.SmartMessaging.IConcatenationInfo" /> objects and returns a value indicating whether one is less than, equal to, or greater than the other. 
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// <list type="table">
		///   <listheader>
		///     <term>Value</term>
		///     <description>Condition</description>
		///   </listheader>
		///   <item>
		///     <term>Less than zero</term>
		///     <description>x is less than y.</description>
		///   </item>
		///   <item>
		///     <term>Zero</term>
		///     <description>x equals y.</description>
		///   </item>
		///   <item>
		///     <term>Greater than zero</term>
		///     <description>x is greater than y.</description>
		///   </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// <para>This method provides a sort order comparison for type <see cref="T:GsmComm.PduConverter.SmartMessaging.IConcatenationInfo" />.</para>
		/// <para>Comparing null with any reference type is allowed and does not generate an exception. A null reference
		/// is considered to be less than any reference that is not null.</para>
		/// </remarks>
		public int Compare(IConcatenationInfo x, IConcatenationInfo y)
		{
			int num;
			if (x != null || y != null)
			{
				if (x != null || y == null)
				{
					if (x == null || y != null)
					{
						if (x.ReferenceNumber != y.ReferenceNumber)
						{
							ushort referenceNumber = x.ReferenceNumber;
							num = referenceNumber.CompareTo(y.ReferenceNumber);
						}
						else
						{
							byte currentNumber = x.CurrentNumber;
							num = currentNumber.CompareTo(y.CurrentNumber);
						}
					}
					else
					{
						num = 1;
					}
				}
				else
				{
					num = -1;
				}
			}
			else
			{
				num = 0;
			}
			return num;
		}
	}
}