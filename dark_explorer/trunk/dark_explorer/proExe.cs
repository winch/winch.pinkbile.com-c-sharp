/*
dark_explorer
Copyright (C) 2005,2006,2007,2008 the_winch

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
//
//Funtions for dbpro exes. Load, save, extract etc
//

using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

class proExe
{
	public static string DbcRemoveNull(string name)
	{
		//filenames in dbc exes have a null terminator
		//this function removes the last character of the input string if it is null
		if (name[name.Length - 1] == '\0')
			name = name.Substring(0, name.Length - 1);
		return name;
	}

	public static string DbcAddNull(string name)
	{
		//adds null termination to name
		name = name + "\0";
		return name;
	}

	public static bool IsCompressed(ListView contents)
	{
		//returns true if the exe or pck is compressed
		bool compressed = false;
		//pck
		if (contents.Items.Count > 1)
		{
			if (contents.Items[0].Text == ListViewStrings.CompressDll)
				compressed = true;
		}
		//exe
		if (contents.Items.Count > 2)
		{
			if (contents.Items[1].Text == ListViewStrings.CompressDll)
				compressed = true;
		}
		return compressed;
	}

	public static void CompressExe(ListView contents, bool dbPro, window win, string oldExe, string newExe, string compressDll)
	{
		FileStream fs = null;
		BinaryReader br = null;
		int exeSection = 0, extraData = 0;
		string tempExe = Path.GetTempFileName();
		//save exe
		SaveExe(contents, tempExe, oldExe, dbPro, win);
		try
		{
			fs = new FileStream(tempExe, FileMode.Open);
			br = new BinaryReader(fs);
			SkipExeSection(fs, br);
			exeSection = (int)fs.Position;
			//get size of extra data
			if (contents.Items[contents.Items.Count - 1].SubItems[(int)ListViewOrder.Name].Text == ListViewStrings.ExtraData)
			{
				ListViewFileItem lvi = (ListViewFileItem)contents.Items[contents.Items.Count - 1];
				extraData = lvi.Size;
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
		try
		{
			CompressDll(oldExe, exeSection, extraData, newExe, compressDll);
		}
		catch(Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);			
		}
		if (File.Exists(tempExe))
			File.Delete(tempExe);
	}
	[DllImport("comp.dll", EntryPoint="compress")]
	private static extern void CompressDll(string fileName, int exeSection, int extraData, string newExe, string compressDll);

	public static void DecompressExe(ListView contents, bool dbPro, window win, string oldExe, string newExe)
	{
		FileStream fs = null;
		BinaryReader br = null;
		FileStream fsDll = null;
		BinaryWriter bwDll = null;
		int dataLength, exeSection = 0, dataOffset = 0;
		string compressDll = Path.GetTempFileName(); 
		string tempExe = Path.GetTempFileName();
		//save exe
		SaveExe(contents, tempExe, oldExe, dbPro, win);
		try
		{
			fs = new FileStream(tempExe, FileMode.Open);
			br = new BinaryReader(fs);
			SkipExeSection(fs, br);
			exeSection = (int)fs.Position;
			//skip compress.dll name
			dataLength = br.ReadInt32();
			fs.Seek(dataLength, SeekOrigin.Current);
			dataLength = br.ReadInt32();
			//write compress.dll
			fsDll = new FileStream(compressDll, FileMode.Create);
			bwDll = new BinaryWriter(fsDll);
			bwDll.Write(br.ReadBytes(dataLength));
			dataOffset = (int)fs.Position; //start of compressed data
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
			if (bwDll != null)
				bwDll.Close();
			if (fsDll != null)
				fsDll.Close();
		}
		try
		{
			DecompressDll(oldExe, exeSection, dataOffset, newExe, compressDll);
		}
		catch(Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);			
		}
		if (File.Exists(compressDll))
			File.Delete(compressDll);
		if (File.Exists(tempExe))
			File.Delete(tempExe);
	}
	[DllImport("comp.dll", EntryPoint="decompress")]
	private static extern void DecompressDll(string fileName, int exeSection, int dataOffset, string newExe, string compressDll);

	private static void SkipExeSection(FileStream fs, BinaryReader br)
	{
		//skips over the exe section of an exe
		fs.Seek(0, SeekOrigin.Begin);
		if (Encoding.ASCII.GetString(br.ReadBytes(2)) != "MZ")
		{
			//no exe signiture
			fs.Seek(0, SeekOrigin.Begin);
			return;
		}
		//skip dos stub
		fs.Seek(60, SeekOrigin.Begin);
		int e_lfanew = br.ReadInt32();
		//MessageBox.Show(e_lfanew.ToString());
		fs.Seek(e_lfanew + 4, SeekOrigin.Begin);
		//IMAGE_FILE_HEADER
		fs.Seek(2, SeekOrigin.Current);
		int NumberOfSections = br.ReadInt16();
		fs.Seek(240, SeekOrigin.Current);
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

	public static void ExtractFile(ListViewFileItem lvi, string exeName, string outName)
	{
		//extract file in ListViewFileItem
		FileStream fsIn = null;
		BinaryReader brIn = null;
		FileStream fsOut = null;
		BinaryWriter bwOut = null;
		try
		{
			if (lvi.SubItems[(int)ListViewOrder.Location].Text == ListViewStrings.LocationExe)
			{
				//internal file
				fsIn = new FileStream(exeName, FileMode.Open);
				brIn = new BinaryReader(fsIn);
			}
			else
			{
				//external file
				fsIn = new FileStream(lvi.SubItems[(int)ListViewOrder.Location].Text, FileMode.Open);
				brIn = new BinaryReader(fsIn);
			}
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

	public static void SaveExe(ListView contents, string fileName, string oldName, bool dbPro, window win)
	{
		FileStream fsExe = null;   //old dbpro exe
		BinaryReader brExe = null;
		FileStream fsOut = null;   //new dbpro exe
		BinaryWriter bwOut = null;
		FileStream fsFile;         //current file being written
		BinaryReader brFile;
		string name;
		bool overWrite = false; //has oldName been overwritten
		int exeSection = -1;    //size of exeSection
		try
		{
			if (fileName == oldName)
			{
				//Trying to overwrite file so move old name to %temp% before opening
				File.Move(oldName, Path.GetTempPath() + Path.GetFileName(oldName));
				oldName = Path.GetTempPath() + Path.GetFileName(oldName);
				overWrite = true;
			}
			if (oldName != "")
			{
				//if there is an old dbpro exe load it
				fsExe = new FileStream(oldName, FileMode.Open);
				brExe = new BinaryReader(fsExe);
			}
			//open new exe
			fsOut = new FileStream(fileName, FileMode.Create);
			bwOut = new BinaryWriter(fsOut);
			foreach (ListViewFileItem lvi in contents.Items)
			{
				if (lvi.SubItems[(int)ListViewOrder.Location].Text == ListViewStrings.LocationExe)
				{
					//internal file
					fsFile = fsExe;
					brFile = brExe;
				}
				else
				{
					//external file
					fsFile = new FileStream(lvi.SubItems[(int)ListViewOrder.Location].Text, FileMode.Open);
					brFile = new BinaryReader(fsFile);
				}
				//seek to data start
				fsFile.Seek(lvi.Offset, SeekOrigin.Begin);
				//name
				if (lvi.SubItems[(int)ListViewOrder.FileType].Text == ListViewStrings.Yes)
				{
					//is a normal file so write name and filedata length
					name = DbcRemoveNull(lvi.Text);
					if (dbPro == false)
						name = DbcAddNull(name);
					bwOut.Write(name.Length);
					bwOut.Write(Encoding.ASCII.GetBytes(name));
				}
				//check for _virtual.dat
				if (lvi.Text == ListViewStrings.VirtualDat)
				{
					//size
					bwOut.Write(lvi.Size);
					//write display settings
					bwOut.Write(win.displayMode);
					bwOut.Write(win.displayWidth);
					bwOut.Write(win.displayHeight);
					bwOut.Write(win.displayDepth);
					//write data
					fsFile.Seek(16, SeekOrigin.Current);
					bwOut.Write(brFile.ReadBytes(lvi.Size - 16));
				}
				else
				{
					//not _virtual.dat
					if (lvi.SubItems[(int)ListViewOrder.FileType].Text == ListViewStrings.No &&
						lvi.SubItems[(int)ListViewOrder.Name].Text == ListViewStrings.ExtraData)
					{
						//extra data so write exeSection size at end of extra data
						bwOut.Write(brFile.ReadBytes(lvi.Size - 4));
						bwOut.Write(exeSection);
					}
					else
					{
						//not _virtual.dat or extra data
						int written = WriteData(fsFile, brFile, lvi, bwOut);
						if (lvi.SubItems[(int)ListViewOrder.Name].Text == ListViewStrings.ExeSection)
						{
							//set exeSection size if it was written
							exeSection = written;
						}
					}
				}
				if (lvi.SubItems[(int)ListViewOrder.Location].Text != ListViewStrings.LocationExe)
				{
					//close external file
					brFile.Close();
					fsFile.Close();
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (brExe != null)
				brExe.Close();
			if (fsExe != null)
				fsExe.Close();
			if (bwOut != null)
				bwOut.Close();
			if (fsOut != null)
				fsOut.Close();
			//delete oldName in temp dir if required
			if (overWrite == true)
			{
				File.Delete(oldName);
			}
		}
	}

	public static void LoadExe(ListView contents, string fileName, window win)
	{
		//debugLog.StartSection("LoadExe");
		int exeSectionSize = 0, extraDataSize = 0;
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			fs = new FileStream(fileName, FileMode.Open);
			br = new BinaryReader(fs);
			ListViewFileItem lvi;
			//debugLog.Log("Loading " + Path.GetFileName(fileName));
			//check exe signiture
			if (Encoding.ASCII.GetString(br.ReadBytes(2)) == "MZ")
			{
				//debugLog.Log("Found exe signiture");
				SkipExeSection(fs, br);
				//add exe to listview
				lvi = new ListViewFileItem();
				lvi.SubItems[(int)ListViewOrder.Name].Text = ListViewStrings.ExeSection;
				lvi.Offset = 0;
				lvi.Size = (int)fs.Position;
				exeSectionSize = lvi.Size;
				lvi.SubItems[(int)ListViewOrder.FileType].Text = ListViewStrings.No;
				lvi.SubItems[(int)ListViewOrder.Upx].Text = ListViewStrings.No;
				lvi.SubItems[(int)ListViewOrder.NullString].Text = ListViewStrings.No;
				lvi.SubItems[(int)ListViewOrder.FileSize].Text = lvi.Size.ToString("n0");
				lvi.SubItems[(int)ListViewOrder.Location].Text = ListViewStrings.LocationExe;
				contents.Items.Add(lvi);
				//debugLog.Log("Exe section size = " + lvi.Size.ToString("n0"));
				//Check for exe with no attached data
				if (lvi.Size == (int)fs.Length)
				{
					//debugLog.Log("Exe has no appended data");
					return;
				}
			}
			else
			{
				//it's a pck file so files start at begining of file
				//debugLog.Log("Exe signiture not found, assuming .pck");
				fs.Seek(0, SeekOrigin.Begin);
			}
			//add attached files
			int nameLength = 1;
			while (nameLength > 0 && nameLength < 500 && fs.Position < fs.Length)
			{
				lvi = new ListViewFileItem();
				nameLength = br.ReadInt32();
				//MessageBox.Show(nameLength.ToString());
				if (nameLength > 0 && nameLength < 500)
				{
					//file
					lvi.SubItems[(int)ListViewOrder.Name].Text = Encoding.ASCII.GetString(br.ReadBytes(nameLength));
					lvi.Size = br.ReadInt32();
					lvi.Offset = (int)fs.Position;
					//debugLog.Log(DbcRemoveNull(lvi.Text).PadRight(26, ' ') + " Size :" + lvi.Size.ToString("n0").PadRight(10, ' ') +
					//			 " Offset :" + lvi.Offset.ToString("n0"));
					//check for _virtual.dat
					if (lvi.SubItems[(int)ListViewOrder.Name].Text == ListViewStrings.VirtualDat)
					{
						//get display settings
						win.displayMode = br.ReadInt32();
						int Width = br.ReadInt32();
						int Height = br.ReadInt32();
						int Depth = br.ReadInt32();
						win.displayWidth = Width;
						win.displayHeight = Height;
						win.displayDepth = Depth;
						contents.ContextMenu.MenuItems[window.MENU_DISPLAY].Text =
							proExe.getDisplayString(Width, Height, Depth, win.displayMode);
						contents.ContextMenu.MenuItems[window.MENU_DISPLAY].Enabled = true;
						fs.Seek(-16, SeekOrigin.Current);
					}
					fs.Seek(lvi.Size, SeekOrigin.Current);
					lvi.SubItems[(int)ListViewOrder.FileType].Text = ListViewStrings.Yes;
					lvi.SubItems[(int)ListViewOrder.Upx].Text = ListViewStrings.No;
					lvi.SubItems[(int)ListViewOrder.NullString].Text = ListViewStrings.No;
					lvi.SubItems[(int)ListViewOrder.FileSize].Text = lvi.Size.ToString("n0");
					lvi.SubItems[(int)ListViewOrder.Location].Text = ListViewStrings.LocationExe;
				}
				else
				{
					//compressed or extra data
					lvi.SubItems[(int)ListViewOrder.Name].Text = ListViewStrings.ExtraData;
					lvi.Offset = (int)fs.Position - 4;
					lvi.Size = (int)(fs.Length - (fs.Position - 4));
					lvi.SubItems[(int)ListViewOrder.FileType].Text = ListViewStrings.No;
					lvi.SubItems[(int)ListViewOrder.Upx].Text = ListViewStrings.No;
					lvi.SubItems[(int)ListViewOrder.NullString].Text = ListViewStrings.No;
					lvi.SubItems[(int)ListViewOrder.FileSize].Text = lvi.Size.ToString("n0");
					lvi.SubItems[(int)ListViewOrder.Location].Text = ListViewStrings.LocationExe;
					fs.Seek(-4, SeekOrigin.End);
					extraDataSize = br.ReadInt32();
					//debugLog.Log("Extra data size :" + lvi.Size.ToString("n0") + " reported exe section size :" +
					//			 extraDataSize.ToString("n0"));
					//if (extraDataSize != exeSectionSize)
					//	debugLog.Log("Warning exe section size reported in extra data does not match actual exe section size");
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
		//debugLog.StopSection();
	}
	public static string getDisplayString(int width, int height, int depth, int mode)
	{
		string displaystring = width.ToString() + "x" + height.ToString() + "x" + depth.ToString();
		switch (mode)
		{
			case 0:
				displaystring += " hidden";
				break;
			case 1:
				displaystring += " windowed";
				break;
			case 2:
				displaystring += " windowed desktop";
				break;
			case 3:
				displaystring += " full exclusive";
				break;
			case 4:
				displaystring += " windowed fullscreen";
				break;
		}
		return displaystring;
	}

	private static int WriteData(FileStream fsIn, BinaryReader brIn, ListViewFileItem lvi, BinaryWriter bwOut)
	{
		//writes filedata to file after modifing it if required (upx, string table null, etc)
		string tempFile;
		FileStream fsTemp = null;
		BinaryWriter bwTemp = null;
		BinaryReader brTemp = null;
		if (lvi.SubItems[(int)ListViewOrder.Upx].Text != ListViewStrings.No ||
			lvi.SubItems[(int)ListViewOrder.NullString].Text == ListViewStrings.Yes)
		{
			//upx or null string table
			if (lvi.SubItems[(int)ListViewOrder.Upx].Text == ListViewStrings.Yes)
			{
				//check for upx.exe
				if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "upx.exe") == false)
				{
					MessageBox.Show("upx.exe not found in program directory.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return 0;
				}
			}
			tempFile = Path.GetTempFileName();
			try
			{
				//write temp file
				fsTemp = new FileStream(tempFile, FileMode.Create);
				bwTemp = new BinaryWriter(fsTemp);
				bwTemp.Write(brIn.ReadBytes(lvi.Size));
				bwTemp.Close();
				bwTemp = null;
				fsTemp.Close();
				fsTemp = null;
				if (lvi.SubItems[(int)ListViewOrder.Upx].Text != ListViewStrings.No)
				{
					//compress with upx
					Process upx = new Process();
					upx.StartInfo.CreateNoWindow = true;
					upx.StartInfo.UseShellExecute = false;
					upx.StartInfo.RedirectStandardOutput = true;
					upx.StartInfo.RedirectStandardError = true;
					upx.StartInfo.FileName = Application.StartupPath + Path.DirectorySeparatorChar + "upx.exe";
					//use lzma compression?
					if (lvi.SubItems[(int)ListViewOrder.Upx].Text == ListViewStrings.UpxLzma)
						upx.StartInfo.Arguments += "--lzma " + tempFile;
					else
						upx.StartInfo.Arguments = tempFile;
					upx.Start();
					upx.WaitForExit();
					if (upx.StandardOutput.ReadToEnd().IndexOf("Packed 1 file.") == -1)
					{
						//upx failed
						if (MessageBox.Show("Upx failed to compress " + lvi.SubItems[(int)ListViewOrder.Name].Text + "\n See upx error?",
							"Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
						{
							//show upx output
							MessageBox.Show(upx.StandardError.ReadToEnd());
						}
					}
				}
				else if(lvi.SubItems[(int)ListViewOrder.NullString].Text == ListViewStrings.Yes)
				{
					//null string table
				}
				//write modified file
				fsTemp = new FileStream(tempFile, FileMode.Open);
				brTemp = new BinaryReader(fsTemp);
				if (lvi.SubItems[(int)ListViewOrder.FileType].Text == ListViewStrings.Yes)
				{
					//attached file so writedata size
					bwOut.Write((int)fsTemp.Length);
				}
				bwOut.Write(brTemp.ReadBytes((int)fsTemp.Length));
				return (int)fsTemp.Length;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				if (bwTemp != null)
					bwTemp.Close();
				if (brTemp != null)
					brTemp.Close();
				if (fsTemp != null)
					fsTemp.Close();
				if (File.Exists(tempFile))
					File.Delete(tempFile);
			}
		}
		//unmodified file data 
		if (lvi.SubItems[(int)ListViewOrder.FileType].Text == ListViewStrings.Yes)
		{
			//attached file so writedata size
			bwOut.Write(lvi.Size);
		}
		//write data
		fsIn.Seek(lvi.Offset, SeekOrigin.Begin);
		bwOut.Write(brIn.ReadBytes(lvi.Size));
		//return ammount of data written
		return lvi.Size;
	}

}