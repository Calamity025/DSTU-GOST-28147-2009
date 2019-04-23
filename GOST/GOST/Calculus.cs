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
		private string key = "Fi1ntKBgSrcFowlw1az49s9RcFNCd6HDI";
		private List<ulong> textBlocksOf64;
		private List<uint> keys;
		private string output;
		private List<ulong> ulongs = new List<ulong>();
		private static readonly byte[,] Table0 = {
			{0x4, 0x2, 0xF, 0x5, 0x9, 0x1, 0x0, 0x8, 0xE, 0x3, 0xB, 0xC, 0xD, 0x7, 0xA, 0x6}, 
			{0xC, 0x9, 0xF, 0xE, 0x8, 0x1, 0x3, 0xA, 0x2, 0x7, 0x4, 0xD, 0x6, 0x0, 0xB, 0x5}, 
			{0xD, 0x8, 0xE, 0xC, 0x7, 0x3, 0x9, 0xA, 0x1, 0x5, 0x2, 0x4, 0x6, 0xF, 0x0, 0xB}, 
			{0xE, 0x9, 0xB, 0x2, 0x5, 0xF, 0x7, 0x1, 0x0, 0xD, 0xC, 0x6, 0xA, 0x4, 0x3, 0x8}, 
			{0x3, 0xE, 0x5, 0x9, 0x6, 0x8, 0x0, 0xD, 0xA, 0xB, 0x7, 0xC, 0x2, 0x1, 0xF, 0x4}, 
			{0x8, 0xF, 0x6, 0xB, 0x1, 0x9, 0xC, 0x5, 0xD, 0x3, 0x7, 0xA, 0x0, 0xE, 0x2, 0x4}, 
			{0x9, 0xB, 0xC, 0x0, 0x3, 0x6, 0x7, 0x5, 0x4, 0x8, 0xE, 0xF, 0x1, 0xA, 0x2, 0xD},
			{0xC, 0x6, 0x5, 0x2, 0xB, 0x0, 0x9, 0xD, 0x3, 0xE, 0x7, 0xA, 0xF, 0x4, 0x1, 0x8}
		};
		
		public Dictionary<string, string> Output { get; set; }
		public Calculus(string data) {
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
						left = right;
						right = roundResult;	
					}
					else {
						ulongs.Add(roundResult | (ulong)right << 32);
					}
				}
			}

			output = Converter.ULongsToString(ulongs);
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
						left = right;
						right = roundResult;
					}
					else {
						ulongs.Add(roundResult | (ulong)right << 32);
					}
				}
			}

			//Output = Converter.SetDictionary(inputText, Converter.ULongsToString(ulongs));
			return Converter.ULongsToString(ulongs);
		}

		private uint SummByMod(uint roundKey, uint block) {
			return (uint)((roundKey + block) % (ulong) Math.Pow(2, 32));
		}

		private uint Shaffle(uint block) {
			List<uint> temp = new List<uint>(8);
			for (int i = 0; i < 8; i++) {
				uint s = (block << 4 * i) >> 28;
				temp.Add(Table0[i, s]);
			}

			return (temp[7] + (temp[6] << 4) + (temp[5] << 8) + (temp[4] << 12) + (temp[3] << 16) + (temp[2] << 20) +
			        (temp[1] << 24) + (temp[0] << 28));
		}

		private uint ShiftFor11(uint block) {
			return (block << 11) | (block >> 21);
		}
	}
}
