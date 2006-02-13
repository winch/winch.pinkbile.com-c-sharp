/*
dark_explorer
Copyright (C) 2005 the_winch

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
//
//viewItem dialog, shows the contents of a ListViewFileItem
//
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

class viewItem : Form
{
	string exeName;
	public string ExeName
	{
		set
		{
			exeName = value;
		}
	}
	ListViewFileItem item;
	public ListViewFileItem Item
	{
		set
		{
			item = value;
		}
	}
	ComboBox itemType;
	//fileSize / offset label
	Label fileSize;
	//picture box
	PictureBox pictureBox;
	//text box
	TextBox textBox;
	//dll string table listbox
	ListBox listBox;

	public viewItem()
	{
		Text = "View item";
		ShowInTaskbar = false;
		Size = new Size(350, 350);
		StartPosition = FormStartPosition.CenterParent;

		//itemType combobox
		itemType = new ComboBox();
		itemType.Parent = this;
		itemType.DropDownStyle = ComboBoxStyle.DropDownList;
		itemType.Width = 140;
		itemType.Location = new Point(this.Width - itemType.Width - 15, 10);
		itemType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		itemType.Items.Add("_virtual.dat");
		itemType.Items.Add("Compressed or extra data");
		itemType.Items.Add("dll");
		itemType.Items.Add("Text");
		itemType.Items.Add("Image");
		itemType.Items.Add("Hex");
		itemType.SelectedIndexChanged += new EventHandler(itemType_SelectedIndexChanged);

		//filesize label
		fileSize = new Label();
		fileSize.Parent = this;
		fileSize.AutoSize = true;
		fileSize.Location = new Point(10, 10);

		//picturebox
		pictureBox = new PictureBox();
		pictureBox.Parent = this;
		pictureBox.BorderStyle = BorderStyle.FixedSingle;
		pictureBox.Location = new Point(10, 40);
		pictureBox.Size = new Size(this.Width - 30, this.Height - 80);
		pictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
		pictureBox.Visible = false;

		//textbox
		textBox = new TextBox();
		textBox.Parent = this;
		textBox.Multiline = true;
		textBox.ScrollBars = ScrollBars.Both;
		textBox.Location = pictureBox.Location;
		textBox.Size = pictureBox.Size;
		textBox.Anchor = pictureBox.Anchor;
		textBox.Visible = false;

		//listbox
		listBox = new ListBox();
		listBox.Parent = this;
		listBox.Location = pictureBox.Location;
		listBox.Size = pictureBox.Size;
		listBox.Anchor = pictureBox.Anchor;
		listBox.Visible = false;

		this.Load += new System.EventHandler(viewItem_Load);
	}
 	private void viewItem_Load(object sender, System.EventArgs e)
	{
		Cursor.Current = Cursors.WaitCursor;
		guessFileType();
		getFileSize();
		showItem();
		this.Text += " (" + item.SubItems[0].Text + ")";
		Cursor.Current = Cursors.Default;
	}

	private void showItem()
	{
		//choses the correct showXXX function for the item type
		if (itemType.SelectedIndex == itemType.Items.IndexOf("Image"))
		{
			//image
			pictureBox.Visible = true;
			showImage();
		}
		else if (itemType.SelectedIndex == itemType.Items.IndexOf("Text"))
		{
			//text
			textBox.Visible = true;
			showText();
		}
		else if (itemType.SelectedIndex == itemType.Items.IndexOf("dll"))
		{
			//dll
			listBox.Visible = true;
			showDll();
		}
		else if (itemType.SelectedIndex == itemType.Items.IndexOf("_virtual.dat"))
		{
			//_virtual.dat
			textBox.Visible = true;
			showVirtualDat();
		}
		else if (itemType.SelectedIndex == itemType.Items.IndexOf("Compressed or extra data"))
		{
			//_virtual.dat
			textBox.Visible = true;
			showExtraData();
		}
		else if (itemType.SelectedIndex == itemType.Items.IndexOf("Hex"))
		{
			//hex
			textBox.Visible = true;
			showHex();
		}
	}

	private void showDll()
	{
		//display dll string table in listbox
		uint hinst = 0;
		FileStream fsIn = null;
		BinaryReader brIn = null;
		FileStream fsOut = null;
		BinaryWriter bwOut = null;
		string fileName = "";
		listBox.Items.Clear();
		listBox.BeginUpdate();
		try
		{
			if (item.SubItems[5].Text == "<exe>")
			{
				//internal file
				fileName = Path.GetTempFileName();
				fsOut = new FileStream(fileName, FileMode.Create);
				bwOut = new BinaryWriter(fsOut);
				fsIn = new FileStream(exeName, FileMode.Open);
				brIn = new BinaryReader(fsIn);
				fsIn.Seek(item.Offset, SeekOrigin.Current);
				bwOut.Write(brIn.ReadBytes(item.Size));
				bwOut.Close();
				fsOut.Close();
				hinst = LoadLibrary(fileName);
			}
			else
			{
				//external file
				hinst = LoadLibrary(item.SubItems[5].Text);
			}
			if (hinst != 0)
			{
				//read string table
				StringBuilder sb = new StringBuilder(255);
				int s = 1;
				int bad = 0;
				while (bad < 5)
				{
					if (LoadString(hinst, s, sb, 255) > 0)
					{
						listBox.Items.Add(sb.ToString());
					}
					else
					{
						bad ++;
					}
					s ++;
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			FreeLibrary(hinst);
			if (fileName != "")
				File.Delete(fileName);
			if (brIn != null)
				brIn.Close();
			if (fsIn != null)
				fsIn.Close();
			if (bwOut != null)
				bwOut.Close();
			if (fsOut != null)
				fsOut.Close();
		}
		listBox.EndUpdate();
	}

	private void showExtraData()
	{
		//show size of exeSection in textbox
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			if (item.SubItems[5].Text == "<exe>")
			{
				//internal file
				fs = new FileStream(exeName, FileMode.Open);
				br = new BinaryReader(fs);
				fs.Seek(item.Offset, SeekOrigin.Current);
			}
			else
			{
				//external file
				fs = new FileStream(item.SubItems[5].Text, FileMode.Open);
				br = new BinaryReader(fs);
			}
			fs.Seek(fs.Length - 4, SeekOrigin.Begin);
			textBox.Text = "Exe Section size = " + br.ReadInt32().ToString("n0");
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (br != null)
				br.Close();
			if (fs != null)
				fs.Close();
		}
	}

	private void showVirtualDat()
	{
		//show _virtual.dat info in textbox
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			if (item.SubItems[5].Text == "<exe>")
			{
				//internal file
				fs = new FileStream(exeName, FileMode.Open);
				br = new BinaryReader(fs);
				fs.Seek(item.Offset, SeekOrigin.Current);
			}
			else
			{
				//external file
				fs = new FileStream(item.SubItems[5].Text, FileMode.Open);
				br = new BinaryReader(fs);
			}
			textBox.Text = "";
			char c;
			int mode = br.ReadInt32();
			string modeString;
			switch (mode)
			{
				case 0:
					modeString = "Hidden";
					break;
				case 1:
					modeString = "Windowed";
					break;
				case 2:
					modeString = "Windowed desktop";
					break;
				case 3:
					modeString = "Fullscreen exclusive";
					break;
				case 4:
					modeString = "Windowed fullscreen";
					break;
				default:
					modeString = "Unknown";
					break;
			}
			textBox.Text += "Mode " + mode.ToString() + " (" + modeString + ")\r\n";
			textBox.Text += "Width " + br.ReadInt32().ToString() + "\r\n";
			textBox.Text += "Height " + br.ReadInt32().ToString() + "\r\n";
			textBox.Text += "Depth " + br.ReadInt32().ToString() + "\r\n";
			textBox.Text += "Window caption :";
			//read window caption
			c = 'a';
			while (c > 0)
			{
				c = br.ReadChar();
				textBox.Text += c.ToString();
			}
			textBox.Text += "\r\n";
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (br != null)
				br.Close();
			if (fs != null)
				fs.Close();
		}
	}

	private void showHex()
	{
		//display file in hex
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			if (item.SubItems[5].Text == "<exe>")
			{
				//internal file
				fs = new FileStream(exeName, FileMode.Open);
				br = new BinaryReader(fs);
			}
			else
			{
				//external file
				fs = new FileStream(item.SubItems[5].Text, FileMode.Open);
				br = new BinaryReader(fs);
			}
			textBox.Text = "Not done yet.";
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (br != null)
				br.Close();
			if (fs != null)
				fs.Close();
		}
	}

	private void showText()
	{
		//display text in textbox
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			if (item.SubItems[5].Text == "<exe>")
			{
				//internal file
				fs = new FileStream(exeName, FileMode.Open);
				br = new BinaryReader(fs);
				fs.Seek(item.Offset, SeekOrigin.Current);
				textBox.Text = Encoding.UTF8.GetString(br.ReadBytes(item.Size));
			}
			else
			{
				//external file
				fs = new FileStream(item.SubItems[5].Text, FileMode.Open);
				br = new BinaryReader(fs);
				textBox.Text = Encoding.UTF8.GetString(br.ReadBytes((int)fs.Length));
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (br != null)
				br.Close();
			if (fs != null)
				fs.Close();
		}
	}

	private void showImage()
	{
		//display image in picturebox
		FileStream fs = null;
		try
		{
			if (item.SubItems[5].Text == "<exe>")
			{
				//internal file
				byte[] buffer = new byte[item.Size];
				fs = new FileStream(exeName, FileMode.Open);
				fs.Seek(item.Offset, SeekOrigin.Begin);
				MemoryStream img = new MemoryStream(item.Size);
				fs.Read(buffer, 0, item.Size);
				img.Write(buffer, 0, item.Size);
				pictureBox.Image = Image.FromStream(img);
			}
			else
			{
				//external file
				fs = new FileStream(item.SubItems[5].Text, FileMode.Open);
				pictureBox.Image = Image.FromStream(fs);
			}
		}
		catch (ArgumentException)
		{
			MessageBox.Show("Not a valid image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (fs != null)
				fs.Close();
		}
	}

	private void getFileSize()
	{
		//set label with file size and offset
		fileSize.Text = "File offset: " + item.Offset.ToString("n0") + " File size: " + item.Size.ToString("n0");
	}
	
	private void guessFileType()
	{
		//guess the type of item from filename extension
		switch (Path.GetExtension(item.SubItems[0].Text))
		{
			case ".dll":
				itemType.SelectedIndex = itemType.Items.IndexOf("dll");
				break;
			case ".txt":
			case ".fx":
				itemType.SelectedIndex = itemType.Items.IndexOf("Text");
				break;
			case ".dat":
				if (item.SubItems[0].Text == "_virtual.dat")
					itemType.SelectedIndex = itemType.Items.IndexOf("_virtual.dat");
				else
					itemType.SelectedIndex = itemType.Items.IndexOf("Other");
				break;
			case ".bmp":
			case ".jpg":
			case ".tgs":
			case ".dds":
			case ".png":
			case ".ico":
				itemType.SelectedIndex = itemType.Items.IndexOf("Image");
				break;
			default:
				if (item.SubItems[0].Text == "Compressed or extra data")
					itemType.SelectedIndex = itemType.Items.IndexOf("Compressed or extra data");
				else
					itemType.SelectedIndex = itemType.Items.IndexOf("Hex");
				break;
		}
	}

	private void itemType_SelectedIndexChanged(object sender, EventArgs e)
	{
		//itemtype changed
		Cursor.Current = Cursors.WaitCursor;
		//hide all containers
		pictureBox.Visible = false;
		textBox.Visible = false;
		listBox.Visible = false;
		showItem();
		Cursor.Current = Cursors.Default;
	}

	[DllImport("user32.dll", EntryPoint="LoadStringA")]
	public static extern int LoadString(uint hinst, int id, StringBuilder buffer, int bufferMax);

	[DllImport("kernel32.dll", EntryPoint="LoadLibrary")]
	public static extern uint LoadLibrary(string fileName);

	[DllImport("kernel32.dll", EntryPoint="FreeLibrary")]
	public static extern void FreeLibrary(uint hinst);
}