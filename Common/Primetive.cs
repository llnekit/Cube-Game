using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common
{
    abstract public class Primetive
    {
        protected int _VBO, _VAO;
        protected Shader _shader;
        protected Texture _texture;

        public Primetive(Shader shader, string path, float[] vertices)
        {
            _shader = shader;
            this.BindVertices(vertices);
            this.LoadTexture(path);
            if (_texture == null) throw new Exception("Текстура для плоскости не загружена");
            if (_shader == null) throw new Exception("Шейдер не загружен");
        }

        public void BindVertices(float[] vertices)
        {
            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        public void LoadTexture(string path) => _texture = Texture.LoadFromFile(path);

        public abstract void BindVAO();
        //public void BindTexture(Texture texture) => _texture = texture;

        public Shader Shader => _shader;
        
        public Texture Texture { get { return _texture; } set { _texture = value; } }

        public virtual void Draw()
        {
            GL.BindVertexArray(_VAO);
            _texture.Use(TextureUnit.Texture0);
            _texture.Use(TextureUnit.Texture1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }


    }
}
