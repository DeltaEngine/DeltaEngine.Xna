using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.Particles;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Shapes3D.Tests
{
	internal class BillboardTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateCamera()
		{
			camera = new LookAtCamera(Resolve<Device>(), Resolve<Window>(), Vector.One, Vector.Zero);
		}

		private Camera camera;

		[Test]
		public void DrawBillboardCameraAligned()
		{
			new Billboard(Vector.Zero, Size.One, new Material(Shader.Position3DColorUv, "DeltaEngineLogo"),BillboardMode.CameraFacing);
		}

		[Test]
		public void DrawBillboardVertical()
		{
			new Billboard(Vector.Zero, Size.One, new Material(Shader.Position3DColorUv, "DeltaEngineLogo"),BillboardMode.Vertical);
		}

		[Test]
		public void DrawBillboardGroundPlane()
		{
			new Billboard(Vector.Zero, Size.One, new Material(Shader.Position3DColorUv, "DeltaEngineLogo"),BillboardMode.GroundPlane);
		}

		[TearDown]
		public void DeactivateCamera()
		{
			camera = null;
		}

		private class Billboard : Entity3D
		{
			public Billboard(Vector position, Size size, Material material,
				BillboardMode billboardMode = BillboardMode.GroundPlane)
				: base(position)
			{
				Add(billboardMode);
				Add(CreatePlaneQuad(size, material));
				OnDraw<BillBoardRenderer>();
			}

			private static PlaneQuad CreatePlaneQuad(Size size, Material material)
			{
				return new PlaneQuad(size, material);
			}

			private class BillBoardRenderer : DrawBehavior
			{
				public BillBoardRenderer(Drawing drawing, Device device)
				{
					this.drawing = drawing;
					this.device = device;
				}

				private readonly Drawing drawing;
				private readonly Device device;

				public void Draw(IEnumerable<DrawableEntity> entities)
				{
					foreach (var entity in entities.OfType<Entity3D>())
					{
						var billboardTransform = SetupBillboardTransform(device.CameraViewMatrix, entity.Position,
							entity.Get<BillboardMode>());
						var plane = entity.Get<PlaneQuad>();
						billboardTransform.Translation = entity.Position;
						drawing.AddGeometry(plane.Geometry,plane.Material, billboardTransform);
					}
				}

				private static Matrix SetupBillboardTransform(Matrix viewMatrix, Vector billboardPosition,
					BillboardMode billboardMode)
				{
					var viewMatrixInverse = Matrix.Invert(viewMatrix);
					Vector look = viewMatrixInverse.Translation - billboardPosition;
					Vector cameraUp = viewMatrix.Up;
					if (billboardMode == BillboardMode.Vertical)
					{
						cameraUp = Vector.UnitZ;
						look.Z = 0;
					}
					else if (billboardMode == BillboardMode.GroundPlane)
					{
						cameraUp = -Vector.UnitZ;
						look = -Vector.UnitX;
					}
					look.Normalize();
					Vector right = Vector.Cross(cameraUp, look);
					Vector up = Vector.Cross(look, right);

					var billboardMatrix = new Matrix();
					billboardMatrix.Right = right;
					billboardMatrix.Up = look;
					billboardMatrix.Forward = up;

					return billboardMatrix;
				}
			}
		}
	}
}