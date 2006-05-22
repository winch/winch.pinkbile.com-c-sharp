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
//insertWild dialog, insert files with wildcard
//

using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

class insertWild: Form
{
	TextBox directory;
	CheckBox mediaPrefix;

	string[] selectedFiles;
	public string[] SelectedFiles
	{
		get
		{
			 return selectedFiles;
		}
	}
	public string SelectedDirectory
	{
		get
		{
			return directory.Text;
		}
	}
	public bool MediaPrefix
	{
		get
		{
			return mediaPrefix.Checked;
		}
	}

	GroupBox gbFiles;
	ListBox files;
	GroupBox gbFiltered;
	ListBox filtered;

	Button btnDirectory;
	CheckBox cbDirectory;

	TextBox filter;
	ErrorProvider filterError;

	Button insertSelected, insertAll;

	string exePath; //path of loaded exe

	public insertWild(string ExePath, bool dbPro)
	{
		exePath = ExePath;
		int y = 5;
		GroupBox gb;
		Text = "Insert multiple files";
		ShowInTaskbar = false;
		Size = new Size(500, 490);
		StartPosition = FormStartPosition.CenterParent;

		selectedFiles = new string[0];

		this.Resize += new EventHandler(insertWild_Resize);

		//filtered contextmenu
		EventHandler mAll = new EventHandler(mAllOnClick);
		EventHandler mNone = new EventHandler(mNoneOnClick);
		EventHandler mInvert = new EventHandler(mInvertOnClick);

		MenuItem[] ami = {new MenuItem("Select &All", mAll),
						  new MenuItem("Select &None", mNone),
						  new MenuItem("&Invert Selection", mInvert)};

		//directory
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Directory";
		gb.Location = new Point(5, y);
		gb.Size = new Size(this.Width - 20, 45);
		gb.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
		directory = new TextBox();
		directory.Parent = gb;
		directory.Location = new Point(10, 15);
		directory.Width = gb.Width - 155;
		directory.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		directory.TextChanged += new EventHandler(directory_TextChanged);
		btnDirectory = new Button();
		btnDirectory.Parent = gb;
		btnDirectory.Location = new Point(directory.Width + directory.Left + 5, 15);
		btnDirectory.Text = "...";
		btnDirectory.Width = 25;
		btnDirectory.Anchor = AnchorStyles.Right;
		btnDirectory.Click += new EventHandler(btnDirectory_Click);
		cbDirectory = new CheckBox();
		cbDirectory.Parent = gb;
		cbDirectory.Location = new Point(btnDirectory.Left + btnDirectory.Width + 5, 15);
		cbDirectory.Text = "Recurse subdirs";
		cbDirectory.Width = 105;
		cbDirectory.Anchor = AnchorStyles.Right;
		cbDirectory.CheckStateChanged += new EventHandler(cbDirectory_CheckStateChanged);
		y += gb.Height + 5;

		gbFiles = new GroupBox();
		gbFiles.Parent = this;
		gbFiles.Location = new Point(5, y);
		gbFiles.Text = "Files in dir";
		gbFiles.Size = new Size((this.Width / 2) - 20, 325);
		gbFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
		files = new ListBox();
		files.Parent = gbFiles;
		files.Location = new Point(10, 15);
		files.Size = new Size(gbFiles.Width - 20, gbFiles.Height - 25);
		files.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;

		gbFiltered = new GroupBox();
		gbFiltered.Parent = this;
		gbFiltered.Location = new Point((this.Width / 2), y);
		gbFiltered.Text = "Files to insert";
		gbFiltered.Size = gbFiles.Size;
		gbFiltered.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
		filtered = new ListBox();
		filtered.Parent = gbFiltered;
		filtered.SelectionMode = SelectionMode.MultiSimple;
		filtered.Location = new Point(10, 15);
		filtered.Size = new Size(gbFiltered.Width - 20, gbFiltered.Height - 25);
		filtered.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
		filtered.ContextMenu = new ContextMenu(ami);
		y += gbFiltered.Height + 5;

		//filter
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Filter regex";
		gb.Location = new Point(5, y);
		gb.Size = new Size(this.Width - 20, 45);
		gb.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		filter = new TextBox();
		filter.Parent = gb;
		filter.Location = new Point(10, 15);
		filter.Width = gb.Width - 35;
		filter.TextChanged += new EventHandler(filter_TextChanged);
		filter.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		filterError = new ErrorProvider();
		filterError.BlinkStyle = ErrorBlinkStyle.NeverBlink;
		y += gb.Height + 5;

		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "Cancel";
		btn.Location = new Point(this.Width - btn.Width - 15, y);
		btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		CancelButton = btn;

		insertAll = new Button();
		insertAll.Parent = this;
		insertAll.Text = "Insert &all";
		insertAll.Location = new Point(btn.Left - insertAll.Width - 10, y);
		insertAll.Click += new EventHandler(insertAll_Click);
		insertAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

		insertSelected = new Button();
		insertSelected.Parent = this;
		insertSelected.Text = "Insert &Selected";
		insertSelected.Width += 20;
		insertSelected.Location = new Point(insertAll.Left - insertSelected.Width - 10, y);
		insertSelected.Click += new EventHandler(insertSelected_Click);
		insertSelected.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

		mediaPrefix = new CheckBox();
		mediaPrefix.Parent = this;
		mediaPrefix.Text = "Insert with \"media\\\" prefix";
		mediaPrefix.Width = 160;
		mediaPrefix.Checked = dbPro;
		mediaPrefix.Location = new Point(5, y);
		mediaPrefix.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
	}

