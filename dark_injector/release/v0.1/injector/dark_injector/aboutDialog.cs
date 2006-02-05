//
// about dialog
//

using System;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Drawing;

class aboutDialog: Form
{
	string Version = "0.1";
	string Date = "19 Nov 2005";
	string Ident = "dark_injector";
	int Build = 0;
	public aboutDialog()
	{
		Text = "About";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(200,195);
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

		//Ok button
		Button	btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(50, 135);
		btn.Width += 20;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;
		CancelButton = btn;

		//updates button
		Button btnUpdate = new Button();
		btnUpdate.Parent = this;
		btnUpdate.Width += 40;
		btnUpdate.Text = "Check for updates";
		btnUpdate.Location = new Point(40, 100);
		btnUpdate.Click += new EventHandler(btnUpdate_Click);
	}

	private void btnUpdate_Click(object sender, EventArgs e)
	{
		//update button
		StringBuilder sb = new StringBuilder();
		int result;
		string url = "http://winch.pinkbile.com/update/" + Ident + ".php";
		try
		{
			HttpWebRequest r = (HttpWebRequest) WebRequest.Create(url);
			HttpWebResponse wr = (HttpWebResponse) r.GetResponse();
			byte[] buffer = new byte[10];
			while (wr.GetResponseStream().Read(buffer, 0, buffer.Length) > 0)
			{
				sb.Append(Encoding.UTF8.GetString(buffer));
			}
		}
		catch (Exception ex)
		{
			string msg = ex.Message + "\n\nUpdate check failed";
			MessageBox.Show(msg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
		result = int.Parse(sb.ToString());
		if (result > Build)
		{
			//updates available
			DialogResult dr = MessageBox.Show("There is a newer version available for download.\n\n Go to download page?", "Update",			MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dr == DialogResult.Yes)
			{
				try
				{
					System.Diagnostics.Process.Start("http://winch.pinkbile.com/" + Ident + ".php?update=1");
				}
				catch
				{
					// above causes file not found exception but still works
				}
			}
		}
		else
		{
			//no updates
			MessageBox.Show("No updates available", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
			// above causes file not found exception but still works
		}
	}

	private void email_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		System.Diagnostics.Process.Start("mailto:dbp@pinkbile.com");
	}
}