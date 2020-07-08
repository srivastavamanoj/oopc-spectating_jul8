// ffmpeg_caller.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <windows.h>

#define EXE_FILE_NAME "ffmpeg.exe"

const int MAX_INTERNAL_ARGC = 64;
const int  FRAME_RATE = 60;
const int  MAX_FILE_NAME_SIZE = 256;
const int  MAX_FRAME_INDEX_DIGITS = 20;
const int  MAX_COMMAND_LENGTH = 2048;

int main(int argc, char **argv)
{
	int inStartIdx;
	int loopCount;
	int argvReplaceCount;
	int inputFileCount;

	char *internalArgv[MAX_INTERNAL_ARGC];
	char frameStartNumber[MAX_FRAME_INDEX_DIGITS];
	char outputName[MAX_FILE_NAME_SIZE];
	char inputName[MAX_FILE_NAME_SIZE];
	char command[MAX_COMMAND_LENGTH];

	FILE *pTestInput;

	//copy argumants
	for (int k = 0; k < argc; k++) {
		internalArgv[k] = argv[k];
	}

	loopCount = 0;
	do {
		//check input files
		inStartIdx = loopCount * FRAME_RATE;
		inputFileCount = 0;
		for (int k = 0; k < argc - 1; k++) {
			if (strcmp(internalArgv[k], "-i") == 0) {
				for (int m = 0; m < FRAME_RATE; m++) {
					sprintf_s<sizeof(inputName)>(inputName, argv[k+1], inStartIdx + m);
					fopen_s(&pTestInput, inputName, "r");
					if (pTestInput != NULL) {
						fclose(pTestInput);
						inputFileCount++;
					}
				}
			}
		}
		if (inputFileCount != FRAME_RATE) {
			//not enough input files
			Sleep(500);
			continue;
		}

		//modify the arguments according to the loop count
		sprintf_s<sizeof(frameStartNumber)>(frameStartNumber, "%d", inStartIdx);
		sprintf_s<sizeof(outputName)>(outputName, "%s-%06d.mp4", argv[argc-1], loopCount);
		loopCount++;

		//replace argv with internal buffers
		internalArgv[argc - 1] = outputName;
		argvReplaceCount = 0;

		for (int k = 0; k < argc - 1; k++) {
			if (strcmp(internalArgv[k], "-start_number") == 0) {
				internalArgv[k + 1] = frameStartNumber;
				argvReplaceCount++;
				break;
			}
		}

		if (argvReplaceCount != 1) {
			printf("please change your command line, it should be:\n");
			printf("ffmpeg -r 30 -f image2 -s 1920x1080 -start_number 1 -i pic%%06d.png -vframes 30 -vcodec libx264 -crf 25  -pix_fmt yuv420p output_name\n");
			return -1;
		}

		//run the command
		strcpy_s<sizeof(command)>(command, EXE_FILE_NAME);
		strcat_s<sizeof(command)>(command, " ");

		for (int k = 1; k < argc; k++) {
			strcat_s<sizeof(command)>(command, internalArgv[k]);
			strcat_s<sizeof(command)>(command, " ");
		}

		printf("run: %s\n", command);
		if (0 != system(command)) {
			break;
		}
	} while (1);

	return 0;
}
