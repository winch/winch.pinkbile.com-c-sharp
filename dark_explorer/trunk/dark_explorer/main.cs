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

	Label exeName;
	ListView contents;
	public int displayWidth = -1;
	public int displayHeight = -1;
	public int displayDepth = -1;
	public int displayMode = -1;

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

		exeName = new Label();
		exeName.Location = new Point(10, 5);
		exeName.Parent = this;
		exeName.AutoSize = true;
		exeName.Text = "";

		//context menu
		EventHandler mDisplay  = new EventHandler(mDisplayOnClick);
		EventHandler mLoad  = new EventHandler(mLoadOnClick);
		EventHandler mSave  = new EventHandler(mSaveOnClick);
		EventHandler mInsert  = new EventHandler(mInsertOnClick);
		EventHandler mRemove  = new EventHandler(mRemoveOnClick);
		EventHandler mReplace  = new EventHandler(mReplaceOnClick);
		EventHandler mExtract = new EventHandler(mExtractOnClick);
		EventHandler mEdit = new EventHandler(mEditOnClick);
		EventHandler mView = new EventHandler(mViewOnClick);
		EventHandler mDecompress = new EventHandler(mDecompressOnClick);
		EventHandler mToggleDebug = new EventHandler(mToggleDebugOnClick);
		EventHandler mAbout = new EventHandler(mAboutOnClick);
		EventHandler mExit = new EventHandler(mExitOnClick);

		MenuItem[] amiTools = {new MenuItem("Decompress Exe", mDecompress) ,
							   new MenuItem("Show/Hide debug log", mToggleDebug) };
		MenuItem[] ami =  { new MenuItem("No display settings", mDisplay),
							new MenuItem("&Load", mLoad),
							new MenuItem("&Save", mSave),
							new MenuItem("-"),
							new MenuItem("&Insert", mInsert),
							new MenuItem("&Remove", mRemove),
						    new MenuItem("Rep&lace", mReplace),
							new MenuItem("&Extract", mExtract),
							new MenuItem("E&dit", mEdit),
						    new MenuItem("&View", mView),
						    new MenuItem("-"),
							new MenuItem("&Tools", amiTools),
							new MenuItem("-"),
							new MenuItem("&About", mAbout),
							new MenuItem("E&xit", mExit)
						  };
		contents.ContextMenu = new ContextMenu(ami);
		contents.ContextMenu.MenuItems[0].Enabled = false; //Display
		contents.ContextMenu.MenuItems[2].Enabled = false; //save
		contents.ContextMenu.MenuItems[5].Enabled = false; //remove
		contents.ContextMenu.MenuItems[6].Enabled = false; //replace
		contents.ContextMenu.MenuItems[7].Enabled = false; //extract
		contents.ContextMenu.MenuItems[8].Enabled = false; //edit
		contents.ContextMenu.MenuItems[9].Enabled = false; //view
		contents.ContextMenu.MenuItems[11].MenuItems[0].Enabled = false; //decompress

		//double click edits item
		contents.ItemActivate += mEdit;
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
			LoadExe(str[0]);
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
			contents.ContextMenu.MenuItems[0].Text = proExe.getDisplayString(displayWidth, displayHeight, displayDepth, displayMode);
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
			FileInfo fi = new FileInfo(ofd.FileName);
			ListViewFileItem lvi = new ListViewFileItem();
			lvi.Text = Path.GetFileName(ofd.FileName);
			lvi.SubItems.Add("Yes");
			lvi.SubItems.Add("No");
			lvi.SubItems.Add("No");
			lvi.SubItems.Add(fi.Length.ToString("n0"));
			lvi.Offset = 0;
			lvi.Size = (int)fi.Length;
			lvi.SubItems.Add(Path.GetFullPath(ofd.FileName));
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
			//check for _virtual.dat
			if (lvi.Text == "_virtual.dat")
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
					contents.ContextMenu.MenuItems[0].Text = proExe.getDisplayString(displayWidth, displayHeight, displayDepth, displayMode);
					contents.ContextMenu.MenuItems[0].Enabled = true;
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
		ofd.Dispose();

		//enable menu items
		if (contents.Items.Count > 0)
		{
			contents.ContextMenu.MenuItems[2].Enabled = true; //save
			contents.ContextMenu.MenuItems[11].MenuItems[0].Enabled = true; //decompress
		}
	}

	private void mRemoveOnClick(object sender, EventArgs e)
	{
		//remove selected items
		foreach (ListViewFileItem lvi in contents.SelectedItems)
		{
			lvi.Remove();
		}
		//check for _virtual.dat and set display settings to - 1 if not found
		bool found = false;
		foreach (ListViewFileItem lvi in contents.Items)
		{
			if (lvi.Text == "_virtual.dat")
				found = true;
		}
		if (found == false)
		{
			displayDepth = -1;
			displayWidth = -1;
			displayHeight = -1;
			displayMode = -1;
			contents.ContextMenu.MenuItems[0].Text = "No display settings";
			contents.ContextMenu.MenuItems[0].Enabled = false;
		}
		//save menu item
		if (contents.Items.Count == 0)
		{
			contents.ContextMenu.MenuItems[2].Enabled = false;
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
				lvi.SubItems[4].Text = fi.Length.ToString("n0");
				lvi.Offset = 0;
				lvi.Size = (int)fi.Length;
				lvi.SubItems[5].Text = Path.GetFullPath(ofd.FileName);
				//check for _virtual.dat
				if (lvi.Text == "_virtual.dat")
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
						contents.ContextMenu.MenuItems[0].Text = proExe.getDisplayString(displayWidth, displayHeight, displayDepth,
																						 displayMode);
						contents.ContextMenu.MenuItems[0].Enabled = true;
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
			editItem ei = new editItem(lvi.Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text, lvi.SubItems[3].Text);
			if (ei.ShowDialog() == DialogResult.OK)
			{
				lvi.Text = ei.FileName;
				lvi.SubItems[1].Text = ei.Filetype;
				lvi.SubItems[2].Text = ei.Upx;
				lvi.SubItems[3].Text = ei.StringNull;
			}
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

	private void mToggleDebugOnClick(object sender, EventArgs e)
	{
		//show/hide debug log
		debugLog.Toggle();
	}

	private void mDecompressOnClick(object sender, EventArgs e)
	{
		//decompress an exe
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save decompressed exe as";
		sfd.Filter = "Exe Files (*.exe)|*.exe|All Files (*.*)|*.*";
		//check for compress.dll to ensure exe is compressed.
		if (contents.Items.Count < 3 || contents.Items[1].Text.ToLower() != "compress.dll")
		{
			MessageBox.Show("Exe is not compressed.", "Error!",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			proExe.DecompressExe(contents, this, exeName.Text, sfd.FileName);
			Cursor.Current = Cursors.Default;
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
		contents.ContextMenu.MenuItems[0].Text = "No display settings";
		contents.ContextMenu.MenuItems[0].Enabled = false;
		proExe.LoadExe(contents, filename, this);
		exeName.Text = filename;
		contents.ContextMenu.MenuItems[2].Enabled = true; //save
		contents.ContextMenu.MenuItems[11].MenuItems[0].Enabled = true; //decompress
		contents.EndUpdate();
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
				proExe.SaveExe(contents, sfd.FileName, exeName.Text, this);
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
			//remove
			contents.ContextMenu.MenuItems[5].Enabled = true;
			//replace
			contents.ContextMenu.MenuItems[6].Enabled = true;
			//extract
			contents.ContextMenu.MenuItems[7].Enabled = true;
			//edit
			contents.ContextMenu.MenuItems[8].Enabled = true;
			//view
			contents.ContextMenu.MenuItems[9].Enabled = true;
		}
		else
		{
			//remove
			contents.ContextMenu.MenuItems[5].Enabled = false;
			//replace
			contents.ContextMenu.MenuItems[6].Enabled = false;
			//extract
			contents.ContextMenu.MenuItems[7].Enabled = false;
			//edit
			contents.ContextMenu.MenuItems[8].Enabled = false;
			//view
			contents.ContextMenu.MenuItems[9].Enabled = false;
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