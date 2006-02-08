//
// window stuff
//

using System;
using System.Drawing;
using System.Windows.Forms;

class window : Form
{

	public ListView Intern;
	public ListView Extern;
	public TextBox Targetexe;
	public Button InternUp;
	public Button InternDown;
	public Button ExternUp;
	public Button ExternDown;
	public CheckBox CheckSum;
	public ComboBox exeType;

	public window()
	{
		Text = "Dbpro tiny exe tool";
		Width = 535;
		Height = 565;

		//internal files
		GroupBox box = new GroupBox();
		box.Parent = this;
		box.Text = "Internal files (build into tiny exe)";
		box.Location = new Point(10,10);
		box.Width = 510;
		box.Height = 200;
		box.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

		Intern = new ListView();
		Intern.Parent = box;
		Intern.Location = new Point(10,15);
		Intern.Width = 450;
		Intern.Height = 175;
		Intern.View = View.Details;
		Intern.Columns.Add("Name",155,HorizontalAlignment.Left);
		Intern.Columns.Add("Location",290,HorizontalAlignment.Left);
		Intern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		Intern.SelectedIndexChanged += new EventHandler(Intern_SelectedIndexChanged);
		Intern.Enabled = false;

		//up downbuttons
		InternUp = new Button();
		InternUp.Parent = box;
		InternUp.Text = "/\\";
		InternUp.Location = new Point(470,15);
		InternUp.Width = 30;
		InternUp.Height = 60;
		InternUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		InternUp.Enabled = false;
		InternUp.Click += new EventHandler(InternUp_Click);

		InternDown = new Button();
		InternDown.Parent = box;
		InternDown.Text = "\\/";
		InternDown.Location = new Point(470,80);
		InternDown.Width = 30;
		InternDown.Height = 60;
		InternDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		InternDown.Enabled = false;
		InternDown.Click += new EventHandler(InternDown_Click);

		//external files
		box = new GroupBox();
		box.Parent = this;
		box.Text = "External files (sourced from dbpro compiler dir)";
		box.Location = new Point(10,220);
		box.Width = 510;
		box.Height = 210;
		box.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		Extern = new ListView();
		Extern.Parent = box;
		Extern.Location = new Point(10,15);
		Extern.Width = 450;
		Extern.Height = 185;
		Extern.View = View.Details;
		Extern.Columns.Add("Name",155,HorizontalAlignment.Left);
		Extern.Columns.Add("Location",290,HorizontalAlignment.Left);
		Extern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
		Extern.SelectedIndexChanged += new EventHandler(Extern_SelectedIndexChanged);
		Extern.Enabled = false;

		//up downbuttons
		ExternUp = new Button();
		ExternUp.Parent = box;
		ExternUp.Text = "/\\";
		ExternUp.Location = new Point(470,15);
		ExternUp.Width = 30;
		ExternUp.Height = 60;
		ExternUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		ExternUp.Enabled = false;
		ExternUp.Click += new EventHandler(ExternUp_Click);

		ExternDown = new Button();
		ExternDown.Parent = box;
		ExternDown.Text = "\\/";
		ExternDown.Location = new Point(470,80);
		ExternDown.Width = 30;
		ExternDown.Height = 60;
		ExternDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		ExternDown.Enabled = false;
		ExternDown.Click += new EventHandler(ExternDown_Click);

		//Intern Menu
		EventHandler mInternAdd = new EventHandler(mInterAddOnClick);
		EventHandler mInternRemove = new EventHandler(mInterRemoveOnClick);
		EventHandler mInternEdit = new EventHandler(mInterEditOnClick);
		EventHandler mInternMove = new EventHandler(mInterMoveOnClick);

		MenuItem[] ami = { new MenuItem("Add",mInternAdd), new MenuItem("Remove",mInternRemove), new MenuItem("Edit",mInternEdit), new MenuItem("Move to External",mInternMove) };
		Intern.ContextMenu = new ContextMenu(ami);
		Intern.ContextMenu.MenuItems[1].Enabled = false;
		Intern.ContextMenu.MenuItems[2].Enabled = false;
		Intern.ContextMenu.MenuItems[3].Enabled = false;
		Intern.ItemActivate += mInternEdit;

		//Extern Menu
		EventHandler mExternAdd = new EventHandler(mExternAddOnClick);
		EventHandler mExternRemove = new EventHandler(mExternRemoveOnClick);
		EventHandler mExternEdit = new EventHandler(mExternEditOnClick);
		EventHandler mExternMove = new EventHandler(mExternMoveOnClick);

		MenuItem[] Eami = { new MenuItem("Add",mExternAdd), new MenuItem("Remove",mExternRemove), new MenuItem("Edit",mExternEdit), new MenuItem("Move to Internal",mExternMove) };
		Extern.ContextMenu = new ContextMenu(Eami);
		Extern.ContextMenu.MenuItems[1].Enabled = false;
		Extern.ContextMenu.MenuItems[2].Enabled = false;
		Extern.ContextMenu.MenuItems[3].Enabled = false;
		Extern.ItemActivate += mExternEdit;

		//checksum checkbox
		CheckSum = new CheckBox();
		CheckSum.Parent = this;
		CheckSum.Text = "Include checksums for external files";
		CheckSum.Text += " (checksums generated from files in dbpro compiler dir not exe)";
		CheckSum.Location = new Point(10,430);
		CheckSum.Width += 415;
		CheckSum.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

		//filename textbox
		box = new GroupBox();
		box.Parent = this;
		box.Text = "Target exe name and type";
		box.Location = new Point(10,455);
		box.Height = 45;
		box.Width = 510;
		box.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		//exe type combobox
		exeType = new ComboBox();
		exeType.Parent = box;
		exeType.Location = new Point(10, 15);
		exeType.Width = 130;
		exeType.DropDownStyle = ComboBoxStyle.DropDownList;
		exeType.Items.Add("New style (compress.dll)");
		exeType.Items.Add("Old style");
		exeType.SelectedIndex = 0;
		exeType.SelectedIndexChanged += new EventHandler(exeType_SelectedIndexChanged);

		Targetexe = new TextBox();
		Targetexe.Parent = box;
		Targetexe.Location = new Point(150,15);
		Targetexe.Width = 350;
		Targetexe.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
		Targetexe.Enabled = false;

	}

