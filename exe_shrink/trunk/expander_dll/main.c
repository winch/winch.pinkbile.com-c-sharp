#include <windows.h>
#include <stdio.h>

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT
#endif

#include "md5.h"

//functions
int getSearchPaths(char *SearchPath[], int count);
int internalFilesSize(void *data);
int externalFilesSize(void *data, char *searchPath[], int searchCount, int md5);
int writeExternalFiles(void *data, HGLOBAL block, char *searchPath[], \
                       int searchCount, int md5);
int useMd5(void *data);

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

HGLOBAL DLL_EXPORT decompress_block(void *data, DWORD dataSize)
{
    int searchCount = 3;           // number of paths in search array
    char *searchPath[searchCount]; // array of paths to search for files
    int internalSize, externalSize, md5;
    void *dataStart = data;

    HGLOBAL block = NULL;

    //fill searchPath array
    if (getSearchPaths(searchPath, searchCount) == 0)
    {
        //error filling serachPath array
        return;
    }

    //find size of internal files
    internalSize = internalFilesSize(data);
    //skip internal files an null int
    data += internalSize + 4;

    //use md5 checksums for external files?
    md5 = useMd5(data);
    data += 1;

    //find size of external files
    externalSize = externalFilesSize(data, searchPath, searchCount, md5);

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
    if (writeExternalFiles(data, block + internalSize, searchPath, searchCount, md5) == 0)
    {
        //error with external files
        MessageBox(GetActiveWindow(), "Copy external files failed.", "Error!", 16);
        return 0;
    }

    return block;
}

int writeExternalFiles(void *data, HGLOBAL block, char *searchPath[], \
                       int searchCount, int md5)
{
    //write exteral files to block
    //returns 0 on failure > 0 on success
    char *dataByte, *fullName, md5FailMsg;
    char name[255];
    char md5Sum[33];
    int nameLen = 1;
    int i;
    int pos;
    void *buffer;
    FILE *f_in;
    //md5 checksum stuff
    md5_state_t state;
	md5_byte_t digest[16];
	int di;
	char md5SumReq[33];  //required md5
	md5SumReq[32] = 0;
	char md5SumFile[33]; //md5 of file
    //loop through files
    while (nameLen > 0)
    {
        dataByte = data;
        nameLen = *dataByte;
        if (nameLen > 0)
        {
            data += 1;
            CopyMemory(name, data, nameLen);
            name[nameLen] = 0;
            data += nameLen;
            if (md5 == 1)
            {
                //read md5
                CopyMemory(md5SumReq, data, 32);
                data += 32;
            }
            //find file in search path
            for (i = 0; i < searchCount; i++)
            {
                fullName = malloc(strlen(searchPath[i]) + strlen(name) + 1);
                fullName[0] = 0;
                sprintf(fullName, "%s%s", searchPath[i], name);
                f_in = fopen(fullName, "rb");
                if (f_in != NULL)
                {
                    break;
                }
            }
            if (f_in != NULL)
            {
                //write file to block
                fseek(f_in, 0, SEEK_END);
                pos = ftell(f_in);
                fseek(f_in, 0, SEEK_SET);
                buffer = malloc(pos);
                fread(buffer, pos, 1, f_in);
                fclose(f_in);
                if (md5 == 1)
                {
                    //check md5
                    md5_init(&state);
                    md5_append(&state, (const md5_byte_t *)buffer, pos);
                    md5_finish(&state, digest);
                    for (di = 0; di < 16; ++di)
                    {
                        sprintf(md5SumFile + di * 2, "%02x", digest[di]);
                    }
                    if (strcmp(md5SumReq ,md5SumFile) != 0)
                    {
                        //checksums don't match !TODO!
                        //md5FailMsg = malloc(10);
                        //free(md5FailMsg);
                        free(buffer);
                        return 0;
                    }
                }
                //write name
                CopyMemory(block, &nameLen, 4);
                block += 4;
                CopyMemory(block, name, strlen(name));
                block += strlen(name);
                //write data
                CopyMemory(block, &pos, 4);
                block += 4;
                CopyMemory(block, buffer, pos);
                block += pos;
                free(buffer);
            }
            else
            {
                //file not found
                return 0;
            }
        }
    }
    return 1;
}

