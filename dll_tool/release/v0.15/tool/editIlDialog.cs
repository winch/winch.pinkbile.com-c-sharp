/*
dll_tool
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
//Dialog for editing il
//

using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text;

class editIlDialog: Form
{
	TextBox textBox;

	public string il
	{
		set
		{
			textBox.Text = value;
			textBox.SelectionLength = 0;
			textBox.SelectionStart = 0;
		}
		get
		{
			return textBox.Text;
		}
	}

	public editIlDialog()
	{
		Text = "Edit il";
		this.Size = new Size(640,480);
		ShowInTaskbar = false;

		textBox = new TextBox();
		textBox.Parent = this;
		textBox.Multiline = true;
		textBox.ScrollBars = ScrollBars.Both;
		textBox.WordWrap = false;
		textBox.Location = new Point(10,10);
		textBox.Size = new Size(610, 400);
		textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		//Cancel button
		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(535, 420);
		btn.Width += 5;
		btn.DialogResult = DialogResult.Cancel;
		btn.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
		CancelButton = btn;

		//Ok button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(435, 420);
		btn.Width += 5;
		btn.DialogResult = DialogResult.OK;
		btn.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
		
		//Load button
		Button load = new Button();
		load.Parent = this;
		load.Text = "Load";
		load.Location = new Point(10, 420);
		load.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		load.Click += new System.EventHandler(load_Click);

		//Save Button
		Button save = new Button();
		save.Parent = this;
		save.Text = "Save";
		save.Location = new Point(load.Left + load.Width + 10, 420);
		save.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		save.Click +=new System.EventHandler(save_Click);
	}

	private void load_Click(object sender, System.EventArgs e)
	{
		//load il code from file
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Load il code";
		ofd.Filter = "il code (*.il)|*.il|Text files(*.txt)|*.txt|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			FileStream fs = null;
			StreamReader sr = null;
			try
			{
				fs = new FileStream(ofd.FileName, FileMode.Open);
				sr = new StreamReader(fs);
				textBox.Text = sr.ReadToEnd();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (sr != null)
					sr.Close();
				if (fs != null)
					fs.Close();
			}
		}
	}

	private void save_Click(object sender, System.EventArgs e)
	{
		//save il code to file
		SaveFileDialog sfd = new SaveFileDialog();
		sfd.Title = "Save il code";
		sfd.Filter = "il code (*.il)|*.il|Text files(*.txt)|*.txt|All Files (*.*)|*.*";
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			FileStream fs = null;
			StreamWriter sw = null;
			try
			{
				fs = new FileStream(sfd.FileName, FileMode.Create);
				sw = new StreamWriter(fs);
				sw.Write(textBox.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (sw != null)
					sw.Close();
				if (fs != null)
					fs.Close();
			}
		}
	}
	public static void editIlasm(ListBox lb)
	{
		//shows a dialog for the user to edit il code then sticks modified il in listbox
		editIlDialog ed = new editIlDialog();
		StringBuilder sb = new StringBuilder();
		bool first = true;
		foreach(object o in lb.Items)
		{
			if (first == true)
			{
				first = false;
			}
			else
			{
				sb.Append("\r\n");
			}
			sb.Append(o.ToString());
		}
		ed.il = sb.ToString();
		if (ed.ShowDialog() == DialogResult.OK)
		{
			string[] str = ed.il.Split("\n".ToCharArray());
			lb.BeginUpdate();
			lb.Items.Clear();
			foreach (string s in str)
			{
				lb.Items.Add(s.Trim());
			}
			lb.EndUpdate();
		}
	}
}
			