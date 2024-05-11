using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Akira.Models
{
    public class Texture2D
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        private uint _textureObject;

        // Генерация массива объектов текстуры
        public void Create(OpenGL gl)
        {
            uint[] ids = new uint[1];
            gl.GenTextures(1, ids);
            _textureObject = ids[0];
        }

        public void Delete(OpenGL gl)
        {
            gl.DeleteTextures(1, new[] { _textureObject });
            _textureObject = 0;
        }

        public void Bind(OpenGL gl) =>
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _textureObject);

        public void Unbind(OpenGL gl) =>
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

        public void SetPatemeter(OpenGL gl, uint parameterName, uint parameterValue) =>
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, parameterName, parameterValue);

        // Метод для создания текстуры из изображения
        public void SetImage(OpenGL gl, Bitmap image)
        {
            // Получаем максимальный размер текстуры, поддерживаемый OpenGL
            int[] textureMaxSize = { 0 };
            gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, textureMaxSize);

            // Просчитываем целевые размеры ширины и высоты
            int targetWidth = textureMaxSize[0];
            int targetHeight = textureMaxSize[0];

            for (int size = 1; size <= textureMaxSize[0]; size *= 2)
            {
                if (image.Width < size)
                {
                    targetWidth = size / 2;
                    break;
                }

                if (image.Width == size)
                {
                    targetWidth = size;
                }
            }

            for (int size = 1; size <= textureMaxSize[0]; size *= 2)
            {
                if (image.Height < size)
                {
                    targetHeight = size / 2;
                    break;
                }

                if (image.Height == size)
                {
                    targetHeight = size;
                }
            }

            // Масштабируем при необходимости
            bool destroyImage = false;
            if (image.Width != targetWidth || image.Height != targetHeight)
            {
                // Изменяем размер изображения
                Image newImage = image.GetThumbnailImage(targetWidth, targetHeight, null, IntPtr.Zero);
                image = (Bitmap)newImage;
                destroyImage = true;
            }

            // Блокируем биты изображения (для передачи их в OGL)
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Устанавливаем ширину и высоту
            Width = (uint)image.Width;
            Height = (uint)image.Height;

            // Привязка текстуры (Делаем его текущей текстурой)
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _textureObject);

            // Устанавливаем данные изображения
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA,
                (int)Width, (int)Height, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE,
                bitmapData.Scan0);

            // Разблокируем изображение
            image.UnlockBits(bitmapData);

            // Удаляем файл изображения, если это промежуточный файл
            if (destroyImage)
            {
                image.Dispose();
            }
        }
    }
}
