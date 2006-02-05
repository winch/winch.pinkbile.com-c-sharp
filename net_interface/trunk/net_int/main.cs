/*
net_int
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
// main source file
//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using Microsoft.Win32;

class main: Form
{

	GroupBox pluginsGb, pluginsUserGb;
	CheckedListBox plugins, pluginsUser;
	TextBox nameSpace, className;
	ComboBox Combotemplate;
	ListView search;
	ArrayList searchList = new ArrayList();
	string darkBasic = ""; // darkbasic pro install location

	public static void Main()
	{
		Application.Run(new main());
	}

	public main()
	{
		Text = ".net interface builder";
		Size = new Size(500,535);

		int y = 10;

		//plugins groupbox
		pluginsGb = new GroupBox();
		pluginsGb.Parent = this;
		pluginsGb.Text = "Plugins";
		pluginsGb.Location = new Point(10, y);
		pluginsGb.Size = new Size(200,240);
		pluginsGb.Anchor = AnchorStyles.Left | AnchorStyles.Top;
		y += pluginsGb.Height + 10;

		//plugins checklistbox
		plugins = new CheckedListBox();
		plugins.Parent = pluginsGb;
		plugins.Location = new Point(10,15);
		plugins.Size = new Size(180, 215);
		plugins.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

		//plugins-user groupbox
		pluginsUserGb = new GroupBox();
		pluginsUserGb.Parent = this;
		pluginsUserGb.Text = "Plugins-user";
		pluginsUserGb.Location = new Point(10, y);
		pluginsUserGb.Size = new Size(200,240);
		pluginsUserGb.Anchor = AnchorStyles.Left | AnchorStyles.Top;
		y += pluginsUserGb.Height + 10;

		//pluginsUser checklistbox
		pluginsUser = new CheckedListBox();
		pluginsUser.Parent = pluginsUserGb;
		pluginsUser.Location = new Point(10, 15);
		pluginsUser.Size = new Size(180, 215);
		pluginsUser.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

		y = 10;

		//namespace label
		Label lab = new Label();
		lab.Parent = this;
		lab.AutoSize = true;
		lab.Text = "Namespace";
		lab.Location = new Point(220, y);

		//class label
		lab = new Label();
		lab.Parent = this;
		lab.AutoSize = true;
		lab.Text = "Class name";
		lab.Location = new Point(360, y);
		y += lab.Height + 0;

		//namespace textbox
		nameSpace = new TextBox();
		nameSpace.Parent = this;
		nameSpace.Location = new Point(220,y);
		nameSpace.Width = 130;		

		//class textbox
		className = new TextBox();
		className.Parent = this;
		className.Location = new Point(355, y);
		className.Width = 130;
		className.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
		y += className.Height + 10;

		//build button
		Button btnBuild = new Button();
		btnBuild.Parent = this;
		btnBuild.Text = "&Build";
		btnBuild.Width -= 20;
		btnBuild.Location = new Point(360, y);
		btnBuild.Click += new EventHandler(btnBuild_Click);

		//about button
		Button btnAbout = new Button();
		btnAbout.Parent = this;
		btnAbout.Text = "&About";
		btnAbout.Width -= 20;
		btnAbout.Location = new Point(360 + btnBuild.Width + 15, y);
		btnAbout.Click += new EventHandler(btnAbout_Click);

		//exit button
		Button btnExit = new Button();
		btnExit.Parent = this;
		btnExit.Text = "E&xit";
		btnExit.Width -= 20;
		btnExit.Location = new Point(360 + btnBuild.Width + 15, y);
		btnExit.Click += new EventHandler(btnExit_Click);
		btnExit.Visible = false;

		//template groupbox
		GroupBox gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Template";
		gb.Location = new Point(220, y);
		gb.Size = new Size(120, 45);
		y += gb.Height + 10;

		//template combobox
		Combotemplate = new ComboBox();
		Combotemplate.Parent = gb;
		Combotemplate.Width = 100;
		Combotemplate.Location = new Point(10, 15);
		Combotemplate.DropDownStyle = ComboBoxStyle.DropDownList;

		//search groupbox
		gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Search functions";
		gb.Location = new Point(220, y);
		gb.Size = new Size(this.Width - gb.Left - 15, this.Height - gb.Height - 40);
		gb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		//search listview
		search = new ListView();
		search.Parent = gb;
		search.Location = new Point(10, 45);
		search.Size = new Size(gb.Width - 20, gb.Height - 55);
		search.View = View.Details;
		search.Columns.Add("Function name", 120, HorizontalAlignment.Left);
		search.Columns.Add("Dll", 120, HorizontalAlignment.Left);
		search.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

		//search textbox
		TextBox searchBox = new TextBox();
		searchBox.Parent = gb;
		searchBox.Location = new Point(10, 15);
		searchBox.Width = 190;
		searchBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
		searchBox.TextChanged += new EventHandler(searchBox_TextChanged);

		//search button
		Button btnSearch = new Button();
		btnSearch.Parent = gb;
		btnSearch.Text = "build";
		btnSearch.Location = new Point(searchBox.Left + searchBox.Width + 5, 15);
		btnSearch.Width = 50;
		btnSearch.Anchor = AnchorStyles.Right | AnchorStyles.Top;
		btnSearch.Click += new EventHandler(btnSearch_Click);

		this.Load += new EventHandler(main_Load);
		this.Resize += new EventHandler(main_Resize);
	}

	private void main_Resize(object sender, EventArgs e)
	{
		//set groupboxes to correct size
		pluginsGb.Height = (this.Height / 2) - 30;
		pluginsUserGb.Height = pluginsGb.Height;
		pluginsUserGb.Top = this.Height / 2 - 10;
	}

	private void main_Load(object sender, EventArgs e)
	{
		//form loaded, get lists of plugins
		string[] files;
		RegistryKey reg = null;
		try
		{
			reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Dark Basic\\Dark Basic Pro");
			if (reg == null)
			{
				string msg;
				msg  = "DarkBASIC pro location not found\n\n";
				msg += "You must have DarkBasic Pro installed for this program\n";
				msg += "and the exes it produces to work correctly";
				MessageBox.Show(msg, "Error!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				darkBasic = reg.GetValue("INSTALL-PATH").ToString() + Path.DirectorySeparatorChar + "compiler" + Path.DirectorySeparatorChar;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		finally
		{
			if (reg != null)
				reg.Close();
		}
		if (darkBasic != "")
		{
			//get dlls in plugins dir
			files = Directory.GetFiles(darkBasic + "plugins", "*.dll");
			plugins.BeginUpdate();
			foreach (string str in files)
			{
				plugins.Items.Add(Path.GetFileName(str), false);
			}
			plugins.EndUpdate();
			//get dlls in plugins-user dir
			files = Directory.GetFiles(darkBasic + "plugins-user", "*.dll");
			pluginsUser.BeginUpdate();
			foreach (string str in files)
			{
				pluginsUser.Items.Add(Path.GetFileName(str), false);
			}
			pluginsUser.EndUpdate();
		}
		//get list of templates
		files = Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "template", "*.txt");
		foreach (string str in files)
		{
			Combotemplate.Items.Add(new template(str));
		}
		if (Combotemplate.Items.Count == 0)
		{
			MessageBox.Show("No templates found!", "Error!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		else
		{
			Combotemplate.SelectedIndex = 0;
		}
	}

	private void btnBuild_Click(object sender, EventArgs e)
	{
		//build inteface file
		template tp = (template) Combotemplate.Items[Combotemplate.SelectedIndex];
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save interface class";
		sfd.Filter = tp.filter;
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			Cursor.Current = Cursors.WaitCursor;
			Build.DoBuild(sfd.FileName, darkBasic, plugins, pluginsUser, nameSpace.Text, className.Text, tp);
			Cursor.Current = Cursors.Default;
		}
	}

	private void btnAbout_Click(object sender, EventArgs e)
	{
		//about button
		aboutDialog ad = new aboutDialog();
		ad.ShowDialog();
	}

	private void btnExit_Click(object sender, EventArgs e)
	{
		//exit button
		Close();
	}

	private void btnSearch_Click(object sender, EventArgs e)
	{
		//build list of search items
		Search.BuildList(darkBasic, plugins, pluginsUser, search, searchList);
	}

	private void searchBox_TextChanged(object sender, EventArgs e)
	{
		//do search
		TextBox st = (TextBox) sender;
		Search.DoSearch(st.Text, search, searchList);
	}
}
		