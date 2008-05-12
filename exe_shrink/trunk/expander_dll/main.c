//
// © the_winch 2008
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
// http://winch.pinkbile.com : thewinch@gmail.com

#include <windows.h>
#include <stdio.h>

#include "compress_block.h"
#include "decompress_block.h"

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

//functions
int getSearchPaths(char *SearchPath[]);

/*
void debug_num(char *text, int num)
{
    char buffer[255];
    buffer[0] = 0;
    sprintf(buffer, "%s : %d", text, num);
    MessageBox(GetActiveWindow(), buffer, "debug", 0);
}

void debug_text(char *text)
{
    MessageBox(GetActiveWindow(), text, "debug", 0);
}
*/

HGLOBAL DLL_EXPORT compress_block(void *data, DWORD dataSize)
{
    HGLOBAL block = NULL;
    int virualDatSize = 0;    //size of _virtual.dat file
    int externalDataSize = 0; //size of external file data
    return NULL;
}

HGLOBAL DLL_EXPORT decompress_block(void *data, DWORD dataSize)
{
    #define SEARCHCOUNT 4 //number of paths in search array
    char *searchPath[SEARCHCOUNT]; // array of paths to search for files
    int internalSize, externalSize, md5;
    void *dataStart = data;

    HGLOBAL block = NULL;

    //fill searchPath array
    if (getSearchPaths(searchPath) == 0)
    {
        //error filling searchPath array
        return NULL;
    }

    //find size of internal files
    internalSize = internalFilesSize(data);
    //skip internal files an null int
    data += internalSize + 4;

    //use md5 checksums for external files?
    md5 = useMd5(data);
    data += 1;

    //find size of external files
    externalSize = externalFilesSize(data, searchPath, md5);
    if (externalSize == 0)
    {
        //external file not found
        return 0;
    }

    //get block
    block = GlobalAlloc(GMEM_FIXED, internalSize + externalSize);
    if (block == NULL)
    {
        MessageBox(GetActiveWindow(), "GlobalAlloc failed.", "Error!", 16);
        return block;
    }

    //copy internal files to block
    data = dataStart;
    CopyMemory(block, data, internalSize);

     //jump internal files + null int + useMd5 byte
    data += internalSize + 5;

    //copy external files to block;
    if (writeExternalFiles(data, block + internalSize, searchPath, md5) == 0)
    {
        //error with external files
        MessageBox(GetActiveWindow(), "Copy external files failed.", "Error!", 16);
        return 0;
    }

    return block;
}

int getSearchPaths(char *searchPath[])
{
    //fills the searchPath array with paths to search for files
    //returns 0 on failure > 0 on success
    char *buffer;
    char *path;
    DWORD reglen;
    HKEY hKey;
    long ret;
    int i;
    //find location of dbpro dir
    buffer = malloc(1024);
    if (buffer == NULL)
    {
        return 0;
    }
    ret = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "SOFTWARE\\Dark Basic\\Dark Basic Pro\\", 0, KEY_QUERY_VALUE, &hKey);
    if (ret == ERROR_SUCCESS)
    {
        ret = RegQueryValueEx(hKey, "INSTALL-PATH", NULL, NULL, (LPBYTE) buffer, &reglen);
        if (ret != ERROR_SUCCESS)
        {
            //dbpro install dir not found
            free(buffer);
            buffer = NULL;
        }
    }
    RegCloseKey(hKey);
    for (i=0; i < SEARCHCOUNT; i++)
    {
        switch (i)
        {
            case 0:
                //program directory
                searchPath[i] = malloc(MAX_PATH);
                if (searchPath[i] == NULL)
                {
                    return 0;
                }
                GetModuleFileName(NULL, searchPath[i], MAX_PATH);
                //strip filename from path
                path = searchPath[i] + strlen(searchPath[i]);
                while (*path != '\\')
                    path --;
                *++path = 0;
            break;
            case 1:
                //plugins
                if (buffer == NULL)
                {
                    searchPath[i] = NULL;
                }
                else
                {
                    searchPath[i] = malloc(strlen(buffer)+21); //"\compiler\plugins\"
                    if (searchPath[i] == NULL)
                    {
                        return 0;
                    }
                    *searchPath[i] = 0;
                    sprintf(searchPath[i], "%s\\compiler\\plugins\\", buffer);
                }
            break;
            case 2:
                //plugins-user
                if (buffer == NULL)
                {
                    searchPath[i] = NULL;
                }
                else
                {
                    searchPath[i] = malloc(strlen(buffer)+25); //"\plugins-user\"
                    if (searchPath[i] == NULL)
                    {
                        return 0;
                    }
                    *searchPath[i] = 0;
                    sprintf(searchPath[i], "%s\\compiler\\plugins-user\\", buffer);
                }
            break;
            case 3:
                //effects
                if (buffer == NULL)
                {
                    searchPath[i] = NULL;
                }
                else
                {
                    searchPath[i] = malloc(strlen(buffer) + 20); //"\effects\"
                    if (searchPath[i] == NULL)
                    {
                        return 0;
                    }
                    *searchPath[i] = 0;
                    sprintf(searchPath[i], "%s\\compiler\\effects\\", buffer);
                }
            break;
        }
    }
    free(buffer);
    return 1;
}


BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            // attach to process
            // return FALSE to fail DLL load
            break;

        case DLL_PROCESS_DETACH:
            // detach from process
            break;

        case DLL_THREAD_ATTACH:
            // attach to thread
            break;

        case DLL_THREAD_DETACH:
            // detach from thread
            break;
    }
    return TRUE; // succesful
}
