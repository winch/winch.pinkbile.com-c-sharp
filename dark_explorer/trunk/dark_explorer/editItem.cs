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
//editItem dialog, lets user edit a ListViewFileItem
//

using System.Windows.Forms;
using System.Drawing;

class editItem : Form
{
	TextBox txtBox;
	CheckBox media;
	ComboBox type;
	ComboBox upx;
	CheckBox str;
	
	public string FileName
	{
		get
		{
			return txtBox.Text;
		}
	}
	public string Filetype
	{
		get
		{
			if (type.SelectedIndex == 0)
				return ListViewStrings.Yes;
			if (type.SelectedIndex == 1)
				return ListViewStrings.No;
			return "";
		}
	}
	public string Upx
	{
		get
		{
			switch (upx.SelectedIndex)
			{
				case 0:
					return ListViewStrings.No;
				case 1:
					return ListViewStrings.UpxStandard;
				case 2:
					return ListViewStrings.UpxLzma;
				default:
					return ListViewStrings.No;
			}
		}
	}
	public string StringNull
	{
		get
		{
			if (str.Checked)
				return ListViewStrings.Yes;
			else
				return ListViewStrings.No;
		}
	}

	public editItem(string text, string file, string useUpx, string nullStrings)
	{
		Text = "Edit item";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(480,125);
		StartPosition = FormStartPosition.CenterParent;

		//text box
		txtBox = new TextBox();
		txtBox.Parent = this;
		txtBox.Location = new Point(10,10);
		txtBox.Size = new Size(455,txtBox.Height);
		txtBox.Text = text;

		//Cancel button
		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(390,70);
		btn.DialogResult = DialogResult.Cancel;
		CancelButton = btn;

		//Ok button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(300,70);
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;

		//media checkbox button
		media = new CheckBox();
		media.Parent = this;
		media.Text = "&Add \"media\\\" prefix";
		media.Location = new Point(10,40);
		media.Width += 25;
		if (txtBox.Text.StartsWith("media\\"))
			media.Checked = true;
		media.CheckStateChanged += new System.EventHandler(media_Click);

		//type combobox
		type = new ComboBox();
		type.Parent = this;
		type.DropDownStyle = ComboBoxStyle.DropDownList;
		type.Location = new Point(150,40);
		type.Items.Add("Attached file");
		type.Items.Add("Special file");
		if (file == ListViewStrings.Yes)
			type.SelectedIndex = 0;
		else
			type.SelectedIndex = 1;

		//upx label
		Label label = new Label();
		label.Parent = this;
		label.Location = new Point(10,70);
		label.AutoSize = true;
		label.Text = "Upx compression:";

		//upx combobox
		upx = new ComboBox();
		upx.Parent = this;
		upx.Location = new Point(105, 70);
		upx.Width -= 50;
		upx.DropDownStyle = ComboBoxStyle.DropDownList;
		upx.Items.Add("None");
		upx.Items.Add("Standard");
		upx.Items.Add("LZMA");
		switch (useUpx)
		{
			case ListViewStrings.No:
				upx.SelectedIndex = 0;
				break;
			case ListViewStrings.UpxStandard:
				upx.SelectedIndex = 1;
				break;
			case ListViewStrings.UpxLzma:
				upx.SelectedIndex = 2;
				break;
		}

		//str null checkbox
		str = new CheckBox();
		str.Parent = this;
		str.Text = "Null string table";
		str.Location = new Point(150 ,70);
		if (nullStrings == ListViewStrings.Yes)
			str.Checked = true;
		str.Visible = false;
	}

	private void media_Click(object sender, System.EventArgs e)
	{
		if (media.Checked == true)
		{
			//add media prefix
			if (txtBox.Text.StartsWith("media\\") == false)
				txtBox.Text = "media\\" + txtBox.Text;
		}
		else
		{
			//remove media prefix
			if (txtBox.Text.StartsWith("media\\") == true)
				txtBox.Text = txtBox.Text.Substring(6, txtBox.Text.Length - 6);
		}
	}
}