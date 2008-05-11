/*
dark_explorer
Copyright (C) 2005,2006,2007,2008 the_winch

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
//
//Dialog for editing display setttings stored in _virtual.dat
//

using System.Windows.Forms;
using System.Drawing;

class displayDialog : Form
{	
	TextBox width;
	TextBox height;
	TextBox depth;
	ComboBox mode;
	ComboBox quick;

	public int displayWidth
	{
		set
		{
			width.Text = value.ToString();
		}
		get
		{
			return int.Parse(width.Text);
		}
	}
	public int displayHeight
	{
		set
		{
			height.Text = value.ToString();
		}
		get
		{
			return int.Parse(height.Text);
		}
	}
	public int displayDepth
	{
		set
		{
			depth.Text = value.ToString();
		}
		get
		{
			return int.Parse(depth.Text);
		}
	}
	public int displayMode
	{
		set
		{
			mode.SelectedIndex = value;
		}
		get
		{
			return mode.SelectedIndex;
		}
	}

	public displayDialog()
	{
		Text = "Edit display settings";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;

		//width
		int y = 10;
		Label lab = new Label();
		lab.Parent = this;
		lab.Text = "Width";
		lab.AutoSize = true;
		lab.Location = new Point(10,y);
		width = new TextBox();
		width.Parent = this;
		width.Location = new Point(lab.Left + lab.Width + 5, y);
		width.Width = 110 - width.Left;
		y += lab.Height + 10;

		//height
		lab = new Label();
		lab.Parent = this;
		lab.Text = "Height";
		lab.AutoSize = true;
		lab.Location = new Point(10,y);
		height = new TextBox();
		height.Parent = this;
		height.Location = new Point(lab.Left + lab.Width + 5, y);
		height.Width = 110 - height.Left;
		y += lab.Height + 10;

		//depth
		lab = new Label();
		lab.Parent = this;
		lab.Text = "Depth";
		lab.AutoSize = true;
		lab.Location = new Point(10,y);
		depth = new TextBox();
		depth.Parent = this;
		depth.Location = new Point(lab.Left + lab.Width + 5, y);
		depth.Width = 110 - depth.Left;
		y += lab.Height + 10;

		//mode
		lab = new Label();
		lab.Parent = this;
		lab.Text = "Mode";
		lab.AutoSize = true;
		lab.Location = new Point(10, y);
		mode = new ComboBox();
		mode.Parent = this;
		mode.Location = new Point(lab.Left + lab.Width + 5, y);
		mode.Width = 175 - depth.Left;
		mode.DropDownStyle = ComboBoxStyle.DropDownList;
		mode.Items.Add("Hidden");
		mode.Items.Add("Windowed");
		mode.Items.Add("Windowed desktop");
		mode.Items.Add("Fullscreen exclusive");
		mode.Items.Add("Windowed fullscreen");
		y+= lab.Height + 10;

		//quick selector
		quick = new ComboBox();
		quick.Parent = this;
		quick.Location = new Point(150, 10);
		quick.Width = 95;
		quick.DropDownStyle = ComboBoxStyle.DropDownList;
		quick.Items.Add("640x480");
		quick.Items.Add("800x600");
		quick.Items.Add("1024x768");
		quick.Items.Add("1280x1024");
		quick.Items.Add("1600x1200");
		quick.SelectedIndexChanged += new System.EventHandler(quick_SelectedIndexChanged);

		y+= 5;
		//Cancel button
		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(170,y);
		btn.DialogResult = DialogResult.Cancel;
		CancelButton = btn;

		//Ok button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(80,y);
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;
		y += btn.Height + 30;

		Height = y;
		Width = 260;

		this.Activated += new System.EventHandler(displayDialog_Activated);
	}

	private void quick_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		//set text boxes to value of quick box
		string[] str = quick.SelectedItem.ToString().Split("x".ToCharArray(), 2);
		width.Text = str[0];
		height.Text = str[1];
	}

	private void displayDialog_Activated(object sender, System.EventArgs e)
	{
		//select item in quick box
		if (quick.Items.IndexOf(width.Text + "x" + height.Text) != -1)
			quick.SelectedIndex = quick.Items.IndexOf(width.Text + "x" + height.Text);
	}
}