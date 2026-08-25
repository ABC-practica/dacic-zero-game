using EventBus;
using Surface;
using System;
using UnityEngine;

public struct EntityMoved : IEvent
{
    public SurfaceType Surface { get; set; }
    public bool IsGrounded { get; set; }
    public Vector3 Velocity { get; set; }
    public EntityMoved(SurfaceType surface, bool grounded, Vector3 velocity)
    {
        Surface = surface;
        IsGrounded = grounded;
        Velocity = velocity;
    }
}
