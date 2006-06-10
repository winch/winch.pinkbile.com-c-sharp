//
// basic window stuff
//
//
// © the_winch 2005
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

class ActionStrings
{
	public static readonly string AddReplace = "Add/Replace";
	public static readonly string Remove = "Remove";
}

class Window : Form
{
	public TextBox name;
	public TextBox filename;
	public TextBox checksum;
	public TextBox info;
	public ListView files;

	public Window()
	{
		Text = "patch builder";
		Size = new Size(550,560);
		
		int y = 5;

		//game name
		Label label = new Label();
		label.Parent = this;
		label.Text = "Window title";
		label.Location = new Point(0,y);
		label.AutoSize = true;
		name = new TextBox();
		name.Parent = this;
		name.Location = new Point(label.Width+label.Left+5,y);
		name.Size = new Size(this.Width-label.Width-label.Left-20,name.Height);
		name.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
		y += name.Height + 10;

		//game filename
		label = new Label();
		label.Parent = this;
		label.Text = "Game filename";
		label.Location = new Point(0,y);
		label.AutoSize = true;
		filename = new TextBox();
		filename.Parent = this;
		filename.Location = new Point(label.Width+label.Left+5,y);
		filename.Size = new Size(this.Width-label.Width-label.Left-60,filename.Height);
		filename.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
		Button getname = new Button();
		getname.Parent = this;
		getname.Text = "...";
		getname.Location = new Point(filename.Left+filename.Width+10,y);
		getname.Width = 30;
		getname.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		getname.Click += new EventHandler(getname_Click);
		y += filename.Height + 10;

		//game filename checksum
		label = new Label();
		label.Parent = this;
		label.Text = "Checksum (blank for none)";
		label.AutoSize = true;
		label.Location = new Point(0,y);
		checksum = new TextBox();
		checksum.Parent = this;
		checksum.Location = new Point(label.Width+label.Left+5,y);
		checksum.Width = this.Width-label.Width-label.Left-20;
		checksum.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;

		y += label.Height + 10;

		//game info
		label = new Label();
		label.Parent = this;
		label.Text = "Info / License";
		label.Location = new Point(0,y);
		label.AutoSize = true;
		info = new TextBox();
		info.Parent = this;
		info.Location = new Point(label.Width+label.Left+5,y);
		info.Size = new Size(this.Width-label.Width-label.Left-20,info.Height*10);
		info.Multiline = true;
		info.WordWrap = false;
		info.ScrollBars = ScrollBars.Both;
		info.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
		y += info.Height + 10;

		//files
		label = new Label();
		label.Parent = this;
		label.Text = "Files";
		label.Location = new Point(0,y);
		label.AutoSize = true;
		files = new ListView();
		files.Parent = this;
		files.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
		files.Size = new Size(this.Width-label.Width-label.Left-20,files.Height*2);
		files.Location = new Point(label.Width+label.Left+5,y);
		files.View = View.Details;
		files.Columns.Add("Name",110,HorizontalAlignment.Left);
		files.Columns.Add("Action",100,HorizontalAlignment.Left);
		files.Columns.Add("Location",235,HorizontalAlignment.Left);
		files.SelectedIndexChanged += new EventHandler(files_SelectedIndexChanged);
		y += files.Height;

		//files menu
		EventHandler mAdd = new EventHandler(mAddOnClick);
		EventHandler mRemove = new EventHandler(mRemoveOnClick);
		EventHandler mEdit = new EventHandler(mEditOnClick);
		files.ItemActivate += mEdit;

		MenuItem[] ami = { new MenuItem("Add", mAdd), new MenuItem("Remove", mRemove), new MenuItem("Edit", mEdit) };
		files.ContextMenu = new ContextMenu(ami);
		files.ContextMenu.MenuItems[1].Enabled = false;
		files.ContextMenu.MenuItems[2].Enabled = false;
	}
	private void mAddOnClick(object obj, EventArgs ea)
	{
		//add item
		addFilesDialog dlg = new addFilesDialog("","",0);
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			if (dlg.name != "")
			{
				ListViewFileItem itm = new ListViewFileItem();
				itm.Text = dlg.name;
				itm.SubItems.Add(dlg.action);
				itm.SubItems.Add(dlg.location);
				//set offset and size to -1 so whole file added to patch
				itm.Offset = -1;
				itm.Size = -1;
				files.Items.Add(itm);
			}
		}
	}
	private void mRemoveOnClick(object obj, EventArgs ea)
	{
		//remove selected item(s)
		foreach (ListViewItem items in files.SelectedItems)
		{
			items.Remove();
		}
	}

	private void mEditOnClick(object obj, EventArgs ea)
	{
		if (files.SelectedItems.Count == 1)
		{
			//edit item
			int i = 0;
			if (files.SelectedItems[0].SubItems[1].Text == ActionStrings.Remove)
				i = 1;

			addFilesDialog dlg = new addFilesDialog(files.SelectedItems[0].Text,files.SelectedItems[0].SubItems[2].Text,i);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				files.SelectedItems[0].Text = dlg.name;
				files.SelectedItems[0].SubItems[1].Text = dlg.action;
				files.SelectedItems[0].SubItems[2].Text = dlg.location;
			}
		}
	}

	private void files_SelectedIndexChanged(object sender, EventArgs e)
	{
		//selected items in the list have changed so enable relevent items in the popup menu accordinly
		if (files.SelectedItems.Count > 0)
		{
			//remove
			files.ContextMenu.MenuItems[1].Enabled = true;
			//edit
			if (files.SelectedItems.Count == 1)
				files.ContextMenu.MenuItems[2].Enabled = true;
			else
				files.ContextMenu.MenuItems[2].Enabled = false;
		}
		else
		{
			files.ContextMenu.MenuItems[1].Enabled = false;
			files.ContextMenu.MenuItems[2].Enabled = false;
		}
	}

	private void getname_Click(object sender, EventArgs e)
	{
		//get filename and md5 checksum
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Select target exe";
		dlg.Filter = "Exe Files (*.exe)|*.exe|All Files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			filename.Text = Path.GetFileName(dlg.FileName);
			//get md5 checksum
			FileStream fs = null;
			BinaryReader br = null;
			try
			{
				fs = new FileStream(dlg.FileName,FileMode.Open);
				br = new BinaryReader(fs);
				System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] result = md5.ComputeHash(br.ReadBytes((int)fs.Length));
				checksum.Text = BitConverter.ToString(result).Replace("-","").ToLower();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			}
			finally
			{
				br.Close();
				fs.Close();
			}
		}
		dlg.Dispose();
	}
}