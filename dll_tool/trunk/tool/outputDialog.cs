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
//Output dialog for displaying output of ilasm and ildasm
//

using System.Windows.Forms;
using System.Drawing;

class outputDialog: Form
{
	TextBox textBox;

	public string outputText
	{
		set
		{
			textBox.Text = value;
			textBox.SelectionLength = 0;
			textBox.SelectionStart = 0;
		}
	}

	public outputDialog()
	{
		Text = "Output";
		this.Size = new Size(640,480);
		ShowInTaskbar = false;

		textBox = new TextBox();
		textBox.Parent = this;
		textBox.Multiline = true;
		textBox.WordWrap = false;
		textBox.Dock = DockStyle.Fill;

	}
}
			