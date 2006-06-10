//
//
//

using System.Windows.Forms;

class ListViewStrings
{
	//common strings in listview
	public static readonly string LocationExe = "<exe>";
	public static readonly string ExtraData = "<Extra Data>";
	public static readonly string ExeSection = "<Standard Head>";
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