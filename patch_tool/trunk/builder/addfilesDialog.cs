//
// � the_winch 2005
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
using System.Windows.Forms;
using System.Drawing;

class addFilesDialog: Form
{
	public TextBox filename;
	public TextBox pathname;
	public ComboBox comboaction;
	
	public string name
	{
		get { return filename.Text; }
	}
	public string location
	{
		get { return pathname.Text; }
	}
	public string action
	{
		get
		{
			return comboaction.Items[comboaction.SelectedIndex].ToString();
		}
	}

	public addFilesDialog(string text,string path,int selectedAction)
	{
		Text = "Edit item name";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(500,125);
		StartPosition = FormStartPosition.CenterParent;

		//filename
		Label label = new Label();
		label.Parent = this;
		label.AutoSize = true;
		label.Text = "Name";
		label.Location = new Point(10,10);
		filename = new TextBox();
		filename.Parent = this;
		filename.Location = new Point(label.Left+label.Width+5,10);
		filename.Size = new Size(340,filename.Height);
		filename.Text = text;

		//pathname
		label = new Label();
		label.Parent = this;
		label.AutoSize = true;
		label.Text = "Location";
		label.Location = new Point(10,40);
		pathname = new TextBox();
		pathname.Parent = this;
		pathname.Text = path;
		pathname.Location = new Point(label.Width+label.Left+5,40);
		pathname.Size = new Size(385,pathname.Height);

		//browse for file button
		Button browse = new Button();
		browse.Parent = this;
		browse.Text = "&...";
		browse.Size = new Size(30,browse.Height);
		browse.Location = new Point(455,40);
		browse.Click += new System.EventHandler(browse_Click);

		//action combobox
		comboaction = new ComboBox();
		comboaction.Parent = this;
		comboaction.Size = new Size(90,comboaction.Height);
		comboaction.Location = new Point(400,10);
		comboaction.DropDownStyle = ComboBoxStyle.DropDownList;
		comboaction.Items.Add(ActionStrings.AddReplace);
		comboaction.Items.Add(ActionStrings.Remove);
		//comboaction.Items.Add("Patch");
		comboaction.SelectedIndex = selectedAction;

		//Cancel button
		Button btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(390,70);
		btn.Width += 20;
		btn.DialogResult = DialogResult.Cancel;
		CancelButton = btn;

		//Ok button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(280,70);
		btn.Width += 20;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;

		//get name from location button
		Button nameloc = new Button();
		nameloc.Parent = this;
		nameloc.Text = "&get name from location";
		nameloc.Location = new Point(5,70);
		nameloc.Width += 70;
		nameloc.Click += new System.EventHandler(nameloc_Click);

	}

	private void browse_Click(object sender, System.EventArgs e)
	{
		//browse for file
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Select file";
		dlg.Filter = "All files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			pathname.Text = dlg.FileName;
		}
		dlg.Dispose();
	}

	private void nameloc_Click(object sender, System.EventArgs e)
	{
		//get name from location
		filename.Text = System.IO.Path.GetFileName(pathname.Text);
	}
}