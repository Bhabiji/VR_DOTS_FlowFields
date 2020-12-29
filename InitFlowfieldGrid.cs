    using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
//To split world up in grid with valued tiles

//for Dynamic buffers/bufferElementStructs/Archetypes/Entities
//https://docs.unity3d.com/Packages/com.unity.entities@0.0/manual/dynamic_buffers.html#:~:text=A%20DynamicBuffer%20is%20a%20type,the%20internal%20capacity%20is%20exhausted.

//custom dynamic buffer element for entities
public struct EBufferElement : IBufferElementData
{
    public Entity entity;

    public static implicit operator Entity (EBufferElement bE){ return bE.entity; }
    public static implicit operator EBufferElement(Entity e) { return new EBufferElement { entity = e }; }

}



public struct FPoint3
{
    float x, y, z;
    //To init with values in C# structs
    public FPoint3(float posX, float posY, float posZ)
    {
        x = posX;
        y = posY;
        z = posZ;
    }
}

public struct IPoint2
{
    float x, y;
    //To init with values in C# structs
    public IPoint2(float posX, float posY)
    {
        x = posX;
        y = posY;
    }
}

public struct EndNode
{
    IPoint2 idx;
    //To init with values in C# structs
    public EndNode(IPoint2 newIdx)
    {
        idx = newIdx;
    }
}
public class InitFlowfieldGrid : SystemBase
{

     private struct NodeInfo
    {
        public float colSize, rowSize;
        public int posX, posY;
        //unsigned char in C# (insert unfunny C++ masterrace joke: C# has no "char"acter)
        public byte cost; 
    };


    private static EntityArchetype m_NodeArchetype;
    private EntityCommandBuffer m_EntityCommandBuffer;

    private EntityCommandBufferSystem m_EntityBufferSystem;
    private DynamicBuffer<Entity> m_EntityBuffer;

    protected void OnCreate()
    {
        m_EntityBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        m_NodeArchetype = EntityManager.CreateArchetype(typeof(NodeData));
    }

    // Update is called once per frame
    void OnUpdate()
    {

        //Updated improvement for scheduling and giving jobs to your entities
        EntityJobScheduling();
    }

    void EntityJobScheduling()
    {
        m_EntityCommandBuffer = m_EntityBufferSystem.CreateCommandBuffer();
        //Lambda doesnt run in normal C#, Unity converts it in a more performant job system construct and burst for better PC resource usage
        Entities.ForEach((Entity entity, in UserData userData,  in UpdatedFFData updatedFFData) =>
        {
           

            m_EntityCommandBuffer.RemoveComponent<UpdatedFFData>(entity);
            DynamicBuffer<EBufferElement> eBuffer = updatedFFData.exists ? GetBuffer<EntityCommandBufferManagedComponentExtensions>(entity) : m_EntityCommandBuffer.AddBuffer<EBufferElement>(entity);
            DynamicBuffer<Entity> m_EntityBuffer = eBuffer.Reinterpret<Entity>();

            InitGrid(updatedFFData);

            //From Elite framework
            IPoint2 idxInWorld = GetNodeIdxFromWorldPos(userData.GetClickedPos(), IPoint2(nrOfCols,nrOfRows), colSize, rowSize );
            EndNode endNode = new EndNode(idxInWorld);
            if(!updatedFFData.exists)
            {
                m_EntityCommandBuffer.AddComponent<EndNode>(entity);
            }
            m_EntityCommandBuffer.SetComponent(entity, endNode);
            m_EntityCommandBuffer.AddComponent<TagObstacleCalc>(entity);

        } ).Run();
    }

    void InitGrid(UpdatedFFData updatedFFData)
    {
        NodeInfo newNodeData;
        newNodeData.colSize = userData.GetColSize;
        newNodeData.rowSize = userData.GetRowSize;

        int nrOfCols = userData.gridSize.nrOfCols;
        int nrOfRows = userData.gridSize.nrOfRows;

        //InitGrid
        for (int i = 0; i < nrOfRows; i++)
        {
            for (int j = 0; j < nrOfCols; j++)
            {
                //implement costField
                //currCell.cost = 
                //unity Y axis == up in 3D space, Z == up in 2D space
                FPoint3 cellPos = new FPoint3(userData.GetRowSize() * i + j * userData.GetRowSize(), 0, i * userData.GetRowSize());

                NodeInfo Cell = new NodeInfo(cellPos, new IPoint2(i, j));
                Entity newCell;
                if(!updatedFFData.exists)
                {
                    //if not existing ff, create entirely new entity of the correct archetype, add the entity to the entire buffer that'll manage the entities
                    newNode = m_EntityCommandBuffer.CreateEntity(m_NodeArchetype);
                    m_EntityBuffer.Add(newNode);
                }
                else
                {
                    //https://www.youtube.com/watch?v=XC84bc95heI&ab_channel=CodeMonkey index needs to be converted to 1D for dynamic entitybuffer
                    newNode = m_EntityBuffer[To1DIdx(i, j, nrOfCols)];
                }

                m_EntityCommandBuffer.SetComponent(newNode, newNodeData);
            }

        }

      


    }

    //Convert to 1D array idx
    int To1DIdx(int x, int y, int totalRows)
    {
        return x * totalRows + y;
    }
}
