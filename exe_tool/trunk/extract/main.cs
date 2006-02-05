//
// main window stuff
//
// see readme.txt for copyright and license info

/*
Todo

Remove string tables from dlls for plugin protection
Show dll information, string tables etc
*/

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Win3;
using System.Diagnostics;

class window : Form
{

	ListView list;
	Label labName;
	Win3_Button btnLoad;
	Win3_Button btnSave;
	CheckBox chkCompress;
	CheckBox chkNull;
	Win3_Button btnExtract;
	Win3_Button btnUp;
	Win3_Button btnDown;
	GroupBox displayBox;
	ComboBox displayType;
	TextBox displayWidth;
	TextBox displayHeight;
	TextBox displayDepth;

	//when an exe is draged and droped this is set to the path of the exe
	//so extract/save dialogs can point to it
	string exePath = "";

	[STAThreadAttribute]
	public static void Main()
	{
		Application.Run(new window());
	}
	public window()
	{
		Text = "Dbpro (and fpsc) exe tool";
		Size = new Size(630,600);

		int y = 10;

		// Load button
		btnLoad = new Win3_Button();
		btnLoad.Parent = this;
		btnLoad.Text = "&Load exe";
		btnLoad.Width += 50;
		btnLoad.Location = new Point(this.Width - btnLoad.Width -15,y);
		btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnLoad.Click += new EventHandler(btnLoad_Click);
		y += 30;

		// Save button
		btnSave = new Win3_Button();
		btnSave.Parent = this;
		btnSave.Text = "&Save exe";
		btnSave.Width += 50;
		btnSave.Location = new Point(this.Width - btnSave.Width -15,y);
		btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnSave.Click += new EventHandler(btnSave_Click);
		y += 25;

		//compress with upx on save checkbox
		chkCompress = new CheckBox();
		chkCompress.Parent = this;
		chkCompress.Text = "use UPX on save";
		chkCompress.Width = btnSave.Width;
		chkCompress.Location = new Point(this.Width - chkCompress.Width -15, y);
		chkCompress.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		//disable if we can't find "upx.exe" in appdir
		if (!File.Exists(Application.StartupPath + "\\upx.exe"))
			chkCompress.Enabled = false;
		y += 25;

		//Null dll resource checkbox
		chkNull = new CheckBox();
		chkNull.Parent = this;
		chkNull.Text = "Null dll resources";
		chkNull.Width = btnSave.Width;
		chkNull.Location = new Point(this.Width - chkNull.Width - 15, y);
		chkNull.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		y += 25;

		// Extract button
		btnExtract = new Win3_Button();
		btnExtract.Parent = this;
		btnExtract.Text = "E&xtract File(s)";
		btnExtract.Width += 50;
		btnExtract.Location = new Point(this.Width - btnExtract.Width -15,y);
		btnExtract.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnExtract.Enabled = false;
		btnExtract.Click += new EventHandler(btnExtract_Click);
		y += 30;
		
		//About button
		Win3_Button btnAbout = new Win3_Button();
		btnAbout.Parent = this;
		btnAbout.Text = "&About";
		btnAbout.Width += 50;
		btnAbout.Location = new Point(this.Width - btnAbout.Width - 15,y);
		btnAbout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnAbout.Click += new EventHandler(btnAbout_Click);
		y += 30;

		//Exit button
		Win3_Button btnExit = new Win3_Button();
		btnExit.Parent = this;
		btnExit.Text = "E&xit";
		btnExit.Width += 50;
		btnExit.Location = new Point(this.Width - btnExit.Width - 15,y);
		btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnExit.Click += new EventHandler(btnExit_Click);
		y += 30;

		//display setting stuff
		displayBox = new GroupBox();
		displayBox.Parent = this;
		displayBox.Text = "Display settings";
		displayBox.Width = btnExit.Width;
		displayBox.Height = 120;
		displayBox.Location = new Point(this.Width - displayBox.Width - 15,y);
		displayBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		displayBox.Enabled = false;

		//width label
		Label label = new Label();
		label.Parent = displayBox;
		label.Text = "Width";
		label.AutoSize = true;
		label.Location = new Point(5,20);

		//width textdisplayBox
		displayWidth = new TextBox();
		displayWidth.Parent = displayBox;
		displayWidth.Width = displayBox.Width - label.Width - 20;
		displayWidth.Location = new Point(label.Left + label.Width + 5,15);

		//height label
		label = new Label();
		label.Parent = displayBox;
		label.Text = "Height";
		label.AutoSize = true;
		label.Location = new Point(5,45);

		//height textBox
		displayHeight = new TextBox();
		displayHeight.Parent = displayBox;
		displayHeight.Width = displayBox.Width - label.Width - 20;
		displayHeight.Location = new Point(label.Left + label.Width + 5,40);

		//depth label
		label = new Label();
		label.Parent = displayBox;
		label.Text = "Depth";
		label.AutoSize = true;
		label.Location = new Point(5,70);

		//depth textBox
		displayDepth = new TextBox();
		displayDepth.Parent = displayBox;
		displayDepth.Width = displayBox.Width - label.Width - 20;
		displayDepth.Location = new Point(label.Left + label.Width + 5, 65);

		//type textBox
		displayType = new ComboBox();
		displayType.DropDownStyle = ComboBoxStyle.DropDownList;
		displayType.Items.Add("hidden");
		displayType.Items.Add("windowed");
		displayType.Items.Add("windowed desktop");
		displayType.Items.Add("fullscreen exclusive");
		displayType.Items.Add("windowed fullscreen");
		displayType.SelectedIndex = 1;
		displayType.Parent = displayBox;
		displayType.Width = displayBox.Width - 20;
		displayType.Location = new Point(10,90);
		y += displayBox.Height + 10;

		//Up button
		btnUp = new Win3_Button();
		btnUp.Parent = this;
		btnUp.Text = "/\\";
		btnUp.Width = 30;
		btnUp.Height = 60;
		btnUp.Location = new Point(this.Width - btnUp.Width - 110,y);
		btnUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnUp.Enabled = false;
		btnUp.Click += new EventHandler(btnUp_Click);
		y += btnUp.Height + 10;

		//down button
		btnDown = new Win3_Button();
		btnDown.Parent = this;
		btnDown.Text = "\\/";
		btnDown.Width = 30;
		btnDown.Height = 60;
		btnDown.Location = new Point(this.Width - btnUp.Width - 110,y);
		btnDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnDown.Enabled = false;
		btnDown.Click += new EventHandler(btnDown_Click);
		y += btnDown.Height + 10;

		// exe name label
		labName = new Label();
		labName.Parent = this;
		labName.Text = "";
		labName.Location = new Point(10,10);
		labName.AutoSize = true;

		// file listview
		list = new ListView();
		list.Parent = this;
		list.Location = new Point(10,30);
		list.Size = new Size(470,535);
		list.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
		list.View = View.Details;
		list.Columns.Add("Name",210,HorizontalAlignment.Left);
		list.Columns.Add("File size",85,HorizontalAlignment.Right);
		list.Columns.Add("Location",170,HorizontalAlignment.Left);
		list.SelectedIndexChanged += new EventHandler(list_SelectedIndexChanged);

		// listview popup menu
		EventHandler mInsert  = new EventHandler(mInsertOnClick);
		EventHandler mInsertmedia = new EventHandler(mInsertmediaOnClick);
		EventHandler mRemove  = new EventHandler(mRemoveOnClick);
		EventHandler mEdit    = new EventHandler(mEditOnClick);
		EventHandler mExtract = new EventHandler(mExtractOnClick);
		EventHandler mCopy    = new EventHandler(mCopyOnClick);

		MenuItem[] ami = { new MenuItem("Insert", mInsert), new MenuItem("Insert wildcard", mInsertmedia), new MenuItem("Remove", mRemove), new MenuItem("Edit name", mEdit), new MenuItem("Extract", mExtract),
			new MenuItem("Copy list to clipboard",mCopy) };
		list.ContextMenu = new ContextMenu(ami);
		list.ContextMenu.MenuItems[2].Enabled = false;
		list.ContextMenu.MenuItems[3].Enabled = false;
		list.ContextMenu.MenuItems[4].Enabled = false;
		list.ItemActivate += mEdit;

		//drag drop stuff
		AllowDrop = true;
		DragOver += new DragEventHandler(window_DragOver);
		DragDrop += new DragEventHandler(window_DragDrop);
	}

