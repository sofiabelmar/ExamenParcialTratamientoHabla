using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Reproductor
{
    class Fades : ISampleProvider
    {
        private ISampleProvider fuente;
        private int muestrasLeidas = 0;
        private float segundosTranscurridos = 0;
        private float duracion;
        private float duracionOut;
        private float inicio;

        public Fades(ISampleProvider fuente,
            float duracion, float duracionOut,float inicio)
        {
            this.fuente = fuente;
            this.duracion = duracion;
            this.duracionOut = duracionOut;
            this.inicio = inicio;
        }
        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }
        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);


            muestrasLeidas += read;
            segundosTranscurridos =
                (float)muestrasLeidas /
                (float)fuente.WaveFormat.SampleRate /
                (float)fuente.WaveFormat.Channels;

            //FADE IN

            if (segundosTranscurridos <= duracion)
            {
                //Aplicar el efecto
                float factorEscala =
                    segundosTranscurridos /
                        duracion;
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }

            //FADE OUT
            if (segundosTranscurridos >= inicio &&
               segundosTranscurridos <= inicio + duracion)
            {
                //Aplicar el efecto
                float factorEscala =
                    1 - ((segundosTranscurridos - inicio) /
                        duracion);
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }
            else if (segundosTranscurridos >= inicio + duracion)
            {
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] = 0.0f;
                }
            }

            return read;
        }
    }
}
