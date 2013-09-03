using System;
using System.Collections.Generic;
using DeltaEngine.Graphics.Vertices;
using Microsoft.Xna.Framework.Graphics;
using VertexElement = DeltaEngine.Graphics.Vertices.VertexElement;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// Converts VertexFormat to Xna format
	/// </summary>
	public class XnaVertexFormat
	{
		public Microsoft.Xna.Framework.Graphics.VertexElement[] ConvertFrom(VertexFormat format)
		{
			var xnaElements = new Microsoft.Xna.Framework.Graphics.VertexElement[format.Elements.Length];
			usageCounts = new Dictionary<VertexElementUsage, int>();
			for (int i = 0; i < xnaElements.Length; i++)
				xnaElements[i] = ConvertToXnaElement(format.Elements[i]);
			return xnaElements;
		}

		private Dictionary<VertexElementUsage, int> usageCounts;

		private Microsoft.Xna.Framework.Graphics.VertexElement ConvertToXnaElement(
			VertexElement element)
		{
			var vertexType = ConvertVertexType(element.ElementType);
			var usage = ConvertVertexUsage(element.ElementType);
			if (usageCounts.ContainsKey(usage))
				usageCounts[usage]++;
			else
				usageCounts.Add(usage, 0);
			return new Microsoft.Xna.Framework.Graphics.VertexElement(element.Offset, vertexType, usage,
				usageCounts[usage]);
		}

		private static VertexElementFormat ConvertVertexType(VertexElementType elementType)
		{
			switch (elementType)
			{
			case VertexElementType.Position2D:
			case VertexElementType.TextureUV:
			case VertexElementType.LightMapUV:
			case VertexElementType.ExtraUV:
				return VertexElementFormat.Vector2;
			case VertexElementType.Position3D:
			case VertexElementType.Normal:
			case VertexElementType.Tangent:
			case VertexElementType.TextureUVW:
				return VertexElementFormat.Vector3;
			case VertexElementType.Color:
				return VertexElementFormat.Color;
			case VertexElementType.SkinIndices:
				return VertexElementFormat.Short2;
			case VertexElementType.SkinWeights:
				return VertexElementFormat.NormalizedShort2;
			default:
				throw new ElementTypeNotSupported(elementType);
			}
		}

		private class ElementTypeNotSupported : Exception
		{
			public ElementTypeNotSupported(VertexElementType elementType)
				: base(elementType.ToString()) {}
		}

		private static VertexElementUsage ConvertVertexUsage(VertexElementType elementType)
		{
			switch (elementType)
			{
			case VertexElementType.Position2D:
			case VertexElementType.Position3D:
				return VertexElementUsage.Position;
			case VertexElementType.Normal:
				return VertexElementUsage.Normal;
			case VertexElementType.Tangent:
				return VertexElementUsage.Tangent;
			case VertexElementType.Color:
				return VertexElementUsage.Color;
			case VertexElementType.TextureUV:
			case VertexElementType.TextureUVW:
			case VertexElementType.LightMapUV:
			case VertexElementType.ExtraUV:
				return VertexElementUsage.TextureCoordinate;
			case VertexElementType.SkinIndices:
				return VertexElementUsage.BlendIndices;
			case VertexElementType.SkinWeights:
				return VertexElementUsage.BlendWeight;
			default:
				throw new ElementUsageNotSupported(elementType);
			}
		}

		private class ElementUsageNotSupported : Exception
		{
			public ElementUsageNotSupported(VertexElementType elementType)
				: base(elementType.ToString()) {}
		}
	}
}