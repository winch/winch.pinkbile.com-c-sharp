//
//
//
//
// © the_winch 2005
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

class analyseDialog : Form
{
	GroupBox gbOld;
	ListView oldExe;
	string oldExeName;
	GroupBox gbNew;
	public ListView newExe;
	public string newExeName;
	public analyseDialog()
	{
		Text = "Analyse old and new exe";
		ShowInTaskbar = false;
		Size = new Size(700,500);
		StartPosition = FormStartPosition.CenterParent;

		//Ok button
		Button	btn = new Button();
		btn.Parent = this;
		btn.Text = "&Ok";
		btn.Location = new Point(this.Width - 190 ,this.Height - btn.Height - 35);
		btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		btn.DialogResult = DialogResult.OK;
		AcceptButton = btn;

		//cancel button
		btn = new Button();
		btn.Parent = this;
		btn.Text = "&Cancel";
		btn.Location = new Point(this.Width - 100, this.Height - btn.Height - 35);
		btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		CancelButton = btn;

		gbOld = new GroupBox();
		gbOld.Parent = this;
		gbOld.Text = "Old exe ()";
		gbOld.Location = new Point(5,5);
		gbOld.Size = new Size((this.Width / 2) - 20, this.Height - 70);
		gbOld.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
		//old listview
		oldExe = new ListView();
		oldExe.Parent = gbOld;
		oldExe.Location = new Point(10,15);
		oldExe.Size = new Size(gbOld.Width - 20, gbOld.Height - 55);
		oldExe.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		oldExe.View = View.Details;
		oldExe.Columns.Add("Filename", 205, HorizontalAlignment.Right);
		oldExe.Columns.Add("md5 hash", 210, HorizontalAlignment.Left);
		oldExe.Columns.Add("Attached File", 75, HorizontalAlignment.Right);
		//select exe button
		Button selectOldExe = new Button();
		selectOldExe.Parent = gbOld;
		selectOldExe.Text = "Select old exe";
		selectOldExe.Width = gbOld.Width - 20;
		selectOldExe.Location = new Point(10, gbOld.Height - 35);
		selectOldExe.Anchor =  AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
		selectOldExe.Click += new EventHandler(selectOldExe_Click);

		gbNew = new GroupBox();
		gbNew.Parent = this;
		gbNew.Text = "New exe ()";
		gbNew.Location = new Point((this.Width / 2) + 5,5);
		gbNew.Size = new Size((this.Width / 2) - 20, this.Height - 70);
		gbNew.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
		//new listview
		newExe = new ListView();
		newExe.Parent = gbNew;
		newExe.Location = new Point(10,15);
		newExe.Size = new Size(gbNew.Width - 20, gbNew.Height - 55);
		newExe.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		newExe.View = View.Details;
		newExe.Columns.Add("Filename", 205, HorizontalAlignment.Right);
		newExe.Columns.Add("Changed?", 70, HorizontalAlignment.Right);
		newExe.Columns.Add("md5 hash", 210, HorizontalAlignment.Left);
		newExe.Columns.Add("Attached File", 75, HorizontalAlignment.Right);
		//select exe button
		Button selectNewExe = new Button();
		selectNewExe.Parent = gbNew;
		selectNewExe.Text = "Select new exe";
		selectNewExe.Width = gbNew.Width - 20;
		selectNewExe.Location = new Point(10, gbNew.Height - 35);
		selectNewExe.Anchor =  AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
		selectNewExe.Click += new EventHandler(selectNewExe_Click);

		//window resize
        this.Resize += new EventHandler(analyseDialog_Resize);
	}

	private void analyse()
	{
		//show files with different md5 has as changed
		string oldSum, newSum;
		bool found;
		newExe.BeginUpdate();
		foreach(ListViewFileItem newItem in newExe.Items)
		{
			found = false;
			foreach(ListViewFileItem oldItem in oldExe.Items)
			{
				if (newItem.Text == oldItem.Text)
				{
					found = true;
					oldSum = oldItem.SubItems[1].Text;
					newSum = newItem.SubItems[2].Text;
					if (oldSum != newSum)
					{
						//files differ
						newItem.SubItems[1].Text = "Yes";
						for (int i = 0; i < newItem.SubItems.Count; i++)
						{
							newItem.SubItems[i].BackColor = Color.LightPink;
						}
					}
					else
					{
						//files same
						newItem.SubItems[1].Text = "No";
						for (int i = 0; i < newItem.SubItems.Count; i++)
						{
							newItem.SubItems[i].BackColor = Color.LightGreen;
						}
					}
				}
			}
			if (found == false)
			{
				//file not in oldexe
				newItem.SubItems[1].Text = "Yes";
				for (int i = 0; i < newItem.SubItems.Count; i++)
				{
					newItem.SubItems[i].BackColor = Color.LightPink;
				}
			}
		}
		newExe.EndUpdate();
	}

	private void analyseDialog_Resize(object sender, EventArgs e)
	{
		//window resized
		gbOld.Size = new Size((this.Width / 2) - 20, this.Height - 70);
		gbNew.Size = gbOld.Size;
		gbNew.Location = new Point((this.Width / 2) + 5,5);
		oldExe.EndUpdate();
	}

	private void selectOldExe_Click(object sender, EventArgs e)
	{
		//select old exe
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Select old exe";
		ofd.Filter = "Dbpro exe (*.exe)|*.exe|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			Cursor.Current = Cursors.WaitCursor;
			oldExeName = ofd.FileName;
			gbOld.Text = "Old exe (" + Path.GetFileName(ofd.FileName) + ")";
			oldExe.BeginUpdate();
			oldExe.Items.Clear();
			proExe.LoadExe(oldExe, false, ofd.FileName);
			foreach (ListViewItem lvi in oldExe.Items)
			{
				lvi.UseItemStyleForSubItems = false;
			}
			oldExe.EndUpdate();
			//analyse if required
			if (newExeName != null)
				analyse();
			Cursor.Current = Cursors.Default;
		}
		ofd.Dispose();
	}

	private void selectNewExe_Click(object sender, EventArgs e)
	{
		//select new exe
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Title = "Select new exe";
		ofd.Filter = "Dbpro exe (*.exe)|*.exe|All Files (*.*)|*.*";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			Cursor.Current = Cursors.WaitCursor;
			newExeName = ofd.FileName;
			gbNew.Text = "New exe (" + Path.GetFileName(ofd.FileName) + ")";
			newExe.BeginUpdate();
			newExe.Items.Clear();
			proExe.LoadExe(newExe, true, ofd.FileName);
			foreach (ListViewItem lvi in newExe.Items)
			{
				lvi.UseItemStyleForSubItems = false;
			}
			newExe.EndUpdate();
			//analyse if required
			if (oldExeName != null)
				analyse();
			Cursor.Current = Cursors.Default;
		}
		ofd.Dispose();
	}
}