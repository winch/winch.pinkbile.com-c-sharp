//
//
//

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

class main : window
{

	string Loaded_ExeName = "";
	string Plugins;
	string Plugins_user;
	string Effects;

	public static void Main()
	{
		Application.Run(new main());
	}

	public main()
	{

		this.Load += new EventHandler(main_Load);
		Button load = new Button();
		load.Parent = this;
		load.Text = "&Load";
		load.Location = new Point(15 ,510);
		load.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		load.Click += new EventHandler(load_Click);

		Button build = new Button();
		build.Parent = this;
		build.Text = "&Build";
		build.Location = new Point(load.Left+load.Width+15,510);
		build.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		build.Click += new EventHandler(build_Click);

		Button about = new Button();
		about.Parent = this;
		about.Text = "&About";
		about.Location = new Point(build.Left+build.Width+15,510);
		about.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		about.Click +=new EventHandler(about_Click);

		Button exit = new Button();
		exit.Parent = this;
		exit.Text = "E&xit";
		exit.Location = new Point(about.Left+about.Width+15,510);
		exit.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
		exit.Click += new EventHandler(exit_Click);
	}

	private void main_Load(object sender, EventArgs e)
	{
		//form load
		//try and find dbpro install location from the registry
		RegistryKey reg = null;
		string str = "";
		try
		{
			reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Dark Basic\\Dark Basic Pro");
			if (reg == null)
			{
				string msg;
				msg  = "DarkBASIC pro location not found\n\n";
				msg += "You must have DarkBasic Pro installed for this program\n";
				msg += "and the exes it produces to work correctly";
				MessageBox.Show(msg, "Error!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				str = reg.GetValue("INSTALL-PATH").ToString();
				str += Path.DirectorySeparatorChar + "compiler" + Path.DirectorySeparatorChar;
				Plugins = str + "plugins" + Path.DirectorySeparatorChar;
				Plugins_user = str + "plugins-user" + Path.DirectorySeparatorChar;
				Effects = str + "effects" + Path.DirectorySeparatorChar;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		finally
		{
			if (reg != null)
				reg.Close();
		}
	}

	private void load_Click(object sender, EventArgs e)
	{
		//load
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Title = "Selet Dbpro exe";
		dlg.Filter = "Exe files (*.exe)|*.exe|All Files(*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			Intern.Enabled = true;
			Intern.Items.Clear();
			Extern.Enabled = true;
			Extern.Items.Clear();
			if (exeType.SelectedIndex == 0)
			{
				//new type
				Targetexe.Enabled = false;
			}
			else
			{
				//old type
				Targetexe.Enabled = true;
			}
			//load exe
			proExe.LoadExe(Extern, dlg.FileName);
			//move required items into Intern
			foreach (ListViewFileItem lvi in Extern.Items)
			{
				if (lvi.Text.StartsWith("<") || lvi.Text == "_virtual.dat")
				{
					lvi.Remove();
					Intern.Items.Add(lvi);
				}
			}
			Loaded_ExeName = dlg.FileName;
			if (Targetexe.Text == "")
				Targetexe.Text = "mini_"+Path.GetFileName(dlg.FileName);
			Intern.EndUpdate();
			Extern.EndUpdate();
		}
		dlg.Dispose();
	}

	private void build_Click(object sender, EventArgs e)
	{
		//build new exe
		if (Loaded_ExeName == "")
		{
			MessageBox.Show("No exe loaded!","Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			return;
		}
		if (exeType.SelectedIndex == 1)
		{
			//old type
			if (Targetexe.Text == "")
			{
				MessageBox.Show("No exe name!","Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}
		}
		//Check files in Extern exist in Plugins and Plugins_user and effects
		bool found; //has the file been found
		foreach (ListViewItem itm in Extern.Items)
		{
			found = false;
			if (File.Exists(Plugins + itm.Text))
				found = true;
			if (File.Exists(Plugins_user + itm.Text))
				found = true;
			if (File.Exists(Effects + itm.Text))
				found = true;
			if (found == false)
			{
				//file not found
				MessageBox.Show("File :"+itm.Text+"\n\n"+"Not found in \n"+Plugins+"\nor\n"+Plugins_user+"\nor\n"+Effects,"Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}
		}

		SaveFileDialog dlg = new SaveFileDialog();
		dlg.Filter = "Exe Files (*.exe)|*.exe|All Files (*.*)|*.*";
		if (dlg.ShowDialog() == DialogResult.OK)
		{
			FileStream fsIn = null;
			BinaryReader brIn = null;
			FileStream fsOut = null;
			BinaryWriter bwOut = null;
			FileStream fsExt = null;
			BinaryReader brExt = null;
			try
			{
				fsIn = new FileStream(Loaded_ExeName, FileMode.Open);
				brIn = new BinaryReader(fsIn);
				File.Copy(Application.StartupPath+"\\expander.dat",dlg.FileName, true);
				fsOut = new FileStream(dlg.FileName, FileMode.Append);
				bwOut = new BinaryWriter(fsOut);

				//write exe name
				bwOut.Write(Targetexe.Text.Length);
				bwOut.Write(Encoding.ASCII.GetBytes(Targetexe.Text));

				//write num of internal files
				bwOut.Write((byte) Intern.Items.Count);

				//write each internal file to patch
				foreach (ListViewFileItem itm in Intern.Items)
				{
					//name
					bwOut.Write(itm.Text.Length);
					bwOut.Write(Encoding.ASCII.GetBytes(itm.Text));
					//data
					//check if file is in <exe> or not
					if (itm.SubItems[1].Text == "<exe>")
					{
						//in <exe>
						fsIn.Seek(itm.Offset, SeekOrigin.Begin);
						//size
						bwOut.Write(itm.Size);
						//data
						bwOut.Write(brIn.ReadBytes(itm.Size));
					}
					else
					{
						//external file
						//filedata
						fsExt = new FileStream(itm.SubItems[1].Text,FileMode.Open);
						brExt = new BinaryReader(fsExt);
						//size
						bwOut.Write((int) fsExt.Length);
						//data
						bwOut.Write(brExt.ReadBytes((int) fsExt.Length));
						brExt.Close();
						fsExt.Close();
					}
				}

				//use md5 checksums?
				if (CheckSum.Checked == true)
					bwOut.Write((byte) 1);
				else
					bwOut.Write((byte) 0);

				//write num of External files
				bwOut.Write((byte) Extern.Items.Count);
				string checksum;

				foreach (ListViewFileItem itm in Extern.Items)
				{
					//write name
					bwOut.Write(itm.Text.Length);
					bwOut.Write(Encoding.ASCII.GetBytes(itm.Text));
					
					//write md5 checksum string if needed
					if (CheckSum.Checked == true)
					{
						found = false;
						if (File.Exists(Plugins + itm.Text))
						{
							fsExt = new FileStream(Plugins + itm.Text,FileMode.Open);
							found = true;
						}
						if (File.Exists(Plugins_user + itm.Text))
						{
							fsExt = new FileStream(Plugins_user + itm.Text,FileMode.Open);
							found = true;
						}
						if (File.Exists(Effects + itm.Text))
						{
							fsExt = new FileStream(Effects + itm.Text,FileMode.Open);
							found = true;
						}
						if (found == true)
						{
							//get md5 checksum
							brExt = new BinaryReader(fsExt);
							System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
							byte[] result = md5.ComputeHash(brExt.ReadBytes((int)fsExt.Length));
							checksum = BitConverter.ToString(result).Replace("-","").ToLower();
							brExt.Close();
							fsExt.Close();
							//write md5 checksum
							bwOut.Write(Encoding.ASCII.GetBytes(checksum));
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			}
			finally
			{
				brIn.Close();
				fsIn.Close();
				bwOut.Close();
				fsOut.Close();
			}
		}
		dlg.Dispose();
	}

	private void about_Click(object sender, EventArgs e)
	{
		//show about dialog
		aboutDialog dlg = new aboutDialog();
		dlg.ShowDialog();
	}

	private void exit_Click(object sender, EventArgs e)
	{
		//exit
		Close();
	}
}
			