using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
//NODE ENTITY COMPONENTS
//attach to entity, can do tasks on each entity with NodeComponent
public struct NodeComponent : IComponentData
{
    public IPoint2 idx;
    public FPoint3 pos;
    //No unsigned char --> byte == 255 max
    public byte travelCost;
    public int bestTravelCost;
    public IPoint2 direction;

    public NodeComponent(IPoint2 newIdx, FPoint3 newPos, byte newCost, int newBestCost, IPoint2 newDir )
    {
        idx = newIdx;
        pos = newPos;
        //No unsigned char --> by
        travelCost = newCost;
        bestTravelCost = newBestCost;
        direction = newDir;
    }

}

//TAG FOR END NODE IDENTIFICATION
public struct EndNodeComponent : IComponentData
{
    public IPoint2 idx;

    public EndNodeComponent(IPoint2 newIdx)
    {
        idx = newIdx;
    }

}

//FOR MOVEMENT ENTITY OPERATIONS
public struct MovementComponent : IComponentData
{
    public float speed;
    public bool hasReachedEndNode;

    public MovementComponent(float newSpeed, bool isAtEnd)
    {
        speed = newSpeed;
        hasReachedEndNode = isAtEnd;
    }

}

public struct FlowFieldComponent : IComponentData
{
    public bool isValid;
    public IPoint2 gridSize;
    public float colSize;
    public float rowSize;

    public FlowFieldComponent(bool setValid, IPoint2 size, float newColSize, float newRowSize )
    {
        isValid = setValid;
        gridSize = size;
        colSize = newColSize;
        rowSize = newRowSize;
    }

}

//authoring component allows you to add an IComponentData directly to a GameObject in a scene within the Unity Editor.
//https://docs.unity3d.com/Packages/com.unity.entities@0.2/manual/gp_overview.html#authoring-overview
[GenerateAuthoringComponent]
public struct FlowFieldControllerComponent : IComponentData
{
    public IPoint2 gridSize;
    public float colSize;
    public float rowSize;

    public FlowFieldControllerComponent(IPoint2 newGridSize, float newColSize, float newRowSize)
    {
        gridSize = newGridSize;
        colSize = newColSize;
        rowSize = newRowSize;
    }
}
