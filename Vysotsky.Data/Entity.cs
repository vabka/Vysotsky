using System;

namespace Vysotsky.Data
{
    public abstract class Entity
    {
        public long Id { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}