dark explorer
+++++++++++++
A darkbasic professional, fps creator and darkbasic classic exe explorer

Requires .Net framework v1.1 to run
Upx compression needs upx.exe in the program directory, download from http://upx.sourceforge.net/

Usage
+++++

The program is controled by the right-click menu.

Todo
++++

Null string tables does not yet work.

Adding media
++++++++++++

To add media you must be using relative paths to load it.
You can add files and media with "inset". For media name of the file in the exe must be prefixed
with "media\" and the dir name the media is in within you project dir.
eg
load image "image\wall.png",1,1
would need a name of
media\image\wall.png
in the exe.

Upx and exe compression notes
+++++++++++++++++++++++++++++

Using upx and/or the built in dbpro compression will give you a smaller exe that is harder to compress.
If you plan to zip your exe then the final zip will usually be bigger when using a compressed exe.
Compressed exes will also be decompressed when the exe is run so they will start up slower
than an uncompressed exe. Upx is much faster at decompressing than dbpro so is less noticable.

Adding non tpc dlls
+++++++++++++++++++

Depending on how the dll is loaded it may or may not need a "media\" prefix to the name.

license
+++++++

dark_explorer
Copyright (C) 2006 the_winch

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 3
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.