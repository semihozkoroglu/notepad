using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;


namespace notepad
{
    public partial class Form1 : Form
    {
        OpenFileDialog file = null;
        int fileCount = 1;

        RegistryKey register;
        StringReader myReader;

        Font f;
        bool bold = false, italic = false, under = false;

        public Form1()
        {
            InitializeComponent();

            register = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\NotepadUygulama\Kullanim");

            sort();

            foreach (FontFamily font in System.Drawing.FontFamily.Families)
            {
                toolStripComboBox1.Items.Add(font.Name);
            }
            toolStripStatusLabel1.Text = "Yeni dosya";
        }

        private void sort()
        {
            recentFileToolStripMenuItem.DropDownItems.Clear();
            for (int i = register.ValueCount - 1; 0 <= i; i--)
            {
                recentFileToolStripMenuItem.DropDownItems.Add(register.GetValueNames()[i]);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
            sort();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeClicked();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeClicked();
        }

        private void registerDel()
        {
            foreach (var v in register.GetValueNames())
            {
                register.DeleteValue(v);
                break;
            }
        }
        private void registerAdd(string path)
        {
            if (register.ValueCount.ToString() == "9")
            {
                if (register.GetValue(path) == null)
                {
                    registerDel();
                    register.SetValue(path, fileCount.ToString());
                }
                else
                {
                    register.SetValue(path, fileCount.ToString());
                    fileCount += 1;
                }
            }
            else
            {
                register.SetValue(path, fileCount.ToString());
            }
        }

        private void closeClicked()
        {
            if (richTextBox1.Modified)
            {
                DialogResult result = MessageBox.Show("Değişlikleriniz Kaydedilmeyecek?", "The Question",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    save();
                }
            }
            else
            {
                Application.Exit();
            }
        }


        private void save()
        {
            if (file == null)
            {
                file = new OpenFileDialog();

                if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
                {
                    saveFileDialog1.Filter = "Rich Text (*.rtf)|*.rtf |txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.RichText);
                    file.FileName = saveFileDialog1.FileName;

                    richTextBox1.Modified = false;

                    toolStripStatusLabel1.Text = System.IO.Path.GetFileName(file.FileName) + " kaydedildi.";

                    registerAdd(saveFileDialog1.FileName);
                }
            }
            else
            {
                if (!richTextBox1.Modified)
                {
                    toolStripStatusLabel1.Text = System.IO.Path.GetFileName(file.FileName) + " 'de değişiklik yok.";

                }
                else
                {
                    richTextBox1.SaveFile(file.FileName, RichTextBoxStreamType.RichText);
                    richTextBox1.Modified = false;

                    toolStripStatusLabel1.Text = System.IO.Path.GetFileName(file.FileName) + " kaydedildi.";

                    registerAdd(file.FileName);
                }
            }
            sort();
        }
        private void openFile(string path = null)
        {
            if (path == null)
            {
                if (richTextBox1.Modified)
                {
                    DialogResult result = MessageBox.Show("Değişlikleriniz Kaydedilmeyecek?", "The Question",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.Yes)
                    {

                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            path = openFileDialog1.FileName;

                            file = openFileDialog1;

                            toolStripStatusLabel1.Text = System.IO.Path.GetFileName(path);

                            richTextBox1.LoadFile(@path, RichTextBoxStreamType.RichText);

                            registerAdd(path);
                        }
                    }
                    else
                    {
                        save();
                    }
                }
                else
                {
                    
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        path = openFileDialog1.FileName;

                        file = openFileDialog1;

                        toolStripStatusLabel1.Text = System.IO.Path.GetFileName(path);

                        richTextBox1.LoadFile(@path, RichTextBoxStreamType.RichText);

                        registerAdd(path);
                    }

                    richTextBox1.Modified = false;
                }
            }
            else
            {
                richTextBox1.LoadFile(@path, RichTextBoxStreamType.RichText);

                registerAdd(path);
            }
        }

        private void recentFileToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            file = new OpenFileDialog();
            file.FileName = e.ClickedItem.Text;
            openFile(e.ClickedItem.Text);
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            file = null;

            toolStripStatusLabel1.Text = "Yeni Dosya";