	private void insertWild_Resize(object sender, EventArgs e)
	{
		//resize groupboxes
		gbFiles.Width = (this.Width / 2) - 20;
		gbFiltered.Width = gbFiles.Width;
		gbFiltered.Left = this.Width / 2;
	}

	private void mAllOnClick(object sender, EventArgs e)
	{
		//select all filtered files
		for(int i = 0; i < filtered.Items.Count; i++)
			filtered.SetSelected(i, true);
	}

	private void mNoneOnClick(object sender, EventArgs e)
	{
		//select no filtered files
		for(int i = 0; i < filtered.Items.Count; i++)
			filtered.SetSelected(i, false);
	}

	private void mInvertOnClick(object sender, EventArgs e)
	{
		//invert filterd files selection
		for(int i = 0; i < filtered.Items.Count; i++)
		{
			if (filtered.SelectedIndices.Contains(i))
                filtered.SetSelected(i, false);
			else
				filtered.SetSelected(i, true);
		}
	}

	private void getFileList(string dir, bool recurse)
	{
		//fill files listbox with files in directory
		if (Directory.Exists(dir))
		{
			string[] dirFiles = Directory.GetFiles(dir, "*");
			foreach (string str in dirFiles)
			{
				if (directory.Text.EndsWith(Path.DirectorySeparatorChar.ToString()))
					files.Items.Add(str.Substring(directory.Text.Length, str.Length - directory.Text.Length));
				else
					files.Items.Add(str.Substring(directory.Text.Length + 1, str.Length - directory.Text.Length - 1));
			}
			if (recurse == true)
			{
				//recusivly add directories
				dirFiles = Directory.GetDirectories(dir, "*");
				foreach (string str in dirFiles)
				{
					getFileList(str, true);
				}
			}
		}
	}

	private void generateFileList()
	{
		//generates new filelist when directory changes
		Cursor = Cursors.WaitCursor;
		files.BeginUpdate();
		files.Items.Clear();
		getFileList(directory.Text, cbDirectory.Checked);
		files.EndUpdate();
		filter_TextChanged(this, new EventArgs());
		Cursor = Cursors.Default;
	}

	private void btnDirectory_Click(object sender, EventArgs e)
	{
		//change directory
		FolderBrowserDialog browse = new FolderBrowserDialog();
		browse.ShowNewFolderButton = false;
		if (Directory.Exists(directory.Text))
			browse.SelectedPath = directory.Text;
		if (browse.ShowDialog() == DialogResult.OK)
		{
			directory.Text = browse.SelectedPath;
			generateFileList();
		}
	}

	private void directory_TextChanged(object sender, EventArgs e)
	{
		//get list for new dir
		if (Directory.Exists(directory.Text))
		{
			files.Items.Clear();
			filtered.Items.Clear();
			generateFileList();
		}
		else
		{
			files.Items.Clear();
		}
	}

	private void filter_TextChanged(object sender, EventArgs e)
	{
		//filter files
		try
		{
			filtered.BeginUpdate();
			Regex r = new Regex(filter.Text, RegexOptions.IgnoreCase);
			Match m;
			//remove unselected items
			for(int i = 0; i < filtered.Items.Count; i++)
			{
				if (filtered.SelectedIndices.Contains(i) == false)
					filtered.Items.RemoveAt(i--);
			}
			foreach (string str in files.Items)
			{
				m = r.Match(str);
				if (m.Success)
				{
					if (filtered.Items.IndexOf(str) == -1)
                        filtered.Items.Add(str);
				}
			}
			filterError.SetError(filter, "");
		}
		catch (Exception ex)
		{
			filterError.SetError(filter, ex.Message);
		}
		finally
		{
			filtered.EndUpdate();
		}
	}


	private void Insert(System.Collections.ICollection items)
	{
		selectedFiles = new string[items.Count];
		items.CopyTo(selectedFiles, 0);
		this.DialogResult = DialogResult.OK;
		this.Close();
	}

	private void insertAll_Click(object sender, EventArgs e)
	{
		//insert all filtered files
		Insert(filtered.Items);
	}

	private void insertSelected_Click(object sender, EventArgs e)
	{
		//insert selected filtered files
		Insert(filtered.SelectedItems);
	}

	private void cbDirectory_CheckStateChanged(object sender, EventArgs e)
	{
		//recurse subdirs changed
		files.Items.Clear();
		generateFileList();
	}
}