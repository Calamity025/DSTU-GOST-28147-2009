using System;
using System.Drawing;
using System.Windows.Forms;

namespace GOST {
	internal class Program {
		public static void Main(string[] args) {
			//int a = -2;
//			int max = Int32.MaxValue;
//			uint b = (uint)max - (uint)a;
//			Console.WriteLine(a.ToString() + " " + b.ToString() + " " + (int)b);
//			Console.ReadKey();
			Console.WriteLine(UInt64.MaxValue + " " + Math.Pow(2,32));
			CipherForm start = new CipherForm();
			start.Text = "ГОСТ";
			start.BackColor = Color.LightGray;
			Application.Run(start);
		}
	}
}