using System;
using System.IO;
using System.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;
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
	public class XnaDevice : Device
	{
		public XnaDevice(Game game, Window window, Settings settings)
			: base(window)
		{
			this.settings = settings;
			CreateAndSetupNativeDeviceManager(game);
			NativeDevice.BlendState = BlendState.NonPremultiplied;
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
				SynchronizeWithVerticalRetrace = false,
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
				Set2DMode();
				return effect;
			}
		}
		private BasicEffect effect;

		public override void SetModelViewProjectionMatrixFor2D()
		{
			if (effect == null)
				return;
			//ModelViewProjectionMatrix = Matrix.CreateOrthographicOffCenter(0,
			//	window.ViewportPixelSize.Width, window.ViewportPixelSize.Height, 0, 0, 1);
			SetProjectionMatrix(ModelViewProjectionMatrix);
			SetModelViewMatrix(Matrix.Identity);
		}

		public override void SetProjectionMatrix(Matrix matrix)
		{
			//convert from matrix to xna projection!
			effect.Projection = XnaMatrix.CreateOrthographicOffCenter(0,
				window.ViewportPixelSize.Width, window.ViewportPixelSize.Height, 0, 0, 1);
		}

		public override void SetModelViewMatrix(Matrix matrix)
		{
			Matrix modelView = matrix * Matrix.CreateTranslation(-0.5f, -0.5f, 0.0f);
			var m = new XnaMatrix();
			m.Forward = new Vector3(modelView.Forward.X, modelView.Forward.Y, modelView.Forward.Z);
			m.Right = new Vector3(modelView.Right.X, modelView.Right.Y, modelView.Right.Z);
			m.Up = new Vector3(modelView.Up.X, modelView.Up.Y, modelView.Up.Z);
			m.Translation = new Vector3(modelView.Translation.X, modelView.Translation.Y,
				modelView.Translation.Z);
			m.M44 = 1.0f;
			effect.View = m;
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
			if (currentTexture == image)
				return;
			Texture2D nativeTexture;
			if (image is VideoImage)
				nativeTexture = ((VideoImage)image).NativeTexture;
			else
				nativeTexture = ((XnaImage)image).NativeTexture;
#if DEBUG
			// Check whether the intialization order was correct in AutofacStarter when the Run()
			// method is called. If it was not, the internal pointer in the native texture (pComPtr)
			// would have a null value (0x00) and the next assignment would crash.
			CheckIfTheInitializationOrderInResolverWasCorrect(nativeTexture);
#endif
			NativeDevice.Textures[0] = nativeTexture;
			NativeDevice.SamplerStates[0] = image.DisableLinearFiltering
				? SamplerState.PointClamp : SamplerState.LinearClamp;
			ShaderEffect.TextureEnabled = true;
			ShaderEffect.Texture = NativeDevice.Textures[0] as Texture2D;
			currentTexture = image;
		}

		private Image currentTexture;

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

		public class InitializationOrderIsWrongCheckIfInitializationHappensInResolverEvent :
			Exception {}

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

		public void DisableTexturing()
		{
			ShaderEffect.TextureEnabled = false;
			currentTexture = null;
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
			return BlendState.AlphaBlend;
		}

		public override CircularBuffer CreateCircularBuffer(ShaderWithFormat shader,
			BlendMode blendMode, VerticesMode drawMode = VerticesMode.Triangles)
		{
			return new XnaCircularBuffer(this, shader, blendMode, drawMode);
		}

		public void SetShaderStates(bool hasColor, bool hasUV)
		{
			ShaderEffect.VertexColorEnabled = hasColor;
			ShaderEffect.TextureEnabled = hasUV;
			ShaderEffect.CurrentTechnique.Passes[0].Apply();
		}
	}
}