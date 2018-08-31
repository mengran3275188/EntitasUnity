using System;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Data
{
    [Chunk]
    public class ChunkComponent : IComponent
    {
        public IChunk Value;
    }
    [Chunk, Unique]
    public sealed class ActiveChunkComponent : IComponent
    {
    }
}
