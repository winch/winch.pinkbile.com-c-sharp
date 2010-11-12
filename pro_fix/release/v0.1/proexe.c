#include <stdio.h>
#include <stdlib.h>
#include <windows.h>

#include "main.h"
#include "proexe.h"

//size of fread/fwrite buffer
#define BUFFER_SIZE 4096

int exeSectionSize(FILE *fIn);
void restoreBackup(char* fileName, char* backupName);

void fixExe(char* fileName)
{
    //makes a backup of the exe then fixes it
    FILE *fIn, *fOut;
    int exeSection;
    long exeSize, written = 0;
    int read;
    char backupName[strlen(fileName) + 5];
    void *buffer;
    strcpy(backupName, fileName);
    strcat(backupName, ".old");

    //backup exe
    if (MoveFile(fileName, backupName) == FALSE)
    {
        errorMsg("Can't backup exe.");
        return;
    }

    if ((fIn = fopen(backupName, "rb")) == NULL)
    {
        restoreBackup(fileName, backupName);
        errorMsg("Can't open backup for reading.");
        return;
    }
    if ((fOut = fopen(fileName, "wb")) == NULL)
    {
        fclose(fIn);
        restoreBackup(fileName, backupName);
        errorMsg("Can't open file for writing");
        return;
    }

    //find size of exe
    fseek(fIn, 0, SEEK_END);
    exeSize = ftell(fIn);
    fseek(fIn, 0, SEEK_SET);

    //find size of exe section
    exeSection = exeSectionSize(fIn);

    if (exeSection == -1)
    {
        //bad exe
        fclose(fIn);
        fclose(fOut);
        restoreBackup(fileName, backupName);
        return;
    }

    if (exeSection == exeSize)
    {
        //exe has no appended data
        fclose(fIn);
        fclose(fOut);
        restoreBackup(fileName, backupName);
        errorMsg("Exe has no appended data.");
    }

    buffer = malloc(BUFFER_SIZE);
    if (buffer == NULL)
    {
        fclose(fIn);
        fclose(fOut);
        restoreBackup(fileName, backupName);
        errorMsg("malloc failed.");
    }

    //copy backup file to new exe except for last 4 bytes
    while ((read = fread(buffer, 1, BUFFER_SIZE, fIn)))
    {
        if (written + read >= exeSize - 4)
        {
            read = exeSize - 4 - written;
        }
        if (read > 0)
        {
            fwrite(buffer, 1, read, fOut);
            written += read;
        }
    }
    //write size of exe section to end of exe
    fwrite(&exeSection, 4, 1, fOut);

    free(buffer);
    fclose(fIn);
    fclose(fOut);
}

int exeSectionSize(FILE *fIn)
{
    //returns size of exe section or -1 on error
    char validSig[] = "MZ";
    char fileSig[3];
    fileSig[2] = 0;
    int e_lfannew, pos, i , size;
    short numberOfSections;

    //check exe sig
    fread(fileSig, 2, 1, fIn);
    if (strcmp(validSig, fileSig) != 0)
    {
        errorMsg("Input file is not an exe.");
        return -1;
    }

    //skip dos stub
    fseek(fIn, 60, SEEK_SET);
    fread(&e_lfannew, 1, 4, fIn);
    fseek(fIn, e_lfannew + 6, SEEK_SET);

    //IMAGE_FILE_HEADER
    fread(&numberOfSections, 1, 2, fIn);
    fseek(fIn, 240, SEEK_CUR);
    //end of IMAGE_OPTIONAL_HEADER
    //section directories
    for (i = 0; i < numberOfSections; i++)
    {
        fseek(fIn, 16, SEEK_CUR);
        fread(&size, 1, 4, fIn);
        fread(&pos, 1, 4, fIn);
        fseek(fIn, 16, SEEK_CUR);
    }
    //end of section directories

    //seek back to start of file
    fseek(fIn, 0, SEEK_SET);
    return pos + size;
}

void restoreBackup(char* fileName, char* backupName)
{
    //move backup file back to original location
    DeleteFile(fileName);
    MoveFile(backupName, fileName);
}
