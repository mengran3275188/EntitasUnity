using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public interface IChunk
    {
    }
    public interface IChunkDoor
    {
        IChunk Chunk { get; }
    }
    public interface IChunkTrigger
    {
        IChunk Chunk { get; }
    }
}

