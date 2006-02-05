/*
net_int
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
// functions related to serching the command list
//
using System;
using System.Windows.Forms;
using System.Collections;
using System.Text;

class Search : IComparable
{
	public string method;
	public string dll;
	public int Relevence;

	public Search(string methodName, string dllName)
	{
		method = methodName;
		dll = dllName;
	}

	public int CompareTo(object o)
	{
		Search se = (Search) o;
		return se.Relevence - this.Relevence;
	}

	public static void BuildList(string darkBasic, CheckedListBox ca, CheckedListBox cb, ListView lv, ArrayList al)
	{
		Cursor.Current = Cursors.WaitCursor;
		int s;
		string[] items;
		lv.BeginUpdate();
		lv.Items.Clear();
		StringBuilder sb = new StringBuilder(255);
		foreach (object ob in ca.Items)
		{
			string str = darkBasic + "plugins\\" + ob.ToString();
			uint hinst = Build.LoadLibrary(str);
			s = 1;
			int bad = 0;
			while (bad < 5)
			{
				if (Build.LoadString(hinst, s, sb, 255) > 0)
				{
					ListViewItem lvi = new ListViewItem();
					items = sb.ToString().Split("%".ToCharArray(), 2);
					lvi.Text = items[0];
					lvi.SubItems.Add(ob.ToString());
					lv.Items.Add(lvi);
					//add item to arraylist
					Search se = new Search(lvi.Text, lvi.SubItems[1].Text);
					al.Add(se);
				}
				else
				{
					bad ++;
				}
				s ++;
			}
			Build.FreeLibrary(hinst);
		}
		foreach (object ob in cb.Items)
		{
			string str = darkBasic + "plugins-user\\" + ob.ToString();
			uint hinst = Build.LoadLibrary(str);
			s = 1;
			int bad = 0;
			while (bad < 5)
			{
				if (Build.LoadString(hinst, s, sb, 255) > 0)
				{
					ListViewItem lvi = new ListViewItem();
					items = sb.ToString().Split("%".ToCharArray(), 2);
					lvi.Text = items[0];
					lvi.SubItems.Add(ob.ToString());
					lv.Items.Add(lvi);
					//add item to arraylist
					Search se = new Search(lvi.Text, lvi.SubItems[1].Text);
					al.Add(se);
				}
				else
				{
					bad ++;
				}
				s ++;
			}
			Build.FreeLibrary(hinst);
		}
		lv.EndUpdate();
		Cursor.Current = Cursors.Default;
	}

	public static void DoSearch(string term, ListView lv, ArrayList al)
	{
		//search al and put matches in lv
		Cursor.Current = Cursors.WaitCursor;
		term = term.ToUpper();
		int rel; //serach relevence, how many terms does this item match
		string[] terms = term.Split(" ".ToCharArray());
		lv.BeginUpdate();
		lv.Items.Clear();
		if (term == "")
		{
			//return all items
			foreach (Search s in al)
			{
				ListViewItem lvi = new ListViewItem();
				lvi.Text = s.method;
				lvi.SubItems.Add(s.dll);
				lv.Items.Add(lvi);
			}
		}
		else
		{
			ArrayList found = new ArrayList();
			foreach(Search s in al)
			{
				rel = 0;
				foreach(string str in terms)
				{
					if (str != "")
					{
						if (s.method.IndexOf(str) != -1)
							rel ++;
					}
				}
				if (rel > 0)
				{
					Search foundItem = new Search(s.method, s.dll);
					foundItem.Relevence = rel;
					found.Add(foundItem);
				}
			}
			if (found.Count > 0)
			{
				found.Sort();
				foreach (Search s in found)
				{
					ListViewItem lvi = new ListViewItem();
					lvi.Text = s.method;
					lvi.SubItems.Add(s.dll);
					lv.Items.Add(lvi);
				}
			}
		}
		lv.EndUpdate();
		Cursor.Current = Cursors.Default;
	}
}