namespace xLibV100.Transceiver
{
    /// <summary>
    /// интерфейс предостовляющий функции для адаптации принятых данных для объекта
    /// </summary>
    public interface IResponseAdapter
    {
        /// <summary>
        /// функция приема данных для парсинга
        /// </summary>
        /// <param name="manager">предоставляет объект менеджера приема данных</param>
        /// <param name="content">предостовляет пакет принятых данные</param>
        /// <returns></returns>
        object Recieve(RxPacketManager manager, xContent content);
    }
}
