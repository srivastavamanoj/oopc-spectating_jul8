
/*
It is a utility to encode three video streams with FFMPEG. To use this utility, please seperate arguments of three videos with '-second' and 'third' keywords. 
For example, following command can encode two video streams,
 -r 30 -f image2 -start_number 0 -i image_%04d.png -vframes 30 -vcodec libx264 -b:v 60M -pix_fmt yuv420p output1 -second -r 30 -f image2 -start_number 0 -i image_%04d.png -vframes 30 -vcodec libx264 -b:v 60M -pix_fmt yuv420p output2
*/

#include <iostream>
#include <windows.h>

#define EXE_FILE_NAME "ffmpeg.exe"

const int  MAX_INTERNAL_ARGC = 64;
const int  DEFAULT_FRAME_RATE = 60;
const int  MAX_FILE_NAME_SIZE = 256;
const int  MAX_FRAME_NAME_SIZE = 256;
const int  MAX_COMMAND_LENGTH = 2048;

static int processVideo(int argc, char ** argv, int loopCount)
{
	int inStartIdx;
	int argvReplaceCount;
	int inputFileCount;
	int frameRate;

	char frameStartNumber[MAX_FRAME_NAME_SIZE];
	char outputName[MAX_FILE_NAME_SIZE];
	char inputName[MAX_FILE_NAME_SIZE];
	char command[MAX_COMMAND_LENGTH];
	FILE *pTestInput;

	//get frame rate
	frameRate = DEFAULT_FRAME_RATE;
	for (int k = 0; k < argc - 1; k++) {
		if (strcmp(argv[k], "-r") == 0) {
			frameRate = atoi(argv[k + 1]);
			break;
		}
	}

	//check input files
	inStartIdx = loopCount * frameRate;
	inputFileCount = 0;
	for (int k = 0; k < argc - 1; k++) {
		if (strcmp(argv[k], "-i") == 0) {
			for (int m = 0; m < frameRate; m++) {
				sprintf_s<sizeof(inputName)>(inputName, argv[k + 1], inStartIdx + m);
				fopen_s(&pTestInput, inputName, "r");
				if (pTestInput != NULL) {
					fclose(pTestInput);
					inputFileCount++;
				}
			}
		}
	}

#if 1
	if (inputFileCount != frameRate) {
		//not enough input files
		Sleep(500);
		return 0;
	}
#else
	for (int k = 0; k < argc - 1; k++) {
		if (strcmp(argv[k], "-i") == 0) {
			strncpy_s(inputName, "image_%04d.png", sizeof(inputName));
			argv[k + 1] = inputName;
			break;
		}
	}
#endif

	//modify the arguments according to the loop count
	sprintf_s<sizeof(frameStartNumber)>(frameStartNumber, "%d", inStartIdx);
	sprintf_s<sizeof(outputName)>(outputName, "%s-%06d.mp4", argv[argc - 1], loopCount);

	//replace argv with internal buffers
	argvReplaceCount = 0;

	for (int k = 0; k < argc - 1; k++) {
		if (strcmp(argv[k], "-start_number") == 0) {
			argv[k + 1] = frameStartNumber;
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

	for (int k = 0; k < argc-1; k++) {
		strcat_s<sizeof(command)>(command, argv[k]);
		strcat_s<sizeof(command)>(command, " ");
	}

	//the last one is output file name
	strcat_s<sizeof(command)>(command, outputName);

	printf("run: %s\n", command);
	if (0 != system(command)) {
		return -1;
	}

#ifndef _DEBUG
	for (int k = 0; k < argc - 1; k++) {
		if (strcmp(argv[k], "-i") == 0) {
			for (int m = 0; m < frameRate; m++) {
				sprintf_s<sizeof(inputName)>(inputName, argv[k + 1], inStartIdx + m);
				sprintf_s<sizeof(command)>(command, "del %s", inputName);
				system(command);
			}
		}
	}
#endif

	return 1;
}

typedef enum videoModeType {
	FIRST, SECOND, THIRD
}videoModeType;

int main(int argc, char **argv)
{
	videoModeType mode;
	int loopCount;
	int firstArgc;
	int secondArgc;
	int thirdArgc;
	char *firstArgv[MAX_INTERNAL_ARGC];
	char *secondArgv[MAX_INTERNAL_ARGC];
	char *thirdArgv[MAX_INTERNAL_ARGC];

	//copy argumants
	firstArgc = 0;
	secondArgc = 0;
	thirdArgc = 0;
	mode = FIRST;

	for (int k = 1; k < argc; k++) {
		if (strcmp(argv[k], "-second") == 0) {
			mode = SECOND;
			continue;
		}
		else if (strcmp(argv[k], "-third") == 0) {
			mode = THIRD;
			continue;
		}

		switch (mode) {
		case FIRST:
			if (firstArgc >= MAX_INTERNAL_ARGC) {
				printf("-E - too many first video argumants\n");
			}

			firstArgv[firstArgc] = argv[k];
			firstArgc++;
			break;
		case SECOND:
			if (secondArgc >= MAX_INTERNAL_ARGC) {
				printf("-E - too many second video argumants\n");
			}

			secondArgv[secondArgc] = argv[k];
			secondArgc++;
			break;
		case THIRD:
			if (thirdArgc >= MAX_INTERNAL_ARGC) {
				printf("-E - too many second video argumants\n");
			}

			thirdArgv[thirdArgc] = argv[k];
			thirdArgc++;
			break;
		}
	}

	loopCount = 0;
	do {
		int ret;

		ret = processVideo(firstArgc, firstArgv, loopCount);
		if (0 == ret) {
			continue; //no stream encoded,
		}
		else if (0 > ret) {
			break;  //error
		}

		if (mode >= SECOND) {
			ret = processVideo(secondArgc, secondArgv, loopCount);
			if (0 == ret) {
				continue; //no stream encoded,
			}
			else if (0 > ret) {
				break;  //error
			}
		}

		if (mode >= THIRD) {
			ret = processVideo(thirdArgc, thirdArgv, loopCount);
			if (0 == ret) {
				continue; //no stream encoded,
			}
			else if (0 > ret) {
				break;  //error
			}
		}
		loopCount++;
	} while (1);

	return 0;
}
