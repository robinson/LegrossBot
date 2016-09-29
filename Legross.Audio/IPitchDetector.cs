using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legross.Audio
{
    public interface IPitchDetector
    {
        float DetectPitch(float[] buffer, int frames);
    }
}
