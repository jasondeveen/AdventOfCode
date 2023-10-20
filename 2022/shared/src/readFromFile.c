#include "../headers/readFromFile.h"
#include <errno.h>

int ReadLine(char path[], char line[], int lengthOfLine)
{
    if(lengthOfLine == 0){
        puts("Very funny");
        return -1;
    }

    if (path == NULL)
    {
        puts("Please provide a path to the input text file (*.txt)");
        return -2;
    }

    static FILE* fp;
    static char* chosenPath = "";
    if (strcmp(chosenPath, path) != 0)
    {
        errno = 0;
        chosenPath = path;
        fp = fopen(path, "r");
        if (fp == NULL)
        {
            perror("Unable to open specified file.");
            fclose(fp);
            return -3;
        }
    }

    if (fgets(line, lengthOfLine, fp) != NULL)
    {
        return 0;
    }
    else
    {
        fclose(fp);
        return 1;
    }
}
