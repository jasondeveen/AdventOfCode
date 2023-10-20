#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "../shared/headers/readFromFile.h"

int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        printf("Please provide a path to the input text file (*.txt)");
        return 0;
    }

    int elvesCalories[1000] = {0};
    int chunkLength = 10;
    char chunk[chunkLength];
    memset(chunk, 0, chunkLength);

    int linesIndex = 0;
    while (ReadLine(argv[1], chunk, chunkLength) == 0)
    {
        if (chunk[0] == '\n')
        {
            linesIndex++;
        }
        else
        {
            elvesCalories[linesIndex] += atoi(chunk);
        }
    }

    int max1 = 0;
    int max2 = 0;
    int max3 = 0;
    for (size_t i = 0; i <= linesIndex; i++)
    {
        if (elvesCalories[i] > max1)
        {
            max2 = max1;
            max1 = elvesCalories[i];
        }
        else if (elvesCalories[i] > max2)
        {
            max3 = max2;
            max2 = elvesCalories[i];
        }
        else if (elvesCalories[i] > max3)
        {
            max3 = elvesCalories[i];
        }
    }

    int top3Total = max1 + max2 + max3;

    printf("Largest carried amount of calories: \nFirst: %i\nSecond: %d\nThird: %d\ntogether they are carrying %d calories\n", max1, max2, max3, top3Total);
}