	//Intern menu stuff

	private void mInterAddOnClick(object obj, EventArgs ea)
	{
        //add item
		addFilesDialog dlg = new addFilesDialog("","");
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			if (dlg.name != "")
			{
				ListViewFileItem itm = new ListViewFileItem();
				itm.Text = dlg.name;
				itm.SubItems.Add(dlg.location);
				Intern.Items.Add(itm);
			}
		}
	}
	private void mInterRemoveOnClick(object obj, EventArgs ea)
	{
		//remove item
		foreach (ListViewFileItem items in Intern.SelectedItems)
		{
			items.Remove();
		}
	}
	private void mInterEditOnClick(object obj, EventArgs ea)
	{
		//edit item
		if (Intern.SelectedItems.Count == 1 && Intern.SelectedItems[0].SubItems[1].Text != "<exe>")
		{
			addFilesDialog dlg = new addFilesDialog(Intern.SelectedItems[0].Text,Intern.SelectedItems[0].SubItems[1].Text);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Intern.SelectedItems[0].Text = dlg.name;
				Intern.SelectedItems[0].SubItems[1].Text = dlg.location;
			}
		}
	}
	private void mInterMoveOnClick(object obj, EventArgs ea)
	{
		//move item to Extern
		foreach (ListViewFileItem items in Intern.SelectedItems)
		{
			items.Remove();
			Extern.Items.Add(items);
		}
	}

	private void Intern_SelectedIndexChanged(object sender, EventArgs e)
	{
		//Selected items in Intern changed so select menu item accordingly
		if (Intern.SelectedItems.Count > 0)
		{
			if (Intern.SelectedItems.Count == 1 && Intern.SelectedItems[0].SubItems[1].Text != "<exe>")
				Intern.ContextMenu.MenuItems[2].Enabled = true;
			else
				Intern.ContextMenu.MenuItems[2].Enabled = false;
			Intern.ContextMenu.MenuItems[1].Enabled = true;
			Intern.ContextMenu.MenuItems[3].Enabled = true;
		}
		else
		{
			Intern.ContextMenu.MenuItems[1].Enabled = false;
			Intern.ContextMenu.MenuItems[2].Enabled = false;
			Intern.ContextMenu.MenuItems[3].Enabled = false;
		}
		if (Intern.SelectedItems.Count == 1)
		{
			//enable up/down buttons
			InternUp.Enabled = true;
			InternDown.Enabled = true;
		}
		else
		{
			//disable up/down buttons
			InternUp.Enabled = false;
			InternDown.Enabled = false;
		}
	}

	//Extern menu stuff

	private void mExternAddOnClick(object obj, EventArgs ea)
	{
		//add item
		addFilesDialog dlg = new addFilesDialog("","");
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			if (dlg.name != "")
			{
				ListViewFileItem itm = new ListViewFileItem();
				itm.Text = dlg.name;
				itm.SubItems.Add(dlg.location);
				Extern.Items.Add(itm);
			}
		}
	}
	private void mExternRemoveOnClick(object obj, EventArgs ea)
	{
		//remove item
		foreach (ListViewFileItem items in Extern.SelectedItems)
		{
			items.Remove();
		}
	}
	private void mExternEditOnClick(object obj, EventArgs ea)
	{
		//edit item
		if (Extern.SelectedItems.Count == 1)
		{
			addFilesDialog dlg = new addFilesDialog(Extern.SelectedItems[0].Text,Extern.SelectedItems[0].SubItems[1].Text);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Extern.SelectedItems[0].Text = dlg.name;
				Extern.SelectedItems[0].SubItems[1].Text = dlg.location;
			}
		}
	}
	private void mExternMoveOnClick(object obj, EventArgs ea)
	{
		//move item to Extern
		foreach (ListViewFileItem items in Extern.SelectedItems)
		{
			items.Remove();
			Intern.Items.Add(items);
		}
	}

	private void Extern_SelectedIndexChanged(object sender, EventArgs e)
	{
		//Selected items in Extern changed so select menu item accordingly
		if (Extern.SelectedItems.Count > 0)
		{
			if (Extern.SelectedItems.Count == 1 && Extern.SelectedItems[0].SubItems[1].Text != "<exe>")
				Extern.ContextMenu.MenuItems[2].Enabled = true;
			else
				Extern.ContextMenu.MenuItems[2].Enabled = false;

			Extern.ContextMenu.MenuItems[1].Enabled = true;
			Extern.ContextMenu.MenuItems[3].Enabled = true;
		}
		else
		{
			Extern.ContextMenu.MenuItems[1].Enabled = false;
			Extern.ContextMenu.MenuItems[2].Enabled = false;
			Extern.ContextMenu.MenuItems[3].Enabled = false;
		}
		if (Extern.SelectedItems.Count == 1)
		{
			//enable up/down buttons
			ExternUp.Enabled = true;
			ExternDown.Enabled = true;
		}
		else
		{
			//disable up/down buttons
			ExternUp.Enabled = false;
			ExternDown.Enabled = false;
		}
	}

	private void InternUp_Click(object sender, EventArgs e)
	{
		//up button
		if (Intern.SelectedItems[0].Index > 0)
		{
			int i = Intern.SelectedItems[0].Index;
			ListViewFileItem item = (ListViewFileItem) Intern.SelectedItems[0];
			Intern.SelectedItems[0].Remove();
			Intern.Items.Insert(i-1,item);
		}
	}

	private void InternDown_Click(object sender, EventArgs e)
	{
		//down button
		if (Intern.SelectedItems[0].Index < Intern.Items.Count - 1)
		{
			int i = Intern.SelectedItems[0].Index;
			ListViewItem item = Intern.SelectedItems[0];
			Intern.SelectedItems[0].Remove();
			Intern.Items.Insert(i+1,item);
		}
	}

	private void ExternUp_Click(object sender, EventArgs e)
	{
		//up button
		if (Extern.SelectedItems[0].Index > 0)
		{
			int i = Extern.SelectedItems[0].Index;
			ListViewItem item = Extern.SelectedItems[0];
			Extern.SelectedItems[0].Remove();
			Extern.Items.Insert(i-1,item);
		}
	}

	private void ExternDown_Click(object sender, EventArgs e)
	{
		//down button
		if (Extern.SelectedItems[0].Index < Extern.Items.Count - 1)
		{
			int i = Extern.SelectedItems[0].Index;
			ListViewItem item = Extern.SelectedItems[0];
			Extern.SelectedItems[0].Remove();
			Extern.Items.Insert(i+1,item);
		}
	}

	private void exeType_SelectedIndexChanged(object sender, EventArgs e)
	{
		//exe type changed
		if (exeType.SelectedIndex == 0)
		{
			//new type
			Targetexe.Enabled = false;
		}
		else
		{
			//old type
			Targetexe.Enabled = true;
		}
	}
}