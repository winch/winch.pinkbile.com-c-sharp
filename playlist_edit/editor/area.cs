//
// Panel area that can contain playlists and filelists
//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Microsoft.DirectX.AudioVideoPlayback;

class Area
{
	public ArrayList view;
	public int selected;
	int count = 1;
	ContextMenu menu = null;
	Audio audio = null;

	private Panel ListPanel, ButtonPanel;

	public Area(Control parent, Point location, Size size, DockStyle dock, AnchorStyles anchor)
	{
		//list panel
		ListPanel = new Panel();
		ListPanel.Location = location;
		ListPanel.Size = new Size(size.Width, size.Height - 30);
		ListPanel.Parent = parent;
		ListPanel.Dock = dock;
		ListPanel.Anchor = anchor;

		view = new ArrayList();

		//button panel
		ButtonPanel = new Panel();
		ButtonPanel.Parent = parent;
		ButtonPanel.Location = new Point(location.X, location.Y + (size.Height - 25));
		ButtonPanel.Size = new Size(size.Width, size.Height - (size.Height - 35));
		//ButtonPanel.BorderStyle = BorderStyle.Fixed3D;
		if (anchor == (AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom))
			anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		else
			anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		ButtonPanel.Anchor = anchor;

		//Panel buttons

		//Selection up / down buttons

		//add button
		Button add = new Button();
		add.Parent = ButtonPanel;
		add.FlatStyle = FlatStyle.Flat;
		add.Text = "+";
		add.Width = 25;
		//add.Location = new Point(ButtonPanel.Width - add.Width - 5, 5);
		//add.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		add.Dock = DockStyle.Right;
		add.Click += new EventHandler(add_Click);
		//remove button
		Button remove = new Button();
		remove.Parent = ButtonPanel;
		remove.FlatStyle = FlatStyle.Flat;
		remove.Text = "—";
		remove.Width = add.Width;
		//remove.Location = new Point(add.Left - remove.Width - 10, 5);
		//remove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		remove.Dock = DockStyle.Right;
		remove.Click += new EventHandler(remove_Click);

		//Context menu
		EventHandler mInfo = new EventHandler(mInfo_Click);
		EventHandler mOpen = new EventHandler(mOpen_Click);
		EventHandler mSave = new EventHandler(mSave_Click);
		EventHandler mSaveAs = new EventHandler(mSaveAs_Click);
		EventHandler mPlay = new EventHandler(mPlay_Click);
		EventHandler mStop = new EventHandler(mStop_Click);
		EventHandler mClear = new EventHandler(mClear_Click);
		EventHandler mClearSel = new EventHandler(mClearSel_Click);
		EventHandler mClearExist = new EventHandler(mClearExist_Click);
		EventHandler mBrowse = new EventHandler(mBrowse_Click);
		EventHandler mBrowseRec = new EventHandler(mBrowseRec_Click);
		MenuItem[] mi = {
							new MenuItem("-"),
							new MenuItem("[] []", mInfo),
							new MenuItem("-"),
							new MenuItem("Open", mOpen),
							new MenuItem("-"),
							new MenuItem("Save", mSave),
							new MenuItem("Save As", mSaveAs),
							new MenuItem("-"),
							new MenuItem("Play", mPlay),
							new MenuItem("Stop", mStop),
							new MenuItem("-"),
							new MenuItem("Clear Selected", mClearSel),
							new MenuItem("Clear All", mClear),
							new MenuItem("Clear non-existent", mClearExist),
							new MenuItem("-"),
							new MenuItem("Add from folder", mBrowse),
							new MenuItem("Add from folder recursive", mBrowseRec),
							new MenuItem("-"),
							new MenuItem("Add list", new EventHandler(add_Click)),
							new MenuItem("Remove list", new EventHandler(remove_Click)),
							new MenuItem("-")
						};

		menu = new ContextMenu(mi);

		//Default list
		NewList();
	}
	void NewList()
	{
		Hashtable ht = new Hashtable();
		ListView lv = new ListView();
		lv.Parent = ListPanel;
		lv.Dock = DockStyle.Fill;
		lv.View = View.Details;
		lv.Columns.Add("FileName", 345, HorizontalAlignment.Left);
		lv.Columns.Add("Size", 55, HorizontalAlignment.Left);
		lv.Columns.Add("File Path", 55, HorizontalAlignment.Left);
		lv.ContextMenu = menu;
		//lv.Items.Add(count.ToString());
		ht.Add("list", lv);
		//add button
		Button btn = new Button();
		btn.Text = count.ToString();
		count ++;
		btn.Parent = ButtonPanel;
		btn.Width = 30;
		btn.FlatStyle = FlatStyle.Flat;
		btn.Dock = DockStyle.Left;
		btn.Click += new EventHandler(change_click);
		btn.BackColor = SystemColors.ControlLightLight;
		ht.Add("button", btn);
		//filename and modified
		ht.Add("filename", "");
		ht.Add("modified", false);
		view.Add(ht);
		selected = view.Count - 1;
		updateMenu();
	}

