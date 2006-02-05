//
// edit item name form
//
// see readme.txt for copyright and license info

using System.Windows.Forms;
using System.Drawing;
using Win3;

class editItemDialog : Form
{
	public TextBox txtBox;
	
	public string name
	{
		get
		{
			return txtBox.Text;
		}
	}

	public editItemDialog(string text)
	{
		Text = "Edit item name";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(500,95);
		StartPosition = FormStartPosition.CenterParent;

		//text box
		txtBox = new TextBox();
		txtBox.Parent = this;
		txtBox.Location = new Point(10,10);
		txtBox.Size = new Size(475,txtBox.Height);
		txtBox.Text = text;

		//Cancel button
		Win3_Button btn = new Win3_Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(390,40);
		btn.Width += 20;
		btn.DialogResult = DialogResult.Cancel;
		CancelButton = btn;

		//Ok button
		btn = new Win3_Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(280,40);
		btn.Width += 20;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;

		//media prefix button
		Win3_Button media = new Win3_Button();
		media.Parent = this;
		media.Text = "&Add \"\\media\\\" prefix";
		media.Location = new Point(10,40);
		media.Width += 60;
		media.Click += new System.EventHandler(media_Click);
	}

	private void media_Click(object sender, System.EventArgs e)
	{
		txtBox.Text = "media\\" + txtBox.Text;
	}
}