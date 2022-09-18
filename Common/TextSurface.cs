using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TextSurface : Primetive
    {
        TextRenderer _text;
        Font _font;
        Brush _defaultFontColor;
        Dictionary<string, Brush> _currentText = new Dictionary<string, Brush>();

        public TextSurface(Shader shader, string path, float[] vertices) : base(shader, path, vertices)
        {
            BindVAO();
            _text = new TextRenderer(900, 900);
            
            _defaultFontColor = Brushes.Black;
            _currentText.Add("", _defaultFontColor);
            _font = new Font(FontFamily.GenericSerif, 12);
            
            RecreateTexture();
        }

        public Font Font { get { return _font; } set { _font = value; RecreateTexture(); } }

        public Brush Brush { get { return _defaultFontColor; } set { _defaultFontColor = value; RecreateTexture(); } }

        private void RecreateTexture()
        {
            _text.Clear();
            PointF pos = new PointF(0.0f, 0.0f);
            foreach (var item in _currentText)
            {
                _text.DrawString(item.Key, _font, item.Value, pos);
                pos.Y += _font.Height;
            }
            
            Texture = new Texture(_text.Texture);
        }

        public List<string> Text => _currentText.Keys.ToList();

        public void AppendText(string text)
        {
            _currentText.Add(text, _defaultFontColor);
            RecreateTexture();
        }

        public void Clear()
        {
            _currentText.Clear();
            _currentText.Add("", _defaultFontColor);
            RecreateTexture();
        }
        

        public void AppendText(string text, Brush brush)
        {
            _currentText.Add(text, brush);
            RecreateTexture();
        }

        public override void BindVAO()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            var positionLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            // Enable variable 0 in the shader.
            GL.EnableVertexAttribArray(0);
        }

        public override void Draw()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            base.Draw();
            GL.Disable(EnableCap.Blend);// выключает прозрачность 
        }
    }
}
