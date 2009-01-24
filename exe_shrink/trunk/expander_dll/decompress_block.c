//
// © the_winch 2008
// Permission to copy, use, modify, sell and distribute this software is
// granted provided this notice appears un-modified in all copies.
// This software is provided as-is without express or implied warranty,
// and with no claim as to its suitability for any purpose.
//
// http://winch.pinkbile.com : thewinch@gmail.com

//contains functions used by decompress_block function

#include <windows.h>
#include <stdio.h>
#include <Wincrypt.h>

#define SEARCHCOUNT 4 //number of paths in search array

int internalFilesSize(void *data)
{
    //find size of all internal files
    int nameLen, dataLen;
    int *dataInt;
    void *dataStart = data;
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

int useMd5(void *data)
{
    //are external files useing checksums?
    char* dataByte;
    dataByte = data;
    return *dataByte;
}

int externalFilesSize(void *data, char *searchPath[], int md5)
{
    //get size of external files
    char *dataByte, *fullName;
    char name[255];
    int nameLen = 1;
    int i, total;
    DWORD fileSize;
    OFSTRUCT los;
    HFILE file = HFILE_ERROR;
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
            for (i = 0; i < SEARCHCOUNT; i++)
            {
                fullName = malloc(strlen(searchPath[i]) + strlen(name) + 1);
                if (fullName == NULL)
                {
                    return 0;
                }
                fullName[0] = 0;
                sprintf(fullName, "%s%s", searchPath[i], name);
                file = OpenFile(fullName, &los, OF_READ);
                free(fullName);
                if (file != HFILE_ERROR)
                {
                    //file found
                    break;
                }
            }
            if (file != HFILE_ERROR)
            {
                //file found
                fileSize = GetFileSize((HANDLE) file, NULL);
                total += fileSize + 8 + strlen(name); // filesize + nameLen + dataLen + name
                CloseHandle((HANDLE) file);
            }
            else
            {
                //file not found in searchPath[]
                dataByte = malloc(255);
                if (dataByte == NULL)
                {
                    return 0;
                }
                sprintf(dataByte, "%s not found in search paths.", name);
                MessageBox(GetActiveWindow(), dataByte, "Error!", 16);
                free(dataByte);
                return 0;
            }
        }
    }
    return total;
}

int writeExternalFiles(void *data, HGLOBAL block, char *searchPath[], int md5)
{
    //write exteral files to block
    //returns 0 on failure > 0 on success
    char *dataByte, *fullName, *md5FailMsg;
    char name[255];
    int nameLen = 1;
    int i;
    int pos;
    void *buffer;
    FILE *f_in = NULL;
    //md5 checksum stuff
    HCRYPTPROV cryptProv;
    HCRYPTHASH hash;
    DWORD hashLen = 16;
    unsigned char hashData[hashLen + 1];
    char md5SumReq[33];  //required md5
    md5SumReq[32] = 0;
    char md5SumFile[33]; //md5 of file
    if (md5 == 1)
    {
        CryptAcquireContext(&cryptProv, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT);
    }
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
            for (i = 0; i < SEARCHCOUNT; i++)
            {
                if (searchPath[i] != NULL)
                {
                    fullName = malloc(strlen(searchPath[i]) + strlen(name) + 1);
                    if (fullName == NULL)
                    {
                        return 0;
                    }
                    fullName[0] = 0;
                    sprintf(fullName, "%s%s", searchPath[i], name);
                    f_in = fopen(fullName, "rb");
                    if (f_in != NULL)
                    {
                        break;
                    }
                }
            }
            if (f_in != NULL)
            {
                //write file to block
                fseek(f_in, 0, SEEK_END);
                pos = ftell(f_in);
                fseek(f_in, 0, SEEK_SET);
                buffer = malloc(pos);
                if (buffer == NULL)
                {
                    return 0;
                }
                fread(buffer, pos, 1, f_in);
                fclose(f_in);
                if (md5 == 1)
                {
                    //check md5
                    CryptCreateHash(cryptProv, CALG_MD5, 0, 0, &hash);
                    CryptHashData(hash, buffer, pos, 0);
                    CryptGetHashParam(hash, HP_HASHVAL, hashData, &hashLen, 0);
                    CryptDestroyHash(hash);
                    for (i = 0; i < 16; i++)
                    {
                        sprintf(md5SumFile + i * 2, "%02x", hashData[i]);
                    }
                    if (strcmp(md5SumReq ,md5SumFile) != 0)
                    {
                        //checksums don't match !TODO!
                        md5FailMsg = malloc(strlen(name) + 47);
                        if (md5FailMsg == NULL)
                        {
                            return 0;
                        }
                        sprintf(md5FailMsg, "%s does not match md5 checksum.\nContinue anyway?", name);
                        i = MessageBox(GetActiveWindow(), md5FailMsg, "Error!", 20);
                        free(md5FailMsg);
                        if (i == 7)
                        {
                            //no button clicked
                            free(buffer);
                            return 0;
                        }
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
    if (md5 == 1)
    {
        CryptReleaseContext(cryptProv, 0);
    }
    return 1;
}
