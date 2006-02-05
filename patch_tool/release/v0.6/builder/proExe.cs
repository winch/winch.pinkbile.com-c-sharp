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
using System.IO;
using System.Text;

class proExe
{
	public static void ExtractFile(ListViewFileItem lvi, string exeName, string outName)
	{
		//extract file in ListViewFileItem
		FileStream fsIn = null;
		BinaryReader brIn = null;
		FileStream fsOut = null;
		BinaryWriter bwOut = null;
		try
		{
			fsIn = new FileStream(exeName, FileMode.Open);
			brIn = new BinaryReader(fsIn);
			fsOut = new FileStream(outName, FileMode.Create);
			bwOut = new BinaryWriter(fsOut);
			fsIn.Seek(lvi.Offset, SeekOrigin.Begin);
			bwOut.Write(brIn.ReadBytes(lvi.Size));
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (brIn != null)
				brIn.Close();
			if (fsIn != null)
				fsIn.Close();
			if (bwOut != null)
				bwOut.Close();
			if (fsOut != null)
				fsOut.Close();
		}
	}

	public static void SaveExe(ListView contents, string fileName, string oldName)
	{
		FileStream fsIn = null;
		BinaryReader brIn = null;
		FileStream fsOut = null;
		BinaryWriter bwOut = null;
		FileStream fsExt = null;
		BinaryReader brExt = null;
		bool overWrite = false; //has oldName been overwritten
		try
		{
			if (fileName == oldName)
			{
				//Trying to overwrite file so move old name to %temp% before opening
				File.Move(oldName, Path.GetTempPath() + Path.GetFileName(oldName));
				oldName = Path.GetTempPath() + Path.GetFileName(oldName);
				overWrite = true;
			}
			fsIn = new FileStream(oldName, FileMode.Open);
			brIn = new BinaryReader(fsIn);
			fsOut = new FileStream(fileName, FileMode.Create);
			bwOut = new BinaryWriter(fsOut);
			foreach (ListViewFileItem lvi in contents.Items)
			{
				if (lvi.SubItems[3].Text == "<exe>")
				{
					//internal file
					//name
					if (lvi.SubItems[1].Text == "Yes")
					{
						//is a normal file so write name and filedata length
						bwOut.Write(lvi.Text.Length);
						bwOut.Write(Encoding.ASCII.GetBytes(lvi.Text));
						//data
						bwOut.Write(lvi.Size);
					}
					fsIn.Seek(lvi.Offset, SeekOrigin.Begin);
					bwOut.Write(brIn.ReadBytes(lvi.Size));
				}
				else
				{
					//external files
					//name
					if (lvi.SubItems[1].Text == "Yes")
					{
						//is a normal file so write name and filedata length
						bwOut.Write(lvi.Text.Length);
						bwOut.Write(Encoding.ASCII.GetBytes(lvi.Text));
						//data
						fsExt = new FileStream(lvi.SubItems[3].Text, FileMode.Open);
						brExt = new BinaryReader(fsExt);
						bwOut.Write((int)fsExt.Length);
					}
					else
					{
						fsExt = new FileStream(lvi.SubItems[3].Text, FileMode.Open);
						brExt = new BinaryReader(fsExt);
					}
					bwOut.Write(brExt.ReadBytes((int)fsExt.Length));
				}
			}
			//delete oldName in temp dir if required
			if (overWrite == true)
			{
				brIn.Close();
				brIn = null;
				fsIn.Close();
				fsIn = null;
				File.Delete(oldName);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (brIn != null)
				brIn.Close();
			if (fsIn != null)
				fsIn.Close();
			if (bwOut != null)
				bwOut.Close();
			if (fsOut != null)
				fsOut.Close();
			if (brExt != null)
				brExt.Close();
			if (fsExt != null)
				fsExt.Close();
		}
	}

	public static void LoadExe(ListView contents, bool hasChangedColumn, string fileName)
	{
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			fs = new FileStream(fileName, FileMode.Open);
			br = new BinaryReader(fs);
			//find size of exe header
			//skip dos stub
			fs.Seek(60, SeekOrigin.Current);
			int e_lfanew = br.ReadInt32();
			//MessageBox.Show(e_lfanew.ToString());
			fs.Seek(e_lfanew + 4, SeekOrigin.Begin);
			//IMAGE_FILE_HEADER
			fs.Seek(2, SeekOrigin.Current);
			int NumberOfSections = br.ReadInt16();
			fs.Seek(16, SeekOrigin.Current);
			//end of IMAGE_FILE_HEADER
			//IMAGE_OPTIONAL_HEADER
			fs.Seek(224, SeekOrigin.Current);
			//end of IMAGE_OPTIONAL_HEADER
			//section directories
			int Size = 0; // size of section
			int Pos = 0;  // position of section
			for (int i=0; i<NumberOfSections; i++)
			{
				fs.Seek(16, SeekOrigin.Current);
				Size = br.ReadInt32();
				Pos = br.ReadInt32();
				fs.Seek(16, SeekOrigin.Current);
			}
			//end of section directories
			fs.Seek(Pos+Size, SeekOrigin.Begin);
			//add exe to listview
			ListViewFileItem lvi = new ListViewFileItem();
			lvi.Text = "Exe section";
			lvi.Offset = 0;
			lvi.Size = Pos + Size;
			if (hasChangedColumn == true)
				lvi.SubItems.Add("");
			//get hash
			lvi.SubItems.Add(getHash(fs, br, lvi.Offset, lvi.Size));
			lvi.SubItems.Add("No");
			contents.Items.Add(lvi);
			//Check for exe with no attached data
			if (lvi.Size == (int)fs.Length)
				return;
			//add attached files
			int nameLength = 1;
			while (nameLength > 0 && nameLength < 50)
			{
				lvi = new ListViewFileItem();
				nameLength = br.ReadInt32();
				//MessageBox.Show(nameLength.ToString());
				if (nameLength > 0 && nameLength < 50)
				{
					//file
					lvi.Text = Encoding.ASCII.GetString(br.ReadBytes(nameLength));
					lvi.Size = br.ReadInt32();
					lvi.Offset = (int)fs.Position;
					fs.Seek(lvi.Size, SeekOrigin.Current);
					if (hasChangedColumn == true)
						lvi.SubItems.Add("");
					//get hash
					lvi.SubItems.Add(getHash(fs, br, lvi.Offset, lvi.Size));
					lvi.SubItems.Add("Yes");
				}
				else
				{
					//compressed or extra data
					lvi.Text = "Compressed or extra data";
					lvi.Offset = (int)fs.Position - 4;
					lvi.Size = (int)(fs.Length - (fs.Position - 4));
					if (hasChangedColumn == true)
						lvi.SubItems.Add("");
					//get hash
					lvi.SubItems.Add(getHash(fs, br, lvi.Offset, lvi.Size));
					lvi.SubItems.Add("No");
				}
				contents.Items.Add(lvi);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (br != null)
				br.Close();
			if (fs != null)
				fs.Close();
		}
	}
	static string getHash(FileStream fs, BinaryReader br, int offset, int size)
	{
		string sum;
		long oldpos = fs.Position;
		fs.Seek(offset, SeekOrigin.Begin);
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] result = md5.ComputeHash(br.ReadBytes(size));
		sum = BitConverter.ToString(result).Replace("-","").ToLower();
		fs.Seek(oldpos, SeekOrigin.Begin);
		return sum;
	}
}

class ListViewFileItem : ListViewItem
{
	int fileoffset;
	int filesize;
	
	public int Offset
	{
		get
		{
			return fileoffset;
		}
		set
		{
			fileoffset = value;
		}
	}
	public int Size
	{
		get
		{
			return filesize;
		}
		set
		{
			filesize = value;
		}
	}
}
