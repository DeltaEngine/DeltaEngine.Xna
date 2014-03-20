using System;
using System.IO;
using System.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = DeltaEngine.Datatypes.Matrix;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

namespace DeltaEngine.Graphics.Xna
{
	/// <summary>
	/// XNA graphics device.
	/// </summary>
	public sealed class XnaDevice : Device
	{
		public XnaDevice(Game game, Window window, Settings settings)
			: base(window)
		{
			this.settings = settings;
			CreateAndSetupNativeDeviceManager(game);
			NativeDevice.BlendState = BlendState.NonPremultiplied;
			OnSet3DMode += () => ShaderEffect.Projection = GetXnaMatrix(CameraProjectionMatrix);
			OnSet3DMode += () => ShaderEffect.View = GetXnaMatrix(CameraViewMatrix);
		}

		private readonly Settings settings;

		private void CreateAndSetupNativeDeviceManager(Game game)
		{
			game.SuppressDraw();
			CreateDeviceManager(game);
			NativeContent = game.Content;
			NativeContent.RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content");
		}

		private void CreateDeviceManager(Game game)
		{
			deviceManager = new GraphicsDeviceManager(game)
			{
				SupportedOrientations =
					DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft |
						DisplayOrientation.LandscapeRight,
				SynchronizeWithVerticalRetrace = settings.UseVSync,
				PreferredBackBufferFormat = SurfaceFormat.Color,
				PreferredBackBufferWidth = (int)settings.Resolution.Width,
				PreferredBackBufferHeight = (int)settings.Resolution.Height,
				GraphicsProfile = GraphicsProfile.HiDef
			};
			deviceManager.IsFullScreen = settings.StartInFullscreen;
			deviceManager.ApplyChanges();
		}

		private GraphicsDeviceManager deviceManager;

		public GraphicsDevice NativeDevice
		{
			get { return deviceManager.GraphicsDevice; }
		}
		
		public override void SetViewport(Size newSizeInPixel)
		{
			NativeDevice.PresentationParameters.BackBufferWidth = (int)newSizeInPixel.Width;
			NativeDevice.PresentationParameters.BackBufferHeight = (int)newSizeInPixel.Height;
			NativeDevice.Reset(NativeDevice.PresentationParameters);
			NativeDevice.Clear(new Color(0, 0, 0));
			SetModelViewProjectionMatrixFor2D();
		}

		protected override void OnFullscreenChanged(Size displaySize, bool isFullScreenEnabled)
		{
			deviceManager.PreferredBackBufferWidth = (int)displaySize.Width;
			deviceManager.PreferredBackBufferHeight = (int)displaySize.Height;
			deviceManager.IsFullScreen = isFullScreenEnabled;
			base.OnFullscreenChanged(displaySize, isFullScreenEnabled);
		}

		public ContentManager NativeContent { get; private set; }

		public BasicEffect ShaderEffect
		{
			get
			{
				if (effect != null)
					return effect;
				effect = new BasicEffect(NativeDevice);
				return effect;
			}
		}
		private BasicEffect effect;

		/// <summary>
		/// The -0.5 pixel offset is needed in DirectX 9 to correctly match the texture coordinates:
		/// http://msdn.microsoft.com/en-us/library/windows/desktop/bb219690(v=vs.85).aspx
		/// </summary>
		public override void SetModelViewProjectionMatrixFor2D()
		{
			const float Offset = -0.5f;
			SetModelMatrix(Matrix.Identity);
			SetProjectionMatrix(Matrix.Identity);
			var pixelSpaceViewport = ScreenSpace.Current.ToPixelSpace(ScreenSpace.Current.Viewport);
			ShaderEffect.View = XnaMatrix.CreateOrthographicOffCenter(
				pixelSpaceViewport.Left + Offset, pixelSpaceViewport.Right + Offset,
				pixelSpaceViewport.Bottom + Offset, pixelSpaceViewport.Top + Offset, 0, 1);
		}

		protected override void EnableClockwiseBackfaceCulling()
		{
			if (isCullingEnabled)
				return;
			isCullingEnabled = true;
			NativeDevice.RasterizerState = RasterizerState.CullClockwise;
		}

		/// <summary>
		/// CullClockwise needs to be set once in XNA (like for all DirectX frameworks)
		/// </summary>
		private bool isCullingEnabled;

		protected override void DisableCulling()
		{
			if (!isCullingEnabled)
				return;
			isCullingEnabled = false;
			NativeDevice.RasterizerState = RasterizerState.CullNone;
		}

		public void SetModelMatrix(Matrix matrix)
		{
			ShaderEffect.World = GetXnaMatrix(matrix);
		}

		public void SetViewMatrix(Matrix matrix)
		{
			ShaderEffect.View = GetXnaMatrix(matrix);
		}

		public void SetProjectionMatrix(Matrix matrix)
		{
			ShaderEffect.Projection = GetXnaMatrix(matrix);
		}

