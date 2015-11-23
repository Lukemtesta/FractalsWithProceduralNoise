using UnityEngine;
using System.Collections;

public delegate float NoiseMethod (Vector3 point, float frequency);

public static class ProceduralNoise 
{
	private static int[] hash = {
		151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
		140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
		247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
		57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
		74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
		60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
		65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
		200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
		52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
		207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
		119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
		129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
		218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
		81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
		184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

		151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
		140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
		247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
		57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
		74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
		60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
		65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
		200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
		52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
		207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
		119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
		129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
		218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
		81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
		184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
	};

	const int hash_mask_size = 511;

	public static NoiseMethod[] FlatValueNoise = {	HashSmoothValue1D,
													HashSmoothValue2D
													};

	private static float FifthOrderSmoothing(float t)
	{
		return Mathf.Pow(t,3) + (t * (t * 6f - 15f) + 10f);
	}

	// Downsamples value by a factor of frequency saturating max resolution
	// resulting in lattice changes. Assigns random value using downsampled
	// value as hash index.
	public static float HashFlatValue1D(Vector3 i_uv, float frequency = 1.0f)
	{
		i_uv *= frequency;

		int i = Mathf.FloorToInt(i_uv.x) & hash_mask_size;
		return ((float)hash[i]) / (float)hash_mask_size;
	}

	public static float HashFlatValue2D(Vector3 i_uv, float frequency = 1.0f)
	{
		// apply downsampling
		i_uv *= frequency;

		// map resolution to hash index (bit mask) then normalise max to 1
		int i = Mathf.FloorToInt(i_uv.x) & hash_mask_size;
		int j = Mathf.FloorToInt(i_uv.y) & hash_mask_size;

		return ((float)hash[hash[i] + j]) / (float)hash_mask_size;
	}

	// Applies a 5th order polynomial to interpolate between 2 values per lattice
	public static float HashSmoothValue1D(Vector3 i_uv, float frequency = 1.0f)
	{
		i_uv *= frequency;

		// Assign two values per lattice
		float t = i_uv.x - Mathf.FloorToInt(i_uv.x);

		int i = Mathf.FloorToInt(i_uv.x) & hash_mask_size;
		int j = i + 1;

		i = hash[i];
		j = hash[j];

		t = FifthOrderSmoothing (t);
		return Mathf.Lerp (i,j,t) / (float)hash_mask_size;
	}
	
	public static float HashSmoothValue2D(Vector3 i_uv, float frequency = 1.0f)
	{
		// apply downsampling
		i_uv *= frequency;

		float tx = i_uv.x - Mathf.FloorToInt(i_uv.x);
		float ty = i_uv.y - Mathf.FloorToInt(i_uv.y);

		// map resolution to hash index (bit mask) then normalise max to 1
		int ix0 = Mathf.FloorToInt(i_uv.x) & hash_mask_size;
		int iy0 = Mathf.FloorToInt(i_uv.y) & hash_mask_size;
		int ix1 = ix0 + 1;
		int iy1 = iy0 + 1;

		int h0 = hash[ix0];
		int h1 = hash[ix1];
		int h00 = hash[h0 + iy0];
		int h10 = hash[h1 + iy0];
		int h01 = hash[h0 + iy1];
		int h11 = hash[h1 + iy1];

		tx = FifthOrderSmoothing(tx);
		ty = FifthOrderSmoothing(ty);

		return Mathf.Lerp(	Mathf.Lerp(h00, h10, tx),
							Mathf.Lerp(h01, h11, tx),
							ty) 
				/ (float)hash_mask_size;
	}

	// Set lattice value as index 
	public static float PointValue(float i_x, float frequency = 1.0f)
	{
		// Apply downsampling
		i_x *= frequency;
		return Mathf.FloorToInt(i_x) & 1;
	}
}
