//
// © the_winch 2005
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

class main: Window
{
	public Button build;
	public Button save;
	public Button auto;

	public static void Main()
	{
		Application.Run(new main());
	}
	public main()
	{

		//save button
		save = new Button();
		save.Parent = this;
		save.Text = "&Save";
		save.Location = new Point(10,505);
		save.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		save.Click += new EventHandler(save_Click);

		//load button
		Button load = new Button();
		load.Text = "&Load";
		load.Parent = this;
		load.Location = new Point(100,505);
		load.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		load.Click += new EventHandler(load_Click);

		//build button
		build = new Button();
		build.Parent = this;
		build.Text = "&Build";
		build.Location = new Point(190,505);
		build.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		build.Click += new EventHandler(build_Click);

		//auto button
		auto = new Button();
		auto.Parent = this;
		auto.Text = "&Analyse";
		auto.Location = new Point(280, 505);
		auto.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		auto.Click += new EventHandler(auto_Click);

		//about button
		Button about = new Button();
		about.Parent = this;
		about.Text = "&About";
		about.Location = new Point(370,505);
		about.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		about.Click += new EventHandler(about_Click);

		//exit button
		Button exit = new Button();
		exit.Parent = this;
		exit.Text = "E&xit";
		exit.Location = new Point(460,505);
		exit.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		exit.Click += new EventHandler(exit_Click);
	}

	private void save_Click(object sender, EventArgs e)
	{
		//save
		SaveFileDialog dlg = new SaveFileDialog();
		dlg.Title = "Save";
		dlg.Filter = "Builder settings file (*.bsf)|*.bsf|All Files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			try
			{
				FileStream fs = new FileStream(dlg.FileName,FileMode.Create);
				BinaryWriter bw = new BinaryWriter(fs);
				bw.Write(name.Text);
				bw.Write(filename.Text);
				bw.Write(checksum.Text);
				bw.Write(info.Text);
			
				//files
				foreach (ListViewFileItem items in files.Items)
				{
					bw.Write(items.Text);
					bw.Write(items.SubItems[1].Text);
					bw.Write(items.SubItems[2].Text);
					bw.Write(items.Offset);
					bw.Write(items.Size);
				}

				bw.Close();
				fs.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}
		dlg.Dispose();
	}

	private void load_Click(object sender, EventArgs e)
	{
		//load
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Load";
		dlg.Filter = "Builder settings files (*.bsf)|*.bsf|All Files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			FileStream fs = null;
			BinaryReader br = null;
			//clear items
			name.Text = "";
			filename.Text = "";
			checksum.Text = "";
			info.Text = "";
			files.Items.Clear();
			try
			{
				fs = new FileStream(dlg.FileName,FileMode.Open);
				br = new BinaryReader(fs);
				name.Text = br.ReadString();
				filename.Text = br.ReadString();
				checksum.Text = br.ReadString();
				info.Text = br.ReadString();
		
				//files
				ListViewFileItem itm;
				while (fs.Position != fs.Length)
				{
					itm = new ListViewFileItem();
					itm.Text = br.ReadString();
					itm.SubItems.Add(br.ReadString());
					itm.SubItems.Add(br.ReadString());
					itm.Offset = br.ReadInt32();
					itm.Size = br.ReadInt32();
					files.Items.Add(itm);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			finally
			{
				br.Close();
				fs.Close();
			}
			//enable build and save buttons
			build.Enabled = true;
			save.Enabled = true;
		}
		dlg.Dispose();
	}

	private void build_Click(object sender, EventArgs e)
	{
		//build
		//check for valid data
		if (name.Text == "")
		{
			MessageBox.Show("No window title","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			return;
		}
		if (filename.Text == "")
		{
			MessageBox.Show("No filename","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			return;
		}
		if (info.Text == "")
		{
			MessageBox.Show("No info text","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			return;
		}
		//check checksum is valid
		if (checksum.Text.Length != 32 && checksum.Text.Length != 0)
		{
			MessageBox.Show("Invalid Checksum value","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			return;
		}
		if (files.Items.Count == 0)
		{
			MessageBox.Show("No files","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			return;
		}
		SaveFileDialog dlg = new SaveFileDialog();
		dlg.Title = "Save Patch";
		dlg.Filter = "Exe Files (*.exe)|*.exe|All Files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			try
			{
				//copy install.exe to new file
				File.Copy(Application.StartupPath+"\\patcher.dat",dlg.FileName, true);
				FileStream fs = new FileStream(dlg.FileName,FileMode.Append);
				BinaryWriter bw = new BinaryWriter(fs);

				//name
				write_text(bw,name.Text);
				//filename
				write_text(bw,filename.Text);
				//checksum
				if (checksum.Text == "")
				{
					bw.Write((byte) 0);
				}
				else
				{
					bw.Write((byte) 1);
					bw.Write(Encoding.ASCII.GetBytes(checksum.Text));
				}
				//info
				write_text(bw,info.Text);
				
				//files
				bw.Write((byte)files.Items.Count);
				for (int i=0; i<files.Items.Count; i++)
				{
					//action "Add/Replace" "Remove"
					if (files.Items[i].SubItems[1].Text == "Add/Replace")
						bw.Write((byte)0);
					if (files.Items[i].SubItems[1].Text == "Remove")
						bw.Write((byte)1);
					//filename
					write_text(bw,files.Items[i].Text);
					//filedata if required
					if (files.Items[i].SubItems[1].Text == "Add/Replace")
					{
						ListViewFileItem lvi = (ListViewFileItem) files.Items[i];
						FileStream fsin = new FileStream(files.Items[i].SubItems[2].Text,FileMode.Open);
						BinaryReader brin = new BinaryReader(fsin);
						if (lvi.Size == -1)
						{
							//normal file
							bw.Write((int)fsin.Length);
							bw.Write(brin.ReadBytes((int)fsin.Length));
						}
						else
						{
							//file with offset and size
							fsin.Seek(lvi.Offset, SeekOrigin.Begin);
							bw.Write(lvi.Size);
							bw.Write(brin.ReadBytes(lvi.Size));
						}
						brin.Close();
						fsin.Close();
					}
				}

				bw.Close();
				fs.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}
		dlg.Dispose();
	}

	private void auto_Click(object sender, EventArgs e)
	{
		//auto build patch
        analyseDialog ad = new analyseDialog();
		if (ad.ShowDialog() == DialogResult.OK)
		{
			foreach (ListViewFileItem lvi in ad.newExe.Items)
			{
				if (lvi.SubItems[1].Text == "Yes")
				{
					//changed item so add to files
					ListViewFileItem item = new ListViewFileItem();
					item.Text = lvi.Text;
					item.SubItems.Add("Add/Replace");
					item.SubItems.Add(ad.newExeName);
					item.Size = lvi.Size;
					item.Offset = lvi.Offset;
					files.Items.Add(item);
				}
			}
		}
	}

	private void write_text(BinaryWriter bw, String str)
	{
		bw.Write(str.Length);
		bw.Write(Encoding.ASCII.GetBytes(str));
	}

	private void about_Click(object sender, EventArgs e)
	{
		aboutDialog dlg = new aboutDialog();
		dlg.ShowDialog();
	}

	private void exit_Click(object sender, EventArgs e)
	{
		//exit
		Close();
	}
}