using System.Drawing;
using System.Windows.Forms;

namespace GOST {
	internal class Program {
		public static void Main(string[] args) {
			CipherForm start = new CipherForm();
			start.Text = "ГОСТ";
			start.BackColor = Color.LightGray;
			Application.Run(start);
		}
	}
}