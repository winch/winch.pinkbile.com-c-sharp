//
//
//

using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

class Build
{
	public static void BuildExe(string newExe, string oldExe, ListView Intern, ListView Extern, bool checkSums)
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
			fsIn = new FileStream(Application.StartupPath + Path.DirectorySeparatorChar + "compress.dat", FileMode.Open);
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
					if (lvi.SubItems[1].Text == ListViewStrings.LocationExe)
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
			//end of internal files null int
			bwOut.Write(0);
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
						if (lvi.SubItems[1].Text == ListViewStrings.LocationExe)
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
}