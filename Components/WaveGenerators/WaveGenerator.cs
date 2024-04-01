using xLibV100.Controls;

namespace xLibV100.Common.WaveGenerators
{
    /// <summary>
    /// предостовляет базовое предоставление параметров и функций генератора
    /// </summary>
    public class WaveGenerator : ModelBase
    {
        protected int points = 1000;

        public WaveGenerator()
        {
            Name = "generator";
        }

        /// <summary>
        /// функция для генерации массива
        /// </summary>
        /// <returns></returns>
        public virtual double[] Generate()
        {
            return null;
        }

        /// <summary>
        /// количество генерируемых точек
        /// </summary>
        [ModelProperty]
        public int Points
        {
            get => points;
            set
            {
                if (value < 1)
                {
                    OnPropertyChanged(nameof(Points));
                    return;
                }

                if (value != points)
                {
                    points = value;
                    OnPropertyChanged(nameof(Points), points);
                }
            }
        }
    }
}
