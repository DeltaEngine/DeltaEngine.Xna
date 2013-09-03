﻿using DeltaEngine.Datatypes;
using DeltaEngine.Rendering;

namespace DeltaEngine.Physics2D
{
	public class Bullet : Entity2D
	{
		public Bullet(Physics physics, Point impulse, Rectangle area, int damage) : base(area)
		{
			PhysicsBody = physics.CreateRectangle(area.Size);
			PhysicsBody.Position = area.Center;
			PhysicsBody.Rotation = impulse.RotationTo(Point.UnitX);
			PhysicsBody.IsStatic = false;
			PhysicsBody.ApplyLinearImpulse(impulse);
			Damage = damage;
		}

		public PhysicsBody PhysicsBody { get; private set; }
		public int Damage { get; private set; }
	}
}