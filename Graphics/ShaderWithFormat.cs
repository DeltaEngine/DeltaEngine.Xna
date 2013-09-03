using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Adds graphics specific features to a shader object like VertexFormat and the shader code.
	/// </summary>
	public abstract class ShaderWithFormat : Shader
	{
		protected ShaderWithFormat(string contentName)
			: base(contentName) {}

		protected ShaderWithFormat(ShaderCreationData creationData)
			: base("<GeneratedShader>")
		{
			Initialize(creationData);
		}

		protected void Initialize(ShaderCreationData data)
		{
			if (data == null)
				throw new UnableToCreateShaderNoCreationDataWasGiven();
			if (data.Format == null || data.Format.Elements.Length == 0)
				throw new UnableToCreateShaderWithoutValidVertexFormat();
			if (string.IsNullOrEmpty(data.VertexCode) || string.IsNullOrEmpty(data.FragmentCode))
				throw new UnableToCreateShaderWithoutValidVertexAndPixelCode();
			Format = data.Format;
			VertexCode = data.VertexCode;
			PixelCode = data.FragmentCode;
			Dx11Code = data.Dx11Code;
			Dx9Code = data.Dx9Code;
		}

		internal class UnableToCreateShaderNoCreationDataWasGiven : NullReferenceException {}

		internal class UnableToCreateShaderWithoutValidVertexFormat : Exception {}

		private class UnableToCreateShaderWithoutValidVertexAndPixelCode : Exception {}

		public VertexFormat Format { get; private set; }
		protected string VertexCode { get; private set; }
		protected string PixelCode { get; private set; }
		protected string Dx11Code;
		protected string Dx9Code;

		protected override void LoadData(Stream fileData)
		{
			if (fileData.Length == 0)
				throw new EmptyShaderFileGiven();
			var data = new BinaryReader(fileData).Create();
			Initialize(data as ShaderCreationData);
			Create();
		}

		public class EmptyShaderFileGiven : Exception {}

		protected abstract void Create();

		protected override bool AllowCreationIfContentNotFound
		{
			get
			{
				return Name == Position2DUv || Name == Position3DUv || Name == Position2DColor ||
					Name == Position3DColor || Name == Position2DColorUv || Name == Position3DColorUv;
			}
		}

		protected override void CreateDefault()
		{
			switch (Name)
			{
			case Position2DUv:
				Initialize(new ShaderCreationData(UvVertexCode, UvFragmentCode, UvHlslCode,
					Dx9Position2DTexture, VertexFormat.Position2DUv));
				break;
			case Position2DColor:
				Initialize(new ShaderCreationData(ColorVertexCode, ColorFragmentCode, ColorHlslCode,
					Dx9Position2DColor, VertexFormat.Position2DColor));
				break;
			case Position2DColorUv:
				Initialize(new ShaderCreationData(ColorUvVertexCode, ColorUvFragmentCode, ColorUvHlslCode,
					Dx9Position2DColorTexture, VertexFormat.Position2DColorUv));
				break;
			case Position3DUv:
				Initialize(new ShaderCreationData(UvVertexCode, UvFragmentCode, UvHlslCode,
					Dx9Position3DTexture, VertexFormat.Position3DUv));
				break;
			case Position3DColor:
				Initialize(new ShaderCreationData(ColorVertexCode, ColorFragmentCode, ColorHlslCode,
					Dx9Position3DColor, VertexFormat.Position3DColor));
				break;
			case Position3DColorUv:
				Initialize(new ShaderCreationData(ColorUvVertexCode, ColorUvFragmentCode, ColorUvHlslCode,
					Dx9Position3DColorTexture, VertexFormat.Position3DColorUv));
				break;
			}
			Create();
		}

		internal const string UvVertexCode = @"uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec2 aTextureUV;
varying vec2 vTexcoord;
void main()
{
	vTexcoord = aTextureUV;
	gl_Position = ModelViewProjection * aPosition;
}";

		internal const string UvFragmentCode = @"precision mediump float;
uniform sampler2D Texture;
varying vec2 vTexcoord;
void main()
{
	gl_FragColor = texture2D(Texture, vTexcoord);
}";

		internal const string ColorVertexCode = @"uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec4 aColor;
varying vec4 diffuseColor;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	diffuseColor = aColor;
}";

		internal const string ColorFragmentCode = @"precision mediump float;
varying vec4 diffuseColor;
void main()
{
	gl_FragColor = diffuseColor;
}";

		internal const string ColorUvVertexCode = @"uniform mat4 ModelViewProjection;
attribute vec4 aPosition;
attribute vec4 aColor;
attribute vec2 aTextureUV;
varying vec4 diffuseColor;
varying vec2 diffuseTexCoord;
void main()
{
	gl_Position = ModelViewProjection * aPosition;
	diffuseColor = aColor;
	diffuseTexCoord = aTextureUV;
}";

		internal const string ColorUvFragmentCode = @"precision mediump float;
uniform sampler2D Texture;
varying vec4 diffuseColor;
varying vec2 diffuseTexCoord;
void main()
{
	gl_FragColor = texture2D(Texture, diffuseTexCoord) * diffuseColor;
}";

		internal const string UvHlslCode = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
};

