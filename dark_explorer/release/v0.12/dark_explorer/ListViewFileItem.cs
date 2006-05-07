/*
dark_explorer
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
// Subclass of ListViewItem that adds fields for Offset and Size.
// Offset is the offset that the item starts in the opened exe
// Size is the size of the file

using System.Windows.Forms;

class ListViewStrings
{
	public static readonly string Yes = "Yes";
	public static readonly string No  = "No";
	public static readonly string LocationExe = "<exe>";
	public static readonly string ExeSection = "Exe section";
	public static readonly string ExtraData = "Compressed or extra data";
}

enum ListViewOrder
{
	//order of items in contents listview
	Name = 0,
	FileType = 1,
	Upx = 2,
	NullString = 3,
	FileSize = 4,
	Location = 5
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
	public ListViewFileItem()
	{
		for (int i = 0; i < 5; i++)
		{
			this.SubItems.Add("");
		}
	}
}