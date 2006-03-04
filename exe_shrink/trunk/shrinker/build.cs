//
//
//

using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

class Build
{
	public static void NewBuild(string newExe, string oldExe, ListView Intern, ListView Extern, bool checkSums)
	{
		//build a new style exe with compress.dll
		int exeSection;
		FileStream fsIn = null;
		BinaryReader brIn = null;
		FileStream fsOut = null;
		BinaryWriter bwOut = null;
		try
		{
			fsOut = new FileStream(newExe, FileMode.Create);
			bwOut = new BinaryWriter(fsOut);
			fsIn = new FileStream(oldExe, FileMode.Open);
			brIn = new BinaryReader(fsIn);
			//write exe section to newExe
			proExe.SkipExeSection(fsIn, brIn);
			exeSection = (int) fsIn.Position;
			fsIn.Seek(0, SeekOrigin.Begin);
			bwOut.Write(brIn.ReadBytes(exeSection));
			brIn.Close();
			fsIn.Close();
			//write compress.dll
			fsIn = new FileStream(Application.StartupPath + Path.DirectorySeparatorChar + "compress.dll", FileMode.Open);
			brIn = new BinaryReader(fsIn);
			bwOut.Write("compress.dll".Length);
			bwOut.Write(Encoding.ASCII.GetBytes("compress.dll"));
			bwOut.Write((int) fsIn.Length);
			bwOut.Write(brIn.ReadBytes((int) fsIn.Length));
			brIn.Close();
			fsIn.Close();
			//write internal files
			foreach (ListViewFileItem lvi in Intern.Items)
			{
				//ignore <Standard Head> and <Extra Data>
				if (lvi.Text.StartsWith("<") == false)
				{
					//name
					bwOut.Write(lvi.Text.Length);
					bwOut.Write(Encoding.ASCII.GetBytes(lvi.Text));
					//data
					if (lvi.SubItems[1].Text == "<exe>")
						fsIn = new FileStream(oldExe, FileMode.Open);
					else
						fsIn = new FileStream(lvi.SubItems[1].Text, FileMode.Open);
					brIn = new BinaryReader(fsIn);
					fsIn.Seek(lvi.Offset, SeekOrigin.Begin);
					bwOut.Write(lvi.Size);
					bwOut.Write(brIn.ReadBytes(lvi.Size));
					brIn.Close();
					fsIn.Close();
				}
			}
			bwOut.Write((byte) 0);
			//Use checksums?
			if (checkSums == true)
				bwOut.Write((byte) 1);
			else
				bwOut.Write((byte) 0);
			//write list of external files
			foreach (ListViewFileItem lvi in Extern.Items)
			{
				//ignore <Standard Head> and <Extra Data>
				if (lvi.Text.StartsWith("<") == false)
				{
					//name
					bwOut.Write((byte) lvi.Text.Length);
					bwOut.Write(Encoding.ASCII.GetBytes(lvi.Text));
					//checksums
					if (checkSums == true)
					{
						string checksum;
						if (lvi.SubItems[1].Text == "<exe>")
							fsIn = new FileStream(oldExe, FileMode.Open);
						else
							fsIn = new FileStream(lvi.SubItems[1].Text, FileMode.Open);
						brIn = new BinaryReader(fsIn);
						fsIn.Seek(lvi.Offset, SeekOrigin.Begin);
						System.Security.Cryptography.MD5CryptoServiceProvider md5 =
							new System.Security.Cryptography.MD5CryptoServiceProvider();
						byte[] result = md5.ComputeHash(brIn.ReadBytes(lvi.Size));
						checksum = BitConverter.ToString(result).Replace("-","").ToLower();
						brIn.Close();
						fsIn.Close();
						//write md5 checksum
						bwOut.Write(Encoding.ASCII.GetBytes(checksum));
					}
				}
			}
			bwOut.Write((byte) 0);
			//write <Extra Data>
			bwOut.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4E, 0x61, 0xBC, 0x00, 0x00, 0x00, 0x00, 0x00 });
			//write exe section size
			bwOut.Write(exeSection);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(),"Error!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
		}
		finally
		{
			if (fsIn != null)
			{
				brIn.Close();
				fsIn.Close();
			}
			if (fsOut != null)
			{
				bwOut.Close();
				fsOut.Close();
			}
		}
	}

	public static void OldBuild(string FileName, main win)
	{
		//build an old style exe.
		FileStream fsIn = null;
		BinaryReader brIn = null;
		FileStream fsOut = null;
		BinaryWriter bwOut = null;
		FileStream fsExt = null;
		BinaryReader brExt = null;
		bool found;
		try
		{
			fsIn = new FileStream(win.Loaded_ExeName, FileMode.Open);
			brIn = new BinaryReader(fsIn);
			File.Copy(Application.StartupPath+"\\expander.dat", FileName, true);
			fsOut = new FileStream(FileName, FileMode.Append);
			bwOut = new BinaryWriter(fsOut);

			//write exe name
			bwOut.Write(win.Targetexe.Text.Length);
			bwOut.Write(Encoding.ASCII.GetBytes(win.Targetexe.Text));

			//write num of internal files
			bwOut.Write((byte) win.Intern.Items.Count);

			//write each internal file to patch
			foreach (ListViewFileItem itm in win.Intern.Items)
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
			if (win.CheckSum.Checked == true)
				bwOut.Write((byte) 1);
			else
				bwOut.Write((byte) 0);

			//write num of External files
			bwOut.Write((byte) win.Extern.Items.Count);
			string checksum;

			foreach (ListViewFileItem itm in win.Extern.Items)
			{
				//write name
				bwOut.Write(itm.Text.Length);
				bwOut.Write(Encoding.ASCII.GetBytes(itm.Text));
					
				//write md5 checksum string if needed
				if (win.CheckSum.Checked == true)
				{
					found = false;
					if (File.Exists(win.Plugins + itm.Text))
					{
						fsExt = new FileStream(win.Plugins + itm.Text,FileMode.Open);
						found = true;
					}
					if (File.Exists(win.Plugins_user + itm.Text))
					{
						fsExt = new FileStream(win.Plugins_user + itm.Text,FileMode.Open);
						found = true;
					}
					if (File.Exists(win.Effects + itm.Text))
					{
						fsExt = new FileStream(win.Effects + itm.Text,FileMode.Open);
						found = true;
					}
					if (found == true)
					{
						//get md5 checksum
						brExt = new BinaryReader(fsExt);
						System.Security.Cryptography.MD5CryptoServiceProvider md5 =
																	new System.Security.Cryptography.MD5CryptoServiceProvider();
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
}