	private void mInfo_Click(object sender, EventArgs e)
	{
		//Shows info about the playlist
		Hashtable ht = new Hashtable();
		ListView lv = new ListView();
		Info info = new Info(lv);
		info.ShowDialog();
	}

	private void updateMenu()
	{
		//updates the menu with filename and item count
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		string text = "[" + Path.GetFileName((string) ht["filename"]) + "]";
		if (((bool) ht["modified"]) == true)
			text += "*";
		text += "  [" + lv.Items.Count.ToString() + " items]";
		menu.MenuItems[1].Text = text;
	}

	private void add_Click(object sender, EventArgs e)
	{
		//add new list
		if (view.Count < 13)
		{
			//add new list
			Hashtable ht;
			ListView lv;
			//hide the old list
			ht = (Hashtable) view[selected];
			lv = (ListView) ht["list"];
			lv.Visible = false;
			//change old button colour back
			Button btn = (Button) ht["button"];
			btn.BackColor = SystemColors.Control;
			//new list
			NewList();
		}
		else
		{
			//too many lists
			MessageBox.Show("Too many existing lists!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
		}
	}

	private void remove_Click(object sender, EventArgs e)
	{
		//remove selected list
		if (view.Count > 1)
		{
			//More that one view so we can remove selected one
			Hashtable ht = (Hashtable) view[selected];
			ListView lv = (ListView) ht["list"];
			lv.Dispose();
			Button btn = (Button) ht["button"];
			btn.Dispose();
			view.RemoveAt(selected);
			if (selected > 0)
				selected --;
			ht = (Hashtable) view[selected];
			lv = (ListView) ht["list"];
			lv.Visible = true;
			btn = (Button) ht["button"];
			btn.BackColor = SystemColors.ControlLightLight;
			updateMenu();
		}
		else
		{
			//MessageBox.Show("Can not remove last list!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
		}
	}

	private void change_click(object sender, EventArgs e)
	{
		//change list view
		//hide selected
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		lv.Visible = false;
		Button btn = (Button) ht["button"];
		btn.BackColor = SystemColors.Control;
		btn = (Button) sender;
		//go through each button to find clicked
		int num = 0;
		for (int i=0; i < view.Count; i++)
		{
			ht = (Hashtable) view[i];
			Button button = (Button) ht["button"];
			if (btn.Text == button.Text)
				num = i;
		}
		ht = (Hashtable) view[num];
		lv = (ListView) ht["list"];
		lv.Visible = true;
		btn = (Button) ht["button"];
		btn.BackColor = SystemColors.ControlLightLight;
		selected = num;
		//change menu to show filename and item count
		updateMenu();
	}

	private void mPlay_Click(object sender, EventArgs e)
	{
		//Play selected file
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		if (lv.SelectedItems.Count > 0)
		{
			string filename = filename = lv.SelectedItems[0].SubItems[2].Text + "\\" + lv.SelectedItems[0].SubItems[0].Text;
			if (File.Exists(filename))
			{
				try
				{
					if (audio != null)
					{
						if (audio.Playing == true)
							audio.Stop();
						audio.Dispose();
					}
					audio = new Audio(filename ,true);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(),"Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}
			}
		}
	}

	private void mStop_Click(object sender, EventArgs e)
	{
		//If a file is playing stop playback
		if (audio != null)
		{
			if (audio.Playing == true)
				audio.Stop();
			audio.Dispose();
			audio = null;
		}
	}
	
	private void mClear_Click(object sender, EventArgs e)
	{
		//clear playlist
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		lv.Items.Clear();
		ht["modified"] = true;
		updateMenu();
	}

	private void mClearSel_Click(object sender, EventArgs e)
	{
		//clear playlist
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		foreach (ListViewItem lvi in lv.SelectedItems)
		{
			lvi.Remove();
		}
		ht["modified"] = true;
		updateMenu();
	}

	private void mClearExist_Click(object sender, EventArgs e)
	{
		//removes all items where the file does not exist
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		foreach (ListViewItem lvi in lv.Items)
		{
			if (! File.Exists(lvi.SubItems[2].Text + "\\" + lvi.SubItems[0].Text))
			{
				lvi.Remove();
				ht["modified"] = true;
				updateMenu();
			}
		}
	}

	private void mOpen_Click(object sender, EventArgs e)
	{
		//open playlist
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Open playlist";
		ofd.Filter = "M3u Playlists (*.m3u)|*.m3u|Pls Playlists (*.pls)|*.pls|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			FileStream fs = null;
			StreamReader sr = null;
			FileStream fsSize = null;
			try
			{
				string filename;
				string path = Path.GetDirectoryName(ofd.FileName);
				lv.BeginUpdate();
				fs = new FileStream(ofd.FileName, FileMode.Open);
				sr = new StreamReader(fs);
				lv.Items.Clear();
				while ((filename = sr.ReadLine()) != null)
				{
					//ignore lines that start with #
					if (! filename.StartsWith("#"))
					{
						if (Path.IsPathRooted(filename) == false)
						{
							//it's a relative path so convert to absolute
							filename = path + "\\" + filename;
						}
						ListViewItem lvi = new ListViewItem(Path.GetFileName(filename));
						//get filesize
						string size = "";
						try
						{
							float length = 0;
							if (File.Exists(filename))
							{
								fsSize = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
								if (fsSize.Length > 1024)
								{
									length = fsSize.Length / 1024;
									size = length.ToString() + " Kb";
									if (length > 1024f)
									{
										length /= 1024;
										size = length.ToString("N") + " Mb";
									}
								}
								else
								{
									size = fsSize.Length.ToString();
								}
							}
							else
							{
								size = "n/a";	//file doesn't exist
							}
						}
						catch
						{
							size = "n/a";
						}
						finally
						{
							fsSize.Close();
						}
						lvi.SubItems.Add(size);
						lvi.SubItems.Add(Path.GetDirectoryName(filename));
						lv.Items.Add(lvi);
					}
				}
				ht["modified"] = false;
				ht["filename"] = ofd.FileName;
				updateMenu();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				sr.Close();
				fs.Close();
				lv.EndUpdate();
			}
		}
	}

	private void mSave_Click(object sender, EventArgs e)
	{
		//save playlist
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		string listpath;
		string filename;
		if ((string) ht["filename"] == "")
		{
			//no filename so save as
			mSaveAs_Click(this, new EventArgs());
		}
		else
		{
			//save list
			FileStream fs = null;
			StreamWriter sw = null;
			listpath = Path.GetDirectoryName((string) ht["filename"]);
			if (Path.GetPathRoot(listpath) != listpath)
				listpath += "\\";
			try
			{
				fs = new FileStream((string) ht["filename"], FileMode.Create);
				sw = new StreamWriter(fs);

				foreach (ListViewItem lvi in lv.Items)
				{
					filename = lvi.SubItems[2].Text + "\\" + lvi.SubItems[0].Text;
					if (filename.StartsWith(listpath))
					{
						filename = filename.Remove(0, listpath.Length);
					}
					sw.WriteLine(filename);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				sw.Close();
				fs.Close();
				ht["modified"] = false;
				updateMenu();
			}
		}
	}

	private void mSaveAs_Click(object sender, EventArgs e)
	{
		//save playlist as
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		if (lv.Items.Count == 0)
		{
			//list empty
			MessageBox.Show("Nothing to save!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}
		else
		{
			//save list
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Save Playlist";
			dlg.Filter = "M3u Playlists (*.m3u)|*.m3u|Pls Playlists (*.pls)|*.pls|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				ht["filename"] = dlg.FileName;
				mSave_Click(this, new EventArgs());
			}
		}
	}

	private void mBrowse_Click(object sender, EventArgs e)
	{
		//add files from folder
		FolderBrowserDialog fbd = new FolderBrowserDialog();
		fbd.ShowNewFolderButton = false;
		if (fbd.ShowDialog() == DialogResult.OK)
		{
			Hashtable ht = (Hashtable) view[selected];
			ListView lv = (ListView) ht["list"];
			lv.Items.Clear();
			lv.BeginUpdate();
			getFiles(fbd.SelectedPath, false);
			lv.EndUpdate();
			ht["modified"] = true;
			updateMenu();
		}
	}

	private void mBrowseRec_Click(object sender, EventArgs e)
	{
		//add files from folder
		FolderBrowserDialog fbd = new FolderBrowserDialog();
		fbd.ShowNewFolderButton = false;
		if (fbd.ShowDialog() == DialogResult.OK)
		{
			Hashtable ht = (Hashtable) view[selected];
			ListView lv = (ListView) ht["list"];
			lv.Items.Clear();
			lv.BeginUpdate();
			getFiles(fbd.SelectedPath, true);
			lv.EndUpdate();
			ht["modified"] = true;
			updateMenu();
		}
	}

	private void getFiles(string dir, bool recurse)
	{
		//adds the files in dir to listview then calls its self foreach directory in dir if recurse == true
		Hashtable ht = (Hashtable) view[selected];
		ListView lv = (ListView) ht["list"];
		string[] files = Directory.GetFiles(dir);
		foreach (string str in files)
		{
			ListViewItem lvi = new ListViewItem(Path.GetFileName(str));
			FileStream fs = null;
			try
			{
				float length = 0;
				string size = "";
				fs = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				if (fs.Length > 1024)
				{
					length = fs.Length / 1024;
					size = length.ToString() + " Kb";
					if (length > 1024f)
					{
						length /= 1024;
						size = length.ToString("N") + " Mb";
					}
				}
				else
				{
					size = fs.Length.ToString();
				}
				lvi.SubItems.Add(size);
			}
			catch
			{
				lvi.SubItems.Add("0");
			}
			finally
			{
				if (fs != null)
                    fs.Close();
			}
			lvi.SubItems.Add(Path.GetDirectoryName(str));
			lv.Items.Add(lvi);
		}
		if (recurse == true)
		{
			foreach(string str in Directory.GetDirectories(dir))
			{
				getFiles(str, recurse);
			}
		}
	}
}