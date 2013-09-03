using DeltaEngine.Core;
using DeltaEngine.Graphics.Vertices;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// Basic functionality for all XNA based circular buffers to render small batches quickly.
	/// </summary>
	public class XnaCircularBuffer : CircularBuffer
	{
		public XnaCircularBuffer(Device device, ShaderWithFormat shader, BlendMode blendMode,
			VerticesMode drawMode = VerticesMode.Triangles)
			: base(device, shader, blendMode, drawMode) {}

		protected override void CreateNative()
		{
			nativeDevice = (device as XnaDevice).NativeDevice;
			nativeVertexBuffer = new DynamicVertexBuffer(nativeDevice,
				(shader as XnaShader).XnaVertexDeclaration, maxNumberOfVertices, BufferUsage.WriteOnly);
			nativeIndexBuffer = new DynamicIndexBuffer(nativeDevice, IndexElementSize.SixteenBits,
				maxNumberOfIndices, BufferUsage.WriteOnly);
			currentDataHint = SetDataOptions.Discard;
		}

		private GraphicsDevice nativeDevice;
		private DynamicVertexBuffer nativeVertexBuffer;
		private DynamicIndexBuffer nativeIndexBuffer;
		private SetDataOptions currentDataHint;

		/// <summary>
		/// As long as we did not put anything new into the buffer we can discard the old content.
		/// However once AddDataNative was called we need to use NoOverwrite until drawing happens.
		/// </summary>
		protected override void BufferIsFullResetToBeginning()
		{
			base.BufferIsFullResetToBeginning();
			currentDataHint = SetDataOptions.Discard;
		}

		protected override void AddDataNative<VertexType>(Chunk textureChunk, VertexType[] vertexData,
			short[] indices, int numberOfVertices, int numberOfIndices)
		{
			nativeDevice.SetVertexBuffer(null);
			nativeVertexBuffer.SetData(totalVertexOffsetInBytes, vertexData, 0, numberOfVertices,
				vertexSize, currentDataHint);
			if (!UsesIndexBuffer)
			{
				currentDataHint = SetDataOptions.NoOverwrite;
				return;
			}
			nativeDevice.Indices = null;
			if (indices == null)
				indices = ComputeIndices(textureChunk.NumberOfVertices, numberOfVertices);
			else if (textureChunk.FirstVertexOffsetInBytes > 0)
				indices = RemapIndices(indices, numberOfIndices);
			nativeIndexBuffer.SetData(totalIndexOffsetInBytes, indices, 0, numberOfIndices,
				currentDataHint);
			currentDataHint = SetDataOptions.NoOverwrite;
		}

		public override void DrawAllTextureChunks()
		{
			nativeDevice.SetVertexBuffer(nativeVertexBuffer);
			if (UsesIndexBuffer)
				nativeDevice.Indices = nativeIndexBuffer;
			if (!UsesTexturing)
				(device as XnaDevice).DisableTexturing();
			base.DrawAllTextureChunks();
			currentDataHint = SetDataOptions.Discard;
		}

		protected override void DrawChunk(Chunk chunk)
		{
			if (UsesIndexBuffer)
			{
				if (chunk.Texture != null)
					(device as XnaDevice).SetDiffuseTexture(chunk.Texture);
				nativeDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, //baseVertex
					chunk.FirstVertexOffsetInBytes / vertexSize, chunk.NumberOfVertices,
					chunk.FirstIndexOffsetInBytes / 2, chunk.NumberOfIndices / 3);
			}
			else
				nativeDevice.DrawPrimitives(PrimitiveType.LineList,
					chunk.FirstVertexOffsetInBytes / vertexSize, chunk.NumberOfVertices / 2);
		}

		/*needed
		public override void DrawVertices(VertexPosition3DColorUV[] vertices, short[] indices)
		{
			EntitiesRunner.Current.CheckIfInDrawState();
			ApplyEffect();
			spritesBuffer.AddData(vertices, indices);
			spritesBuffer.SetDrawMode(VerticesMode.Triangles);
			spritesBuffer.Draw(currentTexture);
			NumberOfVerticesDrawnThisFrame += vertices.Length;
			NumberOfTimesDrawnThisFrame++;
		}

		private void ApplyEffect()
		{
			basicEffect.VertexColorEnabled = true;
			basicEffect.TextureEnabled = nativeDevice.Textures[0] != null;
			if (basicEffect.TextureEnabled)
				basicEffect.Texture = nativeDevice.Textures[0] as Texture2D;
			basicEffect.CurrentTechnique.Passes[0].Apply();
		}

		public override void DrawVertices(VerticesMode mode, VertexPosition3DColor[] vertices,
			short[] indices = null)
		{
			EntitiesRunner.Current.CheckIfInDrawState();
			ApplyEffect();
			shapesBuffer.AddData(vertices, indices);
			shapesBuffer.SetDrawMode(mode);
			shapesBuffer.Draw(currentTexture);
			NumberOfVerticesDrawnThisFrame += vertices.Length;
			NumberOfTimesDrawnThisFrame++;
		}
		protected override void DrawNative()
		{
			var primitiveType = drawMode == VerticesMode.Triangles
				? PrimitiveType.TriangleList : PrimitiveType.LineList;
			nativeDevice.DrawIndexedPrimitives(primitiveType, 0, 0, totalIndicesCount, 0,
				GetPrimitiveCount(totalIndicesCount, primitiveType));
		}

		private static int GetPrimitiveCount(int numIndices, PrimitiveType primitiveType)
		{
			return primitiveType == PrimitiveType.LineList ? numIndices / 2 : numIndices / 3;
		}
		 */

		protected override void DisposeNative()
		{
			nativeVertexBuffer.Dispose();
			nativeIndexBuffer.Dispose();
		}
	}
}