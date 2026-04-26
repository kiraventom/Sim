using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;
using Sim.Utils;

namespace Sim.Model;

internal class PathBuilder(ILogger<PathBuilder> logger, Map map, RaycasterFactory raycasterFactory)
{
    private const double EVADE_DISTANCE_MODIFIER = 2.0;

    public void BuildPath(Movable movable)
    {
        var evadeDist = GetEvadeDistance(movable);
        var raycaster = raycasterFactory.Build([movable.Id]);
        SplitLine(raycaster, movable, movable.Path.StartNode, movable.Path.EndNode, evadeDist);
    }

    private void SplitLine(Raycaster raycaster, Movable movable, LinkedListNode<Point> start, LinkedListNode<Point> end, Size evadeDist)
    {
        var result = raycaster.Cast(start.Value, end.Value);
        if (result.Hits.Count == 0)
            return;

        var hit = result.Hits[0];

        var prevNode = start;

        var objRect = map[hit.Id];
        var hitRect = BuildHitRect(hit);
        var corners = SplitIntoCorners(hit, hitRect, objRect);

        var inflatedObjRect = new Rect(objRect.Left - evadeDist.Width, objRect.Top - evadeDist.Height, objRect.Right + evadeDist.Width, objRect.Bottom + evadeDist.Height);
        foreach (var corner in corners)
        {
            var targetPoint = GetTargetPoint(corner, objRect, inflatedObjRect, hit, movable.Size);
            if (targetPoint.IsInvalid())
            {
                logger.LogError("Generated target point {Target} is not accessible by {Id}, path is probably broken!", targetPoint, movable.Id);
                continue;
            }

            var newNode = movable.Path.AddAfter(prevNode, targetPoint);
            SplitLine(raycaster, movable, prevNode, newNode, evadeDist);
            prevNode = newNode;
        }

        SplitLine(raycaster, movable, prevNode, end, evadeDist);
    }

    private Point GetTargetPoint(Rect corner, Rect objRect, Rect inflatedObjRect, RaycastHit hit, Size movableSize)
    {
        var mainPoint = GetMainPoint(corner, objRect, inflatedObjRect);
        if (mainPoint.IsInvalid())
        {
            logger.LogInformation("Failed to get target point for {Corner} and {ObjRect}, path is probably broken!", corner, objRect);
            return Point.INVALID;
        }

        var point = mainPoint;

        for (int i = 0; i < 2; ++i)
        {
            var targetRect = new Rect(point, movableSize);
            Rect.ClampToMap(ref targetRect);

            var grid = map.GetAreaGrid(targetRect);
            if (map.CanPlace(grid, targetRect))
                return point;

            point = GetAltPoint(objRect, inflatedObjRect, mainPoint, hit);
        }

        return Point.INVALID;
    }

    private Size GetEvadeDistance(Movable movable) => movable.Size * EVADE_DISTANCE_MODIFIER;

    private Point GetAltPoint(Rect objectRect, Rect inflatedRect, Point mainPoint, RaycastHit hit)
    {
        if (CMP.Equals(mainPoint, objectRect.TopLeft, Raycaster.RAYCAST_STEP))
        {
            if (CMP.Equals(hit.Enter.Y, objectRect.Top, Raycaster.RAYCAST_STEP))
                return inflatedRect.TopRight;
            else
                return inflatedRect.BottomLeft;
        }

        if (CMP.Equals(mainPoint, objectRect.TopRight, Raycaster.RAYCAST_STEP))
        {
            if (CMP.Equals(hit.Enter.Y, objectRect.Top, Raycaster.RAYCAST_STEP))
                return inflatedRect.TopLeft;
            else
                return inflatedRect.BottomRight;
        }

        if (CMP.Equals(mainPoint, objectRect.BottomLeft, Raycaster.RAYCAST_STEP))
        {
            if (CMP.Equals(hit.Enter.Y, objectRect.Bottom, Raycaster.RAYCAST_STEP))
                return inflatedRect.BottomRight;
            else
                return inflatedRect.TopLeft;
        }

        if (CMP.Equals(mainPoint, objectRect.BottomRight, Raycaster.RAYCAST_STEP))
        {
            if (CMP.Equals(hit.Enter.Y, objectRect.Bottom, Raycaster.RAYCAST_STEP))
                return inflatedRect.BottomLeft;
            else
                return inflatedRect.TopRight;
        }

        logger.LogWarning("Failed to get alt point for {MainPoint}, {ObjRect}, {Hit}", mainPoint, objectRect, hit);
        return Point.INVALID;
    }

    private Point GetMainPoint(Rect cornerRect, Rect objectRect, Rect inflatedRect)
    {
        if (CMP.Equals(cornerRect.TopLeft, objectRect.TopLeft, Raycaster.RAYCAST_STEP))
            return inflatedRect.TopLeft;

        if (CMP.Equals(cornerRect.TopRight, objectRect.TopRight, Raycaster.RAYCAST_STEP))
            return inflatedRect.TopRight;

        if (CMP.Equals(cornerRect.BottomLeft, objectRect.BottomLeft, Raycaster.RAYCAST_STEP))
            return inflatedRect.BottomLeft;

        if (CMP.Equals(cornerRect.BottomRight, objectRect.BottomRight, Raycaster.RAYCAST_STEP))
            return inflatedRect.BottomRight;

        logger.LogWarning("Failed to get main point for {Corner} and {ObjRect}", cornerRect, objectRect);
        return Point.INVALID;
    }

