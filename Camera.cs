using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Uno2
{
    public class Camera
    {
        public Matrix _transform;
        private Vector2 _position;
        private readonly Viewport _viewport;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            _position = Vector2.Zero;
            _transform = Matrix.Identity;
        }

        public void Follow(Vector2 targetPosition)
        {
            // Directly center the camera on the player
            _position = Vector2.Lerp(_position, targetPosition - new Vector2(_viewport.Width / 2f, _viewport.Height / 2f), 0.04f);
        }

        public void Update()
        {
            // Just translate based on camera position
            _transform = Matrix.CreateTranslation(new Vector3(-_position, 0f));
        }
    }
}