using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Project_Uno2
{

    public class Tile
    {
        public Vector2 _position;
        public Rectangle _hitbox;
        public string _type;
        public bool _solid;
        public Tile(Vector2 position)
        {
            _position = position;
            _type = "Floor";
            _solid = true;
            _hitbox = new Rectangle((int)_position.X - 16, (int)_position.Y - 16, 32, 16);
        }
        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, ContentManager content)
        {
            spriteBatch.Draw(content.Load<Texture2D>("Tiles/" + _type), new Vector2(_position.X - 16, _position.Y - 16), Color.White);
        }
    }
}
