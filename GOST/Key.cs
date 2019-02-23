using System.Collections;
using System.Text;

namespace GOST {
	public class Key {
		private BitArray _key;
		
		public BitArray GetKeyInfo() {
			return _key;
		}

		public void SetKeyInfo(string text) {
			_key = ToBitArray(text);
		}
		
		private BitArray ToBitArray(string text) {
			return new BitArray(Encoding.GetEncoding(866).GetBytes(text));
		}
		
	}
}