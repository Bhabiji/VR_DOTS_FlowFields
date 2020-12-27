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

public class InitFlowfieldGrid : SystemBase
{

     private struct CellInfo
    {
        public float colSize, rowSize;
        public int posX, posY;
    };
    private static EntityArchetype m_GridArchetype;

    private EntityCommandBufferSystem m_EntityBufferSystem;
 
    protected void OnCreate()
    {
        m_EntityBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        m_GridArchetype = EntityManager.CreateArchetype(typeof(CellData));
    }

    // Update is called once per frame
    void OnUpdate()
    {

        //Updated improvement for scheduling and giving jobs to your entities
        EntityJobScheduling();
    }

    void EntityJobScheduling()
    {
        EntityCommandBuffer cBuffer = m_EntityBufferSystem.CreateCommandBuffer();
        //Lambda doesnt run in normal C#, Unity converts it in a more performant job system construct and burst for better PC resource usage
        Entities.ForEach((Entity entity, in FFData fFData,  in UpdatedFFData updatedFFData) =>
        {

            InitGrid(); 
            cBuffer.RemoveComponent<UpdatedFFData>(entity);
        } );
    }

    void InitGrid()
    {
        CellInfo currCellData;
        currCellData.colSize = fFData.GetColSize;
        currCellData.rowSize = fFData.GetRowSize;

        currCellData;

    }
}
