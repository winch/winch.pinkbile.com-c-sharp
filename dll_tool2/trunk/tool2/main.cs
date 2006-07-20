/*
dll_tool2
Copyright (C) 2006 the_winch

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
// http://winch.pinkbile.com
// thewinch@gmail.com
//

using System;
using System.Windows.Forms;

class window : Form
{
	//tools Tools = new tools();
	dll netDll = null;

	public static void Main()
	{
		Application.Run(new window());
	}
	public window()
	{
		this.Load += new EventHandler(window_Load);
	}

	private void window_Load(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Load dll";
		ofd.Filter = "dlls|*.dll";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			netDll = new dll(ofd.FileName);
			netDll.Load();
			netDll.Save(ofd.FileName + ".new");
		}
	}
}