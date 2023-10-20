#include <stdlib.h>
#include <stdio.h>
#include "../shared/headers/readFromFile.h"

int GetScore(char i, char u);
int GetChoiceScore(char c);
int GetWinScore(char i, char u);
char GetChosenMove(char u, char LDW);


int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        printf("Please provide a path to the input text file (*.txt)");
        return 0;
    }

    int chunkLength = 5;
    char chunk[chunkLength];
    memset(chunk, 0, chunkLength);

    long totalScore1 = 0;
    long totalScore2 = 0;
    while (ReadLine(argv[1], chunk, chunkLength) == 0)
    {
        totalScore1 += GetScore(chunk[0], chunk[2]);

        char chosenMove = GetChosenMove(chunk[0], chunk[2]);
        totalScore2 += GetScore(chunk[0], chosenMove);
    }

    printf("Total score part 1: %i\n", totalScore1);
    printf("Total score part 2: %i", totalScore2);

    return 0;
}

int GetScore(char u, char i)
{
    int choiceScore = GetChoiceScore(i);
    int winScore = GetWinScore(i, u);

    return choiceScore + winScore; 
}

int GetChoiceScore(char c)
{
    switch (c)
    {
        case 'X':
            return 1;
        case 'Y':
            return 2;
        case 'Z':
            return 3;
        
        default:
            puts("Invalid choiceScore");
            return 0;
    }
}

char GetChosenMove(char u, char LDW)
{
    /*
        LDW: Loss Draw Win
        X = Loss
        Y = Draw 
        Z = Win
    */
    switch (u)
    {
    case 'A':
        switch (LDW)
        {
        case 'X':
            return 'Z';
        
        case 'Y':
            return 'X';

        case 'Z':
            return 'Y';

        default:
            break;
        }
        break;

    case 'B':
        return LDW;

    case 'C':
        switch (LDW)
        {
        case 'X':
            return 'Y';
        
        case 'Y':
            return 'Z';

        case 'Z':
            return 'X';
        
        default:
            break;
        }
    
    default:
        return 0;
    }
}

int GetWinScore(char i, char u)
{
    // A, X = rock
    // B, Y = paper
    // C, Z = scissors

    switch (i)
    {
        case 'X':
            switch (u)
            {
                case 'A':
                    return 3;
                case 'C':
                    return 6;
                default:
                    break;
            }
            break;

        case 'Y':
            switch (u)
            {
                case 'A':
                    return 6;
                case 'B':
                    return 3;
                default:
                    break;
            }
            break;

        case 'Z':
            switch (u)
            {
                case 'B':
                    return 6;
                case 'C':
                    return 3;
                default:
                    break;
            }
            break;

        default:
            break;

    }
    return 0;
}