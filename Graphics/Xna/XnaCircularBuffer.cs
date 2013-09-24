using System.Collections.Generic;
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

		protected override void DisposeNextFrame()
		{
			buffersToDisposeNextFrame.Add(nativeVertexBuffer);
			if (UsesIndexBuffer)
				buffersToDisposeNextFrame.Add(nativeIndexBuffer);
		}

		private readonly List<GraphicsResource> buffersToDisposeNextFrame = new List<GraphicsResource>();

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

		public override void DisposeUnusedBuffersFromPreviousFrame()
		{
			if (buffersToDisposeNextFrame.Count <= 0)
				return;
			foreach (var buffer in buffersToDisposeNextFrame)
				buffer.Dispose();
			buffersToDisposeNextFrame.Clear();
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
		
		protected override void DisposeNative()
		{
			nativeVertexBuffer.Dispose();
			nativeIndexBuffer.Dispose();
		}
	}
}