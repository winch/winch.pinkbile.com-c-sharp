//
//Gets a filter string and prefix string for insert with wildcard
//
// see readme.txt for copyright and license info

using System.Windows.Forms;
using System.Drawing;
using Win3;

class filterDialog : Form
{
	TextBox txtFilter;
	ComboBox filters;
	TextBox txtPrefix;
	CheckBox chkMedia;
	string exepath;
	string addpath;
	
	public string filter
	{
		get
		{
			return txtFilter.Text;
		}
	}
	public string prefix
	{
		get
		{
			return txtPrefix.Text;
		}
	}
	public string Exepath
	{
		set
		{
			exepath = value;
		}
	}
	public string Addpath
	{
		set
		{
			addpath = value;
		}
	}

	public filterDialog()
	{

		Text = "Set Filter and Prefix";
		FormBorderStyle = FormBorderStyle.FixedDialog;
		ControlBox    = false;
		MaximizeBox   = false;
		MinimizeBox   = false;
		ShowInTaskbar = false;
		Size = new Size(380,195);
		StartPosition = FormStartPosition.CenterParent;

		//filter
		GroupBox box = new GroupBox();
		box.Parent = this;
		box.Text = "Filter";
		box.Location = new Point(10,5);
		box.Size = new Size(355,45);

		//filter text box
		txtFilter = new TextBox();
		txtFilter.Parent = box;
		txtFilter.Location = new Point(10,15);
		txtFilter.Size = new Size(170,txtFilter.Height);
		txtFilter.Text = "*.*";

		filters = new ComboBox();
		filters.Parent = box;
		filters.Location = new Point(190,15);
		filters.Width = 155;
		filters.DropDownStyle = ComboBoxStyle.DropDownList;
		filters.Items.Add("*.*");
		filters.Items.Add("*.png");
		filters.Items.Add("*.jpg");
		filters.Items.Add("*.bmp");
		filters.Items.Add("*.tga");
		filters.Items.Add("*.dds");
		filters.Items.Add("*.x");
		filters.Items.Add("*.dbo");
		filters.Items.Add("*.avi");
		filters.Items.Add("*.wav");
		filters.Items.Add("*.mpg");
		filters.Items.Add("*.dll");
		filters.SelectedIndex = 0;
		filters.SelectedIndexChanged += new System.EventHandler(filters_SelectedIndexChanged);

		//prefix
		box = new GroupBox();
		box.Parent = this;
		box.Text = "Name Prefix";
		box.Location = new Point(10,60);
		box.Size = new Size(355,70);

		//prefix text box
		txtPrefix = new TextBox();
		txtPrefix.Parent = box;
		txtPrefix.Location = new Point(10,15);
		txtPrefix.Width = 335;
		txtPrefix.Text = "media\\";

		//media checkbox
		chkMedia = new CheckBox();
		chkMedia.Parent = box;
		chkMedia.Text = "\"media\\\" prefix";
		chkMedia.Location = new Point(10,40);
		chkMedia.Checked = true;
		chkMedia.CheckedChanged += new System.EventHandler(chkMedia_CheckedChanged);

		//path button
		Win3_Button btnPath = new Win3_Button();
		btnPath.Parent = box;
		btnPath.Text = "Guess prefix";
		btnPath.Width = 100;
		btnPath.Location = new Point(245,40);
		btnPath.Click += new System.EventHandler(btnPath_Click);

		//Cancel button
		Win3_Button btn = new Win3_Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(270,140);
		btn.Width += 20;
		btn.DialogResult = DialogResult.Cancel;
		CancelButton = btn;

		//Ok button
		btn = new Win3_Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(160,140);
		btn.Width += 20;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;
	}

	private void filters_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		txtFilter.Text = filters.SelectedItem.ToString();
	}

	private void chkMedia_CheckedChanged(object sender, System.EventArgs e)
	{
		//media checkbox changed
		if (chkMedia.Checked == true)
		{
			//add "media\"
			if (!txtPrefix.Text.StartsWith("media\\"))
				txtPrefix.Text = "media\\" + txtPrefix.Text;
		}
		else
		{
			//remove "media\"
			if (txtPrefix.Text.StartsWith("media\\"))
				txtPrefix.Text = txtPrefix.Text.Remove(0,"media\\".Length);
		}
	}

	private void btnPath_Click(object sender, System.EventArgs e)
	{
		//guess path button
		string diff;
		if (exepath == "")
			return;
		//if exepath is shorter than addpath then we are not in subdir
		if (addpath.Length < exepath.Length)
			return;
		if (addpath == exepath)
			return;
		//if addpath does not start with exe path then we are not in subdir
		if (addpath.StartsWith(exepath))
		{
			diff = addpath.Substring(exepath.Length+1,addpath.Length-exepath.Length-1) + "\\";
		}
		else
		{
			return;
		}
		if (chkMedia.Checked)
			txtPrefix.Text = "media\\" + diff;
		else
			txtPrefix.Text = diff;
	}
}