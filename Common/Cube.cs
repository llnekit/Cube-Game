using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Cube
    {
        private CubeSurface[] _sides;
        private Vector3 _position;
        //private Vector3 _rotation;
        private int _mainSide;
        public Cube(CubeSurface[] sides)
        {
            if (sides.Length != 6) throw new Exception("Ошибка, у куба должно быть 6 граней");
            _sides = sides;
            _mainSide = 2;
        }

        public Vector3 Position { get { return _position; } set { _position = value; } }
        //public Vector3 Rotation { get { return _rotation; } set { _rotation = value; } }



        public int MainSide 
        {
            get { return _mainSide; } 
            set 
            {
                if (value < 0 || value > 6) throw new Exception("ERROR");
                var tmp = _sides[value].Texture;
                _sides[value].Texture = _sides[_mainSide].Texture;
                _sides[_mainSide].Texture = tmp;

                _mainSide = value;
                //_sides[_mainSide].BindTexture(Texture.LoadFromFile("Resources/red.png"));
            } 
        }

        public void Draw(bool blend = false)
        {
            //MainSide = 0;
            var model = Matrix4.CreateTranslation(Position);
           /* model = model * Matrix4.CreateTranslation(-Position);
            model = model * Matrix4.CreateRotationX(Rotation.X);
            model = model * Matrix4.CreateRotationY(Rotation.Y);
            model = model * Matrix4.CreateRotationZ(Rotation.Z);
            model = model * Matrix4.CreateTranslation(Position);*/
            _sides.First().Shader.SetMatrix4("model", model);

            if (blend)
            {
                _sides[MainSide].Draw();
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
                for (int i = 0; i < _sides.Length; i++)
                    if (i != MainSide) _sides[i].Draw();
                GL.Disable(EnableCap.Blend);
            }
            else
                foreach (var surface in _sides) 
                    surface.Draw();
        }
    }
}
