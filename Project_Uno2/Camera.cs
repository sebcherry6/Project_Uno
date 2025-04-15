using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Uno2
{
    public class Camera
    {
        public Matrix _transform;

        public Vector2 _position;

        public Viewport _viewport;

        public Camera(Viewport viewport)
        {
            this._viewport = viewport;
            this._position = Vector2.Zero;
            this._transform = Matrix.Identity;
        }
        public void Update()
        {
            Matrix transform = Matrix.CreateTranslation(new Vector3(new Vector2(-_position.X + 384, -_position.Y + 384), 0));
            Matrix scale = Matrix.CreateScale(4);

            Matrix rotation = Matrix.CreateRotationZ(0);

            _transform = scale * rotation * transform;

        }

        public void ApplyProjectionMatrix(GraphicsDeviceManager graphics)
        {
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, 0, 0, 0, 0, 0);

            graphics.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Viewport = _viewport;
        }
    }
}