            richTextBox1.Text = "";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = richTextBox1.SelectedText.ToUpper();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = richTextBox1.SelectedText.ToLower();
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] fi = new string[2];

            if (toolStripComboBox1.SelectedItem == null)
            {
                fi[0] = (string)richTextBox1.Font.Name.ToString();
                fi[1] = (string)toolStripComboBox2.SelectedItem;
            }
            else
            {
                fi[0] = (string)toolStripComboBox1.SelectedItem;
                fi[1] = (string)toolStripComboBox2.SelectedItem;
            }

            richTextBox1.SelectionFont = font(fi[0],fi[1]);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] fi = new string[2];

            if (toolStripComboBox2.SelectedItem == null)
            {
                fi[0] = (string)toolStripComboBox1.SelectedItem;
                fi[1] = richTextBox1.Font.Size.ToString();
            }
            else
            {
                fi[0] = (string)toolStripComboBox1.SelectedItem;
                fi[1] = (string)toolStripComboBox2.SelectedItem;
            }

            richTextBox1.SelectionFont = font(fi[0],fi[1]);
        }

        private Font font(string fontname = null, string size = null)
        {
            if (fontname == null)
            {
                if (bold)
                {
                    if (italic && under)
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
                    else if (italic)
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Bold | FontStyle.Italic);
                    else if (under)
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Bold | FontStyle.Underline);
                    else
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Bold);
                }
                else
                {
                    if (italic && under)
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Regular | FontStyle.Italic | FontStyle.Underline);
                    else if (italic)
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Regular | FontStyle.Italic);
                    else if (under)
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Regular | FontStyle.Underline);
                    else
                        f = new Font(richTextBox1.SelectionFont, FontStyle.Regular);
                }
            }
            else
            {
                if (bold)
                {
                    if (italic && under)
                        f = new Font((string)fontname, float.Parse(size) ,FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
                    else if (italic)
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Bold | FontStyle.Italic);
                    else if (under)
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Bold | FontStyle.Underline);
                    else
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Bold);
                }
                else
                {
                    if (italic && under)
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Regular | FontStyle.Italic | FontStyle.Underline);
                    else if (italic)
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Regular | FontStyle.Italic);
                    else if (under)
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Regular | FontStyle.Underline);
                    else
                        f = new Font((string)fontname, float.Parse(size), FontStyle.Regular);
                }
            }
                return f;
        }
        private void toolStripButton3_Click(object sender, EventArgs e) // bold
        {
            if (bold)
            {
                bold = false;
                toolStripButton3.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            else
            {
                bold = true;
                toolStripButton3.BackColor = Color.FromKnownColor(KnownColor.ButtonShadow);
            }

            richTextBox1.SelectionFont = font();
        }

        private void toolStripButton4_Click(object sender, EventArgs e) // italic
        {

            if (italic)
            {
                italic = false;
                toolStripButton4.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            else
            {
                italic = true;
                toolStripButton4.BackColor = Color.FromKnownColor(KnownColor.ButtonShadow);
            }

            richTextBox1.SelectionFont = font();
        }

        private void toolStripButton5_Click(object sender, EventArgs e) // underline
        {
            if (under)
            {
                under = false;
                toolStripButton5.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            else
            {
                under = true;
                toolStripButton5.BackColor = Color.FromKnownColor(KnownColor.ButtonShadow);
            }

            richTextBox1.SelectionFont = font();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            toolStripButton6.BackColor = Color.FromKnownColor(KnownColor.ButtonShadow);
            toolStripButton7.BackColor = Color.FromKnownColor(KnownColor.Control);
            toolStripButton8.BackColor = Color.FromKnownColor(KnownColor.Control);

            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            toolStripButton6.BackColor = Color.FromKnownColor(KnownColor.Control);
            toolStripButton7.BackColor = Color.FromKnownColor(KnownColor.ButtonShadow);
            toolStripButton8.BackColor = Color.FromKnownColor(KnownColor.Control);

            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            toolStripButton6.BackColor = Color.FromKnownColor(KnownColor.Control);
            toolStripButton7.BackColor = Color.FromKnownColor(KnownColor.Control);
            toolStripButton8.BackColor = Color.FromKnownColor(KnownColor.ButtonShadow);

            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (file != null)
            {
                toolStripStatusLabel1.Text = System.IO.Path.GetFileName(file.FileName) + " yazdırılıyor.";

                printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);

                printDialog1.Document = printDocument1;

                string strText = this.richTextBox1.Text;
                myReader = new StringReader(strText);

                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.printDocument1.Print();
                }
                toolStripStatusLabel1.Text = System.IO.Path.GetFileName(file.FileName) + " yazdırıldı.";
            }
            else {
                toolStripStatusLabel1.Text = "Yazdırılacak dosya bulunamadı!.";
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            float linesPerPage = 0;
            float yPosition = 0;
            int count = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            string line = null;
            Font printFont = this.richTextBox1.Font;
            SolidBrush myBrush = new SolidBrush(Color.Black);

            linesPerPage = e.MarginBounds.Height / printFont.GetHeight(e.Graphics);

            while (count < linesPerPage && ((line = myReader.ReadLine()) != null))
            {
                yPosition = topMargin + (count * printFont.GetHeight(e.Graphics));

                e.Graphics.DrawString(line, printFont, myBrush, leftMargin, yPosition, new StringFormat());
                count++;
            }

            if (line != null)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
            myBrush.Dispose();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            richTextBox1.SelectionColor = colorDialog1.Color;
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P)
            {
                toolStripButton9_Click(null, null);
            }
        }
    }
}