float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.texCoord = input.texCoord;
	return output;
}

Texture2D DiffuseTexture;

SamplerState TextureSamplerState
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return DiffuseTexture.Sample(TextureSamplerState, input.texCoord);
}";

		internal const string ColorHlslCode = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.color = input.color;
	return output;
}

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return input.color;
}";

		internal const string ColorUvHlslCode = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 texCoord : TEXCOORD;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 texCoord : TEXCOORD;
};

float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.color = input.color;
	output.texCoord = input.texCoord;
	return output;
}

Texture2D DiffuseTexture;

SamplerState TextureSamplerState
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return DiffuseTexture.Sample(TextureSamplerState, input.texCoord) * input.color;
}";

		internal const string Dx9Position3DColor =
			"float4x4 WorldViewProjection;" + "struct VS_OUTPUT" + "{" + "float4 Pos       : POSITION;" +
				"float4 Color     : COLOR0;" + "};" +
				"VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR )" + "{" +
				"VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);" +
				"output.Color = float4(Color[2], Color[1], Color[0], Color[3]);" + "return output;" + "}" +
				"float4 PS( VS_OUTPUT input ) : COLOR0" + "{" + "return input.Color;" + "}";

		internal const string Dx9Position3DColorTexture =
			"float4x4 WorldViewProjection;" + "struct VS_OUTPUT" + "{" + "float4 Pos       : POSITION;" +
				"float4 Color     : COLOR0;" + "float2 TextureUV : TEXCOORD0;" + "};" +
				"VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR, float2 TextureUV : TEXCOORD0 )" +
				"{" + "VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);" +
				"output.Color = float4(Color[2], Color[1], Color[0], Color[3]);" +
				"output.TextureUV = TextureUV;" + "return output;" + "}" + "sampler DiffuseTexture;" +
				"float4 PS( VS_OUTPUT input ) : COLOR0" + "{" +
				"return tex2D(DiffuseTexture, input.TextureUV) * input.Color;" + "}";

		internal const string Dx9Position3DTexture =
			"float4x4 WorldViewProjection;" + "struct VS_OUTPUT" + "{" + "float4 Pos       : POSITION;" +
				"float2 TextureUV : TEXCOORD0;" + "};" +
				"VS_OUTPUT VS( float3 Pos : POSITION, float2 TextureUV : TEXCOORD0 )" + "{" +
				"VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);" +
				"output.TextureUV = TextureUV;" + "return output;" + "}" + "sampler DiffuseTexture;" +
				"float4 PS( VS_OUTPUT input ) : COLOR0" + "{" +
				"return tex2D(DiffuseTexture, input.TextureUV);" + "}";

		internal const string Dx9Position2DColor =
			"float4x4 WorldViewProjection;" + "struct VS_OUTPUT" + "{" + "float4 Pos       : POSITION;" +
				"float4 Color     : COLOR0;" + "};" +
				"VS_OUTPUT VS( float2 Pos : POSITION, float4 Color : COLOR )" + "{" +
				"VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos[0], Pos[1], 0.0f, 1.0f), WorldViewProjection);" +
				"output.Color = float4(Color[2], Color[1], Color[0], Color[3]);" + "return output;" + "}" +
				"float4 PS( VS_OUTPUT input ) : COLOR0" + "{" + "return input.Color;" + "}";

		internal const string Dx9Position2DColorTexture =
			"float4x4 WorldViewProjection;" + "struct VS_OUTPUT" + "{" + "float4 Pos       : POSITION;" +
				"float4 Color     : COLOR0;" + "float2 TextureUV : TEXCOORD0;" + "};" +
				"VS_OUTPUT VS( float2 Pos : POSITION, float4 Color : COLOR, float2 TextureUV : TEXCOORD0 )" +
				"{" + "VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos[0],Pos[1], 0.0f, 1.0f), WorldViewProjection);" +
				"output.Color = float4(Color[2], Color[1], Color[0], Color[3]);" +
				"output.TextureUV = TextureUV;" + "return output;" + "}" + "sampler DiffuseTexture;" +
				"float4 PS( VS_OUTPUT input ) : COLOR0" + "{" +
				"return tex2D(DiffuseTexture, input.TextureUV) * input.Color;" + "}";

		internal const string Dx9Position2DTexture =
			"float4x4 WorldViewProjection;" + "struct VS_OUTPUT" + "{" + "float4 Pos       : POSITION;" +
				"float2 TextureUV : TEXCOORD0;" + "};" +
				"VS_OUTPUT VS( float2 Pos : POSITION, float2 TextureUV : TEXCOORD0 )" + "{" +
				"VS_OUTPUT output = (VS_OUTPUT)0;" +
				"output.Pos = mul(float4(Pos[0],Pos[1], 0.0f, 1.0f), WorldViewProjection);" +
				"output.TextureUV = TextureUV;" + "return output;" + "}" + "sampler DiffuseTexture;" +
				"float4 PS( VS_OUTPUT input ) : COLOR0" + "{" +
				"return tex2D(DiffuseTexture, input.TextureUV);" + "}";
	}
}