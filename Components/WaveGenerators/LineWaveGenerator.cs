using System;
using xLibV100.Controls;

namespace xLibV100.Common.WaveGenerators
{
    /// <summary>
    /// предостовляет свойства и методы для генерирование линейных графиков
    /// </summary>
    public class LineWaveGenerator : WaveGenerator
    {
        protected double amplitude = 1000.0;
        protected double frequency = 1.0;
        protected double angle = 0.0;
        protected double phase = 0;
        protected double offset;

        public LineWaveGenerator()
        {
            Name = "Line wave";
        }

        public override double[] Generate()
        {
            double[] result = new double[points];
            double step = amplitude / points;
            double anglePhase = 2 * Math.PI * phase / 360;

            for (int i = 0; i < points; i++)
            {
                double angle = i * step;
                result[i] = offset + amplitude * Math.Sin(frequency * angle + anglePhase);
            }

            return result;
        }

        [ModelProperty]
        public double Amplitude
        {
            get => amplitude;
            set
            {
                if (value != amplitude)
                {
                    amplitude = value;
                    OnPropertyChanged(nameof(Amplitude), amplitude);
                }
            }
        }

        [ModelProperty]
        public double Frequency
        {
            get => frequency;
            set
            {
                if (value != frequency)
                {
                    frequency = value;
                    OnPropertyChanged(nameof(Frequency), frequency);
                }
            }
        }

        [ModelProperty]
        public double Phase
        {
            get => phase;
            set
            {
                if (value != phase)
                {
                    phase = value;
                    OnPropertyChanged(nameof(Phase), phase);
                }
            }
        }

        [ModelProperty]
        public double Offset
        {
            get => offset;
            set
            {
                if (value != offset)
                {
                    offset = value;
                    OnPropertyChanged(nameof(Offset), offset);
                }
            }
        }
    }
}
