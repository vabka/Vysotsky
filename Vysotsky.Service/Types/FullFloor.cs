using System;

namespace Vysotsky.Service.Types
{
    public class FullFloor
    {
        public long Id { get; init; }
        public string Number { get; set; } = null!;
        public Room[] Rooms { get; set; } = Array.Empty<Room>();
    }
}