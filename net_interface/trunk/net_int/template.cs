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
// code template
//

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

class template
{
	//public string filter;
	string name;
	string fileName;
	public string filter = "";
	public string header = "";
	public string footer = "";
	public string nameSpaceStart = "";
	public string nameSpaceEnd = "";
	public string classStart = "";
	public string classEnd = "";
	public string method = "";
	public string noRet = "";
	public string noParam = "";
	public string L = "";
	public string D = "";
	public string F = "";
	public string S = "";
	public string Z = "";
	public string RL = "";
	public string RD = "";
	public string RF = "";
	public string RS = "";
	public string RZ = "";
	public string comment = "";

	public template(string filename)
	{
		fileName = filename;
		name = Path.GetFileName(fileName).Replace(".txt", "");
		string tag, data;
		FileStream fs = null;
		BinaryReader br = null;
		try
		{
			fs = new FileStream(fileName, FileMode.Open);
			br = new BinaryReader(fs);
			byte[] b = new byte[1];
			while (fs.Position != fs.Length)
			{
				tag = getTag(br).ToLower();
				data = getData(br);
				switch (tag)
				{
					case "filter":
						filter = data;
						break;
					case "header":
						header = data;
						break;
					case "footer":
						footer = data;
						break;
					case "namespacestart":
						nameSpaceStart = data;
						break;
					case "namespaceend":
						nameSpaceEnd = data;
						break;
					case "classstart":
						classStart = data;
						break;
					case "classend":
						classEnd = data;
						break;
					case "method":
						method = data;
						break;
					case "noret":
						noRet = data;
						break;
					case "noparam":
						noParam = data;
						break;
					case "l":
						L = data;
						break;
					case "d":
						D = data;
						break;
					case "f":
						F = data;
						break;
					case "s":
						S = data;
						break;
					case "Z":
						Z = data;
						break;
					case "rl":
						RL = data;
						break;
					case "rd":
						RD = data;
						break;
					case "rf":
						RF = data;
						break;
					case "rs":
						RS = data;
						break;
					case "rz":
						RZ = data;
						break;
					case "comment":
						comment = data;
						break;
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			if (br != null)
				br.Close();
			if (fs != null)
				fs.Close();
		}
	}

	static string getData(BinaryReader br)
	{
		//read data upto closing tag
		string data = "";
		char c = 'x';
		while (c != '<')
		{
			c = br.ReadChar();
			if (c != '<')
				data += c.ToString();
		}
		while (c != '>')
			c = br.ReadChar();
		//swap %l and %g to < and >
		data = data.Replace("%l", "<").Replace("%g", ">");
		return data;
	}
	static string getTag(BinaryReader br)
	{
		//read a tag from br <tag> and return the tag name
		string tag = "";
		char c = 'x';
		while (c != '<')
            c = br.ReadChar();
		while (c != '>')
		{
			c = br.ReadChar();
			if (c != '>')
				tag += c.ToString();
		}
		return tag;
	}

	public override string ToString()
	{
		return name;
	}

}
