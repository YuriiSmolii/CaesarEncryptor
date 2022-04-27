using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaesarEncryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Text = "English";
            InitTables();
        }

        public bool RichTextBoxChanged = false;
        public bool IsSaved = false;
        public string FileName = String.Empty;
        public DataTable frequenceTableOriginal = new DataTable();
        public DataTable frequenceTableDecrypted = new DataTable();
        public DataTable frequenceTableUkr = new DataTable();
        public DataTable frequenceTableEng = new DataTable();
        public DataTable attackTable = new DataTable();

        private void ClearRichTextBox()
        {
            OriginalText.Text = "";
            DecryptedText.Text = "";
        }

        private void InitTables()
        {
            frequenceTableOriginal.Columns.Add("Character");
            frequenceTableOriginal.Columns.Add("Count");
            frequenceTableDecrypted.Columns.Add("Character");
            frequenceTableDecrypted.Columns.Add("Count");
            frequenceTableUkr.Columns.Add("Character");
            frequenceTableUkr.Columns.Add("Count");
            frequenceTableEng.Columns.Add("Character");
            frequenceTableEng.Columns.Add("Count");
            attackTable.Columns.Add("Key");
            attackTable.Columns.Add("Value");
        }

        private void Open(string OpenFileName)
        {

            if (OpenFileName == "")
            {
                return;
            }
            else
            {
                StreamReader readFile = new StreamReader(OpenFileName);
                while (!readFile.EndOfStream)
                {
                    OriginalText.Text = readFile.ReadToEnd();
                }
                readFile.Close();
                FileName = OpenFileName;
            }
        }

        private void Save(string SaveFileName)
        {
            if (SaveFileName == "")
            {
                return;
            }
            else
            {
                File.WriteAllText(SaveFileName, String.Empty);
                StreamWriter sw = new StreamWriter(SaveFileName);
                sw.WriteLine("Original text ->\n");
                for (int i = 0; i < OriginalText.Lines.Length; i++)
                {
                    sw.WriteLine(OriginalText.Lines[i]);
                }
                sw.WriteLine("\n Decrypted text ->\n");
                for (int i = 0; i < DecryptedText.Lines.Length; i++)
                {
                    sw.WriteLine(DecryptedText.Lines[i]);
                }
                sw.Flush();
                sw.Close();
                FileName = SaveFileName;
            }
        }

        private string Encryption(string inputText, int key, string typeAlphabet)
        {
            char[] alphabet = new char[0];
            string encryptText;
            char[] inputTextArray = inputText.ToCharArray();

            if (typeAlphabet == "English")
            {
                alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz,.!".ToCharArray();
            }
            else if (typeAlphabet == "Ukrainian")
            {
                alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯабвгґдеєжзиіїйклмнопрстуфхцчшщьюя,.!".ToCharArray();
            }
            for (int j = 0; j < inputTextArray.Length; j++)
            {
                for (int i = 0; i < alphabet.Length; i++)
                {
                    if (inputTextArray[j] == alphabet[i])
                    {
                        if (char.IsLower(inputTextArray[j]))
                        {
                            inputTextArray[j] = char.ToLower(alphabet[(i + key) % alphabet.Length]);
                        }
                        else
                        {
                            inputTextArray[j] = char.ToUpper(alphabet[(i + key) % alphabet.Length]);
                        }
                        break;
                    }
                }
            }
            encryptText = new string(inputTextArray);
            return encryptText;
        }

        private string Decryption(string inputText, int key, string typeAlphabet)
        {
            string decryptText = "";
            if (typeAlphabet == "English")
            {
                decryptText = Encryption(inputText, 55 - key, typeAlphabet);
            }
            else if (typeAlphabet == "Ukrainian")
            {
                decryptText = Encryption(inputText, 69 - key, typeAlphabet);
            }
            return decryptText;
        }

        private bool isValidText(string inputText, string typeAlphabet)
        {
            bool isValid;
            if (typeAlphabet == "English")
            {
                isValid = Regex.IsMatch(inputText, "^[a-zA-Z ,.!\n]+$");
                if (isValid)
                {
                    return true;
                }
            }
            else if (typeAlphabet == "Ukrainian")
            {
                isValid = Regex.IsMatch(inputText, "^[А-ЩЬЮЯҐЄІЇа-щьюяґєії ,.!\n]+$");
                if (isValid)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isValidKey(int key, string typeAlphabet)
        {
            if (typeAlphabet == "English")
            {
                if (key < 29 && key >= 0)
                {
                    return true;
                }
            }
            else if (typeAlphabet == "Ukrainian")
            {
                if (key < 36 && key >= 0)
                {
                    return true;
                }
            }
            else if (typeAlphabet == "pic")
            {
                if (key < 256 && key >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            About form = new About();
            form.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //saveToolStripMenuItem.Enabled = false;
            if (RichTextBoxChanged == true)
            {
                DialogResult dialogResult = MessageBox.Show("Save current content?", "Question", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    saveToolStripMenuItem.PerformClick();
                    //ClearRichTextBox();
                    this.Text = "Caesar";
                }
                else if (dialogResult == DialogResult.No)
                {
                    ClearRichTextBox();
                    this.Text = "Caesar";
                }
            }
            else
            {
                ClearRichTextBox();
                this.Text = "Caesar";
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RichTextBoxChanged == true)
            {
                if (DecryptedText.Text != String.Empty)
                {
                    DialogResult dialogResult = MessageBox.Show("Save current content?", "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        saveToolStripMenuItem.PerformClick();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        DialogResult res;
                        res = openFileDialog1.ShowDialog();
                        if (res == DialogResult.OK)
                        {
                            Open(openFileDialog1.FileName);
                            RichTextBoxChanged = false;
                        }
                        FileName = openFileDialog1.FileName;
                        this.Text = Path.GetFileNameWithoutExtension(FileName);
                    }
                }
                else
                {
                    DialogResult res;
                    res = openFileDialog1.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        Open(openFileDialog1.FileName);
                        RichTextBoxChanged = false;
                    }
                    FileName = openFileDialog1.FileName;
                    this.Text = Path.GetFileNameWithoutExtension(FileName);
                }
            }
            else
            {
                DialogResult res;
                res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    Open(openFileDialog1.FileName);
                }
                FileName = openFileDialog1.FileName;
                this.Text = Path.GetFileNameWithoutExtension(FileName);
            }
            saveToolStripMenuItem.Enabled = true;
            IsSaved = true;
            RichTextBoxChanged = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                saveAsToolStripMenuItem.PerformClick();
            }
            else
            {
                Save(FileName);
                RichTextBoxChanged = false;
                IsSaved = true;
                saveToolStripMenuItem.Enabled = true;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res;
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|Word Doucment (*.doc)|*.doc;*.docx|All files (*.*)|*.*";
            res = saveFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                Save(saveFileDialog1.FileName);
                RichTextBoxChanged = false;
                IsSaved = true;
            }
            FileName = saveFileDialog1.FileName;
            saveToolStripMenuItem.Enabled = true;
            this.Text = Path.GetFileNameWithoutExtension(FileName);
        }

        private void encryptButton_Click(object sender, EventArgs e)
        {
            string text = OriginalText.Text;
            int key = Convert.ToInt32(numericUpDown1.Value);
            string alphabet = comboBox1.SelectedItem.ToString();

            if (!isValidText(text, alphabet))
            {
                MessageBox.Show("Text contains forbitten symbols", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!isValidKey(key, alphabet))
            {
                MessageBox.Show("Shift key is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDown1.Value = 1;
                key = 1;
            }

            if (isValidText(text, alphabet))
            {
                DecryptedText.Text = Encryption(text, key, alphabet);
            }
            RichTextBoxChanged = true;
        }

        private void decryptButton_Click(object sender, EventArgs e)
        {
            string text = OriginalText.Text;
            int key = Convert.ToInt32(numericUpDown1.Value);
            string alphabet = comboBox1.SelectedItem.ToString();
            if (!isValidText(text, alphabet))
            {
                MessageBox.Show("Text contains forbitten symbols", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!isValidKey(key, alphabet))
            {
                MessageBox.Show("Shift key is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDown1.Value = 1;
                key = 1;
            }

            if (isValidText(text, alphabet))
            {
                DecryptedText.Text = Decryption(text, key, alphabet);
            }
            RichTextBoxChanged = true;
        }

        private void outputFreuency_Click(object sender, EventArgs e)
        {
            frequenceTableDecrypted.Clear();
            if (!string.IsNullOrEmpty(DecryptedText.Text))
            {
                Dictionary<char, int> dictChar = new Dictionary<char, int>();
                foreach (char ch in DecryptedText.Text)
                {
                    if (dictChar.ContainsKey(ch))
                    {
                        dictChar[ch] += 1;
                    }
                    else
                    {
                        dictChar[ch] = 1;
                    }
                }
                foreach (var item in dictChar)
                {
                    if (!Char.IsWhiteSpace(item.Key))
                    {
                        DataRow row = frequenceTableDecrypted.NewRow();
                        row[0] = item.Key;
                        row[1] = item.Value;
                        frequenceTableDecrypted.Rows.Add(row);
                    }
                }
            }
            else
            {
                MessageBox.Show("Text is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FrequenceDecrypted form = new FrequenceDecrypted();
            form.Show(this);
        }

        private void inputFrequency_Click(object sender, EventArgs e)
        {
            frequenceTableOriginal.Clear();
            if (!string.IsNullOrEmpty(OriginalText.Text))
            {
                Dictionary<char, int> dictChar = new Dictionary<char, int>();
                foreach (char ch in OriginalText.Text)
                {
                    if (dictChar.ContainsKey(ch))
                    {
                        dictChar[ch] += 1;
                    }
                    else
                    {
                        dictChar[ch] = 1;
                    }
                }
                foreach (var item in dictChar)
                {
                    if (!Char.IsWhiteSpace(item.Key))
                    {
                        DataRow row = frequenceTableOriginal.NewRow();
                        row[0] = item.Key;
                        row[1] = item.Value;
                        frequenceTableOriginal.Rows.Add(row);
                    }
                }
            }
            else
            {
                MessageBox.Show("Text is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FrequenceOriginal form = new FrequenceOriginal();
            form.Show(this);
        }

        private void ukrainianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frequenceTableUkr.Clear();
            Dictionary<char, double> dictChar = new Dictionary<char, double>();
            string alphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
            List<double> frequency = new List<double>() { 8.34, 1.53, 5.50, 1.59, 0.01, 3.06, 4.59, 0.39, 0.71, 2.10, 6.00, 1.24, 6.23, 0.84, 4.00, 3.93, 3.02, 7.10, 9.28, 2.84, 5.48, 4.57, 4.77, 3.38, 0.35, 1.17, 1.02, 1.15, 0.71, 0.32, 1.83, 0.70, 2.16 };
            int counter = 0;
            foreach (char ch in alphabet)
            {
                dictChar[ch] = frequency[counter];
                counter++;
            }
            foreach (var item in dictChar)
            {
                if (!Char.IsWhiteSpace(item.Key))
                {
                    DataRow row = frequenceTableUkr.NewRow();
                    row[0] = item.Key;
                    row[1] = item.Value;
                    frequenceTableUkr.Rows.Add(row);
                }
            }
            FrequenceUkr form = new FrequenceUkr();
            form.Show(this);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frequenceTableEng.Clear();
            Dictionary<char, double> dictChar = new Dictionary<char, double>();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            List<double> frequency = new List<double>() { 8.34, 1.54, 2.73, 4.14, 12.60, 2.03, 1.92, 6.11, 6.71, 0.23, 0.87, 4.24, 2.53, 6.80, 7.70, 1.66, 0.09, 5.68, 6.11, 9.37, 2.85, 1.06, 2.34, 0.20, 2.04, 0.06 };
            int counter = 0;
            foreach (char ch in alphabet)
            {
                dictChar[ch] = frequency[counter];
                counter++;
            }
            foreach (var item in dictChar)
            {
                if (!Char.IsWhiteSpace(item.Key))
                {
                    DataRow row = frequenceTableEng.NewRow();
                    row[0] = item.Key;
                    row[1] = item.Value;
                    frequenceTableEng.Rows.Add(row);
                }
            }
            FrequenceEng form = new FrequenceEng();
            form.Show(this);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
            else
            {
                MessageBox.Show("Print Cancelled");
            }
        }

        private void printDocument1_PrintPage_1(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font myFont = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Pixel);
            float leftMargin = e.MarginBounds.Left;
            if (!string.IsNullOrEmpty(DecryptedText.Text))
            {
                e.Graphics.DrawString("Original text:\n" + OriginalText.Text + "\n\nDecrytped with key " + Convert.ToString(numericUpDown1.Value) + " text:\n" + DecryptedText.Text, myFont, Brushes.Black, leftMargin, 150, new StringFormat());
            }
            else if (!string.IsNullOrEmpty(OriginalText.Text))
            {
                e.Graphics.DrawString("Original text:\n" + OriginalText.Text, myFont, Brushes.Black, leftMargin, 150, new StringFormat());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void bruteForceButton_Click(object sender, EventArgs e)
        {
            string text = OriginalText.Text;
            string alphabet = comboBox1.SelectedItem.ToString();
            if (string.IsNullOrEmpty(OriginalText.Text))
            {
                MessageBox.Show("Text is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (!isValidText(text, alphabet))
                {
                    MessageBox.Show("Text contains forbitten symbols", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (!isValidKey(Convert.ToInt32(numericUpDown2.Value), alphabet))
                    {
                        MessageBox.Show("Shift key is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown2.Value = 1;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            Dictionary<int, string> dictAttack = new Dictionary<int, string>();
                            if (alphabet == "English")
                            {
                                int key = Convert.ToInt32(numericUpDown2.Value);
                                for (int iter = 0; iter < 29; ++iter)
                                {
                                    DecryptedText.Text = Decryption(OriginalText.Text, key, "English");
                                    DialogResult dialogResult = MessageBox.Show("Is it correct?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    if(dialogResult == DialogResult.Yes)
                                    {
                                        break;
                                    }
                                    if(key == 29)
                                    {
                                        key = 0;
                                    }
                                    key++;
                                }
                            }
                            else if (alphabet == "Ukrainian")
                            {
                                int key = Convert.ToInt32(numericUpDown2.Value);
                                for (int iter = 0; iter < 36; ++iter)
                                {
                                    DecryptedText.Text = Decryption(OriginalText.Text, key, "Ukrainian"); DialogResult dialogResult = MessageBox.Show("Is it correct?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    if (dialogResult == DialogResult.Yes)
                                    {
                                        break;
                                    }
                                    if (key == 36)
                                    {
                                        key = 0;
                                    }
                                    key++;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int key = Convert.ToInt32(numericUpDown1.Value);
            if (isValidKey(key, "pic"))
            {
                DialogResult res;
                res = openFileDialog1.ShowDialog();
                FileName = openFileDialog1.FileName;
                Bitmap img = new Bitmap(FileName);
                
                string[] subs = FileName.Split('.');
                if(key > 0)
                {
                    for (int i = 0; i < img.Width; i++)
                    {
                        for (int j = 0; j < img.Height; j++)
                        {
                            Color pixel = img.GetPixel(i, j);
                            Color newpixel = Color.FromArgb((pixel.R + key) % 256, (pixel.G + key) % 256, (pixel.B + key) % 256);
                            img.SetPixel(i, j, newpixel);
                        }
                    }
                    img.Save(subs[0] + "_enc." + subs[1]);
                }
                else
                {
                    for (int i = 0; i < img.Width; i++)
                    {
                        for (int j = 0; j < img.Height; j++)
                        {
                            Color pixel = img.GetPixel(i, j);
                            Color newpixel = Color.FromArgb((pixel.R - key + 256) % 256, (pixel.G - key + 256) % 256, (pixel.B - key +256 ) % 256);
                            img.SetPixel(i, j, newpixel);
                        }
                    }
                    img.Save((subs[0].Split('_'))[0] + "_dec." + subs[1]);
                }
            }
            else
            {
                MessageBox.Show("Shift key is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDown1.Value = 1;
                key = 1;
            }
        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int key = Convert.ToInt32(numericUpDown1.Value);
            if (isValidKey(key, "pic"))
            {
            DialogResult res;
            res = openFileDialog1.ShowDialog();
            FileName = openFileDialog1.FileName;
            ;
            if(FileName != String.Empty)
                {
                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(FileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);
                    for(int i = 0; i < imageData.Length; i++)
                    {
                        imageData[i] = (byte)((imageData[i] + key)%256);
                    }
                    string[] subs = FileName.Split('.');
                    using(var ms = new MemoryStream(imageData))
                    {
                        using(var fs2 = new FileStream(subs[0] + "_enc." + subs[1], FileMode.Create))
                        {
                            ms.WriteTo(fs2);
                        }
                    }
                    MessageBox.Show("File encrypted successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            else
            {
                MessageBox.Show("Shift key is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDown1.Value = 1;
                key = 1;
            }
        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int key = Convert.ToInt32(numericUpDown1.Value);
            if (isValidKey(key, "pic"))
            {
                DialogResult res;
                res = openFileDialog1.ShowDialog();
                FileName = openFileDialog1.FileName;
                if (FileName != String.Empty)
                {
                    byte[] imageData = null;
                    FileInfo fileInfo = new FileInfo(FileName);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    imageData = br.ReadBytes((int)imageFileLength);
                    for (int i = 0; i < imageData.Length; i++)
                    {
                        imageData[i] = (byte)((imageData[i] - key) % 256);
                    }
                    string[] subs = FileName.Split('.');
                    using (var ms = new MemoryStream(imageData))
                    {
                        using (var fs2 = new FileStream(subs[0] + "_dec." + subs[1], FileMode.Create))
                        {
                            ms.WriteTo(fs2);
                        }
                    }
                    MessageBox.Show("File decrypted successfuly", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            else
            {
                MessageBox.Show("Shift key is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDown1.Value = 1;
                key = 1;
            }
        }
    }
}
