using System;
using DeltaEngine.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaGeometry : Geometry
	{
		public XnaGeometry(string contentName, Device device)
			: base (contentName)
		{
			this.device = device as XnaDevice;
			nativeDevice = (device as XnaDevice).NativeDevice;
			nativeVertexFormat = new XnaVertexFormat();
		}

		private readonly XnaDevice device;
		private readonly GraphicsDevice nativeDevice;
		private readonly XnaVertexFormat nativeVertexFormat;

		public XnaGeometry(GeometryCreationData creationData, Device device)
			: base (creationData)
		{
			this.device = device as XnaDevice;
			nativeDevice = (device as XnaDevice).NativeDevice;
			nativeVertexFormat = new XnaVertexFormat();
		}

		protected override void SetNativeData(byte[] vertexData, short[] indices)
		{
			if (vertexBuffer == null)
				CreateBuffers();
			vertexBuffer.SetData(vertexData);
			indexBuffer.SetData(0, indices, 0, indices.Length);
		}

		private DynamicVertexBuffer vertexBuffer;

		private void CreateBuffers()
		{
			var vertexDeclaration = new VertexDeclaration(Format.Stride,
				nativeVertexFormat.ConvertFrom(Format));
			vertexBuffer = new DynamicVertexBuffer(nativeDevice, vertexDeclaration, NumberOfVertices,
				BufferUsage.WriteOnly);
			indexBuffer = new DynamicIndexBuffer(nativeDevice, IndexElementSize.SixteenBits,
				NumberOfIndices, BufferUsage.WriteOnly);
		}

		private DynamicIndexBuffer indexBuffer;

		public override void Draw()
		{
			if (vertexBuffer == null)
				throw new UnableToDrawDynamicGeometrySetDataNeedsToBeCalledFirst();
			nativeDevice.SetVertexBuffer(vertexBuffer);
			nativeDevice.Indices = indexBuffer;
			device.ShaderEffect.CurrentTechnique.Passes[0].Apply();//needed in Xna to set new World matrix
			nativeDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
				NumberOfVertices, 0, NumberOfIndices / 3);
		}

		private class UnableToDrawDynamicGeometrySetDataNeedsToBeCalledFirst : Exception { }

		protected override void DisposeData()
		{
			if (vertexBuffer == null)
				return;
			vertexBuffer.Dispose();
			indexBuffer.Dispose();
			vertexBuffer = null;
		}
	}
}