    private IEnumerable<Rect> SplitIntoCorners(RaycastHit hit, Rect hitRect, Rect objectRect)
    {
        // adjust to make sure it touches at least one corner
        var leftOffset = hitRect.Left - objectRect.Left;
        var topOffset = hitRect.Top - objectRect.Top;
        var rightOffset = hitRect.Right - objectRect.Right;
        var bottomOffset = hitRect.Bottom - objectRect.Bottom;

        if (CMP.Equals(leftOffset, 0, Raycaster.RAYCAST_STEP) || CMP.Equals(rightOffset, 0, Raycaster.RAYCAST_STEP))
        {
            if (!CMP.Equals(topOffset, 0, Raycaster.RAYCAST_STEP) && !CMP.Equals(bottomOffset, 0, Raycaster.RAYCAST_STEP))
            {
                if (Math.Abs(topOffset) < Math.Abs(bottomOffset))
                {
                    var newRect = new Rect(hitRect.Left, objectRect.Top, hitRect.Right, hitRect.Bottom);
                    hitRect = newRect;
                }
                else
                {
                    var newRect = new Rect(hitRect.Left, hitRect.Top, hitRect.Right, objectRect.Bottom);
                    hitRect = newRect;
                }
            }
        }

        if (CMP.Equals(topOffset, 0, Raycaster.RAYCAST_STEP) || CMP.Equals(bottomOffset, 0, Raycaster.RAYCAST_STEP))
        {
            if (!CMP.Equals(leftOffset, 0, Raycaster.RAYCAST_STEP) && !CMP.Equals(rightOffset, 0, Raycaster.RAYCAST_STEP))
            {
                if (Math.Abs(leftOffset) < Math.Abs(rightOffset))
                {
                    var newRect = new Rect(objectRect.Left, hitRect.Top, hitRect.Right, hitRect.Bottom);
                    hitRect = newRect;
                }
                else
                {
                    var newRect = new Rect(hitRect.Left, hitRect.Top, objectRect.Right, hitRect.Bottom);
                    hitRect = newRect;
                }
            }
        }

        // If ray hits opposite sides of the objectRect, the hitRect needs to be split into two "corner" rects
        if (CMP.Equals(hitRect.Width, objectRect.Width, Raycaster.RAYCAST_STEP))
        {
            var leftHalf = new Rect(hitRect.Left, hitRect.Top, hitRect.Center.X, hitRect.Bottom);
            var rightHalf = new Rect(hitRect.Center.X, hitRect.Top, hitRect.Right, hitRect.Bottom);

            if (hit.Enter.X < hit.Exit.X)
            {
                foreach (var leftHalfPart in SplitIntoCorners(hit, leftHalf, objectRect))
                    yield return leftHalfPart;

                foreach (var rightHalfPart in SplitIntoCorners(hit, rightHalf, objectRect))
                    yield return rightHalfPart;
            }
            else
            {
                foreach (var rightHalfPart in SplitIntoCorners(hit, rightHalf, objectRect))
                    yield return rightHalfPart;

                foreach (var leftHalfPart in SplitIntoCorners(hit, leftHalf, objectRect))
                    yield return leftHalfPart;
            }

            yield break;
        }

        if (CMP.Equals(hitRect.Height, objectRect.Height, Raycaster.RAYCAST_STEP))
        {
            var topHalf = new Rect(hitRect.Left, hitRect.Top, hitRect.Right, hitRect.Center.Y);
            var bottomHalf = new Rect(hitRect.Left, hitRect.Center.Y, hitRect.Right, hitRect.Bottom);

            if (hit.Enter.Y < hit.Exit.Y)
            {
                foreach (var topHalfPart in SplitIntoCorners(hit, topHalf, objectRect))
                    yield return topHalfPart;

                foreach (var bottomHalfPart in SplitIntoCorners(hit, bottomHalf, objectRect))
                    yield return bottomHalfPart;
            }
            else
            {
                foreach (var bottomHalfPart in SplitIntoCorners(hit, bottomHalf, objectRect))
                    yield return bottomHalfPart;

                foreach (var topHalfPart in SplitIntoCorners(hit, topHalf, objectRect))
                    yield return topHalfPart;
            }

            yield break;
        }

        yield return hitRect;
    }

    private static Rect BuildHitRect(RaycastHit hit)
    {
        var left = Math.Min(hit.Enter.X, hit.Exit.X);
        var top = Math.Min(hit.Enter.Y, hit.Exit.Y);
        var right = Math.Max(hit.Enter.X, hit.Exit.X);
        var bottom = Math.Max(hit.Enter.Y, hit.Exit.Y);

        var size = new Size(right - left, bottom - top);
        var center = new Point(left + size.Width / 2, top + size.Height / 2);

        return new Rect(center, size);
    }
}
