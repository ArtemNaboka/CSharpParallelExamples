// MatrixMultiplyCPP.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <omp.h>
#include <intrin.h>


__declspec(dllexport)
void MultiplyCPP(double** m1, double** m2, double** res, size_t n)
{
	for (size_t i = 0; i < n; i++)
	{
		for (size_t j = 0; j < n; j++)
		{
			double temp = m1[i][j];
			for (size_t k = 0; k < n; k++)
			{
				res[i][k] += temp * m2[j][k];
			}
		}
	}
}