Dbpro exe tool
++++++++++++++

dbp@pinkbile.com
http://winch.pinkbile.com/

Requires .Net framwork to run
Includes (badly written) c# source

Compressing dlls with Upx on save
+++++++++++++++++++++++++++++++++

If you want to do this you must download upx.exe from http://upx.sourceforge.net/ and put upx.exe in the same directory as this
program. The "Use upx on save" checkbox will then be enabled. Then when you save an exe or pck with the checkbox check upx will
be used to compress all the dlls while saving.

A couple of example programs before and after upx compression.
original  with upx
3.89Mb    1.88mb
1.73Mb    887Kb

However using upx has similar problems to the built in dbpro media compression, it will make the exe harder to compress. So the
exe will be smaller but the zip you put on your website may be bigger than if you didn't use upx.

Null dll resources
++++++++++++++++++

This will over write the resource section in the dll with nulls. The idea is to make it harder for people to rip your tpc dlls out
of the exe or from the temp dir and use them in their own projects. If you use this option they will have to write their own
string tables if they want to use your dll.
At the moment the whole resource section is nulled not just the string tables and might cause a problem with some dlls. Delphi
dlls in particular can make use of stuff in the resource section and may not work. If any dlls cause problems just Load the exe
after saving and replace the offending dll with the original version.

Adding media
++++++++++++

To add media you must be using realtive paths to load it.
You can either add the media one item at a time and manually enter the correct prefix
or you can use the "insert with wildcard" to insert many files in one go.
The name of the file in the exe must be prefixed with "media\" and the dir name the media is in within you project dir.
eg
load image "image\wall.png",1,1
would need a name of
media\image\wall.png

If you use "insert with wildcard" then all you have to do is click the "guess prefix" button in the filter and prefix dialog
and it should correctly guess the prefix you need to use.

Adding non tpc dlls
+++++++++++++++++++

Depending on how the dll is loaded it may or may not need a "media\" prefix to the name.

The following dlls require a "media\" prefix
nuclear glory collision (NGCollision.dll)
dbp_netlib (DBP_NETLIB.dll)

The following dlls require no prefix
Newton wrapper (Newton.dll)

Changing the initial display mode
+++++++++++++++++++++++++++++++++

You won't be able to set the resolution if SET DISPLAY MODE is used within the source of the game.
Unfotunatly quite a few people do this instead of setting the display mode in the IDE project manager.
Be aware that the game may have dependancies on a particular screen res and may not work correctly if you change the resolution or depth.

Thanks to Van B for pointing out the initial display mode was easy to change
see http://forum.thegamecreators.com/?m=forum_view&t=38782&b=1

License
+++++++

© the_winch 2005
Permission to copy, use, modify, sell and distribute this software is
granted provided this notice appears un-modified in all copies.
This software is provided as-is without express or implied warranty,
and with no claim as to its suitability for any purpose.