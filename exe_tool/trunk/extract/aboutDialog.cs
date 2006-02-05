//
// about dialog
//
// see readme.txt for copyright and license info

using System;
using System.Windows.Forms;
using System.Drawing;
using Win3;

class aboutDialog : Form
{
	public aboutDialog()
	{
		Text = "About";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(200,170);
		StartPosition = FormStartPosition.CenterParent;

		Label version = new Label();
		version.Parent = this;
		version.Location = new Point(20,20);
		version.Width = this.Width-40;
		version.Height = 20;
		version.Text = "v0.15 RC1 13 June 2005";
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
		Win3_Button	btn = new Win3_Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(50,110);
		btn.Width += 20;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;
		CancelButton = btn;
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