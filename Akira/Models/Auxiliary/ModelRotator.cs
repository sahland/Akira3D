using SharpGL;
using System;

namespace Akira.Models.Auxiliary
{
    public class ModelRotator
    {
        private OpenGL _gl;
        private Single _angleX, _angleY, _angleZ;
        private Boolean _isRotating;

        public ModelRotator(OpenGL gl)
        {
            this._gl = gl;
            this._angleX = 0;
            this._angleY = 0;
            this._angleZ = 0;
            this._isRotating = false;
        }

        public void StartRotating()
        {
            _isRotating = true;
        }

        public void StopRotating()
        {
            _isRotating &= false;
        }

        public void Update()
        {
            if (_isRotating)
            {
                _angleX += 0.1f;
                _angleY += 0.2f;
                _angleZ += 0.3f;
            }
        }

        public Single AngleX { get { return _angleX; } }

        public Single AngleY { get { return _angleY; } }

        public Single AngleZ { get { return _angleZ; } }

}
}
