//
//Funtions for dbpro exes.
//

using System;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace DB
{
	class proExe
	{
		public static void InjectDll(string oldName, string fileName, string dllName)
		{
			//injects dllName as the first attached file into a dbpro exe
			FileStream fsIn = null;
			BinaryReader brIn = null;
			FileStream fsOut = null;
			BinaryWriter bwOut = null;
			FileStream fsDll = null;
			BinaryReader brDll = null;
			string tempName = "";
			bool overWrite = false; //has oldName been overwritten
			bool fail = false;      //has the funtion failed to write a new exe
			try
			{
				if (fileName == oldName)
				{
					//overwriting old exe so move it to the temp dir first
					tempName = Path.GetTempFileName();
					File.Delete(tempName);
					File.Move(oldName, tempName);
					//open temp exe
					fsIn = new FileStream(tempName, FileMode.Open);
					brIn = new BinaryReader(fsIn);
					overWrite = true;
				}
				else
				{
					//open old exe
					fsIn = new FileStream(oldName, FileMode.Open);
					brIn = new BinaryReader(fsIn);
				}
				fsOut = new FileStream(fileName, FileMode.Create);
				bwOut = new BinaryWriter(fsOut);
				fsDll = new FileStream(dllName, FileMode.Open);
				brDll = new BinaryReader(fsDll);
				int exeSection = 0;
				//find exe section size
				//check exe signiture
				if (Encoding.ASCII.GetString(brIn.ReadBytes(2)) == "MZ")
				{
					//skip dos stub
					int Size = 0;
					int Pos = 0;
					fsIn.Seek(60, SeekOrigin.Begin);
					int e_lfanew = brIn.ReadInt32();
					//MessageBox.Show(e_lfanew.ToString());
					fsIn.Seek(e_lfanew + 6, SeekOrigin.Begin);
					int NumberOfSections = brIn.ReadInt16();
					fsIn.Seek(240, SeekOrigin.Current);
					for (int i=0; i<NumberOfSections; i++)
					{
						fsIn.Seek(16, SeekOrigin.Current);
						Size = brIn.ReadInt32();
						Pos = brIn.ReadInt32();
						fsIn.Seek(16, SeekOrigin.Current);
					}
					exeSection = Size + Pos;
					//check there is actuall data after exe section
					if ((exeSection) == fsIn.Length)
					{
						MessageBox.Show("Dbpro exe has no attached files.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
						fail = true;
						return;
					}
					//make sure dbpro exe is not compressed
					fsIn.Seek(Size + Pos + 4, SeekOrigin.Begin);
					if (Encoding.ASCII.GetString(brIn.ReadBytes("compress.dll".Length)) == "compress.dll")
					{
						MessageBox.Show("dark_injector can not inject compressed dbpro exes\n", "Error!", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						fail = true;
						return;
					}
					//copy exe section to new exe
					fsIn.Seek(0, SeekOrigin.Begin);
					bwOut.Write(brIn.ReadBytes(exeSection));
				}
				fsIn.Seek(exeSection, SeekOrigin.Begin);
				//write compress.dll
				bwOut.Write("compress.dll".Length);
				bwOut.Write(Encoding.ASCII.GetBytes("compress.dll"));
				bwOut.Write((int)fsDll.Length);
				bwOut.Write(brDll.ReadBytes((int)fsDll.Length));
				//write the rest of the file
				bwOut.Write(brIn.ReadBytes((int)(fsIn.Length - fsIn.Position)));
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
				if (brDll != null)
					brDll.Close();
				if (fsDll != null)
					fsDll.Close();
				//check for overWrite/fail
				if (fail == true)
				{
					File.Delete(fileName);
				}
				if (overWrite == true)
				{
					if (fail == false)
					{
						//everything went fine so the old exe is not needed
						File.Delete(tempName);
					}
					else
					{
						//a new exe could not be written so copy file back
						File.Delete(oldName);
						File.Move(tempName, oldName);
					}
				}
			}
		}
	}
}