	private void mInsertOnClick(object obj, EventArgs ea)
	{
		//insert new file above selected item
		if (list.SelectedItems.Count > 1)
		{
			MessageBox.Show("Can not insert with more than one item selected.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			return;
		}
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Select file";
		dlg.Filter = "All Files (*.*)|*.*|Dll Files (*.dll)|*.dll";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			try
			{
				string length;
				long len;
				FileStream fs = new FileStream(dlg.FileName,FileMode.Open);
				BinaryReader br = new BinaryReader(fs);
				len = fs.Length;
				// check for _virtual.dat and read display info
				if (Path.GetFileName(dlg.FileName) == "_virtual.dat")
				{
					displayBox.Enabled = true;
					displayType.SelectedIndex = br.ReadInt32();
					displayWidth.Text = br.ReadInt32().ToString();
					displayHeight.Text = br.ReadInt32().ToString();
					displayDepth.Text = br.ReadInt32().ToString();
				}
				br.Close();
				fs.Close();
				if (len > 1024)
				{
					len /= 1024;
					length = len.ToString() + " kB";
				}
				else
				{
					length = len.ToString() + " By";
				}
				ListViewItem itm = new ListViewItem(Path.GetFileName(dlg.FileName));
				itm.SubItems.Add(length);
				itm.SubItems.Add(dlg.FileName);
				if (list.SelectedItems.Count == 0)
				{
					//if there are no selected items add it to the bottom
					list.Items.Add(itm);
				}
				else
				{
					//there is one selcted item so insert above it
					list.Items.Insert(list.SelectedItems[0].Index,itm);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}
		dlg.Dispose();
	}
	private void mInsertmediaOnClick(object obj, EventArgs ea)
	{
		//insert media with wildcards
		string path = "";
		FolderBrowserDialog browse = new FolderBrowserDialog();
		browse.ShowNewFolderButton = false; //does fuck all for me, do it anyway incase it works for others
		//aim browse dialog at directory of opened dbpro exe
		if (labName.Text != "")
		 {
			 browse.SelectedPath = Path.GetDirectoryName(labName.Text);
			 path = Path.GetDirectoryName(labName.Text);
		 }
		if (browse.ShowDialog() == DialogResult.OK)
		{
			filterDialog dlg = new filterDialog();
			dlg.Exepath = path;
			dlg.Addpath = browse.SelectedPath;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				list.BeginUpdate();
				try
				{
					string[] files = Directory.GetFiles(browse.SelectedPath, dlg.filter);
					foreach (string str in files)
					{
						long len;
						string length;
						string name = dlg.prefix + Path.GetFileName(str);
						try
						{
							FileStream fs = new FileStream(str,FileMode.Open);
							BinaryReader br = new BinaryReader(fs);
							len = fs.Length;
							// check for _virtual.dat and read display info
							if (name == "_virtual.dat")
							{
								displayBox.Enabled = true;
								displayType.SelectedIndex = br.ReadInt32();
								displayWidth.Text = br.ReadInt32().ToString();
								displayHeight.Text = br.ReadInt32().ToString();
								displayDepth.Text = br.ReadInt32().ToString();
							}
							br.Close();
							fs.Close();
							if (len > 1024)
							{
								len /= 1024;
								length = len.ToString() + " kB";
							}
							else
							{
								length = len.ToString() + " By";
							}
						}
						catch
						{
							length = "unknown";
						}
						ListViewItem itm = new ListViewItem(name);
						itm.SubItems.Add(length);
						itm.SubItems.Add(str);
						if (list.SelectedItems.Count == 0)
						{
							//if there are no selected items add it to the bottom
							list.Items.Add(itm);
						}
						else
						{
							//there is one selcted item so insert above it
							list.Items.Insert(list.SelectedItems[0].Index,itm);
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(),"Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}				finally
				{
					list.EndUpdate();
				}
			}
		}
	}
	private void mRemoveOnClick(object obj, EventArgs ea)
	{
		//remove all selected items
		foreach (ListViewItem items in list.SelectedItems)
		{
			items.Remove();
		}
	}

	private void mEditOnClick(object obj, EventArgs ea)
	{
		//edit selected item name if it is not in the <exe>
		if (list.SelectedItems.Count > 1)
		{
			MessageBox.Show("More than one file selected");
			return;
		}

		if(list.SelectedItems.Count == 1)
		{
			//make sure selected item isn't in <exe>
			if (list.SelectedItems[0].SubItems[2].Text == "<exe>")
			{
				MessageBox.Show("Can't rename files in <exe>","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				return;
			}
			//get new name
			editItemDialog dlg = new editItemDialog(list.SelectedItems[0].Text);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				list.SelectedItems[0].Text = dlg.name;
			}
		}
	}

	private void mExtractOnClick(object obj, EventArgs ea)
	{
		btnExtract_Click(new object(), new EventArgs());
	}

	private void mCopyOnClick(object obj, EventArgs ea)
	{
		// Copies a list of the files in the exe to the clipboard
		string text = "";
		if (list.Items.Count > 0)
		{
			int max = 0; //length to pad names
			int pad = 0; //ammount to pad name
			int sizeMax = 0; // length to pad file size
			int sizePad = 0; // ammount to pad file size
			string spaces, sizeSpaces;
			//find length of longest name
			foreach (ListViewItem items in list.Items)
			{
				if (items.Text.Length > max)
					max = items.Text.Length;
				if (items.SubItems[1].Text.Length > sizeMax)
					sizeMax = items.SubItems[1].Text.Length;
			}
			max += 1; //make max one longer than longest item so it has a space after it
			foreach (ListViewItem items in list.Items)
			{
				spaces = "";
				sizeSpaces = "";
				pad = max - items.Text.Length;
				for (int i=0; i< pad; i++)
					spaces += " ";
				sizePad = sizeMax - items.SubItems[1].Text.Length;
				for (int i=0; i<sizePad; i++)
					sizeSpaces += " ";
				text += items.Text + spaces + "| " + sizeSpaces + items.SubItems[1].Text + "\r\n";
			}
			Clipboard.SetDataObject(text, true);
		}
	}

	private void btnLoad_Click(object sender, EventArgs e)
	{
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Load Darkbasic Pro exe";
		dlg.Filter = "Dbpro exe and pack files (*.exe,*.pck)|*.exe;*.pck|All Files (*.*)|*.*";
		if (exePath != "")
		{
			dlg.InitialDirectory = exePath;
			exePath = "";
		}
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			load(dlg.FileName);
		}
		dlg.Dispose();
	}

	private void load(string FileName)
	{
		bool pack = true; // is the file a .pck file
		string fileLength = "";
		int length = 0;
		// clear listview
		list.Items.Clear();
		//show busy cursor
		Cursor.Current = Cursors.WaitCursor;
		if (Path.GetExtension(FileName) == ".exe")
			pack = false;
		string find = "";
		FileStream fs;
		BinaryReader br;
		try
		{
			fs = new FileStream(FileName,FileMode.Open);
			br = new BinaryReader(fs);
			labName.Text = FileName;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			Cursor.Current = Cursors.Default;
			return;
		}
		//find PADDINGXXPAD, files start right after
		if (pack == false)
		{
			try
			{
				string start = "PADDINGXXPAD";
				byte[] pad = new byte[12] {0,0,0,0,0,0,0,0,0,0,0,0};
				byte b = 0;
				int i;
				//skip some bytes to speed it up
				fs.Seek(1024*60,SeekOrigin.Current);
				while (find != start && fs.Position < 1024*100)
				{
					b = br.ReadByte();
					//shift the pad array to the left
					for (i = 1; i<12; i++)
						pad[i-1] = pad[i];
					pad[11] = b;
					//conver pad array to string
					find = "";
					for (i=0; i<12; i++)
					{
						find = find + Convert.ToChar(pad[i]);
					}
				}
				//since PADDINGXXPAD may just be the start of a lot of padding continue reading
				//until a value not in PADDINGXXPAD is found
				if (find == start)
				{
					while (start.IndexOf(Convert.ToChar(b), 0) != -1)
					{
						//MessageBox.Show(start.IndexOf(Convert.ToChar(b), 0).ToString());
						b = br.ReadByte();
					}
					//skip back one byte
					fs.Seek(-1 ,SeekOrigin.Current);
				}
				else
				{
					//if we haven't found "PADDINGXXPAD" guess where end of standard exe header is
					fs.Seek(73728,SeekOrigin.Begin);
					//give a warning as it may not actually be a dbpro exe
					MessageBox.Show("PADDINGXXPAD not found\nLoaded exe may not be a dbpro exe","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}
				// add standard exe head to listview
				length = (int) fs.Position;
				ListViewItem itm = new ListViewItem("Standard Exe Header");
				if (length > 1024)
				{
					length /= 1024;
					fileLength = length.ToString() + " kB";
				}
				else
				{
					fileLength = length.ToString() + " By";
				}
				itm.SubItems.Add(fileLength);
				itm.SubItems.Add("<exe>");
				list.Items.Add(itm);
			}
			catch (Exception ex)
			{
				br.Close();
				fs.Close();
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				Cursor.Current = Cursors.Default;
				return;
			}
		}
		//get files in exe and put in listview
		try
		{
			int i;
			string name;
			list.BeginUpdate();
			while (true)
			{
				//filename length
				length = br.ReadInt32();
				if (length > 0 && length < 100)
				{
					name = "";
					for (i=0;i<length;i++)
					{
						name += Convert.ToChar(br.ReadByte());
					}
					//get file length
					length = br.ReadInt32();
					//if name is "_virtual.dat" get display settings from it
					if (name == "_virtual.dat")
					{
						displayBox.Enabled = true;
						displayType.SelectedIndex = br.ReadInt32();
						displayWidth.Text = br.ReadInt32().ToString();
						displayHeight.Text = br.ReadInt32().ToString();
						displayDepth.Text = br.ReadInt32().ToString();
						//seek over file less the bytes we just read
						fs.Seek(length - 16,SeekOrigin.Current);
					}
					else
					{
						fs.Seek(length,SeekOrigin.Current);
					}
					//put info in listview
					ListViewItem itm = new ListViewItem(name);
					if (length > 1024)
					{
						length /= 1024;
						fileLength = length.ToString() + " kB";
					}
					else
					{
						fileLength = length.ToString() + " By";
					}
					itm.SubItems.Add(fileLength);
					itm.SubItems.Add("<exe>");
					list.Items.Add(itm);
				}
				else
				{
					//extra info at end of file or fileblock
					length = 4 + (int)(fs.Length - fs.Position);
					ListViewItem itm = new ListViewItem("Extra or Compressed block");
					if (length > 1024)
					{
						length /= 1024;
						fileLength = length.ToString() + " kB";
					}
					else
					{
						fileLength = length.ToString() + " By";
					}
					itm.SubItems.Add(fileLength);
					itm.SubItems.Add("<exe>");
					list.Items.Add(itm);
					br.Close();
					fs.Close();
					Cursor.Current = Cursors.Default;
					list.EndUpdate();
					return;
				}
			}
		}
		catch
		{
			br.Close();
			fs.Close();
		}

		br.Close();
		fs.Close();
		list.EndUpdate();
		Cursor.Current = Cursors.Default;
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		//saves a new exe or pck file using info in listview
		if (list.Items.Count == 0)
		{
			MessageBox.Show("Nothing to save.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			return;
		}
		SaveFileDialog dlg = new SaveFileDialog();
		dlg.Title = "Save New Darkbasic Pro exe";
		dlg.Filter = "Exe Files (*.exe)|*.exe|Packfile (*.pck)|*.pck|All Files (*.*)|*.*";
		FileStream fsIn;
		BinaryReader brIn;
		FileStream fsOut;
		BinaryWriter brOut;
		FileStream fsExt = null;
		BinaryReader brExt = null;  //external file
		FileStream fsUpx;    //for read & write file for upx compression
		BinaryReader brUpx;
		BinaryWriter bwUpx;
		FileStream fsNull;  //for nulling resources
		BinaryReader brNull;
		long headPos = 0; // position of end of head
		long footPos = 0; // position of extra info at end of exe
		string upx_options = "--best --crp-ms=999999"; //command line options used with upx
		//set dialog to exePath if required
		if (exePath != "")
		{
			dlg.InitialDirectory = exePath;
			exePath = "";
		}
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			if (dlg.FileName == labName.Text)
			{
				MessageBox.Show("Need to extract files from original exe!\nChoose another filename","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				return;
			}
			bool pack = true;    // is the input a .Pck file
			bool inFile = true;  // is there an input file 
			//save new exe
			if (labName.Text == "")
			{
				//no file loaded so we have to make pack file
				pack = true;
				inFile = false;
			}
			if (Path.GetExtension(dlg.FileName) == ".exe")
				pack = false;
			try
			{
				//open output file
				fsOut = new FileStream(dlg.FileName,FileMode.Create);
				brOut = new BinaryWriter(fsOut);
			}
			catch (Exception ea)
			{
				MessageBox.Show(ea.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				return;
			}
			if (inFile == true)
			{
				//open input file
				try
				{
					fsIn = new FileStream(labName.Text,FileMode.Open);
					brIn = new BinaryReader(fsIn);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					return;
				}
				//if input file is not a pack file find where files start  and end
				if (pack == false)
				{
					try
					{
						string start = "PADDINGXXPAD";
						string find = "";
						byte[] pad = new byte[12] {0,0,0,0,0,0,0,0,0,0,0,0};
						byte b = 0;
						int i;
						//skip some bytes to speed it up
						fsIn.Seek(1024*60,SeekOrigin.Current);
						while (find != start && fsIn.Position < 1024*100)
						{
							b = brIn.ReadByte();
							//shift the pad array to the left
							for (i = 1; i<12; i++)
								pad[i-1] = pad[i];
							pad[11] = b;
							//conver pad array to string
							find = "";
							for (i=0; i<12; i++)
							{
								find = find + Convert.ToChar(pad[i]);
							}
						}
						//since PADDINGXXPAD may just be the start of a lot of padding continue reading
						//until a value not in PADDINGXXPAD is found
						if (find == start)
						{
							while (start.IndexOf(Convert.ToChar(b), 0) != -1)
							{
								//MessageBox.Show(start.IndexOf(Convert.ToChar(b), 0).ToString());
								b = brIn.ReadByte();
							}
							//skip back one byte
							fsIn.Seek(-1 ,SeekOrigin.Current);
						}
						else
						{
							//if we haven't found "PADDINGXXPAD" guess where end of standard exe header is
							fsIn.Seek(73728,SeekOrigin.Begin);
						}
						//input file now at end of header
						headPos = fsIn.Position;
						//find position of info at end of file
						try
						{
							int nameLength, fileLength;
							while (footPos == 0)
							{
								nameLength = brIn.ReadInt32();
								if (nameLength > 0 && nameLength < 100)
								{
									fsIn.Seek(nameLength,SeekOrigin.Current);
									fileLength = brIn.ReadInt32();
									fsIn.Seek(fileLength,SeekOrigin.Current);
								}
								else
								{
									footPos = fsIn.Position - 4;
								}
							}
						}
						catch
						{
							// do nothing here
						}
						//close infile
						brIn.Close();
						fsIn.Close();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
						brIn.Close();
						fsIn.Close();
						return;
					}
				}
			}
			//go through each item in listview and append it to the output file
			foreach (ListViewItem items in list.Items)
			{
				//check location of file
				if (items.SubItems[2].Text == "<exe>")
				{
					fsIn = new FileStream(labName.Text,FileMode.Open);
					brIn = new BinaryReader(fsIn);
					//item is in exe
					if (items.Text == "Standard Exe Header" && inFile == true)
					{
						//write head
						brOut.Write(brIn.ReadBytes((int)headPos));
					}
					else
					{
						if (items.Text == "Extra or Compressed block" && inFile == true)
						{
							//write footer
							fsIn.Seek(footPos,SeekOrigin.Begin);
							brOut.Write(brIn.ReadBytes((int)(fsIn.Length-footPos)));
						}
						else
						{
							//nomal file
							int length = 0;
							string name = "";
							//seek to start of files
							fsIn.Seek(headPos,SeekOrigin.Begin);
							while (name != items.Text)
							{
								//get name length
								length = brIn.ReadInt32();
								//get name
								name = "";
								for (int i=0; i<length; i++)
									name += Convert.ToChar(brIn.ReadByte());
								length = brIn.ReadInt32();
								fsIn.Seek(length,SeekOrigin.Current);
							}
							//seek back file length
							fsIn.Seek(length-(length*2),SeekOrigin.Current);
							//write name
							brOut.Write(name.Length);
							for (int i=0; i<name.Length; i++)
								brOut.Write(name[i]);
							//check if the fie is a .dll and if we need to upx or null resources
							if (items.Text.EndsWith(".dll"))
							{
								if (chkCompress.Checked == true || chkNull.Checked == true)
								{
									//write the dll to disk
									fsUpx = new FileStream("___dll.dll", FileMode.Create);
									bwUpx = new BinaryWriter(fsUpx);
									bwUpx.Write(brIn.ReadBytes(length));
									bwUpx.Close();
									fsUpx.Close();
									if (chkNull.Checked == true)
									{
										//null resources
										//input file
										fsNull = new FileStream("___dll.dll", FileMode.Open);
										brNull = new BinaryReader(fsNull);
										//output file
										fsUpx = new FileStream("___upx.dll", FileMode.Create);
										bwUpx = new BinaryWriter(fsUpx);
										//skip dos stub
										fsNull.Seek(60, SeekOrigin.Current);
										int e_lfanew = brNull.ReadInt32();
										fsNull.Seek(e_lfanew + 4, SeekOrigin.Begin);
										//IMAGE_FILE_HEADER
										fsNull.Seek(2, SeekOrigin.Current);
										int NumberOfSections = brNull.ReadInt16();
										fsNull.Seek(12, SeekOrigin.Current);
										int SizeOfOptionalHeader = brNull.ReadInt16();
										fsNull.Seek(2, SeekOrigin.Current);
										//end of IMAGE_FILE_HEADER
										//IMAGE_OPTIONAL_HEADER
										fsNull.Seek(36, SeekOrigin.Current);
										int SectionAlignment = brNull.ReadInt32();
										fsNull.Seek(56, SeekOrigin.Current);
										//DataDirectory[16]
										//seek to resouce index
										fsNull.Seek(16, SeekOrigin.Current);
										int VirtualAddress = brNull.ReadInt32();
										int Size = brNull.ReadInt32();
										fsNull.Seek(104, SeekOrigin.Current);
										//end of IMAGE_OPTIONAL_HEADER
										//section directories
										int rsrcSize = 0; // size of resource data
										int rsrcPos = 0;  // position of resource data
										for (int r=0; r<NumberOfSections; r++)
										{
											string rName = System.Text.Encoding.ASCII.GetString(brNull.ReadBytes(8));
											if (rName.Substring(-0,5) == ".rsrc")
											{
												//resource section directory
												fsNull.Seek(8, SeekOrigin.Current);
												rsrcSize = brNull.ReadInt32();
												rsrcPos = brNull.ReadInt32();
												fsNull.Seek(16, SeekOrigin.Current);
											}
											else
											{
												fsNull.Seek(32, SeekOrigin.Current);
											}
										}
										//end of section directories
										//write upto resource data
										fsNull.Seek(0,SeekOrigin.Begin);
										bwUpx.Write(brNull.ReadBytes(rsrcPos));
										//write empty resource data
										for (int r=0; r<rsrcSize; r++)
											bwUpx.Write((byte)0);
										//write rest of file
										fsNull.Seek(rsrcSize, SeekOrigin.Current);
										bwUpx.Write(brNull.ReadBytes((int)(fsNull.Length - fsNull.Position)));
										//close files
										brNull.Close();
										fsNull.Close();
										File.Delete("___dll.dll");
										bwUpx.Close();
										fsUpx.Close();
									}
									else
									{
										File.Move("___dll.dll", "___upx.dll");
									}
									if (chkCompress.Checked == true)
									{
										//compress with upx
										Process process = Process.Start(Application.StartupPath + "\\upx.exe",upx_options + " ___upx.dll");
										process.WaitForExit();
									}
									//write to brOut
									fsUpx = new FileStream("___upx.dll", FileMode.Open);
									brUpx = new BinaryReader(fsUpx);
									length = (int) fsUpx.Length;
									//write length
									brOut.Write(length);
									//write filedata
									brOut.Write(brUpx.ReadBytes(length));
									brUpx.Close();
									fsUpx.Close();
									//delete "___upx.dll"
									File.Delete("___upx.dll");
								}
							}
							else
							{
								//write file length
								brOut.Write(length);
								//check for "_virtual.dat" and use display settings
								if (items.Text == "_virtual.dat")
								{
									//seek over display info
									fsIn.Seek(16,SeekOrigin.Current);
									//write display info
									brOut.Write(displayType.SelectedIndex);
									brOut.Write(int.Parse(displayWidth.Text));
									brOut.Write(int.Parse(displayHeight.Text));
									brOut.Write(int.Parse(displayDepth.Text));
									//write filedata
									brOut.Write(brIn.ReadBytes(length-16));
								}
								else
								{
									//write filedata
									brOut.Write(brIn.ReadBytes(length));
								}
							}
						}
					}
					brIn.Close();
					fsIn.Close();
				}
				else
				{
					//item is external file
					string name;
					name = items.Text;
					//write namelength
					brOut.Write(name.Length);
					//write name
					for (int i=0;i<name.Length;i++)
						brOut.Write(name[i]);
					//check if the fie is a .dll and if we need to upx it
					if (name.EndsWith(".dll") && chkCompress.Checked == true)
					{
						//copy dll to "___upx.dll"
						File.Copy(items.SubItems[2].Text, "___upx.dll");
						//compress with upx
						Process process = Process.Start(Application.StartupPath + "\\upx.exe",upx_options + " ___upx.dll");
						process.WaitForExit();
						//copy to output
						fsUpx = new FileStream("___upx.dll", FileMode.Open);
						brUpx = new BinaryReader(fsUpx);
						//write length
						brOut.Write((int) fsUpx.Length);
						//write filedata
						brOut.Write(brUpx.ReadBytes((int) fsUpx.Length));
						brUpx.Close();
						fsUpx.Close();
						//delete "___upx.dll"
						File.Delete("___upx.dll");
					}
					else
					{
						fsExt = new FileStream(items.SubItems[2].Text,FileMode.Open);
						brExt = new BinaryReader(fsExt);
						//write length
						brOut.Write((int)fsExt.Length);
						//check for "_virtual.dat" and use display settings
						if (name == "_virtual.dat")
						{
							//seek over display info
							fsExt.Seek(16,SeekOrigin.Current);
							//write display info
							brOut.Write(displayType.SelectedIndex);
							brOut.Write(int.Parse(displayWidth.Text));
							brOut.Write(int.Parse(displayHeight.Text));
							brOut.Write(int.Parse(displayDepth.Text));
							//write filedata
							brOut.Write(brExt.ReadBytes((int)fsExt.Length-16));
						}
						else
						{
							//write filedata
							brOut.Write(brExt.ReadBytes((int)fsExt.Length));
						}
						brExt.Close();
						fsExt.Close();
					}
				}
			}
			//close open files
			brOut.Close();
			fsOut.Close();
		}
		dlg.Dispose();
	}

	public void btnExtract_Click(object sender, EventArgs e)
	{
		// extract selected files
		int numFiles = 0;
		int length,i,c,extract;
		//show busy cursor
		Cursor.Current = Cursors.WaitCursor;
		// get number of selected files
		if (list.SelectedItems.Count == 0)
		{
			Cursor.Current = Cursors.Default;
			MessageBox.Show("No files selected!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			return;
		}
		numFiles = list.SelectedItems.Count;
		string[] files = new string[numFiles+1];
		numFiles = 0;
		// get filenames of selected files
		foreach (ListViewItem items in list.SelectedItems)
		{
			if (items.SubItems[2].Text == "<exe>")
			{
				// the item is in the exe
				files[numFiles] = items.Text;
				numFiles++;
			}
		}
		if (numFiles == 0)
		{
			// none of the selected files are in the exe
			MessageBox.Show("Can only extract files located in <exe>","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			return;
		}
		//Extract the files
		FileStream fs;
		BinaryReader br;
		FileStream fsOut;
		BinaryWriter brOut;
		SaveFileDialog dlg = new SaveFileDialog();
		bool pack = true;
		try
		{
			fs = new FileStream(labName.Text,FileMode.Open);
			br = new BinaryReader(fs);
			if (Path.GetExtension(labName.Text) == ".exe")
				pack = false;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			Cursor.Current = Cursors.Default;
			return;
		}
		//find PADDINGXXPAD, files start right after
		if (pack == false)
		{
			try
			{
				string start = "PADDINGXXPAD";
				string find = "";
				byte[] pad = new byte[12] {0,0,0,0,0,0,0,0,0,0,0,0};
				byte b = 0;
				//skip some bytes to speed it up
				fs.Seek(1024*60,SeekOrigin.Current);
				while (find != start && fs.Position < 1024*100)
				{
					b = br.ReadByte();
					//shift the pad array to the left
					for (i = 1; i<12; i++)
						pad[i-1] = pad[i];
					pad[11] = b;
					//conver pad array to string
					find = "";
					for (i=0; i<12; i++)
					{
						find = find + Convert.ToChar(pad[i]);
					}
				}
				//since PADDINGXXPAD may just be the start of a lot of padding continue reading
				//until a value not in PADDINGXXPAD is found
				if (find == start)
				{
					while (start.IndexOf(Convert.ToChar(b), 0) != -1)
					{
						//MessageBox.Show(start.IndexOf(Convert.ToChar(b), 0).ToString());
						b = br.ReadByte();
					}
					//skip back one byte
					fs.Seek(-1 ,SeekOrigin.Current);
				}
				else
				{
					//if we haven't found "PADDINGXXPAD" guess where end of standard exe header is
					fs.Seek(73728,SeekOrigin.Begin);
				}
				//check if we need to save it
				for (c=0; c<numFiles; c++)
				{
					if (files[c] == "Standard Exe Header")
					{
						dlg.Title = "Save Exe Header";
						dlg.Filter = "Exe FIles (*.exe)|*.exe|All Files(*.*)|*.*";
						dlg.FileName = "head.exe";
						if (exePath != "")
						{
							dlg.InitialDirectory = exePath;
							exePath = "";
						}
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							try
							{
								fsOut = new FileStream(dlg.FileName,FileMode.Create);
								brOut = new BinaryWriter(fsOut);
								length = (int) fs.Position;
								fs.Seek(length-(length*2),SeekOrigin.Current);
								brOut.Write(br.ReadBytes(length));
								brOut.Close();
								fsOut.Close();
							}
							catch
							{
								MessageBox.Show("erer");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				br.Close();
				fs.Close();
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				Cursor.Current = Cursors.Default;
				return;
			}
		}
		// run through the files one by one and if they match a filename extract them
		try
		{
			string name;
			dlg.Filter = "All Files (*.*)|*.*";
			while (true)
			{
				//filename length
				length = br.ReadInt32();
				if (length > 0 && length < 100)
				{
					name = "";
					for (i=0;i<length;i++)
					{
						name += Convert.ToChar(br.ReadByte());
					}
					//get file length
					length = br.ReadInt32();
					//see if we need to save it
					extract = 0;
					for (c=0; c<numFiles; c++)
					{
						if (name == files[c])
						{
							//save file
							dlg.Title = "Save "+name;
							dlg.FileName = Path.GetFileName(name);
							if (exePath != "")
							{
								dlg.InitialDirectory = exePath;
								exePath = "";
							}
							if (dlg.ShowDialog() == DialogResult.OK)
							{
								fsOut = new FileStream(dlg.FileName,FileMode.Create);
								brOut = new BinaryWriter(fsOut);
								//check if file is "_virtual.dat" and use display info
								if (name == "_virtual.dat")
								{
									brOut.Write(displayType.SelectedIndex);
									brOut.Write(int.Parse(displayWidth.Text));
									brOut.Write(int.Parse(displayHeight.Text));
									brOut.Write(int.Parse(displayDepth.Text));
									//seek over display info
									fs.Seek(16,SeekOrigin.Current);
									//write data
									brOut.Write(br.ReadBytes(length-16));
								}
								else
								{
									brOut.Write(br.ReadBytes(length));
								}
								brOut.Close();
								fsOut.Close();
								extract = 1;
							}
						}
					}
					// if no file extracted skip its file length
					if (extract == 0)
						fs.Seek(length,SeekOrigin.Current);
				}
				else
				{
					//extra info at end of file or fileblock
					for (c=0; c<numFiles; c++)
					{
						if (files[c] == "Extra or Compressed block")
						{
							dlg.Title = "Save Extra or Compressed block";
							dlg.FileName = "extra.txt";
							if (exePath != "")
							{
								dlg.InitialDirectory = exePath;
								exePath = "";
							}
							if (dlg.ShowDialog() == DialogResult.OK)
							{
								//seek the file back 4 bytes from incorrect attempt to get name length
								fs.Seek(-4,SeekOrigin.Current);
								fsOut = new FileStream(dlg.FileName,FileMode.Create);
								brOut = new BinaryWriter(fsOut);
								brOut.Write(br.ReadBytes((int)(fs.Length - fs.Position)));
								brOut.Close();
								fsOut.Close();
							}
						}
					}
					br.Close();
					fs.Close();
					return;
				}
			}
		}
		catch
		{
			br.Close();
			fs.Close();
		}
	}
	private void btnExit_Click(object sender, EventArgs e)
	{
		//exit
		Close();
	}

	private void btnAbout_Click(object sender, EventArgs e)
	{
		//show about dialog
		aboutDialog dlg = new aboutDialog();
		dlg.ShowDialog();
		dlg.Dispose();
	}

	private void btnEncrypt_Click(object sender, EventArgs e)
	{
		//encrypt file
	}

	private void btnDecrypt_Click(object sender, EventArgs e)
	{
		//decrypt file
		FileStream fsIn;
		BinaryReader brIn;
		FileStream fsOut;
		BinaryWriter bwOut;
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Select file to decrypt";
		dlg.Filter = "All Files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			SaveFileDialog sDlg = new SaveFileDialog();
			sDlg.Title = "Save decrypted file as";
			sDlg.Filter = "Loaded file type (*"+Path.GetExtension(dlg.FileName)+")|*"+Path.GetExtension(dlg.FileName);
			sDlg.Filter += "|All Files (*.*)|*.*";
			if (sDlg.ShowDialog() == DialogResult.OK)
			{
				if (dlg.FileName == sDlg.FileName)
				{
					MessageBox.Show("Can't decrypt to input file!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					dlg.Dispose();
					sDlg.Dispose();
					return;
				}
				else
				{
					byte input,xor;
					xor = 0x21;
					//xor = 0xDF;
					fsIn = new FileStream(dlg.FileName,FileMode.Open);
					brIn = new BinaryReader(fsIn);
					fsOut = new FileStream(sDlg.FileName,FileMode.Create);
					bwOut = new BinaryWriter(fsOut);

					int i = (int)(fsIn.Position - fsIn.Length);
					while ((int)(fsIn.Length - fsIn.Position) > 28)
					{
						input = brIn.ReadByte();
						input = (byte)(xor ^ input);
						bwOut.Write(input);
						bwOut.Write(brIn.ReadBytes(28));
					}
					if ((int)(fsIn.Length - fsIn.Position) > 0)
					{
						input = brIn.ReadByte();
						input = (byte)(xor ^ input);
						bwOut.Write(input);
					}
					if (fsIn.Position < fsIn.Length)
					bwOut.Write(brIn.ReadBytes((int)(fsIn.Length-fsIn.Position)));

					brIn.Close();
					fsIn.Close();
					bwOut.Close();
					fsOut.Close();
				}
			}
			sDlg.Dispose();

		}
		dlg.Dispose();
	}

	private void list_SelectedIndexChanged(object sender, EventArgs e)
	{
		//selected items in the list have changed so enable relevent items in the popup menu accordinly
		//insert
		if (list.SelectedItems.Count < 2)
		{
			list.ContextMenu.MenuItems[0].Enabled = true;
			list.ContextMenu.MenuItems[1].Enabled = true;
		}
		else
		{
			list.ContextMenu.MenuItems[0].Enabled = false;
			list.ContextMenu.MenuItems[1].Enabled = false;
		}
		//remove/extract
		if (list.SelectedItems.Count > 0)
			list.ContextMenu.MenuItems[2].Enabled = true;
		else
			list.ContextMenu.MenuItems[2].Enabled = false;
		if (list.SelectedItems.Count == 1)
		{
			//up/down buttons
			btnUp.Enabled = true;
			btnDown.Enabled = true;
			//show edit menu item if selected item is not in "<exe>"
			if (list.SelectedItems[0].SubItems[2].Text == "<exe>")
				list.ContextMenu.MenuItems[3].Enabled = false;
			else
				list.ContextMenu.MenuItems[3].Enabled = true;
		}
		else
		{
			//up down buttons
			btnUp.Enabled = false;
			btnDown.Enabled = false;
			//edit
			list.ContextMenu.MenuItems[3].Enabled = false;
		}
		//extract button
		if (list.SelectedItems.Count > 0)
		{
			btnExtract.Enabled = true;
			list.ContextMenu.MenuItems[4].Enabled = true;
		}
		else
		{
			btnExtract.Enabled = false;
			list.ContextMenu.MenuItems[4].Enabled = false;
		}
	}

	private void window_DragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			if ((e.AllowedEffect & DragDropEffects.Copy) != 0)
			{
				e.Effect = DragDropEffects.Copy;
			}
		}
	}

	private void window_DragDrop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string[] str = (string[]) e.Data.GetData(DataFormats.FileDrop);
			load(str[0]);
			exePath = Path.GetFullPath(str[0]);
		}
	}

	private void btnUp_Click(object sender, EventArgs e)
	{
		//up button
		if (list.SelectedItems[0].Index > 0)
		{
			int i = list.SelectedItems[0].Index;
			ListViewItem item = list.SelectedItems[0];
			list.SelectedItems[0].Remove();
			list.Items.Insert(i-1,item);
		}
	}

	private void btnDown_Click(object sender, EventArgs e)
	{
		//down button
		if (list.SelectedItems[0].Index < list.Items.Count - 1)
		{
			int i = list.SelectedItems[0].Index;
			ListViewItem item = list.SelectedItems[0];
			list.SelectedItems[0].Remove();
			list.Items.Insert(i+1,item);
		}
	}

}

