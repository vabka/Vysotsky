using System;

namespace Vysotsky.API.Controllers.Rooms
{
    /// <summary>
    /// Помещение
    /// </summary>
    /// <param name="Id">Идентификатор</param>
    /// <param name="Number">Номер помещения</param>
    /// <param name="Floor">Этаж</param>
    /// <param name="Status">Состояние занятости помещения</param>
    /// <param name="CreationTime">Дата заполнения</param>
    public record Room(string Id, string Number, int Floor, RoomStatus Status, DateTime CreationTime);
}
