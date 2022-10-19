using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text;

namespace Logic
{
    internal class Map
    {
        private List<List<int>> _map;
        private Vector3[] _vecMap;

        private readonly string _defaultPath = "Map/";
        private readonly string _defaultMap = "default.txt";
        public Vector3 _startPos;
        public Vector3 _endPos;
        private Side _startSide = Side.LEFT;
        public Side StartSide => _startSide;
        private Side _endSide = Side.DOWN;
        public Side EndSide => _endSide;
        public Map()
        {
            this.Update(_defaultMap);
        }

        public void Update (string path)
        {
            UpdateIntMap(path);
            UpdateVecMap();
        }

        private void UpdateIntMap(string Path)
        {
            var newMap = new List<List<int>>();
            try
            {
                using (StreamReader reader = new StreamReader(_defaultPath + Path))
                {
                    var startSideFromFile = reader.ReadLine();
                    if (startSideFromFile != null && startSideFromFile.Length == 1) _startSide = (Side)int.Parse(startSideFromFile);

                    var endSideFromFile = reader.ReadLine();
                    if (endSideFromFile != null && endSideFromFile.Length == 1) _endSide = (Side)int.Parse(endSideFromFile);

                    string text = reader.ReadToEnd();

                    var rows = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var row in rows)
                    {
                        var rowElements = new List<int>();
                        var cols = row.Split('\t');
                        if (cols.Count() == 0) cols = row.Split(' ');
                        foreach (var col in cols)
                        {
                            rowElements.Add(Int32.Parse(col));
                        }
                        newMap.Add(rowElements);
                    }
                }

                _map = newMap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении карты {ex.Message}");
            }
            
        }

        internal List<List<int>> IntMap => _map;

        public int getMapValue(int x, int y)
        {
            return _map[_map.Count - y - 1][x];
        }

        private void UpdateVecMap()
        {
            var list = new List<Vector3>();

            bool hasStart = false;
            bool hasEnd = false;

            try
            {
                for (int i = 0; i < _map.Count; i++)
                {
                    for (int j = 0; j < _map[i].Count; j++)
                    {
                        if (_map[i][j] == 1)
                        {
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 2 && !hasStart)
                        {
                            hasStart = true;
                            _startPos = new Vector3((float)j, 1.0f, (float)i);
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 3 && !hasEnd)
                        {
                            hasEnd = true;
                            //_endPos3D = new Vector3((float)j, 0.0f, (float)i);
                            _endPos = new Vector3((float)j, 0.0f, (float)i);
                            list.Add(_endPos);

                        }
                    }
                }

                if (!hasStart) throw new Exception("Не найден старт");
                if (!hasEnd) throw new Exception("Не найден финиш");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении карты: {ex.Message}");
            }


            _vecMap = list.ToArray();
        }

        public Vector3[] VectorMap => _vecMap;

