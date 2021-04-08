namespace Vysotsky.Data
{
    public enum IssueStatus
    {
        /// <summary>
        /// Новая
        /// </summary>
        New,

        /// <summary>
        /// Отменена постановщиком
        /// </summary>
        CancelledByCustomer,

        /// <summary>
        /// Требуется уточнение
        /// </summary>
        NeedInfo,

        /// <summary>
        /// Отклонена
        /// </summary>
        Rejected,

        /// <summary>
        /// В процессе
        /// </summary>
        InProgress,

        /// <summary>
        /// Выполнена
        /// </summary>
        Completed,

        /// <summary>
        /// Принята aka подтверждена
        /// </summary>
        Accepted,

        /// <summary>
        /// Закрыта
        /// </summary>
        Closed
    }
}
