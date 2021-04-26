using System;
using System.Collections.Generic;

namespace Vysotsky.Service.Types
{
    public class FullBuilding
    {
        public long Id { get; init; }
        public IEnumerable<FullFloor> Floors { get; init; } = Array.Empty<FullFloor>();
        public string Name { get; init; } = "";
    }
}
