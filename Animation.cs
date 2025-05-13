using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_Uno2
{
    public class Animation
    {
        public string _name;
        public List<Rectangle> _frames;
        public double _frameTime;

        public Animation(string name, List<Rectangle> frames, double frameTime)
        {
            _name = name;
            _frames = frames;
            _frameTime = frameTime;
        }
    }
}
