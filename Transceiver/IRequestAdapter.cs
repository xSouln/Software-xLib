using System.Collections.Generic;

namespace xLibV100.Transceiver
{
    /// <summary>
    /// интерфейс предостовляющий функции по адаптации данных объекта для передачи
    /// </summary>
    public interface IRequestAdapter
    {
        /// <summary>
        /// получение размера запроса в байтах
        /// </summary>
        /// <returns></returns>
        int GetSize();

        /// <summary>
        /// запрос на добавление запроса в буфер
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int Add(List<byte> buffer);
    }
}
