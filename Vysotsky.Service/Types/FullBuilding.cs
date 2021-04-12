using System;

namespace Vysotsky.Service.Types
{
    public class FullBuilding
    {
        public long Id { get; init; }
        public FullFloor[] Floors { get; set; } = Array.Empty<FullFloor>();
    }
}