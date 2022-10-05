using Common;
using Logic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Main
{

    class Program
    {
        public const int WIDTH = 1600, HEIGHT = 900;
        // In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
        // with several point lights
        public class Window : GameWindow
        {
            private readonly float[] _vertices =
            {
                // Positions          Normals              Texture coords
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
            };

            private Vector3[] _cubePositions;

            // We need the point lights' positions to draw the lamps and to get light the materials properly
            private readonly Vector3[] _pointLightPositions =
            {
                new Vector3(-10.0f, 10.0f, 0.0f),
                new Vector3(0.0f, 10.0f, 10.0f),
                new Vector3(-10.0f, 10.0f, 10.0f),
                new Vector3(0.0f, 10.0f, 0.0f),
            };

            //private Map _gameMap;

            private int _vertexBufferObject;

            private int _vaoModel;

            private int _vaoLamp;

            private Shader _lampShader;

            private Shader _lightingShader;

            private Texture _diffuseMap;

            private Texture _redMap;

            private Texture _specularMap;

            private Camera _camera;

            private bool _firstMove = true;

            private Vector2 _lastPos;

            private Game _game1;
            private Game _game2;

            private TextSurface _textSurface;
            private TextSurface _textSurface2;

            private TextSurface _wayTextSurface;
            private TextSurface _unitedData;

            private Shader _textShader;

            Cube _playCube1;
            Cube _playCube2;

            private bool _freezeResults = false;
            private bool _isReplay = false;
            private int _replayCounter = 0;
            private readonly double waitTime = 2; // в секундах
            private double passed = 0;

            public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
                : base(gameWindowSettings, nativeWindowSettings)
            {
            }

            protected override void OnLoad()
            {
                base.OnLoad();

                _game1 = new Game(false);
                _game2 = new Game(true);

                _cubePositions = _game1.Map;

                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

                GL.Enable(EnableCap.DepthTest);

                // ============= ТЕКСТ ============= // 

                _textShader = new Shader("Shaders/textShader.vert", "Shaders/textShader.frag");
                float offset = 0.0f;
                _textSurface = new TextSurface(_textShader, "Resources/box.png", new float[]
                {
                    // Positions          Texture coords
                    -1.0f, -1.0f, -1.0f,  0.0f, 0.0f,
                     0.0f, -1.0f, -1.0f,  1.0f, 0.0f,
                     0.0f,  1.0f, -1.0f,  1.0f, 1.0f,
                     0.0f,  1.0f, -1.0f,  1.0f, 1.0f,
                    -1.0f,  1.0f, -1.0f,  0.0f, 1.0f,
                    -1.0f, -1.0f, -1.0f,  0.0f, 0.0f,
                });
                _textSurface.Font = new Font(FontFamily.GenericSansSerif, 14);
                _textSurface.Brush = Brushes.Black;

                offset = 1.0f;
                _wayTextSurface = new TextSurface(_textShader, "Resources/box.png", new float[]
                {
                    // Positions          Texture coords
                    -1.0f + offset, -1.0f, -1.0f + offset,  0.0f, 0.0f,
                     0.0f + offset, -1.0f, -1.0f + offset,  1.0f, 0.0f,
                     0.0f + offset,  1.0f, -1.0f + offset,  1.0f, 1.0f,
                     0.0f + offset,  1.0f, -1.0f + offset,  1.0f, 1.0f,
                    -1.0f + offset,  1.0f, -1.0f + offset,  0.0f, 1.0f,
                    -1.0f + offset, -1.0f, -1.0f + offset,  0.0f, 0.0f,
                });
                _wayTextSurface.Font = new Font(FontFamily.GenericSansSerif, 14);
                _wayTextSurface.Brush = Brushes.Green;

                offset = 1.4f;
                _textSurface2 = new TextSurface(_textShader, "Resources/box.png", new float[]
                {
                    // Positions          Texture coords
                    -1.0f + offset, -1.0f, -1.0f + offset,  0.0f, 0.0f,
                     0.0f + offset, -1.0f, -1.0f + offset,  1.0f, 0.0f,
                     0.0f + offset,  1.0f, -1.0f + offset,  1.0f, 1.0f,
                     0.0f + offset,  1.0f, -1.0f + offset,  1.0f, 1.0f,
                    -1.0f + offset,  1.0f, -1.0f + offset,  0.0f, 1.0f,
                    -1.0f + offset, -1.0f, -1.0f + offset,  0.0f, 0.0f,
                });
                _textSurface2.Font = new Font(FontFamily.GenericSansSerif, 14);
                //_textSurface2.Brush = Brushes.Green;

                offset = 0.8f;
                _unitedData = new TextSurface(_textShader, "Resources/box.png", new float[]
                {
                    // Positions          Texture coords
                    -1.0f + offset, -1.0f, -1.0f + offset,  0.0f, 0.0f,
                     0.0f + offset, -1.0f, -1.0f + offset,  1.0f, 0.0f,
                     0.0f + offset,  1.0f, -1.0f + offset,  1.0f, 1.0f,
                     0.0f + offset,  1.0f, -1.0f + offset,  1.0f, 1.0f,
                    -1.0f + offset,  1.0f, -1.0f + offset,  0.0f, 1.0f,
                    -1.0f + offset, -1.0f, -1.0f + offset,  0.0f, 0.0f,
                });
                _unitedData.Font = new Font(FontFamily.GenericSansSerif, 14);
                //_textSurface2.Brush = Brushes.Green;


                // ============= ТЕКСТ ============= // 

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

                _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
                _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");



                {
                    _vaoModel = GL.GenVertexArray();
                    GL.BindVertexArray(_vaoModel);

                    var positionLocation = _lightingShader.GetAttribLocation("aPos");
                    GL.EnableVertexAttribArray(positionLocation);
                    GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                    var normalLocation = _lightingShader.GetAttribLocation("aNormal");
                    GL.EnableVertexAttribArray(normalLocation);
                    GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                    var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
                    GL.EnableVertexAttribArray(texCoordLocation);
                    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                }

                {
                    _vaoLamp = GL.GenVertexArray();
                    GL.BindVertexArray(_vaoLamp);

                    var positionLocation = _lampShader.GetAttribLocation("aPos");
                    GL.EnableVertexAttribArray(positionLocation);
                    GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                }

                _diffuseMap = Texture.LoadFromFile("Resources/box.png");
                _specularMap = Texture.LoadFromFile("Resources/container2_specular.png");
                _redMap = Texture.LoadFromFile("Resources/red.png");


                _camera = new Camera(new Vector3(1.0343208f, 3.559164f, 3.573409f), Size.X / (float)Size.Y);
                _camera.Pitch = -46.798386f;
                _camera.Yaw = 270.59592f;
                CursorState = CursorState.Grabbed;

                _playCube1 = new Cube(new CubeSurface[]
                {
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                         0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
                         0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/red.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                         0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                         0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                         0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
                    }),
                });
                _playCube2 = new Cube(new CubeSurface[]
                {
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                         0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
                         0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
                        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/red.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                         0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                         0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                    }),
                    new CubeSurface (_lightingShader, "Resources/box.png", new float[]
                    {
                        // Positions          Normals              Texture coords
                        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                         0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
                    }),
                });

                _playCube1.MainSide = (int)_game1.CurrentSide;
                _playCube2.MainSide = (int)_game1.FinishSide;
               
            }

            public void DrawFloor()
            {
                GL.BindVertexArray(_vaoModel);

                _diffuseMap.Use(TextureUnit.Texture0);
                _specularMap.Use(TextureUnit.Texture1);
                _lightingShader.Use();

                _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
                _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _lightingShader.SetVector3("viewPos", _camera.Position);

                _lightingShader.SetInt("material.diffuse", 0);
                _lightingShader.SetInt("material.specular", 1);
                _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                _lightingShader.SetFloat("material.shininess", 32.0f);
                _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
                _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
                _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

                // Point lights
                for (int i = 0; i < _pointLightPositions.Length; i++)
                {
                    _lightingShader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
                    _lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                    _lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                    _lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                    _lightingShader.SetFloat($"pointLights[{i}].constant", 1.0f);
                    _lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                    _lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
                }

                for (int i = 0; i < _cubePositions.Length; i++)
                {
                    Matrix4 model = Matrix4.CreateTranslation(_cubePositions[i]);
                    //float angle = 20.0f * i;
                    //model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                    _lightingShader.SetMatrix4("model", model);

                    if (_cubePositions[i] == _game1.EndPos)
                    {
                        _redMap.Use(TextureUnit.Texture0);
                        _redMap.Use(TextureUnit.Texture1);
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                        _diffuseMap.Use(TextureUnit.Texture0);
                        _specularMap.Use(TextureUnit.Texture1);
                    }
                    else
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }


            public bool CheckBidirectionalSearch (double time)
            {
                _game1.GetAIFirsNextStep(time);
                _game2.GetAIFirsNextStep(time);

                

                if (_game1.CheckBidirSearch(_game2.AI) || _game2.CheckBidirSearch(_game1.AI))
                {
                    _game1.CreateAIFinishInfo(true);
                    _game2.CreateAIFinishInfo(false);
                    UnitedInfo.Clear();
                    _unitedData.Clear();
                    UnitedInfo = _game1.CreateUnitedInfo(_game2.AI);
                    return true;
                }

                _game1.GetAISecodNextStep();
                _game2.GetAISecodNextStep();

                return false;
            }

            List<string> UnitedInfo = new List<string>();
            bool displayUnite = true;
            protected override void OnRenderFrame(FrameEventArgs e)
            {
                base.OnRenderFrame(e);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                DrawFloor();

                // ==================== ОТРИСОВКА ИГРОВОГО КУБИКА ==================== // 
                {
                    

                    _playCube1.Position = _game1.PlayerPos;
                    _playCube2.Position = _game2.PlayerPos;
                    _playCube1.Draw(true);
                    _playCube2.Draw(true);

                    if (!_freezeResults)
                    {
                        _textSurface.Clear();
                        _textSurface2.Clear();
                        _textSurface.AppendText($"Текущее состояние {_game1.CurrentState}");
                        _textSurface2.AppendText($"Текущее состояние {_game2.CurrentState}");
                    }

                    if (_game1.AIIsWorking && _game2.AIIsWorking)
                    {
                        _textSurface.AppendText($"Прошедшее время: {_game1.AIWorkingTime.ToString("F2")}");
                        _textSurface2.AppendText($"Прошедшее время: {_game2.AIWorkingTime.ToString("F2")}");
                        

                        //_game2.GetAINextStep(e.Time);
                        if (CheckBidirectionalSearch(e.Time))
                        {
                            _freezeResults = true;
                            _textSurface.Clear();
                            _textSurface.AppendText($"Текущее состояние {_game1.CurrentState}");
                            _textSurface.AppendText($"Прошедшее время: {_game1.AIWorkingTime.ToString("F2")}");
                            _textSurface.AppendText($"Информация: ");
                            foreach (var str in _game1.AIInfo)
                                _textSurface.AppendText(str);

                            _textSurface.AppendText($"Найденный путь:");

                            foreach (var step in _game1.WayToFinish)
                                _textSurface.AppendText($"{step.dirToThisState} => {step.value}");


                            ////////////////////////////

                            _textSurface2.Clear();
                            _textSurface2.AppendText($"Текущее состояние {_game2.CurrentState}");
                            _textSurface2.AppendText($"Прошедшее время: {_game2.AIWorkingTime.ToString("F2")}");
                            _textSurface2.AppendText($"Информация: ");
                            foreach (var str in _game2.AIInfo)
                                _textSurface2.AppendText(str);

                            _textSurface2.AppendText($"Найденный путь:");

                            foreach (var step in _game2.WayToFinish)
                                _textSurface2.AppendText($"{step.dirToThisState} => {step.value}");

                            //////////////////////////////

                            foreach(var item in UnitedInfo)
                                _unitedData.AppendText(item);
                            _unitedData.AppendText("Полный путь:");

                            foreach (var step in _game1.WayToFinish)
                                _unitedData.AppendText($"{step.dirToThisState} => {step.value}");
                            foreach (var step in _game2.WayToFinish)
                                _unitedData.AppendText($"{step.dirToThisState} => {step.value}");



                            _textSurface.AppendText("Press Y to Restart");
                            _textSurface.AppendText("Press T to StartPose");
                            _textSurface.AppendText("Press R to Replay");
                        }

                        _playCube1.MainSide = (int)_game1.CurrentSide;
                        _playCube2.MainSide = (int)_game2.CurrentSide;
                    }
                    else if (!_freezeResults) _textSurface.AppendText("Press U to start AI");



                    if (_isReplay)
                    {
                        if (passed >= waitTime)
                        {
                            if (_game1.WayToFinish.Count == 0)
                            {
                                _isReplay = false;
                                _replayCounter = 0;
                            }
                            else
                            {
                                _wayTextSurface.Clear();
                                for (int i = 0; i < _game1.WayToFinish.Count; i++)
                                {
                                    _wayTextSurface.AppendText($"{_game1.WayToFinish[i].dirToThisState} => {_game1.WayToFinish[i].value}", i == _replayCounter ? Brushes.Yellow : Brushes.Black);
                                }

                                for (int i = 0; i < _game2.WayToFinish.Count; i++)
                                {
                                    _wayTextSurface.AppendText($"{_game2.WayToFinish[i].dirToThisState} => {_game2.WayToFinish[i].value}", i == _replayCounter - _game1.WayToFinish.Count? Brushes.Yellow : Brushes.Black);
                                }
                                if (_replayCounter < _game1.WayToFinish.Count)
                                    _playCube1.MainSide = _game1.MovePlayer(_game1.WayToFinish[_replayCounter].value);
                                else
                                    _playCube2.MainSide = _game2.MovePlayer(_game2.WayToFinish[_replayCounter - _game1.WayToFinish.Count].value);

                                if (_replayCounter + 1 == _game1.WayToFinish.Count + _game2.WayToFinish.Count)
                                {
                                    _isReplay = false;
                                    _replayCounter = 0;
                                }
                                else _replayCounter++;
                                passed = 0;
                            }
                            
                        }
                        else passed += e.Time;
                    }
                    else _wayTextSurface.Clear();

                    _textShader.Use();

                    _textSurface2.Draw();
                    if (displayUnite) _unitedData.Draw();
                    _textSurface.Draw();
                    
                    
                    _wayTextSurface.Draw();

                }
                // ====================================== LAMPS ====================================== //
                GL.BindVertexArray(_vaoLamp);

                _lampShader.Use();

                _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
                _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
                // We use a loop to draw all the lights at the proper position
                for (int i = 0; i < _pointLightPositions.Length; i++)
                {
                    Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
                    lampMatrix = lampMatrix * Matrix4.CreateTranslation(_pointLightPositions[i]);

                    _lampShader.SetMatrix4("model", lampMatrix);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }



                SwapBuffers();
            }

            void UpdateGameMap(string mapName)
            {
                _cubePositions = _game1.UpdateMap(mapName);
                _game2.UpdateMap(mapName);
                _playCube1.MainSide = (int)_game1.CurrentSide;
                _playCube2.MainSide = (int)_game2.CurrentSide;
                _playCube1.Position = _game1.PlayerPos;
                _playCube2.Position = _game2.PlayerPos;
                _freezeResults = false;
                _isReplay = false;
                _replayCounter = 0;
                passed = 0;
                _wayTextSurface.Clear();
                _textSurface.Clear();
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);

                if (!IsFocused)
                {
                    return;
                }

                var input = KeyboardState;

                if (input.IsKeyDown(Keys.Escape))
                {
                    Close();
                }

                const float cameraSpeed = 1.5f;
                const float sensitivity = 0.2f;

                if (input.IsKeyDown(Keys.W))
                {
                    _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
                }
                if (input.IsKeyDown(Keys.S))
                {
                    _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
                }
                if (input.IsKeyDown(Keys.A))
                {
                    _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Keys.D))
                {
                    _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Keys.Space))
                {
                    _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
                }
                if (input.IsKeyReleased(Keys.F1))
                {
                    UpdateGameMap("default.txt");
                }
                if (input.IsKeyReleased(Keys.F2))
                {
                    UpdateGameMap("map1.txt");
                }
                if (input.IsKeyReleased(Keys.F3))
                {
                    UpdateGameMap("map2.txt");
                }
                if (input.IsKeyReleased(Keys.V))
                {
                    displayUnite = !displayUnite;
                }
                if (input.IsKeyReleased(Keys.P))
                {
                    Console.WriteLine($"Pos = {_camera.Position} Pitch = {_camera.Pitch} Yaw = {_camera.Yaw}");
                }
                if (input.IsKeyReleased(Keys.U))
                {
                    if (!_freezeResults && !_isReplay && !_game1.AIIsWorking && !_game2.AIIsWorking)
                    {
                        _wayTextSurface.Clear();
                        _game1.StartAI();
                        _game2.StartAI();
                    }
                }
                if (input.IsKeyReleased(Keys.Y))
                {
                    if (_freezeResults && !_isReplay && !_game1.AIIsWorking && !_game2.AIIsWorking)
                    {
                        _freezeResults = false;
                        _isReplay = false;
                        _replayCounter = 0;
                        passed = 0;
                        _wayTextSurface.Clear();
                        _unitedData.Clear();
                        displayUnite = false;
                    }    
                    
                }
                if (input.IsKeyReleased(Keys.T))
                {
                    _playCube1.MainSide = _game1.MovePlayer(_game1.StartState);
                    _playCube2.MainSide = _game2.MovePlayer(_game2.StartState);
                }
                if (input.IsKeyReleased(Keys.R))
                {
                    if (!_game1.AIIsWorking && !_game2.AIIsWorking)
                    {
                        _replayCounter = 0;
                        passed = 0;
                        _playCube1.MainSide = _game1.MovePlayer(_game1.StartState);
                        _playCube2.MainSide = _game2.MovePlayer(_game2.StartState);
                        _isReplay = true;
                    }
                    
                }
                if (input.IsKeyReleased(Keys.O))
                {
                    _camera.Position = new Vector3(1.0343208f, 3.559164f, 3.573409f);
                    _camera.Pitch = -46.798386f;
                    _camera.Yaw = 270.59592f;
                }
                // ========== ДВИЖЕНИЕ КУБИКА ========== //
                if (!_game1.AIIsWorking && !_game2.AIIsWorking && !_isReplay)
                {
                    if (input.IsKeyReleased(Keys.Left))
                    {
                        _playCube1.MainSide = _game1.MovePlayer(Direction.LEFT);
                    }
                    if (input.IsKeyReleased(Keys.Right))
                    {
                        _playCube1.MainSide = _game1.MovePlayer(Direction.RIGHT);
                    }
                    if (input.IsKeyReleased(Keys.Up))
                    {
                        _playCube1.MainSide = _game1.MovePlayer(Direction.UP);
                    }
                    if (input.IsKeyReleased(Keys.Down))
                    {
                        _playCube1.MainSide = _game1.MovePlayer(Direction.DOWN);
                    }
                }
                
                // ========== ДВИЖЕНИЕ КУБИКА ========== //
                var mouse = MouseState;

                if (_firstMove)
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity;
                }
            }

            protected override void OnMouseWheel(MouseWheelEventArgs e)
            {
                base.OnMouseWheel(e);

                _camera.Fov -= e.OffsetY;
            }

            protected override void OnResize(ResizeEventArgs e)
            {
                base.OnResize(e);

                GL.Viewport(0, 0, Size.X, Size.Y);
                _camera.AspectRatio = Size.X / (float)Size.Y;
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            GameWindowSettings gameWindowSettings = new GameWindowSettings();
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
            {
                Title = "AI LAB 1",
                Size = new Vector2i(WIDTH, HEIGHT)
            };

            Window win = new Window(gameWindowSettings, nativeWindowSettings);
            win.Run();


        }
    }
}