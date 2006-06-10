//
//
//

using System;
using System.Windows.Forms;
using System.IO;
using System.Text;

class proExe
{
	public static void SkipExeSection(FileStream fs, BinaryReader br)
	{
		//skips over the exe section of an exe
		//skip dos stub
		fs.Seek(60, SeekOrigin.Begin);
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
	}

	public static void LoadExe(ListView contents, string fileName)
	{
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			fs = new FileStream(fileName, FileMode.Open);
			br = new BinaryReader(fs);
			//skip exe section
			SkipExeSection(fs, br);
			//add exe to listview
			ListViewFileItem lvi = new ListViewFileItem();
			lvi.Text = ListViewStrings.ExeSection;
			lvi.Offset = 0;
			lvi.Size = (int) fs.Position;
			//lvi.SubItems.Add("No");
			//lvi.SubItems.Add(lvi.Size.ToString());
			lvi.SubItems.Add(ListViewStrings.LocationExe);
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
					//lvi.SubItems.Add("Yes");
					//lvi.SubItems.Add(lvi.Size.ToString());
					lvi.SubItems.Add(ListViewStrings.LocationExe);
				}
				else
				{
					//compressed or extra data
					lvi.Text = ListViewStrings.ExtraData;
					lvi.Offset = (int)fs.Position - 4;
					lvi.Size = (int)(fs.Length - (fs.Position - 4));
					//lvi.SubItems.Add("No");
					//lvi.SubItems.Add(lvi.Size.ToString());
					lvi.SubItems.Add(ListViewStrings.LocationExe);
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
}