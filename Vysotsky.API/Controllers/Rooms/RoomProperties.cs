namespace Vysotsky.API.Controllers.Rooms
{
    /// <summary>
    /// Свойста для регистрации помещения
    /// </summary>
    public class RoomProperties
    {
        /// <summary>
        /// Номер помещения
        /// </summary>
        public string Number { get; init; } = "";

        /// <summary>
        /// Этаж
        /// </summary>
        public int Floor { get; init; }

        /// <summary>
        /// Состояние занятости
        /// </summary>
        public RoomStatus Status { get; init; }
    }
}
