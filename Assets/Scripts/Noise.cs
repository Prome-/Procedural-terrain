
using UnityEngine;

public static class Noise
{
  public static float[,] GenerateNoiseMap (int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
  {
    float[,] noiseMap = new float[mapWidth, mapHeight];

    System.Random rnd = new System.Random(seed); // using the seed in the Random() enables different base noisemaps

    // make sure each octave is sampled from a different position with octave offsets
    Vector2[] octaveOffsets = new Vector2[octaves];
    for (int i = 0; i < octaves; i++)
    {
      float offsetX = rnd.Next(-100000, 100000) + offset.x;
      float offsetY = rnd.Next(-100000, 100000) + offset.y;
      octaveOffsets[i] = new Vector2(offsetX, offsetY);
    }

    if (scale <= 0)
      scale = 0.0001f;

    float maxNoiseHeight = float.MinValue;
    float minNoiseHeight = float.MaxValue;

    // used to make the scale zoom to the center, instead of top right
    float halfWidth = mapWidth / 2.0f;
    float halfheight = mapHeight / 2.0f;

    for (int y = 0; y < mapHeight; y++)
      for (int x = 0; x < mapWidth; x++)
      {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        // octaves are used to make the noise more terrainlike by combining overlapping noises.
        // one noisemap per octave; high octaves get more frequency (x) and less amplitude (y)
        // for example: octave 0 is mountain outline, octave 1 is large boulders and octave 2 is rubble
        // see https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=1
        for (int i = 0; i < octaves; i++)
        {
          float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
          float sampleY = (y - halfheight) / scale * frequency + octaveOffsets[i].y;

          float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
          noiseHeight += perlinValue * amplitude;

          amplitude *= persistence;
          frequency *= lacunarity;
        }

        if (noiseHeight > maxNoiseHeight)
          maxNoiseHeight = noiseHeight;
        else if (noiseHeight < minNoiseHeight)
          minNoiseHeight = noiseHeight;

        noiseMap[x, y] = noiseHeight;
      }

    // normalizes the noisemap values between 0 and 1
    for (int y = 0; y < mapHeight; y++)
      for (int x = 0; x < mapWidth; x++)
        noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);

    return noiseMap;
  }
}
