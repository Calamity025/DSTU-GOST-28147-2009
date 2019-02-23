using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GOST {
	public static class Calculus {
		//всякие кодировки, блоки перестановок, подстановок и тд
		private static Encoding cp866 = Encoding.GetEncoding(866);
		private static Queue<BitArray> input = new Queue<BitArray>();
		private static List<BitArray> keys = new List<BitArray>();
		private static Dictionary<int, BitArray> encrypted = new Dictionary<int, BitArray>();
		private static int[,] sBlocks = {
			{4, 10, 9, 2, 13, 8, 0, 14, 6, 11, 1, 12, 7, 15, 5, 3},
			{14, 11, 4, 12, 6, 13, 15, 10, 2, 3, 8, 1, 0, 7, 5, 9},
			{5, 8, 1, 13, 10, 3, 4, 2, 14, 15, 12, 7, 6, 0, 9, 11},
			{7, 13, 10, 1, 0, 8, 9, 15, 14, 4, 6, 12, 11, 2, 5, 3},
			{6, 12, 7, 1, 5, 15, 13, 8, 4, 10, 9, 14, 0, 3, 11, 2},
			{4, 11, 10, 0, 7, 2, 1, 13, 3, 6, 8, 5, 9, 12, 15, 14},
			{13, 11, 4, 1, 3, 15, 5, 9, 0, 10, 14, 7, 6, 8, 2, 12},
			{1, 15, 13, 0, 5, 7, 10, 4, 9, 2, 3, 14, 6, 11, 8, 12}
		};

		public static string encryptedString = "";
		//функция определения позиции символа в кодировке ср866

		private static void InitInput(string text) {
			BitArray temp;
			for (int i = 0; i < text.Length; i+=8) {
				temp = new BitArray(cp866.GetBytes(text.Substring(i, 8)));
				input.Enqueue(Reverse(temp));
			}
		}

		private static void InitKeys(string key) {
			for (int i = 0; i < key.Length; i+=4) {
				Key roundKey = new Key();
				roundKey.SetKeyInfo(key.Substring(i, 4));
				keys.Add(Reverse(roundKey.GetKeyInfo()));
			}
		}
		
		//функция оборота битового массива, потому что по дефолту младшие биты в нашем понимании тут - старшие и vise versa
		private static BitArray Reverse(BitArray array)
		{
			int length = array.Length;
			int mid = (length / 2);
			for (int i = 0; i < mid; i++)
			{
				bool bit = array[i];
				array[i] = array[length - i - 1];
				array[length - i - 1] = bit;
			}

			return array;
		}

		private static void SliceInHalf(BitArray full, ref BitArray left, ref BitArray right) {
			for (int i = 0; i < full.Count; i++) {
				if (i < 32) {
					left.Set(i, full.Get(i));
				}
				else {
					right.Set(i - 32, full.Get(i));
				}
			}
		}
		//битовый сдвиг
		//стандартный битовый сдвиг не работает с этим типом данных
		//вопрос в том, что проще: своя функция или конвертирование в другой тип данных? :hmmm: 
		private static void Shift(ref BitArray array) {
			for (int i = 0; i < 11; i++) {
				bool temp = array[0];
				for (int j = 1; j < array.Length; j++) {
					array[j - 1] = array[j];
				}
				array[array.Length - 1] = temp;
			}
		}

		private static void Shaffle(ref BitArray array) {
			for (int i = 0; i < 8; i++) {
				BitArray temp = new BitArray(4);
				for (int j = 0; j < temp.Length; j++) {
					temp.Set(j, array[4*i+j]);
				}
				int[] arrayValue = new int[1];
				temp.CopyTo(arrayValue, 0);
				BitArray tempForNumber = new BitArray(new []{(byte)sBlocks[i, arrayValue[0]]});
				for (int j = temp.Length - 1; j >= 0; j--) {
					temp[j] = tempForNumber[j];
				}
				temp = Reverse(temp);
				for (int j = 0; j < temp.Length; j++) {
					array.Set(4*i+j, temp[j]);
				}
			}
		}

		private static void SumBy2In32(BitArray key, ref BitArray array) {
			int[] temp = new int[2];
			key.CopyTo(temp, 0);
			array.CopyTo(temp, 1);
			array = new BitArray(new int[]{temp[0]+temp[1]});
			array = Reverse(array);
		}
		/*private static void SumBy2In32(BitArray key, ref BitArray array) {
			BitArray temp = new BitArray(array.Length, false);
			for (int i = array.Length - 1; i >= 0; i--) {
				int result = Convert.ToInt32(array[i]) + Convert.ToInt32(key[i]) + Convert.ToInt32(temp[i]);
				if (result < 2) {
					temp[i] = Convert.ToBoolean(result);
				}
				else {
					if (i != 0) {
						temp[i] = Convert.ToBoolean(result % 2);
						temp[i - 1] = Convert.ToBoolean(1);
					}
					else {
						temp[i] = Convert.ToBoolean(result % 2);
					}
				}
			}
			array = temp;
		}*/

		private static void ThreadEncrypt(int id, bool isEncryption) {
			BitArray leftPart, rightPart;
			leftPart = rightPart = new BitArray(32);
			SliceInHalf(input.Dequeue(), ref leftPart, ref rightPart);
			for (int i = 0; i < 32; i++) {
				BitArray roundKey;
				if (isEncryption) {
					roundKey = i < 24 ? keys[i % 8] : keys[7 - i % 8];
				}
				else {
					roundKey = i < 8 ? keys[i] : keys[7 - (i % 8)];
				}
				SumBy2In32(roundKey, ref rightPart);
				Shaffle(ref rightPart);
				Shift(ref rightPart);
				leftPart = leftPart.Xor(rightPart);
				if (i != 31) {
					BitArray temp = leftPart;
					leftPart = rightPart;
					rightPart = temp;
				}
				else {
					BitArray output = new BitArray(64);
					for (int j = 0; j < output.Length; j++) {
						output[j] = j < 32 ? leftPart[j] : rightPart[j - 32];
					}
					encrypted.Add(id, output);
				}
			}
		}

		private static string BitArrayListToString() {
			for (int i = 0; i < encrypted.Count; i++) {
				byte[] temp = new byte[8];
				encrypted[i] = Reverse(encrypted[i]);
				encrypted[i].CopyTo(temp, 0);
				encryptedString += cp866.GetString(temp);
			}

			return encryptedString;
		}

		private static void Refresh() {
			keys = new List<BitArray>();
			encrypted = new Dictionary<int, BitArray>();
		}
		public static void Encrypt(string text, string key) {
			InitInput(text);
			InitKeys(key);
			int queue = input.Count;
			for (int i = 0; i < queue; i++) {
				ThreadEncrypt(i, true);
			}
			encryptedString = BitArrayListToString();
			Refresh();
		}

		public static void Decrypt(string text, string key) { //тут все так же как в зашифровке
			InitInput(text);
			InitKeys(key);
			int queue = input.Count;
			for (int i = 0; i < queue; i++) {
				ThreadEncrypt(i, false);
			}
			encryptedString = BitArrayListToString();
			Refresh();
		}
	}
}