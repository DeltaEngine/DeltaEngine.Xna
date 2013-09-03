using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// All OpenGL shaders share this basic functionality.
	/// </summary>
	public class XnaShader : ShaderWithFormat
	{
		public XnaShader(string contentName, XnaDevice device)
			: base(contentName)
		{
			this.device = device;
		}

		private readonly XnaDevice device;

		private XnaShader(ShaderCreationData creationData, XnaDevice device)
			: base(creationData)
		{
			this.device = device;
		}

		protected override sealed void Create()
		{
			nativeFormat = new XnaVertexFormat();
		}

		private XnaVertexFormat nativeFormat;
		public VertexDeclaration XnaVertexDeclaration
		{
			get { return new VertexDeclaration(Format.Stride, nativeFormat.ConvertFrom(Format)); }
		}

		public override void SetModelViewProjectionMatrix(Matrix matrix)
		{
			//set worldView for 3D: device.SetModelViewMatrix(worldViewMatrix);
		}

		public override void SetDiffuseTexture(Image texture)
		{
			device.SetDiffuseTexture(texture);
		}

		public override void Bind()
		{
			device.SetShaderStates(Format.HasColor, Format.HasUV);
		}

		public override void BindVertexDeclaration() {}

		protected override void DisposeData() {}
	}
}