		private static XnaMatrix GetXnaMatrix(Matrix matrix)
		{
			var m = new XnaMatrix();
			m.M11 = matrix[0];
			m.M12 = matrix[1];
			m.M13 = matrix[2];
			m.M14 = matrix[3];
			m.M21 = matrix[4];
			m.M22 = matrix[5];
			m.M23 = matrix[6];
			m.M24 = matrix[7];
			m.M31 = matrix[8];
			m.M32 = matrix[9];
			m.M33 = matrix[10];
			m.M34 = matrix[11];
			m.M41 = matrix[12];
			m.M42 = matrix[13];
			m.M43 = matrix[14];
			m.M44 = matrix[15];
			return m;
		}

		public override void Clear()
		{
			var color = window.BackgroundColor;
			if (color.A > 0)
				NativeDevice.Clear(new Color(color.R, color.G, color.B, color.A));
		}

		public override void Present() {}

		public override void DisableDepthTest()
		{
			if (disableDepthTestState == null)
				disableDepthTestState = new DepthStencilState { DepthBufferEnable = false };
			NativeDevice.DepthStencilState = disableDepthTestState;
		}

		private DepthStencilState disableDepthTestState;

		public override void EnableDepthTest()
		{
			if (enableDepthTestState == null)
				enableDepthTestState = new DepthStencilState();
			NativeDevice.DepthStencilState = enableDepthTestState;
		}

		private DepthStencilState enableDepthTestState;

		public override void Dispose()
		{
			if (effect != null)
				effect.Dispose();
		}

		public void SetDiffuseTexture(Image image)
		{
			Texture2D nativeTexture;
			if (image is VideoImage)
				nativeTexture = ((VideoImage)image).NativeTexture;
			else
				nativeTexture = ((XnaImage)image).NativeTexture;
			if (NativeDevice.Textures[0] == nativeTexture && ShaderEffect.Texture == nativeTexture)
				return;
#if DEBUG
			// Check whether the initialization order was correct in AutofacStarter when the Run()
			// method is called. If it was not, the internal pointer in the native texture (pComPtr)
			// would have a null value (0x00) and the next assignment would crash.
			CheckIfTheInitializationOrderInResolverWasCorrect(nativeTexture);
#endif
			NativeDevice.Textures[0] = nativeTexture;
			NativeDevice.SamplerStates[0] = GetXnaSamplerState(image);
			ShaderEffect.Texture = nativeTexture;
		}
		
#if DEBUG
		private void CheckIfTheInitializationOrderInResolverWasCorrect(Texture2D nativeTexture)
		{
			if (!initializationOrderAlreadyChecked)
			{
				initializationOrderAlreadyChecked = true;
				if (NativePointerIsNull(nativeTexture))
					throw new InitializationOrderIsWrongCheckIfInitializationHappensInResolverEvent();
			}
		}

		public class InitializationOrderIsWrongCheckIfInitializationHappensInResolverEvent
			: Exception {}

		private bool initializationOrderAlreadyChecked;

		private static bool NativePointerIsNull(Texture2D nativeTexture)
		{
			var nativePointerField =
				nativeTexture.GetType().GetField("pComPtr", BindingFlags.Instance | BindingFlags.NonPublic);
			var nativePointer = nativePointerField.GetValue(nativeTexture);
			unsafe
			{
				var nativePointerValue = Pointer.Unbox(nativePointer);
				return nativePointerValue == null;
			}
		}
#endif
		
		private static SamplerState GetXnaSamplerState(Image texture)
		{
			return texture.DisableLinearFiltering
				? (texture.AllowTiling ? SamplerState.PointWrap : SamplerState.PointClamp)
				: (texture.AllowTiling ? SamplerState.LinearWrap : SamplerState.LinearClamp);
		}

		public override void SetBlendMode(BlendMode blendMode)
		{
			if (currentBlendMode == blendMode)
				return;
			NativeDevice.BlendState = GetXnaBlendState(blendMode);
			currentBlendMode = blendMode;
		}

		private BlendMode currentBlendMode = BlendMode.Opaque;

		private static BlendState GetXnaBlendState(BlendMode blendMode)
		{
			if (blendMode == BlendMode.Additive)
				return BlendState.Additive;
			if (blendMode == BlendMode.Opaque)
				return BlendState.Opaque;
			//for premultiplied alpha: AlphaBlend;
			return BlendState.NonPremultiplied;
		}

		public override CircularBuffer CreateCircularBuffer(ShaderWithFormat shader,
			BlendMode blendMode, VerticesMode drawMode = VerticesMode.Triangles)
		{
			return new XnaCircularBuffer(this, shader, blendMode, drawMode);
		}

		public void SetShaderStates(bool hasColor, bool hasUV, bool hasLighting, bool hasFog)
		{
			ShaderEffect.VertexColorEnabled = hasColor;
			ShaderEffect.TextureEnabled = hasUV;
			ShaderEffect.LightingEnabled = hasLighting;
			ShaderEffect.FogEnabled = hasFog;
			ShaderEffect.CurrentTechnique.Passes[0].Apply();
		}
	}
}