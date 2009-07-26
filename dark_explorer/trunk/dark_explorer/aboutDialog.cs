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
// about dialog
//

using System;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

class aboutDialog: Form
{
	string Version = "0.18";
	string Date = "26 July 2009";
	string Ident = "dark_explorer";
	int Build = 17;
	Button btnUpdate;
	ProgressBar progress;
	public aboutDialog()
	{
		Text = "About";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(200,275);
		StartPosition = FormStartPosition.CenterParent;

		Label version = new Label();
		version.Parent = this;
		version.Location = new Point(20,20);
		version.Width = this.Width-40;
		version.Height = 20;
		version.Text = "v" + Version + " " + Date;
		version.TextAlign = ContentAlignment.TopCenter;

		LinkLabel email = new LinkLabel();
		email.Parent = this;
		email.Location = new Point(20,50);
		email.Width = version.Width;
		email.Height = 20;
		email.TextAlign = ContentAlignment.TopCenter;
		email.Text = "dbp@pinkbile.com";
		email.LinkClicked += new LinkLabelLinkClickedEventHandler(email_LinkClicked);

		LinkLabel web = new LinkLabel();
		web.Parent = this;
		web.Location = new Point(20,70);
		web.Width = email.Width;
		web.Height = 20;
		web.TextAlign = ContentAlignment.TopCenter;
		web.Text = "http://winch.pinkbile.com";
		web.LinkClicked += new LinkLabelLinkClickedEventHandler(web_LinkClicked);

		//gpl text label
		version = new Label();
		version.Parent = this;
		version.Width = this.Width - 40;
		version.Height += 25;
		version.TextAlign = ContentAlignment.TopCenter;
		version.Text = "This program is free software and may be distributed according to the terms of the GNU GPL";
		version.Location = new Point(20,100);

		//progress bar
		progress = new ProgressBar();
		progress.Parent = this;
		progress.Height = 15;
		progress.Width = this.Width - 15;
		progress.Location = new Point(5, 160);

		//Ok button
		Button	btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(50, 215);
		btn.Width += 20;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;
		CancelButton = btn;

		//updates button
		btnUpdate = new Button();
		btnUpdate.Parent = this;
		btnUpdate.Width += 40;
		btnUpdate.Text = "Check for updates";
		btnUpdate.Location = new Point(40, 185);
		btnUpdate.Click += new EventHandler(btnUpdate_Click);

		System.Windows.Forms.Timer updateTick = new System.Windows.Forms.Timer();
		updateTick.Interval = 250;
		updateTick.Tick += new EventHandler(updateTick_Tick);
		updateTick.Start();
	}

	enum UpdateState
	{
		Stopped,
		Canceled,
		Running
	}
	UpdateState state = UpdateState.Stopped;

	void UpdateCheck()
	{
		StringBuilder sb = new StringBuilder();
		int result;
		string url = "http://winch.pinkbile.com/update/" + Ident + ".php";
		try
		{
			HttpWebRequest r = (HttpWebRequest) WebRequest.Create(url);
			HttpWebResponse wr = (HttpWebResponse) r.GetResponse();
			byte[] buffer = new byte[10];
			if (state == UpdateState.Canceled)
			{
				wr.Close();
				state = UpdateState.Stopped;
				return;
			}
			while (wr.GetResponseStream().Read(buffer, 0, buffer.Length) > 0)
			{
				if (state == UpdateState.Canceled)
				{
					wr.Close();
					state = UpdateState.Stopped;
					return;
				}
				sb.Append(Encoding.UTF8.GetString(buffer));
			}
		}
		catch (Exception ex)
		{
			if (state == UpdateState.Canceled)
			{
				state = UpdateState.Stopped;
				return;
			}
			string msg = ex.Message + "\n\nUpdate check failed";
			MessageBox.Show(msg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		result = int.Parse(sb.ToString());
		if (result > Build)
		{
			//updates available
			state = UpdateState.Stopped;
			DialogResult dr = MessageBox.Show("There is a newer version available for download.\n\n Go to download page?", "Update",			MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dr == DialogResult.Yes)
			{
				try
				{
					System.Diagnostics.Process.Start("http://winch.pinkbile.com/" + Ident + ".php?update=1");
				}
				catch
				{
					//above causes file not found exception but still works
				}
			}
		}
		else
		{
			//no updates
			state = UpdateState.Stopped;
			MessageBox.Show("No updates available", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}

	private void btnUpdate_Click(object sender, EventArgs e)
	{
		//update button
		if (btnUpdate.Text == "Check for updates" && state == UpdateState.Stopped)
		{
			btnUpdate.Text = "Stop";
			state = UpdateState.Running;
			Thread update = new Thread(new ThreadStart(UpdateCheck));
			update.Start();
		}
		else
		{
			btnUpdate.Text = "Check for updates";
			state = UpdateState.Canceled;
		}
	}

	private void web_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		try
		{
			System.Diagnostics.Process.Start("http://winch.pinkbile.com/");
		}
		catch
		{
			//above causes file not found exception but still works
		}
	}

	private void email_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		System.Diagnostics.Process.Start("mailto:dbp@pinkbile.com");
	}

	private void updateTick_Tick(object sender, EventArgs e)
	{
		//show animation if update check in progress
		switch (state)
		{
			case UpdateState.Running:
					  progress.Increment(10);
				if (progress.Value > 99)
					progress.Value = 0;
				break;
			case UpdateState.Stopped:
					  progress.Value = 0;
					  btnUpdate.Text = "Check for updates";
				break;
		}
	}
}