/*
dark_explorer
Copyright (C) 2005,2006,2007,2008 the_winch

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/*
This dll handles calling functions in compress.dll to (de)compress data.
Since the exact compress.dll needed isn't known at compile time and
C# doesn't (easily) support late binding this dll is used.

This dll is also used to generate the dependancy list of a dbpro TPC dll
for the same late binding reason as above.
*/

#include <windows.h>
#include <stdio.h>

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

//magic "extra data" found at the end of dbpro exes
#define EXTRADATA_LENGTH 16
const char extraData[] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                     0x4E, 0x61, 0xBC, 0x00, 0x00, 0x00, 0x00, 0x00 };

void DLL_EXPORT compress(char *oldExeName, int exeSection, int extraDataSize, const char *newExeName, const char *compressDll)
{
    //compress uncompressed dbpro exe
    void* (*comp)(void*, int);
    FILE *oldExe = fopen(oldExeName, "rb"); //uncompressed exe
    FILE *newExe = fopen(newExeName, "wb"); //compressed exe
    FILE *compDll = fopen(compressDll, "rb");
    void *buffer;
    SIZE_T compSize;
    int dataSize, dllLen;
    HANDLE lib = LoadLibrary(compressDll);
    comp = (void* (*)(void*, int)) GetProcAddress(lib, "compress_block");

    //get uncompressed data size
    fseek(oldExe, 0, SEEK_END);
    dataSize = ftell(oldExe) - exeSection - extraDataSize;
    fseek(oldExe, 0, SEEK_SET);

    //write exe section
    if (exeSection > 0)
    {
        buffer = malloc(exeSection);
        fread(buffer, exeSection, 1, oldExe);
        fwrite(buffer, exeSection, 1, newExe);
        free(buffer);
    }

    //write compress.dll
    dllLen = 12;
    fwrite(&dllLen, 1, 4, newExe); //namelength
    fwrite("compress.dll", 1, 12, newExe); //name
    fseek(compDll, 0, SEEK_END);
    dllLen = ftell(compDll);
    fseek(compDll, 0, SEEK_SET);
    fwrite(&dllLen, 1, 4, newExe); //datalength
    buffer = malloc(dllLen);
    fread(buffer, dllLen, 1, compDll);
    fwrite(buffer, dllLen, 1, newExe);
    free(buffer);

    //load uncompressed data into buffer
    buffer = malloc(dataSize);
    fread(buffer, dataSize, 1, oldExe);

    //compress data
    void *data = comp(buffer, dataSize);
    free(buffer);
    data = GlobalLock((HGLOBAL) data);
    compSize = GlobalSize((HGLOBAL) data);

    //write compressed data
    fwrite(data, compSize, 1, newExe);
    GlobalUnlock((HGLOBAL) data);

    //write extra data if required
    if (exeSection > 0)
    {
        fwrite(extraData, EXTRADATA_LENGTH, 1, newExe);
        //write exeSection size
        fwrite(&exeSection, 4, 1, newExe);
    }

    FreeLibrary(lib);
    fclose(oldExe);
    fclose(newExe);
    fclose(compDll);
}

void DLL_EXPORT decompress(const char *oldExeName, int exeSection, int dataOffset, const char *newExeName, const char *compressDll)
{
    // decompress compressed dbpro exe
    void* (*decomp)(void*, int);
    FILE *oldExe = fopen(oldExeName, "rb"); //compressed exe
    FILE *newExe = fopen(newExeName, "wb"); //uncompressed exe
    HANDLE lib = LoadLibrary(compressDll);  //compress.dll from compressed exe
    SIZE_T decompSize;
    int dataSize;
    void *buffer;

    decomp = (void* (*)(void*, int)) GetProcAddress(lib, "decompress_block");

    //get compressed data size
    fseek(oldExe, 0, SEEK_END);
    dataSize = ftell(oldExe) - dataOffset;
    fseek(oldExe, 0, SEEK_SET);

    //write exe section
    if (exeSection > 0)
    {
        buffer = malloc(exeSection);
        fread(buffer, exeSection, 1, oldExe);
        fwrite(buffer, exeSection, 1, newExe);
        free(buffer);
    }

    //load compressed data into buffer;
    fseek(oldExe, dataOffset, SEEK_SET);
    buffer = malloc(dataSize);
    fread(buffer, dataSize, 1, oldExe);

    //decompress data
    void *data = decomp(buffer, dataSize);
    free(buffer);
    data = GlobalLock((HGLOBAL) data);
    decompSize = GlobalSize((HGLOBAL) data);

    //write decompressed data
    fwrite(data, decompSize, 1, newExe);
    GlobalUnlock((HGLOBAL) data);

    //write extra data if required
    if (exeSection > 0)
    {
        fwrite(extraData, EXTRADATA_LENGTH, 1, newExe);
        //write exeSection size
        fwrite(&exeSection, 4, 1, newExe);
    }

    FreeLibrary(lib);
    fclose(oldExe);
    fclose(newExe);
}

char* DLL_EXPORT getDepString(int num, char *fileName)
{
    const char* dep;
    char* dep_copy;
    const char* (*getDep)(int);
    HANDLE lib = LoadLibrary(fileName);
    getDep = (const char* (*)(int)) GetProcAddress(lib, "?GetDependencyID@@YAPBDH@Z");
    dep = getDep(num);
    dep_copy = malloc(strlen(dep));
    dep_copy[0] = 0;
    strcat(dep_copy, dep);
    FreeLibrary(lib);
    return dep_copy;
}

int DLL_EXPORT getDepCount(char *fileName)
{
    //get count of dependancies
    int count = 0;
    int (*depCount)(void);
    HANDLE lib = LoadLibrary(fileName);
    depCount = (int (*)(void)) GetProcAddress(lib, "?GetNumDependencies@@YAHXZ");
    if (depCount != NULL)
    {
        count = depCount();
    }
    FreeLibrary(lib);
    return count;
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

