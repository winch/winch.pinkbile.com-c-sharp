/*
© the_winch 2005
Permission to copy, use, modify, sell and distribute this software is
granted provided this notice appears un-modified in all copies.
This software is provided as-is without express or implied warranty,
and with no claim as to its suitability for any purpose.

http://winch.pinkbile.com
dbp@pinkbile.com
*/

#include <windows.h>
#include <stdio.h>
#include "md5.h"

//functions
char *getName(FILE *file);
int findHead(FILE *file);
int getByte(FILE *file);

//globals
FILE *f_in, *fopen();  // input file
FILE *f_out, *fopen(); // output file
FILE *f_ext, *fopen(); // external file

int WINAPI WinMain (HINSTANCE hInstance, HINSTANCE hPrevInstance, PSTR CmdLine, int CmdShow)
{
    char *buffer,*name,*tempname;
    char *outputname;   //name of output file
    char *plugins;      //compiler\plugins dir
    char *plugins_user; //compiler\plugins-user dir
    char *effects;      //compiler\effects dir
    DWORD reglen;
    int i,length,datalength,done,checksums;
    int foot = 0; // position of <Extra Data>
    int footlength = 0; //length of <Extra Data>
    int numfiles; //number of files
    
    //md5 checksum stuff
    md5_state_t state;
	md5_byte_t digest[16];
	int di;
	char hex_output[16*2 + 1]; //generated hash
	char hex_input[16*2 + 1];  //hash read from input file
	hex_input[16*2] = 0x0;
    
    //find location of dbpro dir
    HKEY hKey;
    long ret;
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
        MessageBox(GetActiveWindow(), "DarkBASIC pro location not found", "Error!", 16);
        return 0;
    }
    RegCloseKey(hKey);
    //plugins
    plugins = malloc(strlen(buffer)+21); //"\compiler\plugins\"
    *plugins = 0x0;
    strcat(plugins,buffer);
    strcat(plugins,"\\compiler\\plugins\\");
    //plugins-user
    plugins_user = malloc(strlen(buffer)+25); //"\plugins-user\"
    *plugins_user = 0x0;
    strcat(plugins_user, buffer);
    strcat(plugins_user, "\\compiler\\plugins-user\\");
    //effects
    effects = malloc(strlen(buffer) + 20); //"\effects\"
    *effects = 0x0;
    strcat(effects,buffer);
    strcat(effects, "\\compiler\\effects\\");
    free(buffer);
    
    //open input file
    buffer = malloc(512);
    GetModuleFileName(NULL, buffer, 512);
    if ((f_in = fopen(buffer,"rb")) == NULL)
    //if ((f_in = fopen("build.txt","rb")) == NULL)
    {
        MessageBox(GetActiveWindow(), "Can't open input file", "Error!", 16);
        return 0;
    }
    free(buffer);
    int dataStart = findHead(f_in); //position in file patch data starts
    fseek(f_in, dataStart, SEEK_SET);
    
    //open output file
    outputname = getName(f_in);
    if ((f_out = fopen(outputname,"wb")) == NULL)
    {
        MessageBox(GetActiveWindow(), "Can't open output file", "Error!", 16);
        return 0;
    }
    
    //get num of internal files
    numfiles = getByte(f_in);
    //write internal files to output
    for (i=0; i<numfiles; i++)
    {
        name = getName(f_in);
        //get filedata length
        fread(&datalength, 1, 4, f_in);
        //write name if required
        if ( strcmp(name, "<Standard Head>") != 0 && strcmp(name, "<Extra Data>") != 0)
        {
            //write name
            length = strlen(name);
            fwrite(&length,1,4,f_out);
            fwrite(name,1,strlen(name),f_out);
            //write length
            fwrite(&datalength,1,4,f_out);
        }
        //write file data
        if (strcmp(name, "<Extra Data>") == 0)
        {
            //extra data, save start and length
            foot = ftell(f_in);
            footlength = datalength;
            fseek(f_in,datalength,SEEK_CUR);
        }
        else
        {
            buffer = malloc(datalength);
            fread(buffer,1,datalength,f_in);
            fwrite(buffer,1,datalength,f_out);
            free(buffer);
        }
        free(name);
    }
    
    //using md5 checksum strings?
    checksums = getByte(f_in);
    
    //get num of external files
    numfiles = getByte(f_in);
    
    //write external files to output
    for (i=0; i<numfiles; i++)
    {
        name = getName(f_in);
        //read checksum if required
        if (checksums == 1)
        {
            //read checksum
            fread(hex_input,1,32,f_in);
        }
        done = 0;
        //try to locate the file
        //plugins
        tempname = malloc(strlen(plugins) + strlen(name));
        *tempname = 0x0;
        strcat(tempname,plugins);
        strcat(tempname,name);
        if (((f_ext = fopen(tempname,"rb")) != NULL))
        {
            done = 1;
        }
        free(tempname);
        //plugins-user
        if (done == 0)
        {
            tempname = malloc(strlen(plugins_user) + strlen(name));
            *tempname = 0x0;
            strcat(tempname,plugins_user);
            strcat(tempname,name);
            if (((f_ext = fopen(tempname,"rb")) != NULL))
            {
                done = 1;
            }
            free(tempname);
        }
        //effects
        if (done == 0)
        {
            tempname = malloc(strlen(effects) + strlen(name));
            *tempname = 0x0;
            strcat(tempname, effects);
            strcat(tempname, name);
            if (((f_ext = fopen(tempname,"rb")) != NULL))
            {
                done = 1;
            }
            free(tempname);
        }
        //check if we found file
        if (done == 0)
        {
            //file not found, warn user
            MessageBox(GetActiveWindow(),"File not found","Error!",16);
        }
        else
        {            
            //copy the file to f_out
            //write name
            datalength = strlen(name);
            fwrite(&datalength,1,4,f_out);
            fwrite(name,1,datalength,f_out);
            //write data
            fseek(f_ext,0,SEEK_END);
            length = ftell(f_ext);
            fseek(f_ext,0,SEEK_SET);
            //length
            fwrite(&length,1,4,f_out);
            //data
            buffer = malloc(length);
            fread(buffer,1,length,f_ext);
            //check file matches checksum if required
            if (checksums == 1)
            {
                md5_init(&state);
               	md5_append(&state, (const md5_byte_t *)buffer, length);
                md5_finish(&state, digest);
                for (di = 0; di < 16; ++di)
                {
                    sprintf(hex_output + di * 2, "%02x", digest[di]);
                }
                //check checksums match
                if (strcmp(hex_input,hex_output) != 0)
                {
                    //No match
                    char *temp = malloc(50+strlen(name));
                    *temp = 0x0;
                    strcat(temp,name);
                    strcat(temp,"\ndoes not match checksum\nContinue anyway?");
                    int message_result = MessageBox(GetActiveWindow(),temp,"Error",20);
                    free(temp);
                    if (message_result == 7)
                    {
                        //no button clicked, clean up and exit
                        fclose(f_ext);
                        fclose(f_in);
                        fclose(f_out);
                        DeleteFile(outputname);
                        return 0;
                    }
                }
            }
            fwrite(buffer,1,length,f_out);
            free(buffer);
            fclose(f_ext);
        }
        free(name);
    }
    
    //write foot if required
    if (footlength != 0)
    {
        fseek(f_in,foot,SEEK_SET);
        buffer = malloc(footlength);
        fread(buffer,1,footlength,f_in);
        fwrite(buffer,1,footlength,f_out);
        free(buffer);
    }
    
    fclose(f_in);
    fclose(f_out);
    
    //run exe and delete it when finished
    STARTUPINFO si;
    PROCESS_INFORMATION pi;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);
    ZeroMemory(&pi, sizeof(pi));
    if (CreateProcess(outputname, NULL, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
    {
         //wait for program to exit
         WaitForSingleObject(pi.hProcess, INFINITE);
         CloseHandle(pi.hProcess);
         CloseHandle(pi.hThread);
    }
    else
    {
        MessageBox(GetActiveWindow(),"Failed to run exe!","Error!",16);
    }
    Sleep(100);
    DeleteFile(outputname);
    return 0;
}

//functions
char *getName(FILE *file)
{
    //reads a name from file
    int len,i;
    char *str,*pos;
    //get length
    fread(&len,1,4,file);
    //get name
    str = calloc(len+1,1);
    fread(str,1,len,file);
    return(str);
}

int findHead(FILE *file)
{
  //finds the end of exe which is start of attache files in pro exe and patch info in patch
  //skip dos stub
  fseek(file, 60, SEEK_SET);
  int e_lfannew;
  fread(&e_lfannew, 1, 4, file);
  fseek(file, e_lfannew + 6, SEEK_SET);
  //IMAGE_FILE_HEADER
  short NumberOfSections;
  fread(&NumberOfSections, 1, 2, file);
  fseek(file, 240, SEEK_CUR);
  //end of IMAGE_OPTIONAL_HEADER
  //section directories
  int Size = 0; // size of section
  int Pos = 0;  // position of section
  int i;
  for (i=0; i<NumberOfSections; i++)
  {
      fseek(file, 16, SEEK_CUR);
      fread(&Size, 1, 4, file);
      fread(&Pos, 1, 4, file);
      fseek(file, 16, SEEK_CUR);
  }
  //end of section directories
  return (Pos + Size);
}

int getByte(FILE *file)
{
  //reads a byte from file
  unsigned char c;
  fread(&c,1,1,file);
  return((int) c);
}

