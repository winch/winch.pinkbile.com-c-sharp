using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

class main: Form
{

	ListView lv;
	TextBox pre;
	TextBox begin;

	public static void Main()
	{
		Application.Run(new main());
	}

	public main()
	{
		Width = 500;
		Height = 500;

		Button dirBtn = new Button();
		dirBtn.Parent = this;
		dirBtn.Text = "Select Dir";
		dirBtn.Location = new Point(10,10);
		dirBtn.Click += new EventHandler(dirBtn_Click);

		Label lab = new Label();
		lab.Parent = this;
		lab.Text = "prefix:";
		lab.AutoSize = true;
		lab.Location = new Point(100,10);

		pre = new TextBox();
		pre.Parent = this;
		pre.Location = new Point(135,10);
		pre.Width += 20;
		pre.TextChanged += new EventHandler(pre_TextChanged);

		lab = new Label();
		lab.Parent = this;
		lab.Text = "start #";
		lab.AutoSize = true;
		lab.Location = new Point(260,10);

		begin = new TextBox();
		begin.Parent = this;
		begin.Text = "0";
		begin.Location = new Point(300,10);
		begin.Width = 25;
		begin.TextChanged += new EventHandler(begin_TextChanged);

		Button goBtn = new Button();
		goBtn.Parent = this;
		goBtn.Text = "Go";
		goBtn.Location = new Point(380,10);
		goBtn.Click += new EventHandler(goBtn_Click);

		lv = new ListView();
		lv.Parent = this;
		lv.View = View.Details;
		lv.Size = new Size(480,430);
		lv.Location = new Point(10,40);
		lv.Sorting = SortOrder.Ascending;
		lv.Columns.Add("before", 235, HorizontalAlignment.Left);
		lv.Columns.Add("after", 235, HorizontalAlignment.Left);
		lv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
	}

	private void dirBtn_Click(object sender, EventArgs e)
	{
		//select dir
		FolderBrowserDialog br = new FolderBrowserDialog();
		if (br.ShowDialog() == DialogResult.OK)
		{
			//add files to listview
			lv.Items.Clear();
			lv.BeginUpdate();
			pre.Text = "";
			begin.Text = "0";
			string[] files = Directory.GetFiles(br.SelectedPath,"*.jpg");
			foreach (string str in files)
			{
				ListViewItem lvi = new ListViewItem();
				lvi.Text = str;
				lvi.SubItems.Add("");
				lv.Items.Add(lvi);
			}
			lv.EndUpdate();
		}
	}

	private void pre_TextChanged(object sender, EventArgs e)
	{
		//recalculate after text.
		if (lv.Items.Count > 0)
		{
			string before,after;
			foreach (ListViewItem lvi in lv.Items)
			{
				before = lvi.Text;
				after = Path.GetDirectoryName(before) + "\\" + pre.Text + string.Format("{0:D3}", (int.Parse(begin.Text) + lvi.Index)) + Path.GetExtension(before);
				lvi.SubItems[1].Text = after;
			}
		}
	}

	private void goBtn_Click(object sender, EventArgs e)
	{
		//move files.
		foreach(ListViewItem lvi in lv.Items)
		{
			File.Move(lvi.Text, lvi.SubItems[1].Text);
		}
		lv.Items.Clear();
	}

	private void begin_TextChanged(object sender, EventArgs e)
	{
		//start num text changed
		try
		{
			int i = int.Parse(begin.Text);
			pre_TextChanged(this, new EventArgs());
		}
		catch
		{
			//
		}

	}
}