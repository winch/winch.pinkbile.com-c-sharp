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
// dark_explorer
// a darkbasic pro exe explorer
//
// winch.pinkbile.com
// thewinch@gmail.com
//

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

class window : Form
{

	ComboBox exeType;
	Label exeName;
	ListView contents;
	public int displayWidth = -1;
	public int displayHeight = -1;
	public int displayDepth = -1;
	public int displayMode = -1;

	//position of menu items in contents context menu
	public const int MENU_DISPLAY = 0;
	private const int MENU_LOAD = 1;
	private const int MENU_SAVE = 2;
	private const int MENU_INSERT = 4;
	private const int MENU_INSERTWILD = 5;
	private const int MENU_REMOVE = 6;
	private const int MENU_REPLACE = 7;
	private const int MENU_EXTRACT = 8;
	private const int MENU_EDIT = 9;
	private const int MENU_EDITWILD = 10;
	private const int MENU_VIEW = 11;
	private const int MENU_TOOLS = 13;
	private const int MENU_DECOMPRESS = 0;
	private const int MENU_COMPRESS = 1;
	private const int MENU_ABOUT = 14;
	private const int MENU_EXIT = 15;

	public static void Main()
	{
		Application.Run(new window());
	}
	public window()
	{
		Text = "dark_explorer";
		Size = new Size(600,600);

		contents = new ListView();
		contents.Parent = this;
		contents.Location = new Point(10,30);
		contents.Size = new Size(570,535);
		contents.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
		contents.View = View.Details;
		contents.Columns.Add("Name",210,HorizontalAlignment.Left);
		contents.Columns.Add("Attached File",75,HorizontalAlignment.Left);
		contents.Columns.Add("Upx",45,HorizontalAlignment.Left);
		contents.Columns.Add("Null Str table",75,HorizontalAlignment.Left);
		contents.Columns.Add("File size",70,HorizontalAlignment.Right);
		contents.Columns.Add("Location",90,HorizontalAlignment.Left);
		contents.AllowColumnReorder = true;

		exeType = new ComboBox();
		exeType.Parent = this;
		exeType.Location = new Point(10, 5);
		exeType.DropDownStyle = ComboBoxStyle.DropDownList;
		exeType.Items.Add("DbPro");
		exeType.Items.Add("Dbc");
		exeType.SelectedIndex = 0;
		exeType.Width = 60;

		exeName = new Label();
		exeName.Location = new Point(exeType.Left + exeType.Width + 10, 5);
		exeName.Parent = this;
		exeName.AutoSize = true;
		exeName.Text = "";

		//context menu
		EventHandler mDisplay  = new EventHandler(mDisplayOnClick);
		EventHandler mLoad  = new EventHandler(mLoadOnClick);
		EventHandler mSave  = new EventHandler(mSaveOnClick);
		EventHandler mInsert  = new EventHandler(mInsertOnClick);
		EventHandler mInsertWild  = new EventHandler(mInsertWildOnClick);
		EventHandler mRemove  = new EventHandler(mRemoveOnClick);
		EventHandler mReplace  = new EventHandler(mReplaceOnClick);
		EventHandler mExtract = new EventHandler(mExtractOnClick);
		EventHandler mEdit = new EventHandler(mEditOnClick);
		EventHandler mEditWild = new EventHandler(mEditWildOnClick);
		EventHandler mView = new EventHandler(mViewOnClick);
		EventHandler mDecompress = new EventHandler(mDecompressOnClick);
		EventHandler mCompress = new EventHandler(mCompressOnClick);
		EventHandler mAbout = new EventHandler(mAboutOnClick);
		EventHandler mExit = new EventHandler(mExitOnClick);

		//MenuItem[] amiTools = {new MenuItem("Decompress Exe", mDecompress) ,
		//					   new MenuItem("Show/Hide debug log", mToggleDebug) };
		MenuItem[] amiTools = {new MenuItem("Decompress Exe/Pck", mDecompress),
							   new MenuItem("Compress Exe/Pck", mCompress) };
		MenuItem[] ami =  { new MenuItem("No display settings", mDisplay),
							new MenuItem("&Load", mLoad),
							new MenuItem("&Save", mSave),
							new MenuItem("-"),
							new MenuItem("&Insert", mInsert),
							new MenuItem("Insert *", mInsertWild),
							new MenuItem("&Remove", mRemove),
						    new MenuItem("Rep&lace", mReplace),
							new MenuItem("&Extract", mExtract),
							new MenuItem("E&dit", mEdit),
						    new MenuItem("Edit *", mEditWild),
						    new MenuItem("&View", mView),
						    new MenuItem("-"),
							new MenuItem("&Tools", amiTools),
							new MenuItem("-"),
							new MenuItem("&About", mAbout),
							new MenuItem("E&xit", mExit)
						  };
		contents.ContextMenu = new ContextMenu(ami);
		contents.ContextMenu.MenuItems[MENU_DISPLAY].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_SAVE].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_REMOVE].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_REPLACE].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_EXTRACT].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_EDIT].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_EDITWILD].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_VIEW].Enabled = false;

		contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_DECOMPRESS].Enabled = false;
		contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_COMPRESS].Enabled = false;
		this.ContextMenu = contents.ContextMenu;

		//double click edits item
		contents.ItemActivate += mView;
		contents.SelectedIndexChanged += new EventHandler(contents_SelectedIndexChanged);

		AllowDrop = true;
		DragOver += new DragEventHandler(window_DragOver);
		DragDrop += new DragEventHandler(window_DragDrop);
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
			if (File.Exists(str[0]))
			{
				LoadExe(str[0]);
			}
		}
	}

	private void updateAlternatingColours()
	{
		//alternate colours of items in contents listview
		for (int i=0; i < contents.Items.Count; i++)
		{
			if (i % 2 == 0)
				contents.Items[i].BackColor = Color.LightBlue;
			else
				contents.Items[i].BackColor = Color.White;
		}
	}

	private void mDisplayOnClick(object sender, EventArgs e)
	{
		//change display setting
		displayDialog dd = new displayDialog();
		dd.displayWidth = displayWidth;
		dd.displayHeight = displayHeight;
		dd.displayDepth = displayDepth;
		dd.displayMode = displayMode;
		if (dd.ShowDialog() == DialogResult.OK)
		{
			displayWidth = dd.displayWidth;
			displayHeight = dd.displayHeight;
			displayDepth = dd.displayDepth;
			displayMode = dd.displayMode;
			contents.ContextMenu.MenuItems[MENU_DISPLAY].Text =
				proExe.getDisplayString(displayWidth, displayHeight, displayDepth, displayMode);
		}
	}

	private void InsertFile(string fileName, string name, bool upx, bool nullString)
	{
		//insert file
		FileInfo fi = new FileInfo(fileName);
		ListViewFileItem lvi = new ListViewFileItem();
		lvi.SubItems[(int)ListViewOrder.Name].Text = name;
		lvi.SubItems[(int)ListViewOrder.FileType].Text = ListViewStrings.Yes;
		lvi.SubItems[(int)ListViewOrder.Upx].Text = ListViewStrings.No;
		lvi.SubItems[(int)ListViewOrder.NullString].Text = ListViewStrings.No;
		lvi.SubItems[(int)ListViewOrder.FileSize].Text = fi.Length.ToString("n0");
		lvi.SubItems[(int)ListViewOrder.Location].Text = fileName;
		lvi.Offset = 0;
		lvi.Size = (int)fi.Length;
		lvi.SubItems.Add(Path.GetFullPath(fileName));
		if (contents.SelectedItems.Count == 0)
		{
			//no items selected so add at bottom
			contents.Items.Add(lvi);
		}
		else
		{
			//insert before first selected item
			contents.Items.Insert(contents.SelectedItems[0].Index, lvi);
		}
		updateAlternatingColours();
		//check for _virtual.dat
		if (lvi.Text == ListViewStrings.VirtualDat)
		{
			//get display settings from _virtual.dat
			FileStream fsIn = null;
			BinaryReader brIn = null;
			try
			{
				fsIn = new FileStream(fileName, FileMode.Open);
				brIn = new BinaryReader(fsIn);
				displayMode = brIn.ReadInt32();
				displayWidth = brIn.ReadInt32();
				displayHeight = brIn.ReadInt32();
				displayDepth = brIn.ReadInt32();
				contents.ContextMenu.MenuItems[MENU_DISPLAY].Text =
					proExe.getDisplayString(displayWidth, displayHeight, displayDepth, displayMode);
				contents.ContextMenu.MenuItems[MENU_DISPLAY].Enabled = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				brIn.Close();
				fsIn.Close();
			}
		}
	}

	private void mInsertOnClick(object sender, EventArgs ea)
	{
		//insert file
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Select file";
		ofd.Filter = "All Files (*.*)|*.*|Dll Files (*.dll)|*.dll";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			Cursor.Current = Cursors.WaitCursor;
			InsertFile(ofd.FileName, Path.GetFileName(ofd.FileName), false, false);
			Cursor.Current = Cursors.Default;
		}
		ofd.Dispose();

		//enable menu items
		if (contents.Items.Count > 0)
		{
			contents.ContextMenu.MenuItems[MENU_SAVE].Enabled = true;
			contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_DECOMPRESS].Enabled = true;
			contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_COMPRESS].Enabled = true;
		}
	}

	private void mInsertWildOnClick(object sender, EventArgs ea)
	{
		//insert wildcard
		bool dbPro = false;
		if (exeType.Items[exeType.SelectedIndex].Equals("DbPro"))
			dbPro = true;
		insertWild iw = new insertWild(exeName.Text, dbPro);
		if (iw.ShowDialog() == DialogResult.OK)
		{
			//insert files
			Cursor.Current = Cursors.WaitCursor;
			contents.BeginUpdate();
			string prefix, path;
			if (iw.MediaPrefix == true)
				prefix = "media\\";
			else
				prefix = "";
			path = iw.SelectedDirectory;
			if (iw.SelectedDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()) == false)
				path += Path.DirectorySeparatorChar.ToString();
			foreach (string str in iw.SelectedFiles)
			{
				InsertFile(path + str, prefix + str, false, false);
			}
			contents.EndUpdate();
			Cursor.Current = Cursors.Default;
		}
		//enable menu items
		if (contents.Items.Count > 0)
		{
			contents.ContextMenu.MenuItems[MENU_SAVE].Enabled = true;
			contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_DECOMPRESS].Enabled = true;
			contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_COMPRESS].Enabled = true;
		}
	}

	private void mRemoveOnClick(object sender, EventArgs e)
	{
		//remove selected items
		contents.BeginUpdate();
		foreach (ListViewFileItem lvi in contents.SelectedItems)
		{
			lvi.Remove();
		}
		updateAlternatingColours();
		contents.EndUpdate();
		//check for _virtual.dat and set display settings to - 1 if not found
		bool found = false;
		foreach (ListViewFileItem lvi in contents.Items)
		{
			if (lvi.Text == ListViewStrings.VirtualDat)
				found = true;
		}
		if (found == false)
		{
			displayDepth = -1;
			displayWidth = -1;
			displayHeight = -1;
			displayMode = -1;
			contents.ContextMenu.MenuItems[MENU_DISPLAY].Text = "No display settings";
			contents.ContextMenu.MenuItems[MENU_DISPLAY].Enabled = false;
		}
		//disable menu item
		if (contents.Items.Count == 0)
		{
			contents.ContextMenu.MenuItems[MENU_SAVE].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_DECOMPRESS].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_COMPRESS].Enabled = false;
		}
	}

	private void mReplaceOnClick(object sender, EventArgs ea)
	{
		//replace selected items
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = ofd.Filter = "All Files (*.*)|*.*|Dll Files (*.dll)|*.dll|Icon Files (*.ico)|*.ico";
		foreach (ListViewFileItem lvi in contents.SelectedItems)
		{
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Cursor.Current = Cursors.WaitCursor;
				FileInfo fi = new FileInfo(ofd.FileName);
				lvi.SubItems[(int)ListViewOrder.FileSize].Text = fi.Length.ToString("n0");
				lvi.Offset = 0;
				lvi.Size = (int)fi.Length;
				lvi.SubItems[(int)ListViewOrder.Location].Text = Path.GetFullPath(ofd.FileName);
				//check for _virtual.dat
				if (lvi.Text == ListViewStrings.VirtualDat)
				{
					//get display settings from _virtual.dat
					FileStream fsIn = null;
					BinaryReader brIn = null;
					try
					{
						fsIn = new FileStream(ofd.FileName, FileMode.Open);
						brIn = new BinaryReader(fsIn);
						displayMode = brIn.ReadInt32();
						displayWidth = brIn.ReadInt32();
						displayHeight = brIn.ReadInt32();
						displayDepth = brIn.ReadInt32();
						contents.ContextMenu.MenuItems[MENU_DISPLAY].Text =
							proExe.getDisplayString(displayWidth, displayHeight, displayDepth, displayMode);
						contents.ContextMenu.MenuItems[MENU_DISPLAY].Enabled = true;
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					finally
					{
						brIn.Close();
						fsIn.Close();
					}
				}
				Cursor.Current = Cursors.Default;
			}
		}
	}

	private void mExtractOnClick(object sender, EventArgs ea)
	{
		//extract file
		foreach (ListViewFileItem lvi in contents.SelectedItems)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Extract file as";
			sfd.Filter = "All Files (*.*)|*.*";
			//extract file
               sfd.FileName = Path.GetFileName(proExe.DbcRemoveNull(lvi.Text));
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				Cursor.Current = Cursors.WaitCursor;
				proExe.ExtractFile(lvi, exeName.Text, sfd.FileName);
				Cursor.Current = Cursors.Default;
			}
			sfd.Dispose();
		}
	}

	private void mEditOnClick(object sender, EventArgs ea)
	{
		//edit selected items
		foreach (ListViewFileItem lvi in contents.SelectedItems)
		{
			editItem ei = new editItem(lvi.SubItems[(int)ListViewOrder.Name].Text, lvi.SubItems[(int)ListViewOrder.FileType].Text,
									   lvi.SubItems[(int)ListViewOrder.Upx].Text, lvi.SubItems[(int)ListViewOrder.NullString].Text);
			if (ei.ShowDialog() == DialogResult.OK)
			{
				lvi.SubItems[(int)ListViewOrder.Name].Text = ei.FileName;
				lvi.SubItems[(int)ListViewOrder.FileType].Text = ei.Filetype;
				lvi.SubItems[(int)ListViewOrder.Upx].Text = ei.Upx;
				lvi.SubItems[(int)ListViewOrder.NullString].Text = ei.StringNull;
			}
		}
	}

	private void mEditWildOnClick(object sender, EventArgs e)
	{
		//bulk edit
		editWild ew = new editWild();
		if (ew.ShowDialog() == DialogResult.OK)
		{ 
			ew.EditItems(contents.SelectedItems);
		}
	}

	private void mViewOnClick(object sender, EventArgs e)
	{
		//view selected items
		foreach (ListViewFileItem lvi in contents.SelectedItems)
		{
			viewItem vi = new viewItem();
			vi.ExeName = exeName.Text;
			vi.Item = lvi;
			vi.ShowDialog();
		}
	}

	private void mDecompressOnClick(object sender, EventArgs e)
	{
		//decompress loaded exe
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save decompressed exe/pck as";
		sfd.Filter = "Exe and Pck Files (*.exe, *.pck)|*.exe;*.pck|All Files (*.*)|*.*";
		//check for compress.dll to ensure exe is compressed.
		if (proExe.IsCompressed(contents) == false)
		{
			MessageBox.Show("Exe is not compressed.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			//Make sure filenames don't match, this is really just to ensure there is a backup incase decompression fails
			if (exeName.Text == sfd.FileName)
			{
				MessageBox.Show("Compressed exe filename and decompressed exe filename must be different.", "Error!",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Cursor.Current = Cursors.WaitCursor;
			bool dbPro = false;
			if (exeType.SelectedIndex == exeType.Items.IndexOf("DbPro"))
				dbPro = true;
			proExe.DecompressExe(contents, dbPro, this, exeName.Text, sfd.FileName);
			Cursor.Current = Cursors.Default;
		}
		sfd.Dispose();
	}

	private void mCompressOnClick(object sender, EventArgs e)
	{
		//compress exe
		//decompress loaded exe
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save compressed exe/pck as";
		sfd.Filter = "Exe or Pck Files (*.exe, *.pck)|*.exe;*.pck|All Files (*.*)|*.*";
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Select " + ListViewStrings.CompressDll;
		ofd.Filter = ListViewStrings.CompressDll + "|" + ListViewStrings.CompressDll + "|Dll Files (*.dll)|*.dll|All Files (*.*)|*.*";
		//check for compress.dll to ensure exe is not compressed.
		if (proExe.IsCompressed(contents) == true)
		{
			MessageBox.Show("Exe is already compressed.", "Error!",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			//Make sure filenames don't match, this is really just to ensure there is a backup incase compression fails
			if (exeName.Text == sfd.FileName)
			{
				MessageBox.Show("Compressed exe filename and decompressed exe filename must be different.", "Error!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Cursor.Current = Cursors.WaitCursor;
				bool dbPro = false;
				if (exeType.SelectedIndex == exeType.Items.IndexOf("DbPro"))
					dbPro = true;
				proExe.CompressExe(contents, dbPro, this, exeName.Text, sfd.FileName, ofd.FileName);
				Cursor.Current = Cursors.Default;
			}
		}
		sfd.Dispose();
	}

	private void mLoadOnClick(object sender, EventArgs e)
	{
		//load exe
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Select exe";
		ofd.Filter = "Exe or Pck files (*.exe, *.pck)|*.exe;*.pck|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			LoadExe(ofd.FileName);
		}
		ofd.Dispose();
	}

	private void LoadExe(string filename)
	{
		Cursor.Current = Cursors.WaitCursor;
		displayDepth = -1;
		displayWidth = -1;
		displayHeight = -1;
		displayMode = -1;
		contents.BeginUpdate();
		contents.Items.Clear();
		contents.ContextMenu.MenuItems[MENU_DISPLAY].Text = "No display settings";
		contents.ContextMenu.MenuItems[MENU_DISPLAY].Enabled = false;
		proExe.LoadExe(contents, filename, this);
		exeName.Text = filename;
		contents.ContextMenu.MenuItems[MENU_SAVE].Enabled = true;
		contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_DECOMPRESS].Enabled = true;
		contents.ContextMenu.MenuItems[MENU_TOOLS].MenuItems[MENU_COMPRESS].Enabled = true;
		updateAlternatingColours();
		contents.EndUpdate();
		//work out exeType
		foreach (ListViewFileItem lvi in contents.Items)
		{
			if (lvi.SubItems[(int)ListViewOrder.FileType].Text == ListViewStrings.Yes)
			{
				//attached file
				if (lvi.Text.EndsWith("\0"))
				{
					//filename is null terminated so is dbc exe
					exeType.SelectedIndex = exeType.Items.IndexOf("Dbc");
				}
				else
				{
					exeType.SelectedIndex = exeType.Items.IndexOf("DbPro");
				}
				break;
			}
		}
		Cursor.Current = Cursors.Default;
	}

	private void mSaveOnClick(object sender, EventArgs e)
	{
		//save exe
		if (contents.Items.Count > 0)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Exe or Pck files (*.exe, *.pck)|*.exe;*.pck|All Files (*.*)|*.*";
			sfd.Title = "Save exe";
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				Cursor.Current = Cursors.WaitCursor;
				bool dbPro = false;
				if (exeType.SelectedIndex == exeType.Items.IndexOf("DbPro"))
					dbPro = true;
				proExe.SaveExe(contents, sfd.FileName, exeName.Text, dbPro, this);
				if (exeName.Text == "")
					exeName.Text = sfd.FileName;
				Cursor.Current = Cursors.Default;
			}
			sfd.Dispose();
		}
		else
		{
			//nothing to save
			MessageBox.Show("Nothing to save!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	private void contents_SelectedIndexChanged(object sender, EventArgs e)
	{
		//enable / disable menu items appropriatly
		if (contents.SelectedItems.Count > 0)
		{
			contents.ContextMenu.MenuItems[MENU_REMOVE].Enabled = true;
			contents.ContextMenu.MenuItems[MENU_REPLACE].Enabled = true;
			contents.ContextMenu.MenuItems[MENU_EXTRACT].Enabled = true;
			if (contents.SelectedItems.Count < 6)
			{
				contents.ContextMenu.MenuItems[MENU_EDIT].Enabled = true;
			}
			else
			{
				contents.ContextMenu.MenuItems[MENU_EDIT].Enabled = false;
			}
			if (contents.SelectedItems.Count > 1)
			{
				contents.ContextMenu.MenuItems[MENU_EDITWILD].Enabled = true;
			}
			contents.ContextMenu.MenuItems[MENU_VIEW].Enabled = true;
		}
		else
		{
			//remove
			contents.ContextMenu.MenuItems[MENU_REMOVE].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_REPLACE].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_EXTRACT].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_EDIT].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_EDITWILD].Enabled = false;
			contents.ContextMenu.MenuItems[MENU_VIEW].Enabled = false;
		}
	}

	private void mAboutOnClick(object sender, EventArgs ea)
	{
		//show about dialog
		aboutDialog ad = new aboutDialog();
		ad.ShowDialog();
	}

	private void mExitOnClick(object sender, EventArgs ea)
	{
		//exit program
		Close();
	}
}