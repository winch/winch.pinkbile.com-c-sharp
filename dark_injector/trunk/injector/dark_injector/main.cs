//
// dark_injector © 2005 the_winch
//
// http://winch.pinkbile.com
// dbp@pinkbile.com
//

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

class window : Form
{
	TextBox proExe;
	ListBox dllList;
	TextBox dllDesc;

	public static void Main()
	{
		Application.Run(new window());
	}
	public window()
	{
		Text = "dark_injector";
		Size = new Size(450, 370);
		int y = 5;

		//Dbpro exe
		GroupBox gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Dbpro exe";
		gb.Location = new Point(5, y);
		gb.Width = this.Width - 20;
		gb.Height = 45;
		gb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

		proExe = new TextBox();
		proExe.Parent = gb;
		proExe.Location = new Point(5,15);
		proExe.Width = gb.Width - 50;
		proExe.Anchor = AnchorStyles.Left | AnchorStyles.Right;

		Button proExeBrowse = new Button();
		proExeBrowse.Parent = gb;
		proExeBrowse.Text = "...";
		proExeBrowse.Width = 30;
		proExeBrowse.Height = proExe.Height;
		proExeBrowse.Location = new Point(gb.Width - proExeBrowse.Width - 10, 15);
		proExeBrowse.Anchor = AnchorStyles.Right;
		proExeBrowse.Click += new EventHandler(proExeBrowse_Click);
		y+= gb.Height + 5;

		//dlls
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Dll to inject";
		gb.Location = new Point(5, y);
		gb.Width = this.Width - 20;
		gb.Height = 250;
		gb.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
		
		dllList = new ListBox();
		dllList.Parent = gb;
		dllList.Location = new Point(5,15);
		dllList.Width = 150;
		dllList.Height = gb.Height - 20;
		dllList.Sorted = true;
		dllList.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
		dllList.SelectedIndexChanged += new EventHandler(dllList_SelectedIndexChanged);

		dllDesc = new TextBox();
		dllDesc.Parent = gb;
		dllDesc.Multiline = true;
		dllDesc.Location = new Point(dllList.Left + dllList.Width + 10, 15);
		dllDesc.Height = gb.Height - 30;
		dllDesc.Width = 250;
		dllDesc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		y += gb.Height + 10;

		//inject button
		Button btnInject = new Button();
		btnInject.Parent = this;
		btnInject.Text = "&Inject dll";
		btnInject.Location = new Point(5, y);
		btnInject.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		btnInject.Click += new EventHandler(btnInject_Click);

		//about button
		Button btnAbout = new Button();
		btnAbout.Parent = this;
		btnAbout.Text = "&About";
		btnAbout.Location = new Point((this.Width / 2) - (btnAbout.Width / 2), y);
		btnAbout.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		btnAbout.Click += new EventHandler(btnAbout_Click);

		//exit button
		Button btnExit = new Button();
		btnExit.Parent = this;
		btnExit.Text = "E&xit";
		btnExit.Location = new Point(this.Width - btnExit.Width - 15, y);
		btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		btnExit.Click += new EventHandler(btnExit_Click);

		this.Load += new EventHandler(window_Load);
	}

	private void proExeBrowse_Click(object sender, EventArgs e)
	{
		//browse for new proExe
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Select Dbpro exe or pck";
		ofd.Filter = "Exe or Pck Files (*.exe, *.pck)|*.exe;*.pck|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			proExe.Text = ofd.FileName;
		}
		ofd.Dispose();
	}

	private void window_Load(object sender, EventArgs e)
	{
		//generate list of dlls.
		try
		{
			string[] files = Directory.GetDirectories(Application.StartupPath + Path.DirectorySeparatorChar + "dll", "*");
			foreach (string str in files)
			{
				//check for compress.dll in directory
				if (File.Exists(str + "\\compress.dll"))
				{
					dllList.Items.Add(str.Substring(str.LastIndexOf(Path.DirectorySeparatorChar) + 1,
						str.Length - str.LastIndexOf(Path.DirectorySeparatorChar) - 1));
				}
			}
			//Seclect first item in list
			if (dllList.Items.Count > 0)
			{
				dllList.SelectedIndex = 0;
			}
		}
		catch (DirectoryNotFoundException)
		{
			MessageBox.Show("dll subdirectory is missing.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	private void dllList_SelectedIndexChanged(object sender, EventArgs e)
	{
		//Load description text for selected dll
		if (dllList.SelectedIndex > -1)
		{
			string file = Application.StartupPath + Path.DirectorySeparatorChar + "dll" + Path.DirectorySeparatorChar
				+ dllList.Items[dllList.SelectedIndex] + Path.DirectorySeparatorChar + "desc.txt";
			if (File.Exists(file))
			{
				StreamReader sr = new StreamReader(file);
				dllDesc.Text = sr.ReadToEnd();
			}
			else
			{
				dllDesc.Text = "No description found.";
			}
		}
	}

	private void btnInject_Click(object sender, EventArgs e)
	{
		//inject dll
		if (File.Exists(proExe.Text) == false)
		{
			MessageBox.Show("Dbpro Exe not found.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		if (dllList.SelectedIndex == -1)
		{
			MessageBox.Show("No dll selected.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		string selectedDll = Application.StartupPath + Path.DirectorySeparatorChar + "dll" + Path.DirectorySeparatorChar
			+ dllList.Items[dllList.SelectedIndex] + Path.DirectorySeparatorChar + "compress.dll";
		if (File.Exists(selectedDll) == false)
		{
			MessageBox.Show("Selected dll dir does not contain \"compress.dll\".", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save injected exe or pck as";
		sfd.Filter = "Exe and Pck Files (*.exe, *.pck)|*.exe;*.pck|All Files (*.*)|*.*";
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			Cursor.Current = Cursors.WaitCursor;
			DB.proExe.InjectDll(proExe.Text, sfd.FileName, selectedDll);
			Cursor.Current = Cursors.Default;
		}
		sfd.Dispose();
	}

	private void btnAbout_Click(object sender, EventArgs e)
	{
		//About button
		aboutDialog ad = new aboutDialog();
		ad.ShowDialog();
	}

	private void btnExit_Click(object sender, EventArgs e)
	{
		//Exit button
		Application.Exit();
	}
}
