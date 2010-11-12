#include <stdio.h>
#include <stdlib.h>
#include <windows.h>

#include "proexe.h"

void getFileFromDialog(char* fileName);

int main(int argc, char *argv[])
{
    char fileName[MAX_PATH];
    fileName[0] = 0;
    if (argc > 1)
    {
        //get filename from command line args
        strcpy(fileName, argv[1]);
    }

    if (fileName[0] == 0)
    {
        //get filename from open file dialog
        getFileFromDialog(fileName);
    }

    if (fileName[0] == 0)
        return 0;

    fixExe(fileName);

	return 0;
}

void getFileFromDialog(char* fileName)
{
    //show open file dialog
    OPENFILENAME ofn;
    ZeroMemory(&ofn, sizeof(OPENFILENAME));
    ofn.lStructSize = sizeof(ofn);
    ofn.hwndOwner = GetActiveWindow();
    ofn.lpstrFile = 0;
    ofn.lpstrTitle = "Select dbpro/fpsc exe to fix";
    ofn.lpstrFilter = "Exe Files (*.exe)\0*.exe\0All Files (*.*)\0*.*\0\0";
    ofn.nMaxFile = MAX_PATH;
    ofn.nMaxFileTitle = MAX_PATH;
    ofn.lpstrFile = fileName;
    ofn.Flags = OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST;
    GetOpenFileName(&ofn);
}

void errorMsg(char* errorTxt)
{
    //show messagebox with errorTxt
    MessageBox(GetActiveWindow(), errorTxt, "Error!", MB_ICONERROR);
}
