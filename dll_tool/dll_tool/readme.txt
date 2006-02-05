dll_tool - use .net dlls in dbpro
++++++++

dbp@pinkbile.com
http://winch.pinkbile.com/

requires .Net SDK

version history
+++++++++++++++

v0.11 05 July 2005
- improved command line support to work with net_tool

v0.10 03 July 2005
- added command line support
- added update checker on about dialog

v0.9 28 June 2005
- added .dsf file loading and saving, .dsf files contain the settings for the dll.

v0.8 27 June 2005
- fixed stupid bug in auto string table generator
- can now choose which calling convention exported methods will use between Cdecl and StdCall

v0.7 26 June 2005
- yet more slight speedups in load function
- edit il dialog, allows you to view, edit, save and load il code
- edit il before build, shows il edit dialog just before the il is assembled with ilasm

v0.6 26 June 2005
- Tempory files now created in %TEMP% dir not application dir
- exports now use cdecl insted of stdcall
- very minor code speed ups
- fixed problems with method names that are ilasm reserved words

v0.5 03 January 2005
- String table generation completed
- fixed error when .method name had no opening bracket

v0.4 02 January 2005
- Improved the gui again to make handling large dlls much easier
- Started work on string table generator function but it doesn't yet fill in parameters

v0.3 01 January 2005
- Existing .res file is now edited instead or replaced
- Now supports more than 15 string table items
- Confims success or failure of build with option to show ilasm output on failure

v0.2 30 December 2004
-various gui bugs fixed and gui tidyup
-added option to include string table with built dll
-tries to find location of ilasm and ildasm instead of using hardcoded paths

v0.1 26 December 2004
-initial release

usage
+++++

Documentation is also provided in the doc subdirectory.
Start the program and load a .net dll. The dll will then be run through ildasm to get the .il code for the dll. The il
code will be displayed in the listbox at the bottom of the screen.
Any methods in the dll will be displayed in the left hand list box. Select and methods you want to export and press
the ">" button to move them into the exports listview box. You can double click on an item in the export box to edit the export name and calling convention.
To add a string table check "Add string table" then use "Add" button to add strings or generate button to automatically
generate a dbpro string table.
Click the save .dsf button if you want to save the exports and string table in a .dsf file. Then when you modify your .net
dll you can just load the .dsf file instead of moving all the methods and editing the string table.
Then click the build button and select where to save the new dll. The modified il code will then be run through ilasm to
produce a new dll. You can use this dll in dbpro using the CALL DLL commands or as a TPC if you included a string table.

Command line usage
++++++++++++++++++

dll_tool filename.dll
This will load filename.dll and show the gui.

dll_tool -a filename.dll
This will load filename.dll move a method called Do_It to the exports list and then save the dll as filename.dll and
then the program will close without showing the gui.

References
++++++++++
http://www.csharphelp.com/archives3/archive500.html
Inside microsoft .Net il assembler by Serge Lidin
.res file format info from www.wotsit.org

Todo
++++

Dbpro syntax higlighting .ini file and help export
Save and load exports and string table so you don't have to set everything up after you recompile you .net dll
Try to write example code to return strings from dbpro plugin

license
+++++++

dll_tool
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