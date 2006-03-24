/*
dark_explorer
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
//debugLog, logs debug messages
//
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

class debugLog
{
	static debugWindow window = null;
	static string section = null;

	public static void Log(string message)
	{
		//add string to debug log
		if (window == null)
			window = new debugWindow();
		window.log.Items.Add(message);
		if (window.log.Items.Count > 1000)
		{
			window.log.Items.Clear();
			window.log.Items.Add("Log auto cleared");
		}
	}

	public static void StartSection(string sectionName)
	{
		//show simple
		section = sectionName;
		Log("===== Start "+section+" =====");
		window.log.BeginUpdate();
	}

	public static void StopSection()
	{
		if (section != null)
		{
			Log("===== End "+section+" =====");
		}
		window.log.EndUpdate();
		section = null;
	}

	public static void Toggle()
	{
		//show/hide dialog
		if (window == null)
			window = new debugWindow();
		if (window.Visible == true)
            window.Hide();
		else
			window.Show();
	}
}

class debugWindow : Form
{
	public ListBox log;
	public debugWindow()
	{
		Text = "debug log";
		Size = new Size(300, 295);
		ShowInTaskbar = false;

		log = new ListBox();
		log.Parent = this;
		log.Dock = DockStyle.Fill;
		log.Font = new Font(FontFamily.GenericMonospace, log.Font.Size);

		//context menu
		EventHandler mClear  = new EventHandler(mClearOnClick);
		MenuItem[] ami =  { new MenuItem("&Clear", mClear) };
		log.ContextMenu = new ContextMenu(ami);

		this.Closing += new System.ComponentModel.CancelEventHandler(debugWindow_Closing);
	}

	private void mClearOnClick(object sender, EventArgs e)
	{
		//clear log
		log.Items.Clear();
	}

	private void debugWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		//hide window
		this.Hide();
		e.Cancel = true;
	}
}