//
// © the_winch 2008
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
// http://winch.pinkbile.com : thewinch@gmail.com

//contains functions used by decompress_block function

#ifndef DECOMPRESS_BLOCK_H
#define DECOMPRESS_BLOCK_H

//find size of all internal files
int internalFilesSize(void *data);

//are external files useing checksums?
int useMd5(void *data);

//get size of external files
int externalFilesSize(void *data, char *searchPath[], int md5);

//write exteral files to block
int writeExternalFiles(void *data, HGLOBAL block, char *searchPath[], int md5);

#endif
