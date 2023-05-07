// Adam Dernis 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderSharp.RayTracing.Models.BVH;
using RenderSharp.RayTracing.RayCasts;
using System.Numerics;

namespace RenderSharp.Tests.RayTracing.Collision;

[TestClass]
public class AABBCollisionTests
{
    [TestMethod("AABB Collision 1 - Hit")]
    public void AABBCollisionTest1()
    {
        var ray = new Ray
        {
            origin = new Vector3(3,3,3),
            direction = Vector3.Normalize(new Vector3(-1, -1, -1)),
        };
        var box = new AABB
        {
            highCorner = Vector3.One,
            lowCorner = Vector3.Zero,
        };

        TestCollision(ray, box, true);
    }

    [TestMethod("AABB Collision 2 - Miss")]
    public void AABBCollisionTest2()
    {
        var ray = new Ray
        {
            origin = new Vector3(3,3,3),
            direction = Vector3.Normalize(new Vector3(0, -1, -1)),
        };
        var box = new AABB
        {
            highCorner = Vector3.One,
            lowCorner = Vector3.Zero,
        };

        TestCollision(ray, box, false);
    }

    [TestMethod("AABB Collision 3 - Hit")]
    public void AABBCollisionTest3()
    {
        var ray = new Ray
        {
            origin = new Vector3(0,1,0),
            direction = Vector3.Normalize(new Vector3(0, -1, -1)),
        };
        var box = new AABB
        {
            highCorner = Vector3.One,
            lowCorner = Vector3.Zero,
        };

        TestCollision(ray, box, false);
    }

    private static void TestCollision(Ray ray, AABB box, bool expected)
    {
        var result = AABB.IsHit(box, ray, float.MaxValue, float.Epsilon);
        Assert.AreEqual(expected, result);
    }
}