        /*public Vector3[] GetMap()
        {
            var list = new List<Vector3>();

            bool hasStart = false;
            bool hasEnd = false;

            try
            {
                for (int i = 0; i < _map.Count; i++)
                {
                    for (int j = 0; j < _map[i].Count; j++)
                    {
                        if (_map[i][j] == 1)
                        {
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 2 && !hasStart)
                        {
                            hasStart = true;
                            _startPos = new Vector3((float)j, 1.0f, (float)i);
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 3 && !hasEnd)
                        {
                            hasEnd = true;
                            //_endPos3D = new Vector3((float)j, 0.0f, (float)i);
                            _endPos = new Vector3((float)j, 0.0f, (float)i);
                            list.Add(_endPos);

                        }
                    }
                }

                if (!hasStart) throw new Exception("Не найден старт");
                if (!hasEnd) throw new Exception("Не найден финиш");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении карты: {ex.Message}");
            }

            
            return list.ToArray();
        }*/

    }

    public enum Direction { LEFT, RIGHT, UP, DOWN }

    public enum Side { FORWARD, BACK, LEFT, RIGHT, DOWN, UP}

    public class State
    {
        public int X, Y;
        public Side Side;

        public State() { }

        public State (State state) { X = state.X; Y = state.Y; Side = state.Side; }

        public override string ToString()
        {
            return $"[{X};{Y}]: {Side}";
        }
        public static bool operator ==(State a, State b) => a?.X == b?.X && a?.Y == b?.Y && a?.Side == b?.Side;

        public static bool operator !=(State a, State b) => !(a?.X == b?.X && a?.Y == b?.Y && a?.Side == b?.Side);


    }

    internal class Player
    {
        /// <summary>
        /// Координаты игрового кубика
        /// </summary>
        public Vector3 _position;
        public Side _playerSide;
        public int _mapSize;

        public void Move(State state)
        {
            _position.X = state.X;
            _position.Z = _mapSize - state.Y - 1;
            _playerSide = state.Side;
        }
        
    }

    public class Node
    {
        public State value;
        public double fx; // f(x)
        public int gx; // g(x)
        public double hx; // h(x)
        public Node parent;
        public Direction dirToThisState;

        public override string ToString() => value.ToString();

        public static bool operator ==(Node a, Node b) => a?.value == b?.value;

        public static bool operator !=(Node a, Node b) => !(a?.value == b?.value);
    }

    internal class AI
    {
        
        private readonly Game _game;

        private bool _isWorking = false;

        public bool IsWorking => _isWorking;

        private State _start;

        public State StartState => _start;

        private State _finishState;

        public State FinishState => _finishState;

        private State _currentState;

        public State CurrentState { get { return _currentState; } set { _currentState = value; } }

        //private Queue<Node> O = new Queue<Node>();

        private PriorityQueue<Node, double> O = new PriorityQueue<Node, double>();

        private Dictionary<string, Node> C = new Dictionary<string, Node>();

        private bool isFinish(State state) => state == _finishState;

        private double timer = 0;

        public double WorkingTime => timer;

        public AI(Game game) => _game = game;

        private List<Node> _wayToFinish = new List<Node>();

        public List<Node> WayToFinish => _wayToFinish;

        private List<string> _AIInfo = new List<string>();

        public List<string> AIInfo => _AIInfo;

        private int _iterationCounter = 0;

        private int _maxOCount = 0;

        private int _maxOAndCCount = 0;

        public void UpdateStates(State startState, State finishState)
        {
            O.Clear();
            C.Clear();
            _AIInfo.Clear();
            _wayToFinish.Clear();
            _iterationCounter = 0;
            _maxOCount = 0;
            _maxOAndCCount = 0;
            _start = startState;
            _finishState = finishState;
            _currentState = _start;

            var node = new Node { value = startState, parent = null, gx = 0 };
            node.fx = f(node);
            O.Enqueue(node, node.fx);

           
        }

        public void Start()
        {
            timer = 0;
            _isWorking = true;
        }

        public int SearchMode = 0;

        private int g (Node x) => x.gx;

        private double h1 (Node x)
        {
            var wayLength = Math.Abs(x.value.X - _finishState.X) + Math.Abs(x.value.Y - _finishState.Y);
            x.hx = wayLength;
            return wayLength;
        }

        private double h2 (Node x)
        {
            double wayLength = 1000;

            var tempState = new State(x.value);

            while (tempState.X > FinishState.X)
                _game.FallToSide(tempState, Direction.LEFT);
            while (tempState.X < FinishState.X)
                _game.FallToSide(tempState, Direction.RIGHT);
            while (tempState.Y > FinishState.Y)
                _game.FallToSide(tempState, Direction.DOWN);
            while (tempState.Y < FinishState.Y)
                _game.FallToSide(tempState, Direction.UP);


            wayLength = Math.Abs(x.value.X - _finishState.X) + Math.Abs(x.value.Y - _finishState.Y);

            switch(tempState.Side)
            {
                case Side.RIGHT:    wayLength += 1; break;  // 4
                case Side.LEFT:     wayLength += 1; break;  // 4
                case Side.UP:       wayLength += 2; break;  // 6
                case Side.DOWN:     wayLength += 0; break;  // 0
                case Side.FORWARD:  wayLength += 1; break;  // 4
                case Side.BACK:     wayLength += 1; break;  // 4
                
            }

            x.hx = wayLength;
            return wayLength;
        }

        private double f(Node x)
        {
            switch(SearchMode)
            {
                case 1: return h1(x);
                case 2: return g(x) + h1(x);
                case 3: return h2(x);
                case 4: return g(x) + h2(x);
                default:return g(x);
            }
        }

        public bool Work(double time)
        {
            timer += time;
            _iterationCounter++;

            if (_maxOCount < O.Count)
                _maxOCount = O.Count;

            if (_maxOAndCCount < O.Count + C.Count)
                _maxOAndCCount = O.Count + C.Count;

            var node = O.Dequeue();

            _game.MovePlayer(node.value);

            if (isFinish(CurrentState))
            {
                while (node != null)
                {
                    if (_wayToFinish.Contains(node)) break;
                    _wayToFinish.Add(node);
                    node = node.parent;
                }
                
                _wayToFinish.Reverse();
                _wayToFinish.Remove(_wayToFinish.First());

                _AIInfo.Add($"Количество итераций цикла поиска: {_iterationCounter}");
                _AIInfo.Add($"Максимальное количество узлов в списке O: {_maxOCount}");
                _AIInfo.Add($"Количество узлов в списке O в конце поиска: {O.Count}");
                _AIInfo.Add($"Максимальное количество хранимых в памяти узлов: {_maxOAndCCount}");
                

                _isWorking = false;
                return true;
            }

            C[node.ToString()] = node;

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var possibleState = _game.CanMove(dir);
                if (possibleState != null)
                {
                    var tmpNode = new Node { value = possibleState, parent = node, dirToThisState = dir };
                    tmpNode.gx = node.gx + 1;
                    tmpNode.fx = f(tmpNode);

                    if (C.Count(x => x.Value == tmpNode) != 0) continue;

                    /*// смотрим есть ли такое состояние в списаке C
                    var existInC = C.SingleOrDefault(x => x.Value == tmpNode);
                    if (existInC.Key != null && SearchMode != 0)
                    {
                        if (existInC.Value.fx < tmpNode.fx)
                        {
                            
                            existInC.Value.parent = node;
                            //existInC.Value.fx = tmpNode.fx;
                            existInC.Value.gx = tmpNode.gx;
                            //existInC.Value.fx = f(existInC.Value);
                            //existInC.Value.hx = tmpNode.hx;
                            O.Enqueue(existInC.Value, existInC.Value.fx);
                            C.Remove(existInC.Key);
                            continue;
                        }
                    }*/

                    var existInO = O.UnorderedItems.SingleOrDefault(x => x.Element == tmpNode);
                    if (existInO.Element == null)
                    {
                        O.Enqueue(tmpNode, tmpNode.fx);
                    }
                    else
                    {
                        if (existInO.Element.gx < tmpNode.gx)
                        {
                            existInO.Element.parent = node;
                            existInO.Element.gx = tmpNode.gx;
                        }
                    }

                    /*// смотрим есть ли такое состояние в списке O
                    var existInO = O.UnorderedItems.SingleOrDefault(x => x.Element == tmpNode);
                    if (existInO.Element != null)
                    {
                        if (existInO.Priority < tmpNode.fx && SearchMode != 0)
                        {
                            existInO.Priority = tmpNode.fx;
                        }
                    }
                    else
                    {
                        // смотрим есть ли такое состояние в списаке C
                        var existInC = C.SingleOrDefault(x => x.Value == tmpNode);
                        if (existInC.Key != null && SearchMode != 0)
                        {
                            if (existInC.Value.fx < tmpNode.fx)
                            {
                                C.Remove(existInC.Key);
                                existInC.Value.parent = tmpNode;
                                existInC.Value.fx = tmpNode.fx;
                                existInC.Value.gx = tmpNode.gx;
                                //existInC.Value.hx = tmpNode.hx;
                                O.Enqueue(existInC.Value, existInC.Value.fx);
                            }
                        }
                        else // иначе добавляем в список O
                        {
                            O.Enqueue(tmpNode, tmpNode.fx);
                        }
                    }*/



                }
            }
            

            if (O.Count == 0)
            {
                _wayToFinish.Clear();
                _AIInfo.Add("Решений нет");
                _isWorking = false;
                return true;
            }

            return false;
        }
    }

    public class Game
    {
        private Map _gameMap;
        private Player _player;
        private AI _AI;

        public Game()
        {
            _gameMap = new Map();
            _player = new Player();
            _AI = new AI(this);
            _player._mapSize = _gameMap.IntMap.Count;
            _player._position = _gameMap._startPos;
            _player._position.Y += 0.1f;
            _player._playerSide = _gameMap.StartSide;
            UpdateAIStates();
        }

        public void UpdateAIStates()
        {
            var startState = new State
            {
                X = (int)_player._position.X,
                Y = _player._mapSize - (int)_player._position.Z - 1,
                Side = _player._playerSide
            };

            var endState = new State
            {
                X = (int)_gameMap._endPos.X,
                Y = _player._mapSize - (int)_gameMap._endPos.Z - 1,
                Side = _gameMap.EndSide
            };

            _AI.UpdateStates(startState, endState);
        }

        public Vector3[] UpdateMap(string path)
        {
            _gameMap.Update(path);
            _player._mapSize = _gameMap.IntMap.Count;
            _player._position = _gameMap._startPos;
            _player._position.Y += 0.1f;
            _player._playerSide = _gameMap.StartSide;
            UpdateAIStates();
            return _gameMap.VectorMap;
        }

        public Side CurrentSide => _player._playerSide;

        public Vector3[] Map => _gameMap.VectorMap;

        public State CurrentState => _AI.CurrentState;

        public State FinishState => _AI.FinishState;

        public State StartState => _AI.StartState;

        public bool AIIsWorking => _AI.IsWorking;

        /// <summary>
        /// Получить координаты финиша в 3D пространстве
        /// </summary>
        public Vector3 EndPos => _gameMap._endPos;

        /// <summary>
        /// Получить координты игрового кубика в 3D пространстве
        /// </summary>
        public Vector3 PlayerPos => _player._position;

        public void FallToSide(State state, Direction dir)
        {
            switch (dir)
            {
                case Direction.LEFT:
                    switch (state.Side)
                    {
                        case Side.LEFT: state.Side = Side.DOWN; break;
                        case Side.RIGHT: state.Side = Side.UP; break;
                        case Side.FORWARD: state.Side = Side.FORWARD; break;
                        case Side.BACK: state.Side = Side.BACK; break;
                        case Side.UP: state.Side = Side.LEFT; break;
                        case Side.DOWN: state.Side = Side.RIGHT; break;
                    }
                    state.X--;
                    break;
                case Direction.RIGHT:
                    switch (state.Side)
                    {
                        case Side.LEFT: state.Side = Side.UP; break;
                        case Side.RIGHT: state.Side = Side.DOWN; break;
                        case Side.FORWARD: state.Side = Side.FORWARD; break;
                        case Side.BACK: state.Side = Side.BACK; break;
                        case Side.UP: state.Side = Side.RIGHT; break;
                        case Side.DOWN: state.Side = Side.LEFT; break;
                    }
                    state.X++;
                    break;
                case Direction.UP:
                    switch (state.Side)
                    {
                        case Side.LEFT: state.Side = Side.LEFT; break;
                        case Side.RIGHT: state.Side = Side.RIGHT; break;
                        case Side.FORWARD: state.Side = Side.DOWN; break;
                        case Side.BACK: state.Side = Side.UP; break;
                        case Side.UP: state.Side = Side.FORWARD; break;
                        case Side.DOWN: state.Side = Side.BACK; break;
                    }
                    state.Y++;
                    break;
                case Direction.DOWN:
                    switch (state.Side)
                    {
                        case Side.LEFT: state.Side = Side.LEFT; break;
                        case Side.RIGHT: state.Side = Side.RIGHT; break;
                        case Side.FORWARD: state.Side = Side.UP; break;
                        case Side.BACK: state.Side = Side.DOWN; break;
                        case Side.UP: state.Side = Side.BACK; break;
                        case Side.DOWN: state.Side = Side.FORWARD; break;
                    }
                    state.Y--;
                    break;
            }
        }

        public State CanMove(Direction dir)
        {
            var state = new State(_AI.CurrentState);

            try
            {
                FallToSide(state, dir);
                //return state;
                var mapValue = _gameMap.getMapValue(state.X, state.Y);
                
                if (mapValue != 0) return state;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Выход за пределы");
            }
            return null;
        }

        public int MovePlayer(State state)
        {
            if (state != null)
            {
                _player.Move(state);

                _AI.CurrentState = state;
            }

            return (int)_player._playerSide;
        }

        public int MovePlayer(Direction dir) 
        {
            var newState = CanMove(dir);

            return MovePlayer(newState);
        }

        public void StartAI()
        {
            UpdateAIStates();
            _AI.Start();
        }

        public List<Node> WayToFinish => _AI.WayToFinish;

        public List<string> AIInfo => _AI.AIInfo;

        public bool GetAINextStep(double time) => _AI.Work(time);

        public double AIWorkingTime => _AI.WorkingTime;

        public int ReplayStep(int currentStep)
        {
            return 0;
        }

        public int AISearchMode { get { return _AI.SearchMode;} set { _AI.SearchMode = value; } }

    }
    
}