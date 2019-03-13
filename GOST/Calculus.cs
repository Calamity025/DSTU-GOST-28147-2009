using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GOST {
	public class Calculus {
		private string inputText = "12345678";
		private string key = "we are going to jail ! GAIL !Ree";
		private List<ulong> textBlocksOf64;
		private List<uint> keys;
		private string output;
		private List<ulong> ulongs = new List<ulong>();
		private uint[,] sBlocks = {
			{4, 10, 9, 2, 13, 8, 0, 14, 6, 11, 1, 12, 7, 15, 5, 3},
			{14, 11, 4, 12, 6, 13, 15, 10, 2, 3, 8, 1, 0, 7, 5, 9},
			{5, 8, 1, 13, 10, 3, 4, 2, 14, 15, 12, 7, 6, 0, 9, 11},
			{7, 13, 10, 1, 0, 8, 9, 15, 14, 4, 6, 12, 11, 2, 5, 3},
			{6, 12, 7, 1, 5, 15, 13, 8, 4, 10, 9, 14, 0, 3, 11, 2},
			{4, 11, 10, 0, 7, 2, 1, 13, 3, 6, 8, 5, 9, 12, 15, 14},
			{13, 11, 4, 1, 3, 15, 5, 9, 0, 10, 14, 7, 6, 8, 2, 12},
			{1, 15, 13, 0, 5, 7, 10, 4, 9, 2, 3, 14, 6, 11, 8, 12}
		};

		public string Output { get; set; }
		public Calculus(string data, string _key) {
			textBlocksOf64 = Converter.StringToULongData(data);
			keys = Converter.StringToUIntKeys(key);
		}
		public string Encrypt() {
			for (int j = 0; j < textBlocksOf64.Count; j++) {
				uint left = (uint) (textBlocksOf64[j] >> 32);
				uint right = (uint) ((textBlocksOf64[j] << 32) >> 32);
				for (int i = 0; i < 32; i++) {
					uint roundKey = i > 23 ? keys[7 - i % 8] : keys[i%8];
					uint roundResult = SummByMod(roundKey, right);
					roundResult = Shaffle(roundResult);
					roundResult = ShiftFor11(roundResult);
					roundResult ^= left;
					if (i != 31) {
						right = roundResult;
						left = right;
					}
					else {
						ulongs.Add(roundResult | (ulong)right << 32);
					}
				}
			}

			Output = Converter.ULongsToString(ulongs);
			Console.WriteLine(Output);
			return Converter.ULongsToString(ulongs);
		}

		public string Decrypt() {
			for (int j = 0; j < textBlocksOf64.Count; j++) {
				uint left = (uint) (textBlocksOf64[j] >> 32);
				uint right = (uint) ((textBlocksOf64[j] << 32) >> 32);
				for (int i = 0; i < 32; i++) {
					uint roundKey = i < 8 ? keys[i] : keys[7 - i % 8];
					uint roundResult = SummByMod(roundKey, right);
					roundResult = Shaffle(roundResult);
					roundResult = ShiftFor11(roundResult);
					roundResult ^= left;
					if (i != 31) {
						right = roundResult;
						left = right;
					}
					else {
						ulongs.Add(roundResult | (ulong)right << 32);
					}
				}
			}

			Output = Converter.ULongsToString(ulongs);
			Console.WriteLine(Output);
			return Converter.ULongsToString(ulongs);
		}

		private uint SummByMod(uint roundKey, uint block) {
			return (uint)((roundKey + block) % (ulong) Math.Pow(2, 32));
		}

		private uint Shaffle(uint block) {
			List<uint> temp = new List<uint>(8);
			for (int i = 0; i < 8; i++) {
				uint s = (block << 4 * i) >> 28;
				temp.Add(sBlocks[i, s]);
			}

			return (temp[7] + (temp[6] << 4) + (temp[5] << 8) + (temp[4] << 12) + (temp[3] << 16) + (temp[2] << 20) +
			        (temp[1] << 24) + (temp[0] << 28));
		}

		private uint ShiftFor11(uint block) {
			return (block << 11) | (block >> 21);
		}
	}
}
