/*
net_tool
Copyright (C) 2005 the_winch

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
//
// dbpro exe stuff
//
using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

class ProExe
{
	public static void DoExe(BinaryWriter bwOut, FileStream fsIn, BinaryReader brIn, main win)
	{
		//write out the new exe with net dll swapped
		FileStream fsExt;
		BinaryReader brExt;
		long currPos;
		//find size of exe header
		//skip dos stub
		fsIn.Seek(60, SeekOrigin.Begin);
		int e_lfanew = brIn.ReadInt32();
		//MessageBox.Show(e_lfanew.ToString());
		fsIn.Seek(e_lfanew + 4, SeekOrigin.Begin);
		//IMAGE_FILE_HEADER
		fsIn.Seek(2, SeekOrigin.Current);
		int NumberOfSections = brIn.ReadInt16();
		fsIn.Seek(16, SeekOrigin.Current);
		//end of IMAGE_FILE_HEADER
		//IMAGE_OPTIONAL_HEADER
		fsIn.Seek(224, SeekOrigin.Current);
		//end of IMAGE_OPTIONAL_HEADER
		//section directories
		int Size = 0; // size of section
		int Pos = 0;  // position of section
		for (int i=0; i<NumberOfSections; i++)
		{
			fsIn.Seek(16, SeekOrigin.Current);
			Size = brIn.ReadInt32();
			Pos = brIn.ReadInt32();
			fsIn.Seek(16, SeekOrigin.Current);
		}
		//end of section directories
		fsIn.Seek(Pos+Size, SeekOrigin.Begin);
		currPos = fsIn.Position;
		//write exe section
		fsIn.Seek(0, SeekOrigin.Begin);
		bwOut.Write(brIn.ReadBytes((int)currPos));
		win.log.Items.Add("Wrote exe header");
		//write attached files
		int nameLength = 1;
		int dataLength;
		string name;
		while (nameLength > 0 && nameLength < 50 && fsIn.Position < fsIn.Length)
		{
			nameLength = brIn.ReadInt32();
			if (nameLength > 0 && nameLength < 50)
			{
				name = Encoding.ASCII.GetString(brIn.ReadBytes(nameLength));
				dataLength = brIn.ReadInt32();
				//write name
				bwOut.Write(nameLength);
				bwOut.Write(Encoding.ASCII.GetBytes(name));
				if (name == "net_int.dll")
				{
					//replace with netDll
					fsExt = new FileStream(win.netDll + ".new", FileMode.Open);
					brExt = new BinaryReader(fsExt);
					//write length
					bwOut.Write((int) fsExt.Length);
					//write data
					bwOut.Write(brExt.ReadBytes((int)fsExt.Length));
					//seek over existing dll in exe
					fsIn.Seek(dataLength, SeekOrigin.Current);
					brExt.Close();
					fsExt.Close();
					win.log.Items.Add("Replaced net_int.dll");
				}
				else if (name == "_virtal.dat")
				{
					//write displaysettings first
					bwOut.Write(dataLength);
					//write display settings
					bwOut.Write(win.dispMode.SelectedIndex);
					bwOut.Write(int.Parse(win.dispWidth.Text));
					bwOut.Write(int.Parse(win.dispHeight.Text));
					bwOut.Write(int.Parse(win.dispDepth.Text));
					//seek over settings
					fsIn.Seek(16, SeekOrigin.Current);
					//write data
					bwOut.Write(brIn.ReadBytes(dataLength - 16));
					win.log.Items.Add("Wrote _virtual.dat with display settings");
				}
				else
				{
					//normal file
					bwOut.Write(dataLength);
					bwOut.Write(brIn.ReadBytes(dataLength)); 
				}
			}
		}
		//write extra data
		fsIn.Seek(-4, SeekOrigin.Current);
		bwOut.Write(brIn.ReadBytes((int)(fsIn.Length - fsIn.Position)));
		win.log.Items.Add("wrote extra data");
	}
}