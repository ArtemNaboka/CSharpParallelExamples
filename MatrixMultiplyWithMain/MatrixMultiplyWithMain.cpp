// MatrixMultiplyWithMain.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <omp.h>
#include <intrin.h>
#include <cstdlib>
#include <float.h>
#include <time.h>

// Ініціалізація матриць
void InitMatrix(double** m, size_t n, bool useDefaultValue = false, double defaultValue = 0)
{
	for (size_t i = 0; i < n; i++)
	{
		m[i] = new double[n];
		for (size_t j = 0; j < n; j++)
		{
			m[i][j] = useDefaultValue ? defaultValue : rand();
		}
	}
}

// Перевірка матриць на еквівалентність
bool AreMatricesEqual(double** m1, double** m2, size_t n)
{
	for (size_t i = 0; i < n; i++)
	{
		for (size_t j = 0; j < n; j++)
		{
			if (m1[i][j] != m2[i][j])
				return false;
		}
	}

	return true;
}

// Послідовний алгоритм множення
void MultiplyCPP(double** m1, double** m2, double** res, size_t n)
{
	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < n; j++)
		{
			double temp = m1[i][j];
			for (int k = 0; k < n; k++)
			{
				res[i][k] += temp * m2[j][k];
			}
		}
	}
}

// Паралельний алгоритм множення
void MultiplyCPPOMP(double** m1, double** m2, double** res, size_t n)
{
	#pragma omp parallel for
	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < n; j++)
		{
			double temp = m1[i][j];
			for (int k = 0; k < n; k++)
			{
				res[i][k] += temp * m2[j][k];
			}
		}
	}
}

int main()
{
	srand(time(NULL));
	double start, finish, minTime, minTimeOMP;
	minTime = minTimeOMP = DBL_MAX;
	const int tryCount = 3;

	const size_t length = 2048;
	double** m1 = new double*[length];
	double** m2 = new double*[length];
	// Результат при послідовному множенні
	double** res = new double*[length];
	// Результат при паралельному множенні
	double** resOMP = new double*[length];
	InitMatrix(m1, length);
	InitMatrix(m2, length);
	InitMatrix(res, length, true, 0);
	InitMatrix(resOMP, length, true, 0);

	for (size_t i = 0; i < tryCount; i++)
	{
		start = omp_get_wtime();
		MultiplyCPP(m1, m2, res, length);
		finish = omp_get_wtime();
		if (finish - start < minTime)
			minTime = finish - start;

		start = omp_get_wtime();
		MultiplyCPPOMP(m1, m2, resOMP, length);
		finish = omp_get_wtime();
		if (finish - start < minTimeOMP)
			minTimeOMP = finish - start;
	}

	// Перевірка результатів
	if (!AreMatricesEqual(res, resOMP, length))
	{
		printf("ALARM!! Different results in Sequence and Parallel multiply\n");
	}

	printf("Min time seq: %lg secs\n", minTime);
	printf("Min time OMP: %lg secs\n", minTimeOMP);


	for (size_t i = 0; i < length; i++)
	{
		delete[] m1[i];
		delete[] m2[i];
		delete[] res[i];
		delete[] resOMP[i];
	}

	delete[] m1;
	delete[] m2;
	delete[] res;
	delete[] resOMP;

    return 0;
}

