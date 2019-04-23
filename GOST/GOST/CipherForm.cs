using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GOST.Ciphers;
using GOST.Types;

namespace GOST {
    class CipherForm : Form {
        public TextBox Input, Output, Key;
        private RadioButton XOR, CFB, MAC, Sub, selectedrb;
        private GroupBox gb;

        GOSTManaged manager = new GOSTManaged();

        //конструктор формы
        public CipherForm() {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(485, 340);
            FormBorderStyle = FormBorderStyle.FixedDialog;

            Input = new TextBox();
            Input.Size = new Size(250, 115);
            Input.Location = new Point(10, 15);
            Input.Multiline = true;
            Input.Font = new Font(Input.Font.FontFamily, 14);
            Controls.Add(Input);

            Output = new TextBox();
            Output.Location = new Point(10, 185);
            Output.Size = new Size(250, 110);
            Output.Multiline = true;
            Output.Font = new Font(Output.Font.FontFamily, 14);
            Controls.Add(Output);

            gb = new GroupBox();
            gb.Location = new Point(270, 10);
            gb.Text = "Режим роботи";
            gb.Size = new Size(190, 150);

            XOR = new RadioButton();
            XOR.Location = new Point(10, 20);
            XOR.Size = new Size(170, 15);
            XOR.Text = "Гаммування";
            XOR.CheckedChanged += radioButton_CheckedChanged;

            CFB = new RadioButton();
            CFB.Location = new Point(10, 40);
            CFB.Size = new Size(170, 35);
            CFB.Text = "Гаммування зі зворотнім \n зв'язком";
            CFB.CheckedChanged += radioButton_CheckedChanged;

            MAC = new RadioButton();
            MAC.Location = new Point(10, 80);
            MAC.Size = new Size(170, 15);
            MAC.Text = "Проста заміна";
            MAC.CheckedChanged += radioButton_CheckedChanged;

            Sub = new RadioButton();
            Sub.Location = new Point(10, 100);
            Sub.Size = new Size(170, 35);
            Sub.Text = "Просте шифрування \n (довжина блоку кратна 8)";
            Sub.CheckedChanged += radioButton_CheckedChanged;

            gb.Controls.Add(XOR);
            gb.Controls.Add(CFB);
            gb.Controls.Add(MAC);
            gb.Controls.Add(Sub);
            Controls.Add(gb);

            Button encryptButton = new Button();
            encryptButton.Location = new Point(10, 140);
            encryptButton.Size = new Size(100, 30);
            encryptButton.Text = "Зашифрувати";
            encryptButton.Click += EncryptClick;
            Controls.Add(encryptButton);

            Button decryptButton = new Button();
            decryptButton.Location = new Point(160, 140);
            decryptButton.Size = new Size(100, 30);
            decryptButton.Text = "Дешифрувати";
            decryptButton.Click += DecryptClick;
            Controls.Add(decryptButton);
        }

        //ивенты нажатий на кнопки
        void EncryptClick(object sender, EventArgs e) {
            switch (selectedrb.Text) {
                case "Гаммування":
                    Output.Text = Encoding.GetEncoding(866).GetString(manager.XOREncode(
                        Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                        Encoding.GetEncoding(866).GetBytes("12345678"),
                        Encoding.GetEncoding(866).GetBytes(Input.Text),
                        SBlockTypes.CryptoProA));
                    break;
                case "Гаммування зі зворотнім \n зв'язком":
                    Output.Text = Encoding.GetEncoding(866).GetString(manager.CFBEncode(
                        Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                        Encoding.GetEncoding(866).GetBytes("12345678"),
                        Encoding.GetEncoding(866).GetBytes(Input.Text),
                        SBlockTypes.CryptoProA));
                    break;
                case "Проста заміна":
                    Output.Text = Encoding.GetEncoding(866).GetString(manager.SimpleSubstitutionEncode(
                        Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                        Encoding.GetEncoding(866).GetBytes(Input.Text),
                        SBlockTypes.CryptoProA));
                    break;
                case "Просте шифрування \n (довжина блоку кратна 8)":
                    if (Input.Text.Length % 8 != 0) {
                        MessageBox.Show(
                            "Введіть повідомлення з кількістю символів кратну 8 або виберіть інший режим роботи");
                    }
                    else {
                        Output.Text = Encoding.GetEncoding(866).GetString(manager.SubstitutionEncode(
                            Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                            Encoding.GetEncoding(866).GetBytes(Input.Text),
                            SBlockTypes.CryptoProA));
                    }
                    break;
                default:
                    MessageBox.Show(
                        "Виберіть режим роботи");
                    break;
            }

        }

        void DecryptClick(object sender, EventArgs e) {
                switch (selectedrb.Text) {
                    case "Гаммування":
                        Output.Text = Encoding.GetEncoding(866).GetString(manager.XORDecode(
                            Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                            Encoding.GetEncoding(866).GetBytes("12345678"),
                            Encoding.GetEncoding(866).GetBytes(Input.Text),
                            SBlockTypes.CryptoProA));
                        break;
                    case "Гаммування зі зворотнім \n зв'язком":
                        Output.Text = Encoding.GetEncoding(866).GetString(manager.CFBDecode(
                            Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                            Encoding.GetEncoding(866).GetBytes("12345678"),
                            Encoding.GetEncoding(866).GetBytes(Input.Text),
                            SBlockTypes.CryptoProA));
                        break;
                    case "Проста заміна":
                        Output.Text = Encoding.GetEncoding(866).GetString(manager.SimpleSubstitutionDecode(
                            Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                            Encoding.GetEncoding(866).GetBytes(Input.Text),
                            SBlockTypes.CryptoProA));
                        break;
                    case "Просте шифрування \n (довжина блоку кратна 8)":
                        if (Input.Text.Length % 8 != 0) {
                            MessageBox.Show(
                                "Введіть повідомлення з кількістю символів кратну 8 або виберіть інший режим роботи");
                        }
                        else {
                            Output.Text = Encoding.GetEncoding(866).GetString(manager.SubstitutionDecode(
                                Encoding.GetEncoding(866).GetBytes("12345678901234567890123456789012"),
                                Encoding.GetEncoding(866).GetBytes(Input.Text),
                                SBlockTypes.CryptoProA));
                        }
                        break;
                    default:
                        MessageBox.Show(
                            "Виберіть режим роботи");
                        break;
                }
            }
        
        void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
            {
                MessageBox.Show("Sender is not a RadioButton");
                return;
            }
            if (rb.Checked)
            {
                selectedrb = rb;
            }
        }
    }
}