int externalFilesSize(void *data, char *searchPath[], int searchCount, int md5)
{
    //get size of external files
    char *dataByte, *fullName;
    char name[255];
    int nameLen = 1;
    int i, total;
    DWORD fileSize;
    OFSTRUCT los;
    HFILE file;
    total = 0;
    //loop through files
    while (nameLen > 0)
    {
        dataByte = data;
        nameLen = *dataByte;
        if (nameLen > 0)
        {
            data += 1;
            CopyMemory(name, data, nameLen);
            name[nameLen] = 0;
            data += nameLen;
            if (md5 == 1)
            {
                //skip file md5
                data += 32;
            }
            //find file in search path
            for (i = 0; i < searchCount; i++)
            {
                fullName = malloc(strlen(searchPath[i]) + strlen(name) + 1);
                fullName[0] = 0;
                sprintf(fullName, "%s%s", searchPath[i], name);
                file = OpenFile(fullName, &los, OF_READ);
                free(fullName);
                if (file != HFILE_ERROR)
                {
                    break;
                }
            }
            if (file != 0)
            {
                fileSize = GetFileSize((HANDLE) file, NULL);
                total += fileSize + 8 + strlen(name);
                CloseHandle((HANDLE) file);
            }
            else
            {
                //file not found in searchPath[]
                dataByte = malloc(255);
                sprintf(dataByte, "%s not found in search paths.", name);
                MessageBox(GetActiveWindow(), dataByte, "Error!", 16);
                free(dataByte);
                return 0;
            }
        }
    }
    return total;
}

int useMd5(void *data)
{
    //are external files useing checksums?
    char* dataByte;
    dataByte = data;
    return *dataByte;
}

int internalFilesSize(void *data)
{
    //write internal files to block
    int nameLen, dataLen;
    int *dataInt;
    void *dataStart = data;
    //find size of all internal files
    nameLen = 1;
    while (nameLen > 0)
    {
        //name
        dataInt = data;
        nameLen = *dataInt;
        if (nameLen > 0)
        {
            data += 4 + nameLen;
            //data
            dataInt = data;
            dataLen = *dataInt;
            data += 4 + dataLen;
        }
    }
    return data - dataStart;
}

int getSearchPaths(char *searchPath[], int count)
{
    //fills the searchPath array with paths to search for files
    //returns 0 on failure > 0 on success
    char *buffer;
    DWORD reglen;
    HKEY hKey;
    long ret;
    //find location of dbpro dir
    ret = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "SOFTWARE\\Dark Basic\\Dark Basic Pro\\", 0, KEY_QUERY_VALUE, &hKey);
    if (ret != ERROR_SUCCESS)
    {
        MessageBox(GetActiveWindow(), "DarkBASIC pro location not found", "Error!", 16);
        return 0;
    }
    buffer = malloc(1024);
    ret = RegQueryValueEx(hKey, "INSTALL-PATH", NULL, NULL, (LPBYTE) buffer, &reglen);
    if (ret != ERROR_SUCCESS)
    {
        MessageBox(GetActiveWindow(), "DarkBASIC pro location not found.", "Error!", 16);
        return 0;
    }
    RegCloseKey(hKey);
    switch (count)
    {
        case 0:
            //plugins
            searchPath[count] = malloc(strlen(buffer)+21); //"\compiler\plugins\"
            *searchPath[count] = 0;
            sprintf(searchPath[count], "%s\\compiler\\plugins\\", buffer);
        break;
        case 1:
            //plugins-user
            searchPath[count] = malloc(strlen(buffer)+25); //"\plugins-user\"
            *searchPath[count] = 0;
            sprintf(searchPath[count], "%s\\compiler\\plugins-user\\", buffer);
        break;
        case 2:
            //effects
            searchPath[count] = malloc(strlen(buffer) + 20); //"\effects\"
            *searchPath[count] = 0;
            sprintf(searchPath[count], "%s\\compiler\\effects\\", buffer);
        break;
    }
    free(buffer);
    //add extra search paths here
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
