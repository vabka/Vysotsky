namespace Vysotsky.API.Controllers.Rooms
{
    /// <summary>
    /// Состояние помещения
    /// </summary>
    public enum RoomStatus
    {
        /// <summary>
        /// Недоступно
        /// </summary>
        Inaccessible,
        /// <summary>
        /// Свободно
        /// </summary>
        Free,
        /// <summary>
        /// В аренде
        /// </summary>
        Rent,
        /// <summary>
        /// Выкуплено
        /// </summary>
        Owned
    }
}