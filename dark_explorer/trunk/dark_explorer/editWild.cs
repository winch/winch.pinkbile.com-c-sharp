/*
dark_explorer
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
//editItem dialog, lets user edit a ListViewFileItem
//

using System.Windows.Forms;
using System.Drawing;
using System.Collections;

class editWild : Form
{
	TextBox name;

	CheckBox attached;
	CheckBox upx;
	CheckBox nullStr;

	public editWild()
	{
		Text = "Edit multiple items";
		Size = new Size(330, 160);
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;
		FormBorderStyle = FormBorderStyle.FixedDialog;

		int y = 5, x = 10;
		GroupBox gb = new GroupBox();
		gb.Parent = this;
		gb.Text = "Name";
		gb.Size = new Size(this.Width - 20, 60);
		gb.Location = new Point(5, y);
		gb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		y += gb.Height + 10;

		name = new TextBox();
		name.Parent = gb;
		name.Location = new Point(10, 15);
		name.Width = gb.Width - 20;
		name.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		name.Text = "%n";

		Label lab = new Label();
		lab.Parent = gb;
		lab.AutoSize = true;
		lab.Location = new Point(5, 40);
		lab.Text = "%n = name";

		//attached file checkbox
		attached = new CheckBox();
		attached.Parent = this;
		attached.Text = "Attached File";
		attached.CheckState = CheckState.Indeterminate;
		attached.Width -= 15;
		attached.Location = new Point(x, y);
		x += attached.Width;

		//upx checkbox
		upx = new CheckBox();
		upx.Parent = this;
		upx.Text = "Compress with Upx";
		upx.CheckState = CheckState.Indeterminate;
		upx.Width += 20;
		upx.Location = new Point(x, y);
		x += upx.Width;

		//null string table checkbox
		nullStr = new CheckBox();
		nullStr.Parent = this;
		nullStr.Text = "Null string table";
		nullStr.CheckState = CheckState.Indeterminate;
		nullStr.Location = new Point(x, y);

		//Cancel button
		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(this.Width - 90, this.Height - btn.Height - 35);
		btn.DialogResult = DialogResult.Cancel;
		btn.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
		CancelButton = btn;

		//Ok button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(this.Width - 180, this.Height - btn.Height - 35);
		btn.DialogResult = DialogResult.OK;
		btn.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
		AcceptButton = btn;
	}

	private void setCheckBoxValue(ListViewFileItem lvi, int subItem, CheckState state)
	{
		//set the text for a subitem determined by a checkbox
		if (state != CheckState.Indeterminate)
		{
			if (state == CheckState.Checked)
			{
				lvi.SubItems[subItem].Text = ListViewStrings.Yes;
			}
			else
			{
				lvi.SubItems[subItem].Text = ListViewStrings.No;
			}
		}
	}

	public void EditItems(ICollection items)
	{
		//edit item
		foreach (ListViewFileItem lvi in items)
		{
			setCheckBoxValue(lvi, (int)ListViewOrder.FileType, attached.CheckState);
			setCheckBoxValue(lvi, (int)ListViewOrder.Upx, upx.CheckState);
			setCheckBoxValue(lvi, (int)ListViewOrder.NullString, nullStr.CheckState);
			lvi.SubItems[(int)ListViewOrder.Name].Text = name.Text.Replace("%n", lvi.SubItems[(int)ListViewOrder.Name].Text);
		}
	}
}