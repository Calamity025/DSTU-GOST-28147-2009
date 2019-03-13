using System;
using System.Collections.Generic;
using System.Text;

namespace GOST {
	public class Converter {
		
		public static List<ulong> StringToULongData(string text) {
			List<ulong> data = new List<ulong>();
			byte[] stringInBytes = Encoding.GetEncoding(866).GetBytes(text);
			for (int i = 0; i < stringInBytes.Length / 8; i++) {
				data.Add(BitConverter.ToUInt32(stringInBytes, i*8));
			}
			return data;
		}

		public static List<uint> StringToUIntKeys(string plainKey) {
			List<uint> keys = new List<uint>(8);
			byte[] stringInBytes = Encoding.GetEncoding(866).GetBytes(plainKey);
			for (int i = 0; i < 8; i++) {
				keys.Add(BitConverter.ToUInt32(stringInBytes, i*4));
			}

			return keys;
		}

		public static string ULongsToString(List<ulong> ulongs) {
			string output = "";
			foreach (var _ulong in ulongs) {
				output += Encoding.GetEncoding(866).GetString(BitConverter.GetBytes(_ulong));
			}

			return output;
		}
	}
}