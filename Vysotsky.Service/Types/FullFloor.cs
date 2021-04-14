using System;
using System.Collections.Generic;

namespace Vysotsky.Service.Types
{
    public class FullFloor
    {
        public long Id { get; init; }
        public string Number { get; set; } = null!;
        public IEnumerable<Room> Rooms { get; set; } = Array.Empty<Room>();
    }
}