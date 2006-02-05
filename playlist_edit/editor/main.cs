//
//
//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

class main: Form
{

	Area a = null;
	Area b = null;

	Panel CenterPanel;
	Button RSingle, RAll, LSingle, LAll;

	public static void Main()
	{
		Application.Run(new main());
	}
	public main()
	{
		Text = "Playlist editor";
		Width = 1024;
		Height = 768;

		//Center panel that holds the center buttons
		CenterPanel = new Panel();
		CenterPanel.Parent = this;
		CenterPanel.Location = new Point((this.Width / 2) - 35, 10);
		CenterPanel.Size = new Size(65, this.Height - 50);
		CenterPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		CenterPanel.Resize += new EventHandler(CenterPanel_Resize);

		//Move selection right
		RSingle = new Button();
		RSingle.Parent = CenterPanel;
		RSingle.FlatStyle = FlatStyle.Flat;
		RSingle.Text = ">";
		RSingle.Height = CenterPanel.Height / 4;
		RSingle.Dock = DockStyle.Bottom;
		RSingle.Click += new EventHandler(RSingle_Click);

		//Move all right
		RAll = new Button();
		RAll.Parent = CenterPanel;
		RAll.FlatStyle = FlatStyle.Flat;
		RAll.Text = ">>";
		RAll.Dock = DockStyle.Bottom;
		RAll.Height = CenterPanel.Height / 4;
		RAll.Click += new EventHandler(RAll_Click);

		//Move all left
		LAll = new Button();
		LAll.Parent = CenterPanel;
		LAll.FlatStyle = FlatStyle.Flat;
		LAll.Text = "<<";
		LAll.Dock = DockStyle.Bottom;
		LAll.Height = CenterPanel.Height / 4;
		LAll.Click += new EventHandler(LAll_Click);

		//Move Selection left
		LSingle = new Button();
		LSingle.Parent = CenterPanel;
		LSingle.FlatStyle = FlatStyle.Flat;
		LSingle.Text = "<";
		LSingle.Height = CenterPanel.Height / 4;
		LSingle.Dock = DockStyle.Bottom;
		LSingle.Click += new EventHandler(LSingle_Click);

		a = new Area(this, new Point(10, 10), new Size((this.Width / 2) - 50, this.Height - 50), DockStyle.Left, AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom);
		b = new Area(this, new Point((this.Width / 2) + 35, 10), new Size((this.Width / 2) - 50, this.Height - 50), DockStyle.Right, AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);

	}

	private void CenterPanel_Resize(object sender, EventArgs e)
	{
		//Resize the center buttons to fit the new panel size
		RSingle.Height = CenterPanel.Height / 4;
		RAll.Height = CenterPanel.Height / 4;
		LAll.Height = CenterPanel.Height / 4;
		LSingle.Height = CenterPanel.Height / 4;
	}

	private void RSingle_Click(object sender, EventArgs e)
	{
		//move selected right
		Hashtable hta = (Hashtable) a.view[a.selected];
		Hashtable htb = (Hashtable) b.view[b.selected];
		ListView la = (ListView) hta["list"];
		ListView lb = (ListView) htb["list"];
		foreach (ListViewItem lvi in la.SelectedItems)
		{
			lvi.Remove();
			lb.Items.Add(lvi);
		}
	}

	private void RAll_Click(object sender, EventArgs e)
	{
		//move all right
		Hashtable hta = (Hashtable) a.view[a.selected];
		Hashtable htb = (Hashtable) b.view[b.selected];
		ListView la = (ListView) hta["list"];
		ListView lb = (ListView) htb["list"];
		la.BeginUpdate();
		lb.BeginUpdate();
		foreach (ListViewItem lvi in la.Items)
		{
			lvi.Remove();
			lb.Items.Add(lvi);
		}
		la.EndUpdate();
        lb.EndUpdate();
	}

	private void LAll_Click(object sender, EventArgs e)
	{
		//move all left
		Hashtable hta = (Hashtable) a.view[a.selected];
		Hashtable htb = (Hashtable) b.view[b.selected];
		ListView la = (ListView) hta["list"];
		ListView lb = (ListView) htb["list"];
		la.BeginUpdate();
		lb.BeginUpdate();
		foreach (ListViewItem lvi in lb.Items)
		{
			lvi.Remove();
			la.Items.Add(lvi);
		}
		la.EndUpdate();
		lb.EndUpdate();
	}

	private void LSingle_Click(object sender, EventArgs e)
	{
		//move selected left
		Hashtable hta = (Hashtable) a.view[a.selected];
		Hashtable htb = (Hashtable) b.view[b.selected];
		ListView la = (ListView) hta["list"];
		ListView lb = (ListView) htb["list"];
		foreach (ListViewItem lvi in lb.SelectedItems)
		{
			lvi.Remove();
			la.Items.Add(lvi);
		}
	}
}