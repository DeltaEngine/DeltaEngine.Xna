using DeltaEngine.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = DeltaEngine.Datatypes.Matrix;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// All Xna shaders share this basic functionality, we use
	/// </summary>
	public class XnaShader : ShaderWithFormat
	{
		private XnaShader(ShaderWithFormatCreationData creationData, XnaDevice device)
			: this((ShaderCreationData)creationData, device) {}

		private XnaShader(ShaderCreationData creationData, XnaDevice device)
			: base(creationData)
		{
			this.device = device;
			TryCreateShader();
		}

		private readonly XnaDevice device;

		protected override sealed void CreateShader()
		{
			nativeFormat = new XnaVertexFormat();
		}

		private XnaVertexFormat nativeFormat;
		public VertexDeclaration XnaVertexDeclaration
		{
			get { return new VertexDeclaration(Format.Stride, nativeFormat.ConvertFrom(Format)); }
		}

		public override void SetModelViewProjection(Matrix matrix)
		{
			device.SetViewMatrix(matrix);
		}

		public override void SetModelViewProjection(Matrix model, Matrix view, Matrix projection)
		{
			device.SetModelMatrix(model);
			device.SetViewMatrix(view);
			device.SetProjectionMatrix(projection);
		}

		public override void SetJointMatrices(Matrix[] jointMatrices)
		{
			Core.Logger.Info("Skinning is not supported yet in XNA");
		}

		public override void SetDiffuseTexture(Image texture)
		{
			device.SetDiffuseTexture(texture);
		}

		public override void SetLightmapTexture(Image texture)
		{
			Core.Logger.Info("Lightmaps are not supported yet in XNA");
		}

		public override void SetSunLight(SunLight light)
		{
			if (!isLightingAlreadySet)
			{
				device.ShaderEffect.AmbientLightColor = Vector3.One * AmbientLightIntensity;
				isLightingAlreadySet = true;
			}
			device.ShaderEffect.DirectionalLight0.DiffuseColor =
				new Vector3(light.Color.RedValue, light.Color.GreenValue, light.Color.BlueValue);
			device.ShaderEffect.DirectionalLight0.Direction =
				new Vector3(-light.Direction.X, -light.Direction.Y, -light.Direction.Z);
		}

		private bool isLightingAlreadySet;

		private const float AmbientLightIntensity = 0.2f;

		public override void Bind()
		{
			device.SetShaderStates(Format.HasColor, Format.HasUV, Format.HasNormal,
				Flags.HasFlag(ShaderFlags.Fog));
		}

		public override void BindVertexDeclaration() {}

		public override void ApplyFogSettings(FogSettings fogSettings)
		{
			device.ShaderEffect.FogEnabled = Flags.HasFlag(ShaderFlags.Fog);
			device.ShaderEffect.FogColor = new Vector3(fogSettings.FogColor.RedValue,
				fogSettings.FogColor.GreenValue, fogSettings.FogColor.BlueValue);
			device.ShaderEffect.FogStart = fogSettings.FogStart;
			device.ShaderEffect.FogEnd = fogSettings.FogEnd;
		}

		protected override void DisposeData() {}
	}
}