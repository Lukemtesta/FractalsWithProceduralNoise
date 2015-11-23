using UnityEngine;
using System.Collections;

[System.Serializable]
public class TextureProperties
{
	public string 	m_name = "Procedural Noise";
	
	[Range(2,512)]
	public int 		m_width = 256;
	[Range(2,512)]
	public int 		m_height = 256;
	
	public int 	    m_anisotropic = 9;
	
	public bool		m_mipmap = true;
	
	public FilterMode 		m_filter = FilterMode.Trilinear;
	public TextureWrapMode 	m_wrap = TextureWrapMode.Clamp;
}

public class TextureManager : MonoBehaviour {

	public TextureProperties 	m_texture_config;
	private Texture2D 			m_texture;

	[Range(1,2)]
	public int 		m_noise_dimensions = 1;
	public float 	m_noise_frequency = 64;

	bool CreateTexture()
	{
		bool ret = true;

		try
		{
			// Instantiate texture with RGB 8 using user settings.
			// Attach to current GameObject if MeshRenderer component Exists
			m_texture = new Texture2D (m_texture_config.m_width, 
			                           m_texture_config.m_height, 
			                           TextureFormat.RGB24, 
			                           m_texture_config.m_mipmap);

			m_texture.name = m_texture_config.m_name;
			m_texture.wrapMode = m_texture_config.m_wrap;
			m_texture.filterMode = m_texture_config.m_filter;
			m_texture.anisoLevel = m_texture_config.m_anisotropic;

			GetComponent<MeshRenderer>().material.mainTexture = m_texture;

			FillTexture();
		}
		catch(UnityException)
		{
			ret = false;
		}

		return ret;
	}

	public void ResizeTexture()
	{
		if (m_texture_config.m_width != m_texture.width 
			|| m_texture_config.m_height != m_texture.height) 
		{
			CreateTexture();
		}
	}

	private void FillGradientTexture()
	{
		float x_stride = 1.0f / (float)m_texture_config.m_width;
		float y_stride = 1.0f / (float)m_texture_config.m_height;

		// Fill texture with noise
		for (int x = 0; x < m_texture_config.m_width; ++x) 
		{
			for(int y = 0; y < m_texture_config.m_height; ++y)
			{
				// Offset uv co-ordinates to fit lattice centers
				m_texture.SetPixel(x,y, new Color((x + 0.5f) * x_stride,
				                                  (y + 0.5f) * y_stride,
				                                  0));
			}
		}

		m_texture.Apply();
	}

	private void FillTexture()
	{
		NoiseMethod noise = ProceduralNoise.FlatValueNoise[m_noise_dimensions - 1];

		float x_stride = 1.0f / (float)m_texture_config.m_width;
		float y_stride = 1.0f / (float)m_texture_config.m_height;

		// Fill texture with noise
		for (int x = 0; x < m_texture_config.m_width; ++x) 
		{
			for(int y = 0; y < m_texture_config.m_height; ++y)
			{
				Vector3 uv = new Vector3((x + 0.5f) * x_stride,
				                         (y + 0.5f) * y_stride,
				                         0);

				// Offset uv co-ordinates to fit lattice centers
				m_texture.SetPixel(x,y, Color.white * noise(uv, m_noise_frequency));
				//m_texture.SetPixel(x,y, Color.white * ProceduralNoise.HashSmoothValue1D(uv, m_noise_frequency));
			}
		}
		
		m_texture.Apply();
	}

	void OnEnable()
	{
		if (m_texture == null) 
		{
			CreateTexture();
		}
	}
}
