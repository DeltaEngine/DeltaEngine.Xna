using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Models
{
	/// <summary>
	/// Models are collections of meshes to be rendered and in addition can have animation data.
	/// </summary>
	public class Model : Entity3D
	{
		public Model(string modelContentName, Vector position)
			: this(ContentLoader.Load<ModelData>(modelContentName), position) {}

		public Model(ModelData data, Vector position)
			: this(data, position, Quaternion.Identity) {}

		public Model(ModelData data, Vector position, Quaternion orientation)
			: base(position, orientation)
		{
			Add(data);
			OnDraw<ModelRenderer>();
		}

		public class ModelRenderer : DrawBehavior
		{
			public ModelRenderer(Drawing drawing)
			{
				this.drawing = drawing;
			}

			private readonly Drawing drawing;

			public void Draw(IEnumerable<DrawableEntity> entities)
			{
				foreach (var entity in entities.OfType<Entity3D>())
				{
					var modelTranform = Matrix.FromQuaternion(entity.Get<Quaternion>());
					modelTranform.Translation = entity.Get<Vector>();
					var data = entity.Get<ModelData>();
					foreach (var mesh in data.Meshes)
						drawing.AddGeometry(mesh.Geometry, mesh.Material, modelTranform);
				}
			}
		}
	}
}