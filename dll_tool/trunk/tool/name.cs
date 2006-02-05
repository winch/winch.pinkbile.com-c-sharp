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
//Method name or stringtable item input dialog
//

using System.Windows.Forms;
using System.Drawing;

class nameDialog: Form
{
	TextBox nameBox;
	Label methodLabel;
	ComboBox callConv;

	public string method
	{
		set { methodLabel.Text = value; }
	}

	public string name
	{
		get { return nameBox.Text; }
		set
		{
			nameBox.Text = value;
			nameBox.SelectionStart = value.Length;
			nameBox.SelectionLength = 0;
		}
	}

	public string CallingConvention
	{
		get
		{
			if (callConv.SelectedIndex == 0)
				return "Cdecl";
			else if (callConv.SelectedIndex == 1)
				return "StdCall";
			else
				return "";
		}
		set
		{
			if (value == "Cdecl")
				callConv.SelectedIndex = 0;
			else if (value == "StdCall")
				callConv.SelectedIndex = 1;
		}
	}

	public nameDialog(string title, bool isExport)
	{
		Text = title;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(300,130);
		StartPosition = FormStartPosition.CenterParent;

		//method label
		methodLabel = new Label();
		methodLabel.Parent = this;
		methodLabel.AutoSize = true;
		methodLabel.Location = new Point(10,10);

		//calling convention combobox
		callConv = new ComboBox();
		callConv.Parent = this;
		callConv.Width = 100;
		callConv.Location = new Point(this.Width - callConv.Width - 15, 10);
		callConv.DropDownStyle = ComboBoxStyle.DropDownList;
		callConv.Items.Add("Cdecl");
		callConv.Items.Add("StdCall");
		if (isExport == false)
			callConv.Visible = false;

		//name textbox
		nameBox = new TextBox();
		nameBox.Parent = this;
		nameBox.Location = new Point(10,40);
		nameBox.Width = 275;

		//Cancel button
		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(205,70);
		btn.Width += 5;
		btn.DialogResult = DialogResult.Cancel;
		CancelButton = btn;

		//Ok button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(120,70);
		btn.Width += 5;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;
	}
}